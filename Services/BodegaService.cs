using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Services;

public class BodegaService : IBodegaService
{
    private readonly IBodegaRepository _bodegaRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<BodegaService> _logger;

    public BodegaService(
        IBodegaRepository bodegaRepository,
        AppDbContext context,
        ILogger<BodegaService> logger)
    {
        _bodegaRepository = bodegaRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BodegaDto>> GetAll()
    {
        var bodegas = await _bodegaRepository.GetAll();

        return bodegas.Select(b => new BodegaDto
        {
            Id = b.Id,
            Codigo = b.Codigo,
            Nombre = b.Nombre,
            Direccion = b.Direccion,
            Responsable = b.Responsable,
            Telefono = b.Telefono,
            CapacidadM3 = b.CapacidadM3,
            Activa = b.Activa,
            TotalProductos = b.Stocks?.Count ?? 0,
            FechaCreacion = b.FechaCreacion
        });
    }

    public async Task<BodegaDto?> GetById(int id)
    {
        var bodega = await _bodegaRepository.GetById(id);
        if (bodega == null)
            return null;

        return new BodegaDto
        {
            Id = bodega.Id,
            Nombre = bodega.Nombre,
            Codigo = bodega.Codigo,
            Direccion = bodega.Direccion,
            Responsable = bodega.Responsable,
            Telefono = bodega.Telefono,
            CapacidadM3 = bodega.CapacidadM3,
            Activa = bodega.Activa,
            TotalProductos = bodega.Stocks?.Count ?? 0,
            FechaCreacion = bodega.FechaCreacion
        };
    }

    public async Task<(IEnumerable<BodegaDto>, int)> GetWithFilters(FiltroBodegaDto filtro)
    {
        var (bodegas, total) = await _bodegaRepository.GetWithFilters(filtro);

        var bodegasDto = bodegas.Select(b => new BodegaDto
        {
            Id = b.Id,
            Nombre = b.Nombre,
            Codigo = b.Codigo,
            Direccion = b.Direccion,
            Responsable = b.Responsable,
            Telefono = b.Telefono,
            CapacidadM3 = b.CapacidadM3,
            Activa = b.Activa,
            TotalProductos = b.Stocks?.Count ?? 0,
            FechaCreacion = b.FechaCreacion
        });

        return (bodegasDto, total);
    }

    public async Task<BodegaDto> Create(CrearBodegaDto dto, int usuarioId)
    {
        // Validar que no exista el nombre
        if (await _bodegaRepository.ExisteNombre(dto.Nombre))
        {
            throw new InvalidOperationException($"Ya existe una bodega con el nombre '{dto.Nombre}'");
        }

        var bodega = new Bodega
        {
            Nombre = dto.Nombre,
            Direccion = dto.Direccion,
            Responsable = dto.Responsable,
            Telefono = dto.Telefono,
            CapacidadM3 = dto.CapacidadM3,
            Activa = true
        };

        bodega = await _bodegaRepository.Create(bodega);

        // Crear stock para todos los productos activos
        await CrearStockParaTodosLosProductos(bodega.Id);

        // Registrar en bitácora
        await RegistrarBitacora(usuarioId, $"Creación de bodega: {bodega.Nombre}");

        return await GetById(bodega.Id)
            ?? throw new Exception("Error al obtener la bodega creada");
    }

    public async Task<BodegaDto> Update(int id, ActualizarBodegaDto dto, int usuarioId)
    {
        var bodega = await _bodegaRepository.GetById(id);
        if (bodega == null)
        {
            throw new KeyNotFoundException($"Bodega con ID {id} no encontrada");
        }

        // Validar que no exista otro con el mismo nombre
        if (await _bodegaRepository.ExisteNombre(dto.Nombre, id))
        {
            throw new InvalidOperationException($"Ya existe otra bodega con el nombre '{dto.Nombre}'");
        }

        bodega.Nombre = dto.Nombre;
        bodega.Direccion = dto.Direccion;
        bodega.Responsable = dto.Responsable;
        bodega.Telefono = dto.Telefono;
        bodega.CapacidadM3 = dto.CapacidadM3;
        bodega.Activa = dto.Activa;

        await _bodegaRepository.Update(bodega);

        // Registrar en bitácora
        await RegistrarBitacora(usuarioId, $"Actualización de bodega: {bodega.Nombre}");

        return await GetById(id)
            ?? throw new Exception("Error al obtener la bodega actualizada");
    }

    public async Task<bool> Delete(int id, int usuarioId)
    {
        var bodega = await _bodegaRepository.GetById(id);
        if (bodega == null)
            return false;

        // Verificar que no tenga stock activo
        var tieneStock = await _context.Stocks
            .AnyAsync(s => s.IdBodega == id && s.CantidadActual > 0);

        if (tieneStock)
        {
            throw new InvalidOperationException(
                "No se puede eliminar la bodega porque tiene productos con stock. " +
                "Debe transferir el stock a otra bodega primero."
            );
        }

        var resultado = await _bodegaRepository.Delete(id);

        if (resultado)
        {
            await RegistrarBitacora(usuarioId, $"Eliminación (soft delete) de bodega: {bodega.Nombre}");
        }

        return resultado;
    }

    public async Task<bool> CambiarEstado(int id, bool activa, int usuarioId)
    {
        var bodega = await _bodegaRepository.GetById(id);
        if (bodega == null)
            return false;

        bodega.Activa = activa;
        await _bodegaRepository.Update(bodega);

        await RegistrarBitacora(
            usuarioId,
            $"Cambio de estado de bodega '{bodega.Nombre}' a: {(activa ? "Activa" : "Inactiva")}"
        );

        return true;
    }

    public async Task<IEnumerable<StockAlertaDto>> GetAlertasStock(int idBodega)
    {
        // Usar la vista v_stock_alertas creada en la BD
        var alertas = await _context.Database
            .SqlQuery<StockAlertaDto>($@"
                SELECT 
                    id_stock as IdStock,
                    bodega as Bodega,
                    codigo_producto as CodigoProducto,
                    producto as Producto,
                    cantidad_actual as CantidadActual,
                    cantidad_minima as CantidadMinima,
                    cantidad_reservada as CantidadReservada,
                    cantidad_disponible as CantidadDisponible,
                    nivel_alerta as NivelAlerta,
                    ultima_entrada as UltimaEntrada,
                    ultima_salida as UltimaSalida
                FROM v_stock_alertas 
                WHERE bodega IN (SELECT nombre FROM bodega WHERE id_bodega = {idBodega})
                AND nivel_alerta IN ('SIN_STOCK', 'CRITICO', 'BAJO')
                ORDER BY 
                    CASE nivel_alerta
                        WHEN 'SIN_STOCK' THEN 1
                        WHEN 'CRITICO' THEN 2
                        WHEN 'BAJO' THEN 3
                        ELSE 4
                    END,
                    producto
            ")
            .ToListAsync();

        return alertas;
    }

    public async Task<bool> ExisteNombre(string nombre, int? idExcluir = null)
    {
        return await _bodegaRepository.ExisteNombre(nombre, idExcluir);
    }

    // Métodos privados auxiliares

    private async Task CrearStockParaTodosLosProductos(int idBodega)
    {
        var productos = await _context.Productos
            .Where(p => p.Activo)
            .ToListAsync();

        foreach (var producto in productos)
        {
            // Verificar que no exista ya
            var existeStock = await _context.Stocks
                .AnyAsync(s => s.IdProducto == producto.Id && s.IdBodega == idBodega);

            if (!existeStock)
            {
                var stock = new Stock
                {
                    IdProducto = producto.Id,
                    IdBodega = idBodega,
                    CantidadActual = 0,
                    CantidadMinima = producto.StockMinimo,
                    CantidadReservada = 0
                };

                _context.Stocks.Add(stock);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task RegistrarBitacora(int usuarioId, string accion)
    {
        try
        {
            var bitacora = new Bitacora
            {
                IdUsuario = usuarioId,
                Fecha = DateTime.UtcNow,
                Accion = accion,
                Modulo = "Bodegas"
            };

            _context.Bitacoras.Add(bitacora);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar en bitácora");
            // No lanzar excepción para no afectar la operación principal
        }
    }
}