using System;
using System.Collections;
using System.Collections.Generic;
using Game.Inventory;
using Unity.Netcode;

namespace Extensions.GameObjects.Rpc
{
    public enum SerializationTypes
    {
        Enum,
        Int,
        UInt,
        Long,
        ULong,
        Bool,
        Float,
        Double,
        String,
        InventoryItemStack,
        List,
    }

    public static class Serialization
    {
        public static SerializationTypes GetEnumFromType(Type type)
        {
            return type switch
            {
                not null when type.IsEnum => SerializationTypes.Enum,
                not null when type == typeof(int) => SerializationTypes.Int,
                not null when type == typeof(uint) => SerializationTypes.UInt,
                not null when type == typeof(long) => SerializationTypes.Long,
                not null when type == typeof(ulong) => SerializationTypes.ULong,
                not null when type == typeof(bool) => SerializationTypes.Bool,
                not null when type == typeof(float) => SerializationTypes.Float,
                not null when type == typeof(double) => SerializationTypes.Double,
                not null when type == typeof(string) => SerializationTypes.String,
                not null when type == typeof(InventoryItemStack) => SerializationTypes.InventoryItemStack,
                not null when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) => SerializationTypes.List,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public static Type GetTypeFromEnum(SerializationTypes serializationType)
        {
            return serializationType switch
            {
                SerializationTypes.Enum => typeof(Enum),
                SerializationTypes.Int => typeof(int),
                SerializationTypes.UInt => typeof(uint),
                SerializationTypes.Long => typeof(long),
                SerializationTypes.ULong => typeof(ulong),
                SerializationTypes.Bool => typeof(bool),
                SerializationTypes.Float => typeof(float),
                SerializationTypes.Double => typeof(double),
                SerializationTypes.String => typeof(string),
                SerializationTypes.InventoryItemStack => typeof(InventoryItemStack),
                SerializationTypes.List => typeof(IList),
                _ => throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null)
            };
        }
        public static void SerializeField<T>(object field, BufferSerializer<T> bufferSerializer) where T : IReaderWriter
        {
            var writer = bufferSerializer.GetFastBufferWriter();
            switch (field)
            {
                case Enum enumField:
                    writer.WriteValueSafe(SerializationTypes.Enum);
                    writer.WriteValueSafe(Convert.ToInt32(enumField));
                    break;
                case int intField:
                    writer.WriteValueSafe(SerializationTypes.Int);
                    writer.WriteValueSafe(intField);
                    break;
                case uint uintField:
                    writer.WriteValueSafe(SerializationTypes.UInt);
                    writer.WriteValueSafe(uintField);
                    break;
                case long longField:
                    writer.WriteValueSafe(SerializationTypes.UInt);
                    writer.WriteValueSafe(longField);
                    break;
                case ulong ulongField:
                    writer.WriteValueSafe(SerializationTypes.UInt);
                    writer.WriteValueSafe(ulongField);
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
                case InventoryItemStack inventoryItemStack:
                    writer.WriteValueSafe(SerializationTypes.InventoryItemStack);
                    writer.WriteValueSafe(inventoryItemStack.ItemId);
                    writer.WriteValueSafe(inventoryItemStack.InventoryId);
                    writer.WriteValueSafe(inventoryItemStack.Quantity);
                    break;
                case IList list:
                    writer.WriteValueSafe(SerializationTypes.List);
                    writer.WriteValueSafe(GetEnumFromType(list.GetType().GetGenericArguments()[0]));
                    writer.WriteValueSafe(list.Count);
                    foreach (var item in list)
                    {
                        SerializeField(item, bufferSerializer);
                    }
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
                case SerializationTypes.Enum:
                    reader.ReadValueSafe(out int enumValue);
                    return enumValue;
                case SerializationTypes.Int:
                    reader.ReadValueSafe(out int intField);
                    return intField;
                case SerializationTypes.UInt:
                    reader.ReadValueSafe(out uint uintField);
                    return uintField;
                case SerializationTypes.Long:
                    reader.ReadValueSafe(out long longField);
                    return longField;
                case SerializationTypes.ULong:
                    reader.ReadValueSafe(out ulong ulongField);
                    return ulongField;
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
                case SerializationTypes.InventoryItemStack:
                    reader.ReadValueSafe(out int itemId);
                    reader.ReadValueSafe(out int inventoryId);
                    reader.ReadValueSafe(out int quantity);
                    return new InventoryItemStack
                    {
                        ItemId = itemId,
                        InventoryId = inventoryId,
                        Quantity = quantity
                    };
                case SerializationTypes.List:
                    reader.ReadValueSafe(out SerializationTypes serializationTypeElement);
                    reader.ReadValueSafe(out int count);
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(GetTypeFromEnum(serializationTypeElement));

                    var list = (IList) Activator.CreateInstance(constructedListType);
                    for (var i = 0; i < count; i++)
                    {
                        list.Add(DeserializeField(bufferSerializer));
                    }
                    return list;
                default:
                    throw new ArgumentException($"Field of type {serializationType} is not supported.");
            }
        }
    }
}