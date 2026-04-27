using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.DeleteComboExperience
{
    public class DeleteComboExperienceCommand : IRequest<DeleteComboExperienceResponse>
    {
        public Guid Id { get; set; }
        public Guid GuideId { get; set; }
    }
}
