using Service.Interface;
using System;

namespace OkaneFlow_UnitTests.FakeRepos
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public Guid UserGuid { get; set; }
        public bool IsAuthenticated { get; set; } = true;

        public FakeCurrentUserService(Guid userGuid)
        {
            UserGuid = userGuid;
            UserId = userGuid.ToString();
            UserName = "TestUser";
        }

        public FakeCurrentUserService() : this(Guid.NewGuid()) { }
    }
}
