namespace ProyectoGraduación.Services;

public interface IPasswordGeneratorService
{
    string GenerarPasswordTemporal();
    bool ValidarFortalezaPassword(string password);
}

public class PasswordGeneratorService : IPasswordGeneratorService
{
    private static readonly Random _random = new Random();

    public string GenerarPasswordTemporal()
    {
        const string mayusculas = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string minusculas = "abcdefghijkmnopqrstuvwxyz";
        const string numeros = "23456789";
        const string especiales = "#@!$%";

        var password = new char[13];
        password[0] = 'T';
        password[1] = 'e';
        password[2] = 'm';
        password[3] = 'p';

        for (int i = 4; i < 8; i++)
            password[i] = mayusculas[_random.Next(mayusculas.Length)];

        password[8] = especiales[_random.Next(especiales.Length)];

        for (int i = 9; i < 11; i++)
            password[i] = minusculas[_random.Next(minusculas.Length)];

        for (int i = 11; i < 13; i++)
            password[i] = numeros[_random.Next(numeros.Length)];

        return new string(password);
    }

    public bool ValidarFortalezaPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 8)
            return false;

        bool tieneMayuscula = password.Any(char.IsUpper);
        bool tieneMinuscula = password.Any(char.IsLower);
        bool tieneNumero = password.Any(char.IsDigit);
        bool tieneEspecial = password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

        return tieneMayuscula && tieneMinuscula && tieneNumero && tieneEspecial;
    }
}