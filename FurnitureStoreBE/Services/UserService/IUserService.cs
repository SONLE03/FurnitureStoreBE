using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.DTOs.Response.UserResponse;

namespace FurnitureStoreBE.Services.UserService
{
    public interface IUserService
    {
        List<UserResponse> GetAllUsers(int role);
        Task<ClaimsResult> GetClaimsByRole(int role);
        Task<UserResponse> CreateUser(UserRequestCreate userRequest, string roleName);
        Task<UserResponse> UpdateUser(string userId, UserRequestUpdate userRequest);
        Task DeleteUser(string userId);
        Task BanUser(string userId);
        Task UnbanUser(string userId);
        Task UpdateUserClaims(string userId, UserClaimsRequest userClaimsRequest);
        Task ChangeAvatar(string userId, IFormFile avatar);
    }
}
