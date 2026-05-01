using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;

public class UpdateMarketplaceProductValidator : AbstractValidator<UpdateMarketplaceProductCommand>
{
    public UpdateMarketplaceProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Product ID format.");

        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("Partner ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Partner ID format.");

        When(x => x.CategoryId.HasValue, () =>
        {
            RuleFor(x => x.CategoryId)
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Category ID format.");
        });

        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(255).WithMessage("Product name must not exceed 255 characters.");
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá sản phẩm không được âm.");
        });

        When(x => x.PromotionalPrice.HasValue, () =>
        {
            RuleFor(x => x.PromotionalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá khuyến mãi không được âm.");

            When(x => x.Price.HasValue, () =>
            {
                RuleFor(x => x.PromotionalPrice)
                    .LessThan(x => x.Price).WithMessage("Giá khuyến mãi phải nhỏ hơn giá gốc.");
            });
        });

        When(x => x.Stock.HasValue, () =>
        {
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        });

        When(x => !string.IsNullOrEmpty(x.MainImage), () =>
        {
            RuleFor(x => x.MainImage)
                .Must(BeAValidUrl).WithMessage("Đường dẫn ảnh chính không hợp lệ.");
        });

        When(x => x.HasVariants == true, () =>
        {
            RuleFor(x => x.Attributes)
                .NotEmpty().WithMessage("Sản phẩm có biến thể bắt buộc phải khai báo ít nhất 1 thuộc tính (VD: Màu sắc).");

            RuleFor(x => x.Variants)
                .NotEmpty().WithMessage("Sản phẩm có biến thể bắt buộc phải khai báo ít nhất 1 biến thể.");
        });


        When(x => x.Attributes != null && x.Attributes.Any(), () =>
        {
            RuleForEach(x => x.Attributes).ChildRules(attribute =>
            {
                attribute.RuleFor(a => a.Name)
                    .NotEmpty().WithMessage("Tên thuộc tính không được để trống (VD: Kích thước, Màu sắc).")
                    .MaximumLength(100).WithMessage("Tên thuộc tính không được vượt quá 100 ký tự.");

                attribute.RuleFor(a => a.OptionsJson)
                    .NotEmpty().WithMessage("Danh sách lựa chọn không được để trống.")
                    .Must(BeAValidJsonArray).WithMessage("Danh sách lựa chọn phải là định dạng JSON Array hợp lệ (VD: [\"S\", \"M\"]).");
            });
        });


        When(x => x.Variants != null && x.Variants.Any(), () =>
        {
            RuleForEach(x => x.Variants).ChildRules(variant =>
            {
                variant.RuleFor(v => v.Sku)
                    .NotEmpty().WithMessage("Mã SKU của biến thể không được để trống.")
                    .MaximumLength(100).WithMessage("Mã SKU không được vượt quá 100 ký tự.");

                variant.RuleFor(v => v.Price)
                    .GreaterThanOrEqualTo(0).WithMessage("Giá của biến thể không được âm.");

                variant.When(v => v.PromotionalPrice.HasValue, () =>
                {
                    variant.RuleFor(v => v.PromotionalPrice)
                        .GreaterThanOrEqualTo(0).WithMessage("Giá khuyến mãi không được âm.")
                        .LessThan(v => v.Price).WithMessage("Giá khuyến mãi của biến thể phải nhỏ hơn giá gốc.");
                });

                variant.RuleFor(v => v.Stock)
                    .GreaterThanOrEqualTo(0).WithMessage("Tồn kho của biến thể không được âm.");

                variant.RuleFor(v => v.AttributesJson)
                    .NotEmpty().WithMessage("Tổ hợp thuộc tính của biến thể không được để trống.")
                    .Must(BeAValidJsonObject).WithMessage("Tổ hợp thuộc tính phải là định dạng JSON Object (VD: {\"Size\": \"M\"}).");
            });
        });
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    private bool BeAValidJsonArray(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        json = json.Trim();
        return json.StartsWith("[") && json.EndsWith("]");
    }

    private bool BeAValidJsonObject(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        json = json.Trim();
        return json.StartsWith("{") && json.EndsWith("}");
    }
}