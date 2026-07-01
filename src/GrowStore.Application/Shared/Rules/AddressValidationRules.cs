using System.Text.RegularExpressions;

namespace GrowStore.Application.Shared.Rules;

public static class AddressValidationRules
{
    private static readonly Regex ZipCodeRegex = new(
        @"^\d{5}-?\d{3}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250)
    );

    public static bool BeAValidZipCode(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            return false;

        var cleaned = Regex.Replace(zipCode, @"[^\d]", "");
        return cleaned.Length == 8 && ZipCodeRegex.IsMatch(zipCode);
    }

    public static string FormatZipCode(string zipCode)
    {
        var cleaned = Regex.Replace(zipCode, @"[^\d]", "");

        if (cleaned.Length != 8)
            throw new ArgumentException("Zip code must contain 8 digits for formatting.", nameof(zipCode));

        return $"{cleaned[..5]}-{cleaned[5..]}";
    }
}
