namespace GrowStore.Application.Addresses.DTOs;

public class CreateAddressDto
{
    public string Logradouro { get; set; } = string.Empty; 
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty; // Sigla de 2 dígitos (Ex: "RS")
    public string Cep { get; set; } = string.Empty;
}