﻿namespace MediaService.Utils.Exceptions;

public class InternalServerErrorException : Exception
{
    public InternalServerErrorException(string message) : base(message) { }
    public InternalServerErrorException(string message, Exception innerException) : base(message, innerException) { }
}
