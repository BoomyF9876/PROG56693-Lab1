using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class CharacterObject
    {
        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "PrimaryAttack")]
        public string PrimaryAttack { get; set; }

        [BsonElement(elementName: "SecondaryAttack")]
        public string SecondaryAttack { get; set; }

        [BsonElement(elementName: "AttackStrength")]
        public double AttackStrength { get; set; }

        [BsonElement(elementName: "DefenseStrength")]
        public double DefenseStrength { get; set; }

        [BsonElement(elementName: "HealthStrength")]
        public double HealthStrength { get; set; }

        public CharacterObject(
            string n, string p, string s, double a, double d, double h)
        {
            Name = n;
            PrimaryAttack = p;
            SecondaryAttack = s;
            AttackStrength = a;
            DefenseStrength = d;
            HealthStrength = h;
        }

        [JsonConstructor]
        public CharacterObject() { }
    }
}
