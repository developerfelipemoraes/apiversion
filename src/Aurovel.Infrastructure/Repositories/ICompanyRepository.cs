
using Aurovel.Domain.Documents;

namespace Aurovel.Infrastructure.Repositories;

public interface ICompanyRepository
{
    Task<Company> CreateAsync(Company entity);
    Task<(IEnumerable<Company> items, long total)> ListAsync(int page, int limit, string? status, int? kycMin, int? kycMax, string? search, string sortBy, string sortOrder);
    Task<Company?> GetByIdAsync(string id);
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task<Company?> UpdateAsync(string id, Company update);
    Task<bool> DeleteAsync(string id);
}
