namespace OpenBank.API.Application.Enum;

public enum StatusCode : ushort
{
    Sucess = 200,
    Created = 201,
    BadRequest = 400,
    Authentication = 401,
    Forbidden = 402,
    NotFound = 404,
    ServerError = 500
}