
using Aurovel.Domain.Documents;

namespace Aurovel.Infrastructure.Repositories;

public interface ICompanyRepository
{
    Task<CompanyDataDocument> CreateAsync(CompanyDataDocument entity);
    Task<(IEnumerable<CompanyDataDocument> items, long total)> ListAsync(int page, int limit, string? status, int? kycMin, int? kycMax, string? search, string sortBy, string sortOrder);
    Task<CompanyDataDocument?> GetByIdAsync(string id);
    Task<CompanyDataDocument?> GetByCnpjAsync(string cnpj);
    Task<CompanyDataDocument?> UpdateAsync(string id, CompanyDataDocument update);
    Task<bool> DeleteAsync(string id);
}
