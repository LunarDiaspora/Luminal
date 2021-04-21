using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Entities;
using Luminal.Entities.World;
using Luminal.Logging;
using Luminal.OpenGL;
using Luminal.OpenGL.Models;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Luminal.Player.Core
{
    public static class Reflect
    {
        public static Type GetFromEverywhere(string n)
        {
            var domain = AppDomain.CurrentDomain;
            var asms = domain.GetAssemblies();

            foreach (var asm in asms)
            {
                var t = asm.GetType(n);
                if (t != null)
                    return t;
            }

            return null;
        }
    }

    public class MaterialSerializer : JsonConverter<Material>
    {
        public override Material ReadJson(JsonReader reader, Type objectType, Material existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Material value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(value.Name);
            writer.WriteEndObject();
        }
    }

    public class Object3DSerializer : JsonConverter<Object3D>
    {
        public override Object3D ReadJson(JsonReader reader, Type objectType, Object3D existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Object3D value, JsonSerializer serializer)
        {
            if (value.Internal)
            {
                return;
            }

            writer.WriteStartObject();

            var s = JsonSerializer.CreateDefault(new()
            {
                Converters =
                {
                    new QuatSerializer()
                }
            });

            writer.WritePropertyName("Name");
            writer.WriteValue(value.Name);
            writer.WritePropertyName("Active");
            writer.WriteValue(value.Active);
            writer.WritePropertyName("Quat");
            s.Serialize(writer, value.Quat);
            writer.WritePropertyName("Position");
            s.Serialize(writer, value.Position);

            writer.WritePropertyName("components");
            s.Serialize(writer, value.components);

            writer.WriteEndObject();
        }
    }

    public class QuatSerializer : JsonConverter<Quaternion>
    {
        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var q = new Quaternion();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return q;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var v = reader.Value;
                    var t = reader.ReadAsDouble();
                    switch (v)
                    {
                        case "X":
                            q.X = (float)t;
                            break;
                        case "Y":
                            q.Y = (float)t;
                            break;
                        case "Z":
                            q.Z = (float)t;
                            break;
                        case "W":
                            q.W = (float)t;
                            break;
                    }
                }
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("X");
            writer.WriteValue(value.X);

            writer.WritePropertyName("Y");
            writer.WriteValue(value.Y);

            writer.WritePropertyName("Z");
            writer.WriteValue(value.Z);

            writer.WritePropertyName("W");
            writer.WriteValue(value.W);

            writer.WriteEndObject();
        }
    }

    public class TextureSerializer : JsonConverter<GLTexture>
    {
        public override GLTexture ReadJson(JsonReader reader, Type objectType, GLTexture existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, GLTexture value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Image");
            writer.WriteValue(value.Path);
            writer.WriteEndObject();
        }
    }

    public class ModelSerializer : JsonConverter<Model>
    {
        public override Model ReadJson(JsonReader reader, Type objectType, Model existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Path");
            writer.WriteValue(value.Path);

            writer.WriteEndObject();
        }
    }

    public static class SceneSerialiser
    {
        public static string Serialise()
        {
            var t = ECSScene.objects;
            var opt = new JsonSerializerSettings
            {
                Converters =
                {
                    new MaterialSerializer(),
                    new Object3DSerializer(),
                    new QuatSerializer(),
                    new TextureSerializer(),
                    new ModelSerializer()
                }
            };
            var str = JsonConvert.SerializeObject(t, opt);
            return str;
        }
    }
}
