using System;
using System.Collections.Generic;
using static System.String;

namespace Marketplace.EventSourcing
{
    public static class TypeMapper
    {
        static readonly Dictionary<Type, string> NamesByType
            = new Dictionary<Type, string>();

        static readonly Dictionary<string, Type> TypesByName
            = new Dictionary<string, Type>();

        public static void Map(Type type, string name = null)
        {
            if (IsNullOrWhiteSpace(name))
                name = type.FullName;

            if (TypesByName.ContainsKey(name))
                throw new InvalidOperationException(
                    $"'{type}' is already mapped to the following name: {TypesByName[name]}"
                );

            TypesByName[name] = type;
            NamesByType[type] = name;
        }

        public static bool TryGetType(string name, out Type type) => TypesByName.TryGetValue(name, out type);

        public static bool TryGetTypeName(Type type, out string name) => NamesByType.TryGetValue(type, out name);

        public static void Map<T>(string name) => Map(typeof(T), name);

        public static string GetTypeName(Type type)
        {
            if (!TryGetTypeName(type, out var name))
                throw new Exception($"Failed to find name mapped with '{type}'");

            return name;
        }

        public static Type GetType(string name)
        {
            if (!TryGetType(name, out var type))
                throw new Exception($"Failed to find type mapped with '{name}'");

            return type;
        }
    }
}