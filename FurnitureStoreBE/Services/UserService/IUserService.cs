using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.UserService
{
    public interface IUserService
    {
        Task<PaginatedList<UserResponse>> GetAllUsers(string role, PageInfo pageInfo);
        Task<List<UserClaimsResponse>> GetUserClaims(string userId);
        Task<ClaimsResult> GetClaimsByRole(int role);
        Task<UserResponse> CreateUser(UserRequestCreate userRequest, string roleName);
        Task<UserResponse> UpdateUser(string userId, UserRequestUpdate userRequest);
        Task DeleteUser(string userId);
        Task BanUser(string userId);
        Task UnbanUser(string userId);
        Task UpdateUserClaims(string userId, List<UserClaimsRequest> userClaimsRequest);
        Task ChangeAvatar(string userId, IFormFile avatar);
        Task<List<AddressResponse>> GetAddressesByUserId(string userId);
        Task<AddressResponse> CreateUserAddress(string userId, AddressRequest addressRequest);
        Task<AddressResponse> UpdateUserAddress(Guid addressId, AddressRequest addressRequest);
        Task DeleteUserAddress(Guid addressId);
    }
}
