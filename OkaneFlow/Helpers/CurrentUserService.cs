using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Service.Interface;

namespace OkaneFlow.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

        public bool IsAuthenticated => HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? UserName => HttpContext?.User?.Identity?.Name;

        public string? UserId => HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? Role => HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public Guid UserGuid
        {
            get
            {
                var id = UserId;
                if (Guid.TryParse(id, out var g)) return g;
                return Guid.Empty;
            }
        }

    }
}
