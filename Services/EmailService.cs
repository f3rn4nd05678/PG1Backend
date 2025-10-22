using System.Net;
using System.Net.Mail;

namespace ProyectoGraduación.Services;

public interface IEmailService
{
    Task EnviarCredencialesNuevoUsuario(string destinatario, string nombreUsuario, string passwordTemporal);
    Task EnviarNotificacionCambioPassword(string destinatario, string nombreUsuario);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnviarCredencialesNuevoUsuario(
        string destinatario,
        string nombreUsuario,
        string passwordTemporal)
    {
        var asunto = "Bienvenido al Sistema de Gestión - Plastihogar S.A.";

        var cuerpo = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #2c3e50; text-align: center;'>
                        Bienvenido a Plastihogar S.A.
                    </h2>
                    
                    <p>Hola <strong>{nombreUsuario}</strong>,</p>
                    
                    <p>Tu cuenta ha sido creada exitosamente en el Sistema de Gestión de Inventarios y Facturación.</p>
                    
                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0;'>
                        <h3 style='margin-top: 0; color: #007bff;'>Credenciales de Acceso</h3>
                        <p><strong>Usuario (Correo):</strong> {destinatario}</p>
                        <p><strong>Contraseña Temporal:</strong> <code style='background: #e9ecef; padding: 5px 10px; border-radius: 3px; font-size: 14px;'>{passwordTemporal}</code></p>
                    </div>
                    
                    <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
                        <h4 style='margin-top: 0; color: #856404;'>⚠️ Importante</h4>
                        <ul style='margin-bottom: 0;'>
                            <li>Esta es una <strong>contraseña temporal</strong></li>
                            <li>Deberás cambiarla en tu primer inicio de sesión</li>
                            <li>Por seguridad, no compartas esta contraseña</li>
                            <li>El enlace de acceso es: <a href='https://sistema.plastihogar.com'>https://sistema.plastihogar.com</a></li>
                        </ul>
                    </div>
                    
                    <h4>Pasos para tu primer acceso:</h4>
                    <ol>
                        <li>Ingresa al sistema con las credenciales proporcionadas</li>
                        <li>El sistema te solicitará cambiar tu contraseña</li>
                        <li>Elige una contraseña segura que cumpla con:
                            <ul>
                                <li>Mínimo 8 caracteres</li>
                                <li>Al menos una letra mayúscula</li>
                                <li>Al menos una letra minúscula</li>
                                <li>Al menos un número</li>
                                <li>Al menos un carácter especial (!@#$%^&*)</li>
                            </ul>
                        </li>
                    </ol>
                    
                    <p style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 12px;'>
                        Si no solicitaste esta cuenta o tienes problemas para acceder, 
                        contacta al administrador del sistema.
                    </p>
                    
                    <p style='text-align: center; margin-top: 20px;'>
                        <strong>Equipo de Plastihogar S.A.</strong>
                    </p>
                </div>
            </body>
            </html>
        ";

        await EnviarCorreo(destinatario, asunto, cuerpo);
    }

    public async Task EnviarNotificacionCambioPassword(string destinatario, string nombreUsuario)
    {
        var asunto = "Contraseña Actualizada - Sistema Plastihogar";

        var cuerpo = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h3>Hola {nombreUsuario},</h3>
                <p>Tu contraseña ha sido actualizada exitosamente.</p>
                <p>Si no realizaste este cambio, contacta inmediatamente al administrador del sistema.</p>
                <p><strong>Equipo de Plastihogar S.A.</strong></p>
            </body>
            </html>
        ";

        await EnviarCorreo(destinatario, asunto, cuerpo);
    }

    private async Task EnviarCorreo(string destinatario, string asunto, string cuerpo)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "Sistema Plastihogar";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                _logger.LogWarning("Configuración de correo no disponible. Correo no enviado a {Destinatario}", destinatario);
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Correo enviado exitosamente a {Destinatario}", destinatario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {Destinatario}", destinatario);
        }
    }
}