using Microsoft.Azure.Functions.Worker;

namespace Kuisin.Infrastructure.Interfaces
{
    public interface IFunctionContextAccessor
    {
        FunctionContext? FunctionContext { get; set; }
    }
}
