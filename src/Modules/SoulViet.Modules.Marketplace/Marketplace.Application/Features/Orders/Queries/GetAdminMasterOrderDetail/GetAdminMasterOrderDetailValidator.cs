using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAdminMasterOrderDetail;

public class GetAdminMasterOrderDetailValidator : AbstractValidator<GetAdminMasterOrderDetailQuery>
{
    public GetAdminMasterOrderDetailValidator()
    {
        RuleFor(x => x.MasterOrderId)
            .NotEmpty().WithMessage("MasterOrderId is required.")
            .Must(id => id != Guid.Empty).WithMessage("MasterOrderId must be a valid GUID.");
    }
}