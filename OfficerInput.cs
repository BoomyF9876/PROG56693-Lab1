using System.Security.Claims;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class OfficerInput
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Race")]
        public string Race { get; set; }

        [BsonElement(elementName: "Attack Strength")]
        public string AttackStrength { get; set; }

        [BsonElement(elementName: "Defence Strength")]
        public string DefenceStrength { get; set; }

        [BsonElement(elementName: "Health Strength")]
        public string HealthStrength { get; set; }

        [BsonElement(elementName: "Overall Strength")]
        public string OverallStrength { get; set; }

        [BsonElement(elementName: "Ship Specialty")]
        public string ShipSpecialty { get; set; }

        [BsonElement(elementName: "Home Planet System")]
        public string HomePlanetSystem { get; set; }

        [JsonConstructor]
        public OfficerInput()
        {
            id = ObjectId.GenerateNewId();
            Name = "";
            Race = "";
            AttackStrength = "";
            DefenceStrength = "";
            HealthStrength = "";
            OverallStrength = "";
            ShipSpecialty = "";
            HomePlanetSystem = "";
        }
        public OfficerInput(OfficerObject _obj)
        {
            id = _obj.id;
            Name = _obj.Name;
            Race = _obj.Race;
            AttackStrength = _obj.AttackStrength.ToString();
            DefenceStrength = _obj.DefenceStrength.ToString();
            HealthStrength = _obj.HealthStrength.ToString();
            OverallStrength = _obj.OverallStrength.ToString();
            ShipSpecialty = _obj.ShipSpecialty;
            HomePlanetSystem = _obj.HomePlanetSystem;
        }
    }
}
