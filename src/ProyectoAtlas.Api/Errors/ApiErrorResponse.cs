using System.ComponentModel;

namespace ProyectoAtlas.Api.Errors;

public sealed record ApiErrorResponse(
    [property: Description("HTTP status code returned by the API.")]
    int StatusCode,
    [property: Description("Developer-oriented message that explains the error.")]
    string Message,
    [property: Description("Stable application error code intended for frontend flows.")]
    string Code);
