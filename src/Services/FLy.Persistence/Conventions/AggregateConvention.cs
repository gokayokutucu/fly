using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fly.Persistence.Conventions.Serializers;

namespace Fly.Persistence.Conventions
{
    public class AggregationConvention : ConventionBase, IClassMapConvention
    {
        private readonly BindingFlags _bindingFlags;

        public AggregationConvention() : this(BindingFlags.Instance | BindingFlags.Public) { }

        public AggregationConvention(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
        }

        public void Apply(BsonClassMap classMap)
        {
            if (classMap.ClassType.IsAbstract)
                return;

            var allProps = classMap
                .ClassType
                .GetTypeInfo()
                .GetProperties(_bindingFlags)
                .Where(p => IsEntityProperty(classMap, p))
                .ToList();

            foreach (var prop in allProps)
            {
                if (IsListProperty(prop))
                {
                    var infc = prop.PropertyType.GenericTypeArguments.Single();
                    Type generic = typeof(CollectionSerializer<>);

                    // Create an array of types to substitute for the type parameters of Dictionary. The key is of type string, and the type to be contained in the Dictionary is Test.
                    Type[] typeArgs = { infc };

                    // Create a Type object representing the constructed generic type.
                    Type constructed = generic.MakeGenericType(typeArgs);

                    var instance = Activator.CreateInstance(constructed);
                    classMap.MapProperty(prop.Name).SetSerializer((IBsonSerializer)instance);
                }
                classMap.MapProperty(prop.Name);
            }
        }

        private bool IsEntityProperty(BsonClassMap classMap, PropertyInfo propertyInfo)
        {
            var baseType = classMap.ClassType.BaseType;
            if (baseType == null) return true;

            if (baseType.GetTypeInfo().GetProperties().Any(p => p.Name == propertyInfo.Name))
                return false;
            return true;
        }

        private bool IsListProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsGenericType) return false;
            var type = propertyInfo.PropertyType.GenericTypeArguments.Single();
            if (type.IsPrimitive || type == typeof(Decimal) || type == typeof(String) || type == typeof(DateTime))
                return false;
            return propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}