using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class OfficerObject
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Race")]
        public string Race { get; set; }

        [BsonElement(elementName: "Attack Strength")]
        public int AttackStrength { get; set; }

        [BsonElement(elementName: "Defence Strength")]
        public int DefenceStrength { get; set; }

        [BsonElement(elementName: "Health Strength")]
        public int HealthStrength { get; set; }

        [BsonElement(elementName: "Overall Strength")]
        public int OverallStrength { get; set; }

        [BsonElement(elementName: "Ship Specialty")]
        public string ShipSpecialty { get; set; }

        [BsonElement(elementName: "Home Planet System")]
        public string HomePlanetSystem { get; set; }

        [JsonConstructor]
        public OfficerObject(ObjectId i, string n, string s, int a, int b, int c, int o, string p, string h)
        {
            id = i;
            Name = n;
            Race = s;
            AttackStrength = a;
            DefenceStrength = b;
            HealthStrength = c;
            OverallStrength = o;
            ShipSpecialty = p;
            HomePlanetSystem = h;
        }

        public OfficerObject(ObjectId _id) {
            id = _id;
            Name = "";
            Race = "";
            AttackStrength = 0;
            DefenceStrength = 0;
            HealthStrength = 0;
            OverallStrength = 0;
            ShipSpecialty = "";
            HomePlanetSystem = "";
        }
    }
}
