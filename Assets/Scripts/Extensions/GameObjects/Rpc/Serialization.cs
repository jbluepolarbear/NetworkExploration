using System;
using Unity.Netcode;

namespace Extensions.GameObjects.Rpc
{
    public enum SerializationTypes
    {
        Int,
        UInt,
        Bool,
        Float,
        Double,
        String,
    }

    public static class Serialization
    {
        public static void SerializeField<T>(object field, BufferSerializer<T> bufferSerializer) where T : IReaderWriter
        {
            var writer = bufferSerializer.GetFastBufferWriter();
            switch (field)
            {
                case int intField:
                    writer.WriteValueSafe(SerializationTypes.Int);
                    writer.WriteValueSafe(intField);
                    break;
                case uint uintField:
                    writer.WriteValueSafe(SerializationTypes.UInt);
                    writer.WriteValueSafe(uintField);
                    break;
                case bool boolField:
                    writer.WriteValueSafe(SerializationTypes.Bool);
                    writer.WriteValueSafe(boolField);
                    break;
                case float floatField:
                    writer.WriteValueSafe(SerializationTypes.Float);
                    writer.WriteValueSafe(floatField);
                    break;
                case double doubleField:
                    writer.WriteValueSafe(SerializationTypes.Double);
                    writer.WriteValueSafe(doubleField);
                    break;
                case string stringField:
                    writer.WriteValueSafe(SerializationTypes.String);
                    writer.WriteValueSafe(stringField);
                    break;
                default:
                    throw new ArgumentException($"Field of type {field.GetType()} is not supported.");
            }
        }

        public static object DeserializeField<T>(BufferSerializer<T> bufferSerializer)
            where T : IReaderWriter
        {
            var reader = bufferSerializer.GetFastBufferReader();
            reader.ReadValueSafe(out SerializationTypes serializationType);
            switch (serializationType)
            {
                case SerializationTypes.Int:
                    reader.ReadValueSafe(out int intField);
                    return intField;
                case SerializationTypes.UInt:
                    reader.ReadValueSafe(out uint uintField);
                    return uintField;
                case SerializationTypes.Bool:
                    reader.ReadValueSafe(out bool boolField);
                    return boolField;
                case SerializationTypes.Float:
                    reader.ReadValueSafe(out float floatField);
                    return floatField;
                case SerializationTypes.Double:
                    reader.ReadValueSafe(out double doubleField);
                    return doubleField;
                case SerializationTypes.String:
                    reader.ReadValueSafe(out string stringField);
                    return stringField;
                default:
                    throw new ArgumentException($"Field of type {serializationType} is not supported.");
            }
        }
    }
}