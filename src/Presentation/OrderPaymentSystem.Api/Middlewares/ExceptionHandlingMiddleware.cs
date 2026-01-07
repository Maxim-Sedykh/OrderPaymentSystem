using OrderPaymentSystem.Shared.Result;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using ILogger = Serilog.ILogger;

namespace OrderPaymentSystem.Api.Middlewares;

/// <summary>
/// Обработчик исключений
/// </summary>
/// <summary>
/// Единый обработчик ошибок (глобальный try - catch)
/// </summary>
public class ExceptionHandlingMiddleware(ILogger logger, RequestDelegate next)
{
    /// <summary>
    /// Выполнить глобальную обработку ошибок асинхронно
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Обработать ошибку
    /// </summary>
    /// <param name="httpContext">Контекст выполнения запроса</param>
    /// <param name="exception">Исключение</param>
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        logger.Error(exception, exception.Message);

        var errorMessage = exception.Message;
        var response = exception switch
        {
            UnauthorizedAccessException => BaseResult.Failure((int)HttpStatusCode.Unauthorized, errorMessage),
            _ => BaseResult.Failure((int)HttpStatusCode.InternalServerError, errorMessage),
        };

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = response.Error.Code;

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}