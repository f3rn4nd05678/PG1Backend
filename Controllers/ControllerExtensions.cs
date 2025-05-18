
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Extensions;

public static class ControllerExtensions
{

    public static ActionResult ApiOk<T>(this ControllerBase controller, T data, string message = "Operación completada con éxito")
    {
        var response = ApiResponse<T>.Success(data, message);
        return controller.StatusCode(response.StatusCode, response);
    }

    public static ActionResult ApiOk(this ControllerBase controller, string message = "Operación completada con éxito")
    {
        var response = ApiResponse<object>.Success(message);
        return controller.StatusCode(response.StatusCode, response);
    }


    public static ActionResult ApiCreated<T>(this ControllerBase controller, T data, string message = "Recurso creado con éxito")
    {
        var response = ApiResponse<T>.Success(data, message, 201);
        return controller.StatusCode(response.StatusCode, response);
    }


    public static ActionResult ApiError(this ControllerBase controller, string message = "Error al procesar la solicitud", int statusCode = 400)
    {
        var response = ApiResponse<object>.Error(message, statusCode);
        return controller.StatusCode(response.StatusCode, response);
    }


    public static ActionResult ApiNotFound(this ControllerBase controller, string message = "Recurso no encontrado")
    {
        var response = ApiResponse<object>.Error(message, 404);
        return controller.StatusCode(response.StatusCode, response);
    }


    public static ActionResult ApiUnauthorized(this ControllerBase controller, string message = "No autorizado")
    {
        var response = ApiResponse<object>.Error(message, 401);
        return controller.StatusCode(response.StatusCode, response);
    }
}