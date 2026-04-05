using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.DeleteMarketplaceProduct;

public class DeleteMarketplaceProductHandler : IRequestHandler<DeleteMarketplaceProductCommand, bool>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMarketplaceProductHandler(IMarketplaceProductRepository marketplaceProductRepository, IUnitOfWork unitOfWork)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMarketplaceProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _marketplaceProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException("Product not found");

        if (product.PartnerId != request.PartnerId)
            throw new ForbiddenException("You are not allowed to delete this product");

        await _marketplaceProductRepository.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}