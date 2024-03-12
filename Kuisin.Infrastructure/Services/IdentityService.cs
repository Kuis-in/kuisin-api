using Kuisin.Core.Interfaces;
using Kuisin.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace Kuisin.Infrastructure.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public IdentityService(IFunctionContextAccessor functionContextAccessor)
        {
            _functionContextAccessor = functionContextAccessor;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var requestData = await _functionContextAccessor.FunctionContext!.GetHttpRequestDataAsync();
            if (requestData != null && requestData.Headers.TryGetValues("X-MS-CLIENT-PRINCIPAL-ID", out var values))
            {
                return values.First();
            }
            throw new UnauthorizedAccessException("User is not authenticated");
        }
    }
}
