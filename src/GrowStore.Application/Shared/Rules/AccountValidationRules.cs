using System.Text.RegularExpressions;

namespace GrowStore.Application.Shared.Rules;

public static class AccountValidationRules
{
    public static bool BeAStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?"":{}|<>]).+$");
    }

    public static bool BeAValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            return new System.Net.Mail.MailAddress(email).Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }
}
