using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;

namespace ProyectoGraduación.Services;

public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly IMovimientoInventarioRepository _movimientoRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IBodegaRepository _bodegaRepository;
    private readonly ILogger<MovimientoInventarioService> _logger;

    public MovimientoInventarioService(
        IMovimientoInventarioRepository movimientoRepository,
        IStockRepository stockRepository,
        IProductoRepository productoRepository,
        IBodegaRepository bodegaRepository,
        ILogger<MovimientoInventarioService> logger)
    {
        _movimientoRepository = movimientoRepository;
        _stockRepository = stockRepository;
        _productoRepository = productoRepository;
        _bodegaRepository = bodegaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetAll()
    {
        var movimientos = await _movimientoRepository.GetAll();
        return movimientos.Select(MapToDto);
    }

    public async Task<MovimientoInventarioDto?> GetById(int id)
    {
        var movimiento = await _movimientoRepository.GetById(id);
        return movimiento != null ? MapToDto(movimiento) : null;
    }

    public async Task<(IEnumerable<MovimientoInventarioDto>, int)> GetWithFilters(FiltroMovimientoInventarioDto filtro)
    {
        var (movimientos, total) = await _movimientoRepository.GetWithFilters(filtro);
        var movimientosDto = movimientos.Select(MapToDto);
        return (movimientosDto, total);
    }

    public async Task<MovimientoInventarioDto> RegistrarEntrada(CrearEntradaInventarioDto entrada, int usuarioId)
    {
        // Validar que el producto existe
        var producto = await _productoRepository.GetById(entrada.IdProducto);
        if (producto == null)
        {
            throw new InvalidOperationException("El producto no existe");
        }

        // Validar que la bodega existe
        var bodega = await _bodegaRepository.GetById(entrada.IdBodega);
        if (bodega == null)
        {
            throw new InvalidOperationException("La bodega no existe");
        }

        if (!bodega.Activa)
        {
            throw new InvalidOperationException("La bodega no está activa");
        }

        // Validar cantidad
        if (entrada.Cantidad <= 0)
        {
            throw new InvalidOperationException("La cantidad debe ser mayor a 0");
        }

        // Crear el movimiento
        var movimiento = new MovimientoInventario
        {
            IdProducto = entrada.IdProducto,
            IdBodega = entrada.IdBodega,
            Tipo = "Entrada",
            Cantidad = entrada.Cantidad,
            PrecioUnitario = entrada.PrecioUnitario,
            Observacion = entrada.Observacion,
            Referencia = entrada.Referencia,
            TipoReferencia = "Compra",
            IdUsuario = usuarioId,
            Fecha = DateTime.UtcNow
        };

        // Guardar el movimiento
        var movimientoCreado = await _movimientoRepository.Create(movimiento);

        // Actualizar el stock
        await _stockRepository.ActualizarStock(
            entrada.IdProducto,
            entrada.IdBodega,
            entrada.Cantidad,
            "Entrada"
        );

        // Recargar el movimiento con las relaciones
        var movimientoCompleto = await _movimientoRepository.GetById(movimientoCreado.Id);

        return MapToDto(movimientoCompleto!);
    }

    public async Task<MovimientoInventarioDto> RegistrarSalida(CrearEntradaInventarioDto salida, int usuarioId)
    {
        // Validar que el producto existe
        var producto = await _productoRepository.GetById(salida.IdProducto);
        if (producto == null)
        {
            throw new InvalidOperationException("El producto no existe");
        }

        // Validar que la bodega existe
        var bodega = await _bodegaRepository.GetById(salida.IdBodega);
        if (bodega == null)
        {
            throw new InvalidOperationException("La bodega no existe");
        }

        // Validar cantidad
        if (salida.Cantidad <= 0)
        {
            throw new InvalidOperationException("La cantidad debe ser mayor a 0");
        }

        // Validar que hay stock disponible
        var stock = await _stockRepository.GetByProductoYBodega(salida.IdProducto, salida.IdBodega);
        if (stock == null || stock.CantidadDisponible < salida.Cantidad)
        {
            throw new InvalidOperationException(
                $"Stock insuficiente. Disponible: {stock?.CantidadDisponible ?? 0}, Solicitado: {salida.Cantidad}"
            );
        }

        // Crear el movimiento
        var movimiento = new MovimientoInventario
        {
            IdProducto = salida.IdProducto,
            IdBodega = salida.IdBodega,
            Tipo = "Salida",
            Cantidad = salida.Cantidad,
            PrecioUnitario = salida.PrecioUnitario,
            Observacion = salida.Observacion,
            Referencia = salida.Referencia,
            TipoReferencia = "Venta",
            IdUsuario = usuarioId,
            Fecha = DateTime.UtcNow
        };

        // Guardar el movimiento
        var movimientoCreado = await _movimientoRepository.Create(movimiento);

        // Actualizar el stock
        await _stockRepository.ActualizarStock(
            salida.IdProducto,
            salida.IdBodega,
            salida.Cantidad,
            "Salida"
        );

        // Recargar el movimiento con las relaciones
        var movimientoCompleto = await _movimientoRepository.GetById(movimientoCreado.Id);

        return MapToDto(movimientoCompleto!);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetByProducto(int idProducto)
    {
        var movimientos = await _movimientoRepository.GetByProducto(idProducto);
        return movimientos.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetByBodega(int idBodega)
    {
        var movimientos = await _movimientoRepository.GetByBodega(idBodega);
        return movimientos.Select(MapToDto);
    }

    private static MovimientoInventarioDto MapToDto(MovimientoInventario movimiento)
    {
        return new MovimientoInventarioDto
        {
            Id = movimiento.Id,
            IdProducto = movimiento.IdProducto,
            CodigoProducto = movimiento.Producto?.Codigo ?? "",
            NombreProducto = movimiento.Producto?.Nombre ?? "",
            IdBodega = movimiento.IdBodega,
            NombreBodega = movimiento.Bodega?.Nombre ?? "",
            Tipo = movimiento.Tipo,
            Cantidad = movimiento.Cantidad,
            PrecioUnitario = movimiento.PrecioUnitario,
            Fecha = movimiento.Fecha,
            Observacion = movimiento.Observacion,
            NombreUsuario = movimiento.Usuario?.Nombre,
            Referencia = movimiento.Referencia
        };
    }
}