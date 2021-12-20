using Fly.Common.Extensions;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;

namespace Fly.Common
{
    /// <summary>
    /// Author: Gokay Okutucu
    /// </summary>
    public class DynamicType : DynamicObject
    {
        private object m_object;
        public AssemblyBuilder Assembly { get; private set; }
        public ModuleBuilder Module { get; private set; }
        public TypeBuilder Type { get; private set; }

        public object Holder => m_object;

        public static DynamicType Make(string name)
        {
            var dynamicType = new DynamicType()
            {
                Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Passbridge.Dynamic"), AssemblyBuilderAccess.Run)
            };

            dynamicType.Module = dynamicType.Assembly.DefineDynamicModule("Messaging");
            dynamicType.Type = dynamicType.Module.DefineType(name, TypeAttributes.Public);

            return dynamicType;
        }

        public override bool Equals(object obj)
        {
            // If this and obj do not refer to the same type, then they are not equal.
            if (obj != null && obj.GetType() != this.GetType()) return false;

            // Return true if  x and y fields match.
            var other = (DynamicType)obj;
            return other != null && (this.Assembly == other.Assembly) && (this.Module == other.Module) && (this.Type == other.Type);
        }

        // Return the XOR of the x and y fields.
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        // Return the point's value as a string.
        public override string ToString()
        {
            return Type?.FullName;
        }

        // Return a copy of this point object by making a simple field copy.
        public DynamicType Copy()
        {
            return (DynamicType)this.MemberwiseClone();
        }

        public DynamicType CastTo(object value)
        {
            //string str = JsonConvert.SerializeObject(value);
            //m_object = JsonConvert.DeserializeObject(str);
            m_object = value;
            return this;
        }

        public int IntProperty { get; set; }

        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;

            return _dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys;
        }
    }
    public static class DynamicTypeExtension
    {
        public static Object Create(this DynamicType @dynamicType)
        {
            return @dynamicType.Holder;
        }
        public static DynamicType ExtendWith(this DynamicType @dynamicType, Type source)
        {
            try
            {
                var properties = new List<PropertyInfo>();
                if (source is { })
                {
                    foreach (var @interface in source.GetInterfaces())
                    {
                        //type.AddInterfaceImplementation(@interface);
                        properties.AddRange(@interface.GetProperties());
                    }

                    @dynamicType.Type.AddInterfaceImplementation(source);
                    properties.AddRange(source.GetProperties());
                }

                AddProps(properties.Distinct(), @dynamicType.Type);

                return @dynamicType.CastTo(Activator.CreateInstance(@dynamicType.Type.CreateType()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public static DynamicType ExtendWith(this DynamicType @dynamicType, Type source, Type shrink)
        {
            try
            {
                var properties = new List<PropertyInfo>();
                if (source is { })
                {
                    foreach (var @interface in source.GetInterfaces().Where(t => t != shrink))
                    {
                        properties.AddRange(@interface.GetProperties());
                    }

                    properties.AddRange(source.GetProperties());
                }

                AddProps(properties.Distinct(), @dynamicType.Type);

                return @dynamicType.CastTo(Activator.CreateInstance(@dynamicType.Type.CreateType()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public static DynamicType AddProperty(this DynamicType @dynamicType, string name, object value)
        {
            (@dynamicType.Holder as dynamic).GetType().GetProperty(name).SetValue(@dynamicType.Holder, value, null);
            return @dynamicType;
        }

        private static void AddProps(IEnumerable<PropertyInfo> propertyInfos, TypeBuilder type)
        {
            foreach (var v in propertyInfos)
            {
                var field = type.DefineField("_" + v.Name.ToCamelCase(), v.PropertyType, FieldAttributes.Private);
                var property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                var getter = type.DefineMethod("get_" + v.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType,
                    new Type[0]);
                var setter = type.DefineMethod("set_" + v.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null,
                    new Type[] { v.PropertyType });
                var getGenerator = getter.GetILGenerator();
                var setGenerator = setter.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, field);
                getGenerator.Emit(OpCodes.Ret);
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Stfld, field);
                setGenerator.Emit(OpCodes.Ret);
                property.SetGetMethod(getter);
                property.SetSetMethod(setter);
                type.DefineMethodOverride(getter, v.GetGetMethod());
                //type.DefineMethodOverride(setter, v.GetSetMethod());
            }
        }
    }
}
