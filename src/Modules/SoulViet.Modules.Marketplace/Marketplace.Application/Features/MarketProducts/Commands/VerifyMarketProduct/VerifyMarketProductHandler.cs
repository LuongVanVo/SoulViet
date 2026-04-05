using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.VerifyMarketProduct;

public class VerifyMarketProductHandler : IRequestHandler<VerifyMarketProductCommand, bool>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IUnitOfWork _unitOfWork;
    public VerifyMarketProductHandler(IMarketplaceProductRepository marketplaceProductRepository, IUnitOfWork unitOfWork)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(VerifyMarketProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _marketplaceProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException("Product not found");

        await _marketplaceProductRepository.VerifyProductByIdAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}