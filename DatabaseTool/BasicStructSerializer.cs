using System.Collections.Generic;
using System.Reflection;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DatabaseTool
{
    // https://stackoverflow.com/questions/26788855/how-do-you-serialize-value-types-with-mongodb-c-sharp-serializer

    public class BasicStructSerializer<T> : SerializerBase<T> where T: struct
    {
        public override void Serialize(
            BsonSerializationContext context,
            BsonSerializationArgs args, T value)
        {
            var nominalType = args.NominalType;

            var fields = nominalType.GetFields(
                BindingFlags.Instance | BindingFlags.Public);
            var propsAll = nominalType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public);

            var props = new List<PropertyInfo>();
            foreach (var prop in propsAll)
            {
                if (prop.CanWrite)
                {
                    props.Add(prop);
                }
            }

            var bsonWriter = context.Writer;

            bsonWriter.WriteStartDocument();

            foreach (var field in fields)
            {
                bsonWriter.WriteName(field.Name);
                BsonSerializer.Serialize(
                    bsonWriter, field.FieldType, field.GetValue(value));
            }
            foreach (var prop in props)
            {
                bsonWriter.WriteName(prop.Name);
                BsonSerializer.Serialize(
                    bsonWriter, prop.PropertyType, prop.GetValue(value, null));
            }

            bsonWriter.WriteEndDocument();
        }

        public override T Deserialize(
            BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            //boxing is required for SetValue to work
            var obj = (object)(new T());
            var actualType = args.NominalType;
            var bsonReader = context.Reader;

            bsonReader.ReadStartDocument();

            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var name = bsonReader.ReadName();

                var field = actualType.GetField(name);
                if (field != null)
                {
                    var value = BsonSerializer.Deserialize(
                        bsonReader, field.FieldType);
                    field.SetValue(obj, value);
                }

                var prop = actualType.GetProperty(name);
                if (prop != null)
                {
                    var value = BsonSerializer.Deserialize(
                        bsonReader, prop.PropertyType);
                    prop.SetValue(obj, value, null);
                }
            }

            bsonReader.ReadEndDocument();

            return (T)obj;
        }
    }
}
