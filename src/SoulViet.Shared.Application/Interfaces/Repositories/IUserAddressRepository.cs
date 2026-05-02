using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Interfaces.Repositories;

public interface IUserAddressRepository
{
    // Lấy danh sách địa chỉ của User (Sắp xếp mặc định lên đầu)
    Task<List<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Lấy chi tiết 1 địa chỉ theo Id
    Task<UserAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Đếm số lượng địa chỉ hiện có của User (để giới hạn tối đa 5 địa chỉ)
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Lấy địa chỉ mặc định của user
    Task<UserAddress?> GetDefaultAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Các thao tác thay đổi dữ liệu
    Task AddAsync(UserAddress address, CancellationToken cancellationToken = default);
    void Update(UserAddress address);
    void Remove(UserAddress address);

    // Save changes
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}