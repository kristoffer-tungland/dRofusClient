namespace dRofusClient.Exceptions;

public class dRofusClientCreateException(string message) : dRofusClientException(message);
public class dRofusClientLoginException(string message) : dRofusClientException(message);
public class dRofusClientException(string message) : Exception(message);