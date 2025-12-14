using System.Net;

namespace LevelUp.API.Utilities;

public class ApiResponse<TResponse>
{
    public int Status { get; set; }
    public string Message { get; set; }
    public TResponse? Data { get; set; }
    public int? Total { get; set; }

    public ApiResponse(TResponse? data)
    {
        Status = StatusCodes.Status200OK;
        Message = nameof(HttpStatusCode.OK);
        Data = data;
        Total = null;
    }

    public ApiResponse(string message)
    {
        Status = StatusCodes.Status200OK;
        Message = message;
        Data = default;
        Total = null;
    }

    public ApiResponse(int statusCode, string message)
    {
        Status = statusCode;
        Message = message;
        Data = default;
        Total = null;
    }

    public ApiResponse(int statusCode, string message, TResponse? data)
    {
        Status = statusCode;
        Message = message;
        Data = data;
        Total = null;
    }

    public ApiResponse(int statusCode, string message, TResponse? data, int total)
    {
        Status = statusCode;
        Message = message;
        Data = data;
        Total = total;
    }
}
