using AhoyContracts.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AhoyChat.Entities;

public record Message(string Id, DateTime Date, string Type, Content Content);

public class ChatRecord
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public string UserId { get; set; }

    public Customer Customer { get; set; }

    public List<Message> MessageHistory { get; set; }
}