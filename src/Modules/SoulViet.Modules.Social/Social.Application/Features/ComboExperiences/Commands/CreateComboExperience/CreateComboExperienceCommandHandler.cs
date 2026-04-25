using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.CreateComboExperience
{
    public class CreateComboExperienceCommandHandler : IRequestHandler<CreateComboExperienceCommand, ComboExperienceResponse>
    {
        private readonly IComboExperienceRepository _comboExperienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateComboExperienceCommandHandler(IComboExperienceRepository comboExperienceRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _comboExperienceRepository = comboExperienceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ComboExperienceResponse> Handle(CreateComboExperienceCommand request, CancellationToken cancellationToken)
        {
            var combo = new SocialComboExperience
            {
                GuideId = request.GuideId,
                ServicePartnerId = request.ServicePartnerId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                PromotionalPrice = request.PromotionalPrice,
                MediaUrls = request.MediaUrls ?? new List<string>(),
                IsActive = true, 
                IsDeleted = false
            };
            await _comboExperienceRepository.AddAsync(combo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var response = _mapper.Map<ComboExperienceResponse>(combo);
            response.Success = true;
            response.Message = "Create Combo Experience successfully.";
            return response;
        }
    }
}
