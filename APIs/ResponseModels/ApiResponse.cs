using ManageUsers.Application.DTOs;
using System;
using System.Net;

public class APIResponse<T>
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public List<string> ErrorMessage { get; set; } = new();

    public bool IsSuccess { get; set; } = true;

    public Result<T> Result { get; set; } = new();
}

public class Result<T>
{
    public T Data { get; set; }
}

public class APIResponseList<T>
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public List<string> ErrorMessage { get; set; } = new();

    public bool IsSuccess { get; set; } = true;

    public ResultList<T> Result { get; set; } = new();
}

public class ResultList<T>
{
    public T Data { get; set; }
    public PaginationDTO pagination { get; set; } = new();
}