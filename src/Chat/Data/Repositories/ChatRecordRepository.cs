using MongoDB.Driver;
using AhoyChat.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace AhoyChat.Data.Repositories;

public class ChatRecordRepository
{
    private readonly IMongoCollection<ChatRecord> _chatRecords;

    public ChatRecordRepository(IConfiguration configuration)
    {
        var pack = new ConventionPack();
        pack.Add(new CamelCaseElementNameConvention());
        ConventionRegistry.Register("CamelCase", pack, t => true);

        var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
        var database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
        _chatRecords = database.GetCollection<ChatRecord>(configuration["MongoDb:CollectionName"]);
    }

    public async Task AddNewOutgoingMessage(ChatRecord chatRecord)
    {
        var filter = Builders<ChatRecord>.Filter.And(
            Builders<ChatRecord>.Filter.Eq("userId", chatRecord.UserId),
            Builders<ChatRecord>.Filter.Eq("customer.contact", chatRecord.Customer.Contact)
        );

        var update = Builders<ChatRecord>.Update.Push("messageHistory", chatRecord.MessageHistory[0]);

        var options = new FindOneAndUpdateOptions<ChatRecord> { IsUpsert = true };

        await _chatRecords.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task AddNewIncomingMessage(ChatRecord chatRecord)
    {
        var filter = Builders<ChatRecord>.Filter.And(
            Builders<ChatRecord>.Filter.Eq("userId", chatRecord.UserId),
            Builders<ChatRecord>.Filter.Eq("customer.contact", chatRecord.Customer.Contact)
        );

        var update = Builders<ChatRecord>.Update
            .Set("userId", chatRecord.UserId)
            .Set("customer", new BsonDocument
            {
                { "name", chatRecord.Customer.Name },
                { "contact", chatRecord.Customer.Contact },
                { "profilePicUrl", chatRecord.Customer.ProfilePicUrl }
            })
            .Push("messageHistory", chatRecord.MessageHistory[0]);

        var options = new FindOneAndUpdateOptions<ChatRecord> { IsUpsert = true };

        await _chatRecords.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task<List<ChatRecord>> GetMessagesFromUser(Guid userId)
    {
        var filter = Builders<ChatRecord>.Filter.And(
            Builders<ChatRecord>.Filter.Eq("userId", userId.ToString())
        );

        return await (_chatRecords.FindAsync(filter).Result.ToListAsync());
    }
}
