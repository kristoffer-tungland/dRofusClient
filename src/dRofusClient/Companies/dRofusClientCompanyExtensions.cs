using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using dRofusClient.PropertyMeta;

namespace dRofusClient.Companies;

/// <summary>
/// Data transfer object for Company
/// </summary>
public record dRofusCompany : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Companies in dRofus API
/// </summary>
public static class dRofusClientCompanyExtensions
{
    /// <summary>
    /// Get list of Companies with the specified options
    /// </summary>
    public static Task<List<dRofusCompany>> GetCompaniesAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusCompany>(dRofusType.Companies.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Create new Company
    /// </summary>
    public static Task<dRofusCompany> CreateCompanyAsync(this IdRofusClient client, dRofusCompany company, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<dRofusCompany>(dRofusType.Companies.ToRequest(), company.ToPostOption(), cancellationToken);
    }

    /// <summary>
    /// Get specified Company by ID
    /// </summary>
    public static Task<dRofusCompany> GetCompanyAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusCompany>(dRofusType.Companies.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Update an existing Company
    /// </summary>
    public static Task<dRofusCompany> UpdateCompanyAsync(this IdRofusClient client, dRofusCompany company, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusCompany>(dRofusType.Companies.CombineToRequest(company.Id), company.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get meta information about available properties for Companies
    /// </summary>
    public static Task<List<dRofusPropertyMeta>> GetCompanyPropertyMetaAsync(this IdRofusClient client, 
        dRofusPropertyMetaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        return client.OptionsListAsync<dRofusPropertyMeta>(dRofusType.Companies.ToRequest(), options, cancellationToken);
    }
}