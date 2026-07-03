using FluentValidation;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Pagination;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Products.Interfaces;
using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductDto> _createProductValidator;
    private readonly IValidator<UpdateProductDto> _updateProductValidator;

    public ProductService(
        IProductRepository productRepository,
        IValidator<CreateProductDto> createProductValidator,
        IValidator<UpdateProductDto> updateProductValidator)
    {
        _productRepository = productRepository;
        _createProductValidator = createProductValidator;
        _updateProductValidator = updateProductValidator;
    }

    public async Task<Result<ResponseProductDto>> CreateAsync(CreateProductDto dto)
    {
        var validationResult = await _createProductValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<ResponseProductDto>.Failure(
                Error.Validation("Product.Validation", errorMessage));
        }

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

        if (dto.Variants is not null)
        {
            foreach (var variantDto in dto.Variants)
            {
                if (!string.IsNullOrWhiteSpace(variantDto.Sku) && await _productRepository.SkuExistsAsync(variantDto.Sku))
                {
                    return Result<ResponseProductDto>.Failure(
                        Error.Conflict("ProductVariant.SkuAlreadyExists", $"A product variant with SKU '{variantDto.Sku}' already exists."));
                }
            }
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

        // reload product including navigation properties so CategoryName is populated
        var savedProduct = await _productRepository.GetByIdAsync(product.Id);
        if (savedProduct is null)
        {
            return Result<ResponseProductDto>.Failure(
                Error.Failure("Product.CreateFailed", "Failed to create product."));
        }

        return Result<ResponseProductDto>.Success(MapToResponse(savedProduct));
    }

    public async Task<Result<PagedResult<ResponseProductDto>>> GetAllPagedAsync(ProductFilterDto filter)
    {
        if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
        {
            return Result<PagedResult<ResponseProductDto>>.Failure(
                Error.Validation("Product.InvalidPriceRange", "MinPrice cannot be greater than MaxPrice."));
        }

        if (filter.CategoryId.HasValue && !await _productRepository.CategoryExistsAsync(filter.CategoryId.Value))
        {
            return Result<PagedResult<ResponseProductDto>>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        var page = filter.Page < 1 ? 1 : filter.Page;
        var limit = filter.Limit is < 1 or > 100 ? 10 : filter.Limit;

        var (products, totalCount) = await _productRepository.GetAllPagedAsync(
            name: filter.Name,
            categoryId: filter.CategoryId,
            minPrice: filter.MinPrice,
            maxPrice: filter.MaxPrice,
            page: page,
            limit: limit);

        var items = products.Select(MapToResponse);

        return Result<PagedResult<ResponseProductDto>>.Success(
            PagedResult<ResponseProductDto>.Create(items, page, limit, totalCount));
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
        var validationResult = await _updateProductValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<ResponseProductDto>.Failure(
                Error.Validation("Product.Validation", errorMessage));
        }

        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return Result<ResponseProductDto>.Failure(
                Error.NotFound("Product.NotFound", "Product not found."));
        }

        product.Update(dto.Name, dto.Description, dto.Price, dto.ImageUrl);
        await _productRepository.UpdateAsync(product);

        var updatedProduct = await _productRepository.GetByIdAsync(id);
        if (updatedProduct is null)
        {
            return Result<ResponseProductDto>.Failure(
                Error.Failure("Product.UpdateFailed", "Failed to update product."));
        }

        return Result<ResponseProductDto>.Success(MapToResponse(updatedProduct));
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

    public async Task<Result> UpdateStockAsync(Guid variantId, UpdateStockDto dto)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);

        if (variant is null)
            return Result.Failure(
                Error.NotFound("ProductVariant.NotFound", "Product variant not found."));

        if (dto.Quantity < 0)
            return Result.Failure(
                Error.Validation("Stock.Validation", "Stock quantity cannot be negative."));

        variant.SetStock(dto.Quantity);
        await _productRepository.UpdateAsync(variant.Product!);

        return Result.Success();
    }
}
