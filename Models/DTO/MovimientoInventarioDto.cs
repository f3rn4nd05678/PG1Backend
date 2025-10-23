namespace ProyectoGraduación.DTOs;

// DTO para crear entrada
public class CrearEntradaInventarioDto
{
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
    public decimal Cantidad { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public string? Observacion { get; set; }
    public string? Referencia { get; set; }
}

// DTO para listar movimientos
public class MovimientoInventarioDto
{
    public int Id { get; set; }
    public int IdProducto { get; set; }
    public string CodigoProducto { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public int IdBodega { get; set; }
    public string NombreBodega { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observacion { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Referencia { get; set; }
}

// DTO para filtros
public class FiltroMovimientoInventarioDto
{
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
    public string? TerminoBusqueda { get; set; }
    public int? IdBodega { get; set; }
    public int? IdProducto { get; set; }
    public string? Tipo { get; set; } // Entrada, Salida, Ajuste
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string OrdenarPor { get; set; } = "fecha";
    public bool Descendente { get; set; } = true;
}

// DTO para respuesta de listado
public class ListadoMovimientosResponseDto
{
    public List<MovimientoInventarioDto> Movimientos { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}