using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.UserService
{
    public interface IUserService
    {
        Task<PaginatedList<User>> GetAllUsers(string role, PageInfo pageInfo);
        Task<List<UserClaimsResponse>> GetUserClaims(string userId);
        Task<ClaimsResult> GetClaimsByRole(int role);
        Task<UserResponse> CreateUser(UserRequestCreate userRequest, string roleName);
        Task<UserResponse> UpdateUser(string userId, UserRequestUpdate userRequest);
        Task DeleteUser(string userId);
        Task BanUser(string userId);
        Task UnbanUser(string userId);
        Task UpdateUserClaims(string userId, List<UserClaimsRequest> userClaimsRequest);
        Task ChangeAvatar(string userId, IFormFile avatar);
        Task AddUserAddress(string userId, AddressRequest addressRequest);
        Task UpdateUserAddress(string userId, AddressRequest addressRequest);
    }
}
