namespace ProyectoGraduación.DTOs;

public class StockDto
{
    public int Id { get; set; }
    public int IdProducto { get; set; }
    public string CodigoProducto { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public string CategoriaProducto { get; set; } = string.Empty;
    public int IdBodega { get; set; }
    public string NombreBodega { get; set; } = string.Empty;
    public decimal CantidadActual { get; set; }
    public decimal CantidadMinima { get; set; }
    public decimal CantidadReservada { get; set; }
    public decimal CantidadDisponible { get; set; }
    public string NivelAlerta { get; set; } = "NORMAL"; // NORMAL, BAJO, CRITICO, SIN_STOCK
    public DateTime? UltimaEntrada { get; set; }
    public DateTime? UltimaSalida { get; set; }
}

public class FiltroStockDto
{
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 20;
    public string? TerminoBusqueda { get; set; }
    public int? IdBodega { get; set; }
    public int? IdCategoria { get; set; }
    public string? NivelAlerta { get; set; } // NORMAL, BAJO, CRITICO, SIN_STOCK
    public string OrdenarPor { get; set; } = "producto";
    public bool Descendente { get; set; } = false;
}

public class ListadoStockResponseDto
{
    public List<StockDto> Stocks { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}