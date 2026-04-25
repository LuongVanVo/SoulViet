using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.UpdateComboExperience;

public class UpdateComboExperienceCommandHandler : IRequestHandler<UpdateComboExperienceCommand, ComboExperienceResponse>
{
    private readonly IComboExperienceRepository _comboExperienceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateComboExperienceCommandHandler(
        IComboExperienceRepository comboExperienceRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _comboExperienceRepository = comboExperienceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ComboExperienceResponse> Handle(UpdateComboExperienceCommand request, CancellationToken cancellationToken)
    {
        var comboExperience = await _comboExperienceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (comboExperience is null)
        {
            throw new NotFoundException($"ComboExperience with id '{request.Id}' was not found.");
        }

        if (comboExperience.GuideId != request.GuideId)
        {
            throw new ForbiddenException("You are not allowed to update this Combo Experience.");
        }
        comboExperience.Name = request.Name;
        comboExperience.Description = request.Description;
        comboExperience.Price = request.Price;
        comboExperience.PromotionalPrice = request.PromotionalPrice;
        comboExperience.MediaUrls = request.MediaUrls ?? new List<string>();
        comboExperience.IsActive = request.IsActive;

        _comboExperienceRepository.Update(comboExperience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var response = _mapper.Map<ComboExperienceResponse>(comboExperience);
        response.Success = true;
        response.Message = "Combo experience updated successfully.";

        return response;
    }
}