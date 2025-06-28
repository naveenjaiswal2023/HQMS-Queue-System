

using HQMS.Application.DTO;
using HQMS.Infrastructure.Data;

namespace HQMS.Domain.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateToken(ApplicationUser user);
    }
}
