using HQMS.Web.Models;

namespace HQMS.Web.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, TokenDto? TokenData, string? ErrorMessage)> LoginAsync(LoginModel model);
    }
}
