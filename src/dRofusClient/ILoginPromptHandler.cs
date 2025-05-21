namespace dRofusClient;

public interface ILoginPromptHandler
{
    Task Handle(IdRofusClient client, CancellationToken cancellationToken);
}