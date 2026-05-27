using System;

namespace Application;


public class ErrorDto
{
    public int StatusCode { get; init;}
    public string? Message { get; init;}
    public string? Details { get; init;}
    public DateTime TimeStamp { get; init;}

    public ErrorDto(int statuscode, string? message, string? details, DateTime timestamp)
    {
        StatusCode = statuscode;
        Message = message;
        Details = details;
        TimeStamp = timestamp;

    }



}


