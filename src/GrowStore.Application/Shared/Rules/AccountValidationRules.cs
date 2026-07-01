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
}
