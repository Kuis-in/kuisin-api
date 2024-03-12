namespace Kuisin.Core.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetCurrentUserIdAsync();
    }
}
