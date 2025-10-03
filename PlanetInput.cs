using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class PlanetInput
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Indigenous Race")]
        public string Race { get; set; }

        [BsonElement(elementName: "Number of Planets")]
        public string NumPlanets { get; set; }

        [JsonConstructor]
        public PlanetInput()
        {
            id = ObjectId.GenerateNewId();
            Name = "";
            Race = "";
            NumPlanets = "";
        }

        public PlanetInput(PlanetObject _obj) {
            id = _obj.id;
            Name = _obj.Name;
            Race = _obj.Race;
            NumPlanets = _obj.NumPlanets.ToString();
        }
    }
}
