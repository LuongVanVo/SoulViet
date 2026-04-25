using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IPostShareRepository
    {
        Task AddAsync(PostShare postShare, CancellationToken cancellationToken = default);
    }
}
