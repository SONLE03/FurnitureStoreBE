using FurnitureStoreBE.DTOs.Response.UserResponse;

namespace FurnitureStoreBE.Services.UserService
{
    public interface IUserService
    {
        Task<TypeClaimsReponse> GetTypeClaimsByRole(string roleName);

    }
}
