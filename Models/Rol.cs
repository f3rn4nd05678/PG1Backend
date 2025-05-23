﻿namespace ProyectoGraduación.Models;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; }
    public ICollection<RolPermiso> RolesPermisos { get; set; }
}