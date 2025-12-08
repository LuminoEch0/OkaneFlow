using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OkaneFlow.Helpers
{
    // Lightweight helper to access authenticated user info throughout the web project.
    public class ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ICurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

        public bool IsAuthenticated => HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? UserName => HttpContext?.User?.Identity?.Name;

        public string? UserId => HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public Guid UserGuid
        {
            get
            {
                var id = UserId;
                if (Guid.TryParse(id, out var g)) return g;
                return new Guid();
            }
        }

        public string? Role => HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
