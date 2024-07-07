using Sharedlaibary.DTOs;
using static Sharedlaibary.DTOs.ServiceResponse;

namespace ApiWIthJWTAuth.Contacts
{
    public interface IUserAccount
    {
        Task<GeneralResponse>CreateAccount(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    }
}
