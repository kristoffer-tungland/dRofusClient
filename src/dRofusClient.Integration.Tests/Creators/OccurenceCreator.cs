using dRofusClient.Occurrences;
using dRofusClient.Items;

namespace dRofusClient.Integration.Tests.Creators;

internal class OccurenceCreator(IdRofusClient client) : IAsyncDisposable
{
    public Occurence? Occurence { get; private set; }

    public async Task<Occurence> InitializeAsync(Item item)
    {
        var occurenceToCreate = CreateOccurence.Of(item);
        Occurence = await client.CreateOccurrenceAsync(occurenceToCreate);
        return Occurence;
    }

    public async ValueTask DisposeAsync()
    {
        if (Occurence is null)
            throw new InvalidOperationException("Occurrence has not been created or initialized.");

        await client.DeleteOccurrenceAsync(Occurence.GetId());
    }
}

public class OccurenceFixture : ItemFixture
{
    private OccurenceCreator? _occurenceCreator;
    private bool _isDisposed;

    public Occurence Occurence => _occurenceCreator?.Occurence ?? throw new InvalidOperationException("Occurence has not been created or initialized.");

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _occurenceCreator = new OccurenceCreator(Client);
        await _occurenceCreator.InitializeAsync(Item);
    }

    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        try
        {
            if (_occurenceCreator is null)
                throw new InvalidOperationException("OccurenceCreator has not been initialized.");

            await _occurenceCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}

[CollectionDefinition(nameof(OccurenceTestCollection))]
public class OccurenceTestCollection : ICollectionFixture<OccurenceFixture> { }