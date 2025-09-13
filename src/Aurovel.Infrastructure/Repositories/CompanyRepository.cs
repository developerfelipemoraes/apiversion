
using Aurovel.Domain.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aurovel.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly Mongo.MongoContext _ctx;
    public CompanyRepository(Mongo.MongoContext ctx) => _ctx = ctx;

    public async Task<Company> CreateAsync(Company entity)
    {
        await _ctx.Companies.InsertOneAsync(entity);
        return entity;
    }

    public async Task<(IEnumerable<Company> items, long total)> ListAsync(int page, int limit, string? status, int? kycMin, int? kycMax, string? search, string sortBy, string sortOrder)
    {
        var filter = Builders<Company>.Filter.Empty;
        var f = new List<FilterDefinition<Company>>();
        if (!string.IsNullOrWhiteSpace(status)) f.Add(Builders<Company>.Filter.Eq("profile.status", status));
        if (kycMin.HasValue) f.Add(Builders<Company>.Filter.Gte("profile.kycScore", kycMin.Value));
        if (kycMax.HasValue) f.Add(Builders<Company>.Filter.Lte("profile.kycScore", kycMax.Value));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new BsonRegularExpression(search, "i");
            f.Add(Builders<Company>.Filter.Or(
                Builders<Company>.Filter.Regex("basicInfo.legalName", regex),
                Builders<Company>.Filter.Regex("identification.cnpj", regex)
            ));
        }
        if (f.Count > 0) filter = Builders<Company>.Filter.And(f);

        var sort = sortOrder?.ToLowerInvariant() == "asc"
            ? Builders<Company>.Sort.Ascending(sortBy)
            : Builders<Company>.Sort.Descending(sortBy);

        var total = await _ctx.Companies.CountDocumentsAsync(filter);
        var items = await _ctx.Companies.Find(filter)
            .Sort(sort)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Company?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        return await _ctx.Companies.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Company?> GetByCnpjAsync(string cnpj)
    {
        return await _ctx.Companies.Find(Builders<Company>.Filter.Eq("identification.cnpj", cnpj)).FirstOrDefaultAsync();
    }

    public async Task<Company?> UpdateAsync(string id, Company update)
    {
        if (!ObjectId.TryParse(id, out _)) return null;

        update.UpdatedAt = DateTime.UtcNow;

        var res = await _ctx.Companies.FindOneAndReplaceAsync<Company, Company>(
            c => c.Id == id,
            update,
            new FindOneAndReplaceOptions<Company, Company>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: default
        );
        return res;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var res = await _ctx.Companies.DeleteOneAsync(x => x.Id == id);
        return res.DeletedCount > 0;
    }
}
