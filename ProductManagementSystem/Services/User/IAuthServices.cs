using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Models.DTO;

namespace ProductManagementSystem.Services.User
{
    public interface IAuthServices
    {
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task LoggOffAsync(RegisterModel model);
        Task<SignInResult> LoginAsync(LoginModel model);
    }
}
