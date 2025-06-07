using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQMS.Test.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<(bool, TokenDto, string)> LoginAsync(LoginModel model)
        {
            if (model.Email == "admin@qms.com" && model.Password == "Admin@123")
            {
                return Task.FromResult((true, new TokenDto
                {
                    Token = "mock-token",
                    UserId = "1",
                    Role = "Admin",
                    Expiration = DateTime.UtcNow.AddHours(1)
                }, string.Empty));
            }

            return Task.FromResult<(bool, TokenDto?, string)>((false, null, "Invalid credentials"));
        }
    }
}
