using System.Net;
using Grpc.Core;
using Newtonsoft.Json;
using FluentValidation;

namespace ApiGateway.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException rpcEx)
        {
            _logger.LogWarning(rpcEx, "gRPC ошибка: {Code} - {Message}", rpcEx.StatusCode, rpcEx.Status.Detail);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = rpcEx.StatusCode switch
            {
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new { error = rpcEx.Status.Detail };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        catch (ValidationException valEx)
        {
            _logger.LogWarning(valEx, "Ошибка валидации");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new
            {
                error = valEx.Errors.Select(e => e.ErrorMessage)
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неперехваченная ошибка");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { error = "Внутренняя ошибка сервера" };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
