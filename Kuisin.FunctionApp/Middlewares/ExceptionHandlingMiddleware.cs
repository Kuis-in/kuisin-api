using Kuisin.Core.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;

namespace Kuisin.FunctionApp.Middlewares
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteErrorResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task WriteErrorResponseAsync(FunctionContext context, HttpStatusCode statusCode, string message)
        {
            var httpReqData = await context.GetHttpRequestDataAsync();
            if (httpReqData != null)
            {
                var newHttpResponse = httpReqData.CreateResponse(statusCode);
                await newHttpResponse.WriteAsJsonAsync(new GeneralResponse(message), newHttpResponse.StatusCode);

                var invocationResult = context.GetInvocationResult();

                var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
                if (httpOutputBindingFromMultipleOutputBindings is not null)
                {
                    httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
                }
                else
                {
                    invocationResult.Value = newHttpResponse;
                }
            }
        }

        private static OutputBindingData<HttpResponseData>? GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context)
        {
            // The output binding entry name will be "$return" only when the function return type is HttpResponseData
            var httpOutputBinding = context.GetOutputBindings<HttpResponseData>()
                .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");

            return httpOutputBinding;
        }
    }
}
