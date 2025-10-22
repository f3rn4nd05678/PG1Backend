namespace ProyectoGraduación.DTOs;

// ===== DTOs de Usuario =====


public class FiltroUsuarioDto
{
    public string? TerminoBusqueda { get; set; }
    public int? RolId { get; set; }
    public bool? Activo { get; set; }
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
}

public class ListadoUsuariosResponseDto
{
    public IEnumerable<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}

public class ObtenerUsuarioPorIdDto
{
    public int Id { get; set; }
}

public class ActualizarUsuarioRequest
{
    public int Id { get; set; }
    public RegistroUsuarioDto Datos { get; set; } = null!;
}

public class EliminarUsuarioDto
{
    public int Id { get; set; }
}

public class ValidarCorreoDto
{
    public string Correo { get; set; } = string.Empty;
    public int? IdExcluir { get; set; }
}

public class CambiarPasswordDto
{
    public int Id { get; set; }
    public string NuevaPassword { get; set; } = string.Empty;
}

public class ActivarDesactivarUsuarioDto
{
    public int Id { get; set; }
    public bool Activo { get; set; }
}

// ===== DTOs de Rol =====

public class FiltroRolDto
{
    public string? TerminoBusqueda { get; set; }
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
}

public class ListadoRolesResponseDto
{
    public IEnumerable<RolDto> Roles { get; set; } = new List<RolDto>();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}

public class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class CrearRolDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ActualizarRolDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ActualizarRolRequest
{
    public int Id { get; set; }
    public ActualizarRolDto Datos { get; set; } = null!;
}

public class ObtenerRolPorIdDto
{
    public int Id { get; set; }
}

public class EliminarRolDto
{
    public int Id { get; set; }
}

// ===== DTOs de Permiso =====

public class PermisoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class AsignarPermisoRequest
{
    public int RolId { get; set; }
    public int PermisoId { get; set; }
}

public class RemoverPermisoRequest
{
    public int RolId { get; set; }
    public int PermisoId { get; set; }
}

public class ListarPermisosPorRolDto
{
    public int RolId { get; set; }
}