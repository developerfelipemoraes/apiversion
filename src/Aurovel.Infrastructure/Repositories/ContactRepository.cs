
using Aurovel.Domain.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aurovel.Infrastructure.Repositories;

public class ContactRepository(Mongo.MongoContext ctx) : IContactRepository
{
    public async Task<Contact> CreateAsync(Contact entity)
    {
        await ctx.Contacts.InsertOneAsync(entity);

        return entity;
    }

    public async Task<(IEnumerable<Contact> items, long total)> ListAsync(
        int page, int limit, string? status, string? riskLevel, int? kycMin, int? kycMax,
        string? search, string sortBy, string sortOrder)
    {
        var filter = Builders<Contact>.Filter.Empty;
        var f = new List<FilterDefinition<Contact>>();
        
        if (!string.IsNullOrWhiteSpace(status)) 
            f.Add(Builders<Contact>.Filter.Eq("profile.status", status));
        
        if (!string.IsNullOrWhiteSpace(riskLevel)) 
            f.Add(Builders<Contact>.Filter.Eq("profile.riskLevel", riskLevel));
        
        if (kycMin.HasValue) 
            f.Add(Builders<Contact>.Filter.Gte("profile.kycScore", kycMin.Value));
        
        if (kycMax.HasValue) 
            f.Add(Builders<Contact>.Filter.Lte("profile.kycScore", kycMax.Value));
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new BsonRegularExpression(search, "i");
            f.Add(Builders<Contact>.Filter.Or(
                Builders<Contact>.Filter.Regex("personalInfo.fullName", regex),
                Builders<Contact>.Filter.Regex("personalInfo.cpf", regex)
            ));
        }
        
        if (f.Count > 0) filter = Builders<Contact>.Filter.And(f);

        var sort = sortOrder?.ToLowerInvariant() == "asc"
            ? Builders<Contact>.Sort.Ascending(sortBy)
            : Builders<Contact>.Sort.Descending(sortBy);

        var total = await ctx.Contacts.CountDocumentsAsync(filter);
        var items = await ctx.Contacts.Find(filter)
            .Sort(sort)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Contact?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        return await ctx.Contacts.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Contact?> GetByCpfAsync(string cpf)
    {
        return await ctx.Contacts
            .Find(Builders<Contact>.Filter.Eq("personalInfo.cpf", cpf))
            .FirstOrDefaultAsync();
    }

    public async Task<Contact?> UpdateAsync(string id, Contact update)
    {
        if (!ObjectId.TryParse(id, out _)) return null;

        update.UpdatedAt = DateTime.UtcNow;

        var res = await ctx.Contacts.FindOneAndReplaceAsync<Contact, Contact>(
            c => c.Id == id,
            update,
            new FindOneAndReplaceOptions<Contact, Contact>
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

        var res = await ctx.Contacts.DeleteOneAsync(x => x.Id == id);

        return res.DeletedCount > 0;
    }

    public async Task<Contact?> LinkToCompanyAsync(string contactId, string companyId, string role)
    {
        if (!ObjectId.TryParse(contactId, out _) || !ObjectId.TryParse(companyId, out _)) return null;

        var update = Builders<Contact>.Update.AddToSet("linkedCompanies", new BsonDocument{
            { "companyId", new ObjectId(companyId) },
            { "role", role },
            { "addedAt", DateTime.UtcNow }
        });
        
        return await ctx.Contacts.
            FindOneAndUpdateAsync<Contact>(x => x.Id == contactId, update, 
            new FindOneAndUpdateOptions<Contact>{ ReturnDocument = ReturnDocument.After });
    }

    public async Task<Contact?> UnlinkFromCompanyAsync(string contactId, string companyId)
    {
        if (!ObjectId.TryParse(contactId, out _) || !ObjectId.TryParse(companyId, out _)) return null;
        
        var pull = Builders<Contact>.Update
            .PullFilter("linkedCompanies", Builders<BsonDocument>
            .Filter.Eq("companyId", new ObjectId(companyId)));

        return await ctx.Contacts.
            FindOneAndUpdateAsync<Contact>(x => x.Id == contactId, pull, 
            new FindOneAndUpdateOptions<Contact>{ 
                ReturnDocument = ReturnDocument.After 
            });
    }
}
