using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HQMS.Tests.Common
{
    public abstract class ControllerTestBase
    {
        protected ClaimsPrincipal GetMockJwtUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin") // add other claims if needed
            }, "jwt"));
        }

        protected void SetUserContext(Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetMockJwtUser()
                }
            };
        }
    }
}
