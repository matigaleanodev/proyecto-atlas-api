using System.Net;

namespace ProyectoAtlas.Application.Errors;

public abstract class KnownException(string message, string code, HttpStatusCode statusCode) : Exception(message)
{
  public string Code { get; } = code;

  public HttpStatusCode StatusCode { get; } = statusCode;
}
