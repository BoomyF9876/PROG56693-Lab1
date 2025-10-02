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
    internal class SpaceshipInput
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Class")]
        public string Class { get; set; }

        [BsonElement(elementName: "Special Ability")]
        public string SpecialAbility { get; set; }

        [BsonElement(elementName: "Strength")]
        public string Strength { get; set; }

        [BsonElement(elementName: "Warp Range")]
        public string WarpRange { get; set; }

        [BsonElement(elementName: "Warp Speed")]
        public string WarpSpeed { get; set; }

        public SpaceshipInput(
            ObjectId i, string n, string p, string s, string a, string d, string h)
        {
            id = i;
            Name = n;
            Class = p;
            SpecialAbility = s;
            Strength = a;
            WarpRange = d;
            WarpSpeed = h;
        }

        [JsonConstructor]
        public SpaceshipInput() {
            id = new ObjectId();
        }
    }
}
