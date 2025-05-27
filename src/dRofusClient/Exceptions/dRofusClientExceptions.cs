namespace dRofusClient.Exceptions;

public class dRofusClientWrongCredentialsException() : dRofusClientLoginException("Wrong credentials provided.");

public class dRofusClientModernLoginException(string message) : dRofusClientLoginException(message)
{
    public required string ErrorDescription { get; init; }
}

public class dRofusClientCreateException(string message) : dRofusClientException(message);
public class dRofusClientLoginException(string message) : dRofusClientException(message);
public class dRofusClientException(string message) : Exception(message);