using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Products.Interfaces;
using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ResponseProductDto>> CreateAsync(CreateProductDto dto)
    {
        if (await _productRepository.NameExistsAsync(dto.Name))
        {
            return Result<ResponseProductDto>.Failure(
                Error.Conflict("Product.NameAlreadyExists", "A product with this name already exists."));
        }

        if (!await _productRepository.CategoryExistsAsync(dto.CategoryId))
        {
            return Result<ResponseProductDto>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.ImageUrl, dto.CategoryId);

        if (dto.Variants.Count != 0)
        {
            foreach (var variantDto in dto.Variants)
            {
                var variant = ProductVariant.Create(
                    product.Id,
                    variantDto.Color,
                    variantDto.Size,
                    variantDto.Stock,
                    variantDto.Price,
                    variantDto.Sku);

                product.AddVariant(variant);
            }
        }

        await _productRepository.AddAsync(product);

        return Result<ResponseProductDto>.Success(MapToResponse(product));
    }

    public async Task<Result<IEnumerable<ResponseProductDto>>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var response = products.Select(MapToResponse);

        return Result<IEnumerable<ResponseProductDto>>.Success(response);
    }

    public async Task<Result<ResponseProductDto>> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ResponseProductDto>.Failure(
                Error.NotFound("Product.NotFound", "Product not found."));
        }

        return Result<ResponseProductDto>.Success(MapToResponse(product));
    }

    public async Task<Result<ResponseProductDto>> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ResponseProductDto>.Failure(
                Error.NotFound("Product.NotFound", "Product not found."));
        }

        product.Update(dto.Name, dto.Description, dto.Price, dto.ImageUrl);
        await _productRepository.UpdateAsync(product);

        return Result<ResponseProductDto>.Success(MapToResponse(product));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result.Failure(
                Error.NotFound("Product.NotFound", "Product not found."));
        }

        await _productRepository.DeleteAsync(id);

        return Result.Success();
    }

    private static ResponseProductDto MapToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        ImageUrl = product.ImageUrl,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name,
        Variants = product.Variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            Color = v.Color,
            Size = v.Size,
            Stock = v.Stock,
            Price = v.Price,
            Sku = v.Sku
        }).ToList(),
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };
}
