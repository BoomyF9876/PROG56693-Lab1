using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class PlanetObject
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Indigenous Race")]
        public string Race { get; set; }

        [BsonElement(elementName: "Number of Planets")]
        public int NumPlanets { get; set; }

        [JsonConstructor]
        public PlanetObject(
            ObjectId i, string n, string s, int a)
        {
            id = i;
            Name = n;
            Race = s;
            NumPlanets = a;
        }

        public PlanetObject(ObjectId _id) {
            id = _id;
            Name = "";
            Race = "";
            NumPlanets = 0;
        }
    }
}
