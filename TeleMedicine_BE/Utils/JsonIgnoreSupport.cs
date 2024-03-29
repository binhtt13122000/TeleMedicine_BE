﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyRenameAndIgnoreSerializerContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public enum IgnoreMode
        {
            IGNORE,
            EXCEPT
        }

        public void EcxceptProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                _ignores[type].Add(prop.Name);
            }
            foreach (var prop in jsonPropertyNames)
            {
                _ignores[type].Remove(prop);
            }
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();
            PropertyInfo[] props = type.GetProperties();
            foreach (var prop in jsonPropertyNames)
            {
                _ignores[type].Add(prop);
            }
        }

        public string JsonIgnore<T>(Type type, string[] jsonPropertyNames, Paged<T> paged, IgnoreMode mode)
        {
            PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
            if(mode == IgnoreMode.EXCEPT)
            {
                jsonIgnore.EcxceptProperty(type, jsonPropertyNames);
            }else if(mode == IgnoreMode.IGNORE)
            {
                jsonIgnore.IgnoreProperty(type, jsonPropertyNames);
            }
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = jsonIgnore;

            var json = JsonConvert.SerializeObject(paged, serializerSettings);
            return json;
        }

        public string JsonIgnoreObject<T>(Type type, string[] jsonPropertyNames, T paged, IgnoreMode mode)
        {
            PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
            if (mode == IgnoreMode.EXCEPT)
            {
                jsonIgnore.EcxceptProperty(type, jsonPropertyNames);
            }
            else if (mode == IgnoreMode.IGNORE)
            {
                jsonIgnore.IgnoreProperty(type, jsonPropertyNames);
            }
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = jsonIgnore;

            var json = JsonConvert.SerializeObject(paged, serializerSettings);
            return json;
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }
            else
            {
                property.PropertyName = char.ToLowerInvariant(property.PropertyName[0]) + property.PropertyName[1..];
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
                property.PropertyName = newJsonPropertyName;

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
                return false;

            return _ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
}
