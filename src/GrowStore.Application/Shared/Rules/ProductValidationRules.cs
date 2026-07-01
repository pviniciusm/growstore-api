using System.Text.RegularExpressions;

namespace GrowStore.Application.Shared.Rules;

public static class ProductValidationRules
{
    private static readonly Regex UrlRegex = new(
        @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250)
    );

    private static readonly Regex SkuRegex = new(
        @"^[A-Z0-9_-]+$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250)
    );

    public static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return UrlRegex.IsMatch(url) && url.Length <= 500;
    }

    public static bool BeAValidSku(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return true;

        return SkuRegex.IsMatch(sku);
    }
}
