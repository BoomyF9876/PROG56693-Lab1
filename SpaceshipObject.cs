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

enum ShipSpecialty
{
    Battleship,
    Explorer,
    Interceptor,
    Miner
}

namespace W2CharacterEditor
{
    [BsonIgnoreExtraElements]
    internal class SpaceshipObject
    {
        [BsonElement(elementName: "_id")]
        public ObjectId id { get; set; }

        [BsonElement(elementName: "Name")]
        public string Name { get; set; }

        [BsonElement(elementName: "Class")]
        public ShipSpecialty Class { get; set; }

        [BsonElement(elementName: "Special Ability")]
        public string SpecialAbility { get; set; }

        [BsonElement(elementName: "Strength")]
        public int Strength { get; set; }

        [BsonElement(elementName: "Warp Range")]
        public int WarpRange { get; set; }

        [BsonElement(elementName: "Warp Speed")]
        public decimal WarpSpeed { get; set; }

        [JsonConstructor]
        public SpaceshipObject(
            ObjectId i, string n, ShipSpecialty p, string s, int a, int d, decimal h)
        {
            id = i;
            Name = n;
            Class = p;
            SpecialAbility = s;
            Strength = a;
            WarpRange = d;
            WarpSpeed = h;
        }

        public SpaceshipObject(ObjectId _id) {
            id = _id;
            Name = "";
            SpecialAbility = "";
            Strength = 0;
            WarpRange = 0;
            WarpSpeed = 0;
        }
    }
}
