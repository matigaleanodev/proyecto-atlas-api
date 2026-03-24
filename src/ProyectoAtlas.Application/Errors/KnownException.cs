using System.Net;

namespace ProyectoAtlas.Application.Errors;

public abstract class KnownException : Exception
{
  protected KnownException(string message, string code, HttpStatusCode statusCode)
      : base(message)
  {
    Code = code;
    StatusCode = statusCode;
  }

  public string Code { get; }

  public HttpStatusCode StatusCode { get; }
}
