namespace HQMS.UI.Handlers
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var token = context?.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Clear session if needed
                context?.Session.Clear();

                // Redirect only if it's a regular web request (not an API call)
                if (context?.Request.Path.StartsWithSegments("/api") == false)
                {
                    context.Response.Redirect("/Auth/Login");
                }
            }

            return response;
        }
    }
}
