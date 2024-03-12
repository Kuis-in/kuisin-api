using Kuisin.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Kuisin.FunctionApp.Middlewares
{
    public class FunctionContextAccessorMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public FunctionContextAccessorMiddleware(IFunctionContextAccessor functionContextAccessor)
        {
            _functionContextAccessor = functionContextAccessor;
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (_functionContextAccessor.FunctionContext != null)
            {
                // This should never happen because the context should be localized to the current Task chain.
                // But if it does happen (perhaps the implementation is bugged), then we need to know immediately so it can be fixed.
                throw new InvalidOperationException($"Unable to initalize {nameof(IFunctionContextAccessor)}: context has already been initialized.");
            }

            _functionContextAccessor.FunctionContext = context;

            return next(context);
        }
    }
}
