using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class MissionObject
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Rewards")]
        public string Rewards { get; set; }

        [BsonElement(elementName: "Description")]
        public string Description { get; set; }

        [BsonElement(elementName: "Location")]
        public string Location { get; set; }

        [JsonConstructor]
        public MissionObject(
            ObjectId i, string n, string s, string a, string l)
        {
            id = i;
            Name = n;
            Rewards = s;
            Description = a;
            Location = l;
        }

        public MissionObject() {
            id = ObjectId.GenerateNewId();
            Name = "";
            Rewards = "";
            Description = "";
            Location = "";
        }
    }
}
