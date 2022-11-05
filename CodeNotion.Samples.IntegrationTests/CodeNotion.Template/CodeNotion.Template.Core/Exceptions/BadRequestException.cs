using System;
using CodeNotion.Template.Business.Enums;

namespace CodeNotion.Template.Business.Exceptions;

public class BadRequestException : Exception
{
    public readonly ErrorType ErrorType;
    public new readonly object? Data;

    public BadRequestException(ErrorType errorType, string? message = null, object? data = null) : base(message)
    {
        ErrorType = errorType;
        Data = data;
    }
}