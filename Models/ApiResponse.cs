namespace ProyectoGraduación.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Detail { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Operación completada con éxito", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Message = message,
            Detail = data
        };
    }

    public static ApiResponse<T> Success(string message = "Operación completada con éxito", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Message = message,
            Detail = default
        };
    }

    public static ApiResponse<T> Error(string message = "Error al procesar la solicitud", int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Message = message,
            Detail = default
        };
    }
}