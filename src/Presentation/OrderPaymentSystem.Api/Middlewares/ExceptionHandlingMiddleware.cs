using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Result;
using System.Net.Mime;

namespace OrderPaymentSystem.Api.Middlewares;

/// <summary>
/// Обработчик исключений
/// </summary>
/// <summary>
/// Единый обработчик ошибок (глобальный try - catch)
/// </summary>
public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
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
        logger.LogError(exception, exception.Message);

        var errorMessage = exception.Message;
        var response = exception switch
        {
            UnauthorizedAccessException => BaseResult.Failure(ErrorCodes.Unauthorized, errorMessage),
            BusinessException businessException => BaseResult.Failure(businessException.ErrorCode, businessException.Message),
            _ => BaseResult.Failure(ErrorCodes.InternalServerError, errorMessage),
        };

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = response.Error!.Code;

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}