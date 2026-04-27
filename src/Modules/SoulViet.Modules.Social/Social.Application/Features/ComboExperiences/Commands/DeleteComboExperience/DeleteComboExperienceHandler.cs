using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Results;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.DeletePost;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.DeleteComboExperience
{
    public class DeleteComboExperienceHandler : IRequestHandler<DeleteComboExperienceCommand, DeleteComboExperienceResponse>
    {
        private readonly IComboExperienceRepository _comboExperienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteComboExperienceHandler (IComboExperienceRepository comboExperienceRepository, IUnitOfWork unitOfWork)
        {
            _comboExperienceRepository = comboExperienceRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteComboExperienceResponse> Handle(DeleteComboExperienceCommand request, CancellationToken cancellationToken)
        {
            var comboExperience = await _comboExperienceRepository.GetByIdAsync(request.Id, cancellationToken);
            if(comboExperience is null)
            {
                throw new NotFoundException("Combo Experience not found");
            }
            if (comboExperience.GuideId != request.GuideId)
            {
                throw new ForbiddenException("You are not allowed to delete this combo experience");
            }

            await _comboExperienceRepository.SoftDeleteAsync(comboExperience, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new DeleteComboExperienceResponse
            {
                Success = true,
                Message = "Combo experience deleted successfully."
            };

        }
    }
}
