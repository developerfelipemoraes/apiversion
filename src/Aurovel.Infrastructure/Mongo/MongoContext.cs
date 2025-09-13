
using Aurovel.Domain.Documents;
using MongoDB.Driver;

namespace Aurovel.Infrastructure.Mongo;

public class MongoContext
{
    public IMongoDatabase Db { get; }
    public IMongoCollection<Contact> Contacts => Db.GetCollection<Contact>("contacts");
    public IMongoCollection<Company> Companies => Db.GetCollection<Company>("companies");

    public MongoContext(IMongoDatabase db) => Db = db;
}
