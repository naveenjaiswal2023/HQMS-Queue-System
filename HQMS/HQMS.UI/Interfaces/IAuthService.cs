using HospitalQueueSystem.Web.Models;

namespace HospitalQueueSystem.Web.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, TokenDto? TokenData, string? ErrorMessage)> LoginAsync(LoginModel model);
    }
}
