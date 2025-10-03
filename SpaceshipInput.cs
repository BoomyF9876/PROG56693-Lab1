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

        public SpaceshipInput()
        {
            id = ObjectId.GenerateNewId();
            Name = "";
            Class = "";
            SpecialAbility = "";
            Strength = "";
            WarpRange = "";
            WarpSpeed = "";
        }

        [JsonConstructor]
        public SpaceshipInput(SpaceshipObject _obj) {
            id = _obj.id;
            Name = _obj.Name;
            Class = _obj.Class;
            SpecialAbility = _obj.SpecialAbility;
            Strength = _obj.Strength.ToString();
            WarpRange = _obj.WarpRange.ToString();
            WarpSpeed = _obj.WarpSpeed.ToString();
        }
    }
}
