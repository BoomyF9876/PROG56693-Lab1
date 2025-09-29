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
    internal class SpaceshipObject
    {
        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Class")]
        public string Class { get; set; }

        [BsonElement(elementName: "Special Ability")]
        public string SpecialAbility { get; set; }

        [BsonElement(elementName: "Strength")]
        public float Strength { get; set; }

        [BsonElement(elementName: "Warp Range")]
        public int WarpRange { get; set; }

        [BsonElement(elementName: "Warp Speed")]
        public float WarpSpeed { get; set; }

        public SpaceshipObject(
            string n, string p, string s, float a, int d, float h)
        {
            Name = n;
            Class = p;
            SpecialAbility = s;
            Strength = a;
            WarpRange = d;
            WarpSpeed = h;
        }

        [JsonConstructor]
        public SpaceshipObject() { }
    }
}
