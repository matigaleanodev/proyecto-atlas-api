using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Api.Errors;

public sealed partial class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (KnownException exception)
    {
      await WriteErrorResponse(context, (int)exception.StatusCode, exception.Message, exception.Code);
    }
    catch (Exception exception)
    {
      LogUnhandledException(logger, exception, context.Request.Method, context.Request.Path);

      await WriteErrorResponse(
          context,
          (int)HttpStatusCode.InternalServerError,
          "An unexpected error occurred.",
          AtlasErrorCodes.InternalServerError);
    }
  }

  private static async Task WriteErrorResponse(
      HttpContext context,
      int statusCode,
      string message,
      string code)
  {
    if (context.Response.HasStarted)
    {
      return;
    }

    ApiErrorResponse response = new(statusCode, message, code);

    context.Response.Clear();
    context.Response.StatusCode = statusCode;
    await context.Response.WriteAsJsonAsync(response);
  }

  [LoggerMessage(
      EventId = 1,
      Level = LogLevel.Error,
      Message = "Unhandled exception while processing request {Method} {Path}.")]
  private static partial void LogUnhandledException(
      ILogger logger,
      Exception exception,
      string method,
      string path);
}
