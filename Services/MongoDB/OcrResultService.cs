using MongoDB.Bson;
using MongoDB.Driver;

namespace ViFoodAPI.Services.MongoDB;

public class OcrResultService
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public OcrResultService(IMongoDatabase database)
    {
        _collection = database.GetCollection<BsonDocument>("Product");
    }

    public async Task<List<BsonDocument>> GetAllProductsAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
    public async Task<BsonDocument?> GetProductByIdAsync(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
