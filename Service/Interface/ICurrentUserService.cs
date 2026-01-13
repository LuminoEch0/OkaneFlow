using System;

namespace Service.Interface
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        Guid UserGuid { get; }
        bool IsAuthenticated { get; }
    }
}
