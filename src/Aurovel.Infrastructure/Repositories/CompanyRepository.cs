
using Aurovel.Domain.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aurovel.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly Mongo.MongoContext _ctx;
    public CompanyRepository(Mongo.MongoContext ctx) => _ctx = ctx;

    public async Task<CompanyDataDocument> CreateAsync(CompanyDataDocument entity)
    {
        await _ctx.Companies.InsertOneAsync(entity);
        return entity;
    }

    public async Task<(IEnumerable<CompanyDataDocument> items, long total)> ListAsync(int page, int limit, string? status, int? kycMin, int? kycMax, string? search, string sortBy, string sortOrder)
    {
        var filter = Builders<CompanyDataDocument>.Filter.Empty;
        var f = new List<FilterDefinition<CompanyDataDocument>>();
        if (!string.IsNullOrWhiteSpace(status)) f.Add(Builders<CompanyDataDocument>.Filter.Eq("profile.status", status));
        if (kycMin.HasValue) f.Add(Builders<CompanyDataDocument>.Filter.Gte("profile.kycScore", kycMin.Value));
        if (kycMax.HasValue) f.Add(Builders<CompanyDataDocument>.Filter.Lte("profile.kycScore", kycMax.Value));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new BsonRegularExpression(search, "i");
            f.Add(Builders<CompanyDataDocument>.Filter.Or(
                Builders<CompanyDataDocument>.Filter.Regex("basicInfo.legalName", regex),
                Builders<CompanyDataDocument>.Filter.Regex("identification.cnpj", regex)
            ));
        }
        if (f.Count > 0) filter = Builders<CompanyDataDocument>.Filter.And(f);

        var sort = sortOrder?.ToLowerInvariant() == "asc"
            ? Builders<CompanyDataDocument>.Sort.Ascending(sortBy)
            : Builders<CompanyDataDocument>.Sort.Descending(sortBy);

        var total = await _ctx.Companies.CountDocumentsAsync(filter);
        var items = await _ctx.Companies.Find(filter)
            .Sort(sort)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<CompanyDataDocument?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        return await _ctx.Companies.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<CompanyDataDocument?> GetByCnpjAsync(string cnpj)
    {
        return await _ctx.Companies.Find(Builders<CompanyDataDocument>.Filter.Eq("identification.cnpj", cnpj)).FirstOrDefaultAsync();
    }

    public async Task<CompanyDataDocument?> UpdateAsync(string id, CompanyDataDocument update)
    {
        if (!ObjectId.TryParse(id, out _)) return null;

        update.UpdatedAt = DateTime.UtcNow;

        var res = await _ctx.Companies.FindOneAndReplaceAsync<CompanyDataDocument, CompanyDataDocument>(
            c => c.Id == id,
            update,
            new FindOneAndReplaceOptions<CompanyDataDocument, CompanyDataDocument>
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
