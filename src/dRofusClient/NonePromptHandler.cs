namespace dRofusClient;

public class NonePromptHandler : ILoginPromptHandler
{
    public Task Handle(IdRofusClient client, CancellationToken cancellationToken)
    {
        // No prompt logic, just throw an exception
        throw new dRofusClientWrongCredentialsException();
    }
}
