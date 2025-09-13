
using Aurovel.Domain.Documents;

namespace Aurovel.Infrastructure.Repositories;

public interface IContactRepository
{
    Task<Contact> CreateAsync(Contact entity);
    Task<(IEnumerable<Contact> items, long total)> ListAsync(
        int page, int limit, string? status, string? riskLevel, int? kycMin, int? kycMax,
        string? search, string sortBy, string sortOrder);
    Task<Contact?> GetByIdAsync(string id);
    Task<Contact?> GetByCpfAsync(string cpf);
    Task<Contact?> UpdateAsync(string id, Contact update);
    Task<bool> DeleteAsync(string id);
    Task<Contact?> LinkToCompanyAsync(string contactId, string companyId, string role);
    Task<Contact?> UnlinkFromCompanyAsync(string contactId, string companyId);
}
