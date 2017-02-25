using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("DevelopmentInProgress.DipType.Test")]

namespace DevelopmentInProgress.DipType
{
    public abstract class TypeHelper<T>
    {
        public abstract T CreateInstance();
        public abstract object GetValue(object obj, string propertyName);
        public abstract void SetValue(object obj, string propertyName, object value);
        public virtual string SupportedProperties { get; set; }
    }

    public static class TypeHelper
    {
        internal static readonly IDictionary<Type, object> cache = new ConcurrentDictionary<Type, object>();

        private static AssemblyBuilder assemblyBuilder;

        private static ModuleBuilder moduleBuilder;

        private static int counter;

        public static TypeHelper<T> CreateInstance<T>()
        {
            if (cache.ContainsKey(typeof(T)))
            {
                return (TypeHelper<T>)cache[typeof(T)];
            }

            var genericTypeHelper = BuildInstance<T>();
            cache.Add(typeof(T), genericTypeHelper);
            return genericTypeHelper;
        }

        private static TypeHelper<T> BuildInstance<T>()
        {
            var t = typeof (T);
            var typeHelperType = typeof(TypeHelper<T>);
            IEnumerable<PropertyInfo> propertyInfos = GetPropertyInfos<T>();

            var supportedPropertyNames = String.Empty;
            foreach (var propertyInfo in propertyInfos)
            {
                supportedPropertyNames += propertyInfo.Name + ",";
            }

            supportedPropertyNames = supportedPropertyNames.Remove(supportedPropertyNames.Length - 1, 1);

            if (assemblyBuilder == null)
            {
                assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("TypeHelperAssembly"), AssemblyBuilderAccess.RunAndSave);

                moduleBuilder = assemblyBuilder.DefineDynamicModule("TypeHelperModule");
            }

            var attribs = typeHelperType.Attributes;
            attribs = (attribs | TypeAttributes.Sealed | TypeAttributes.Public) &
                      ~(TypeAttributes.Abstract | TypeAttributes.NotPublic);

            var typeBuilder = moduleBuilder.DefineType("TypeHelper." + t.Name + "_" + GetNextCounterValue(),
                attribs, typeHelperType);

            var genericTypeParameterBuilder = typeBuilder.DefineGenericParameters(new[] {"T"});

            genericTypeParameterBuilder[0].SetGenericParameterAttributes(
                GenericParameterAttributes.DefaultConstructorConstraint |
                GenericParameterAttributes.ReferenceTypeConstraint);

            var supportedPropertiesField = typeBuilder.DefineField("supportedProperties", typeof(string),
                FieldAttributes.Private);

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                Type.EmptyTypes);

            var ctorIl = ctor.GetILGenerator();

            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldstr, supportedPropertyNames);
            ctorIl.Emit(OpCodes.Stfld, supportedPropertiesField);
            ctorIl.Emit(OpCodes.Ret);

            var supportedPropertiesGet = typeBuilder.DefineMethod("get_SupportedProperties",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                MethodAttributes.Virtual, typeof (string),
                Type.EmptyTypes);

            var supportedPropertiesGetIL = supportedPropertiesGet.GetILGenerator();

            supportedPropertiesGetIL.Emit(OpCodes.Ldarg_0);
            supportedPropertiesGetIL.Emit(OpCodes.Ldfld, supportedPropertiesField);
            supportedPropertiesGetIL.Emit(OpCodes.Ret);

            var baseNew = typeHelperType.GetMethod("CreateInstance");

            var newBody = typeBuilder.DefineMethod(baseNew.Name, baseNew.Attributes & ~MethodAttributes.Abstract,
                baseNew.ReturnType, Type.EmptyTypes);

            var newIl = newBody.GetILGenerator();

            newIl.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
            newIl.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(newBody, baseNew);

            var baseGetValue = typeHelperType.GetMethod("GetValue");

            var getValueBody = typeBuilder.DefineMethod(baseGetValue.Name,
                baseGetValue.Attributes & ~MethodAttributes.Abstract,
                typeof(object), new Type[] { typeof(object), typeof(string) });

            var getValueIL = getValueBody.GetILGenerator();

            GetSetValueIL<T>(getValueIL, propertyInfos, true);

            typeBuilder.DefineMethodOverride(getValueBody, baseGetValue);

            var baseSetValue = typeHelperType.GetMethod("SetValue");

            var setValueBody = typeBuilder.DefineMethod(baseSetValue.Name,
                baseSetValue.Attributes & ~MethodAttributes.Abstract,
                null, new Type[] { typeof(object), typeof(string), typeof(object) });

            var setValueIL = setValueBody.GetILGenerator();

            GetSetValueIL<T>(setValueIL, propertyInfos, false);

            typeBuilder.DefineMethodOverride(setValueBody, baseSetValue);

            var genericTypeHelper =
                (TypeHelper<T>)
                    Activator.CreateInstance(typeBuilder.CreateType().MakeGenericType(new Type[] {t}), Type.EmptyTypes);

            return genericTypeHelper;
        }

        internal static IEnumerable<PropertyInfo> GetPropertyInfos<T>()
        {
            var propertyInfoResults = new List<PropertyInfo>();

            PropertyInfo[] propertyInfos = typeof (T).GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                if (UnsupportedProperty(propertyInfo))
                {
                    continue;
                }

                propertyInfoResults.Add(propertyInfo);
            }

            return propertyInfoResults;
        }

        private static bool UnsupportedProperty(PropertyInfo propertyInfo)
        {
            // Skip non-public properties and properties that are either 
            // classes (but not strings), interfaces, lists, generic 
            // lists or arrays.
            var propertyType = propertyInfo.PropertyType;

            if (propertyType != typeof(string)
                && (propertyType.IsClass
                    || propertyType.IsInterface
                    || propertyType.IsArray
                    || propertyType.GetInterfaces()
                        .Any(
                            i =>
                                (i.GetTypeInfo().Name.Equals(typeof(IEnumerable).Name)
                                 || (i.IsGenericType &&
                                     i.GetGenericTypeDefinition().Name.Equals(typeof(IEnumerable<>).Name))))))
            {
                return true;
            }

            return false;
        }

        private static int GetNextCounterValue()
        {
            return Interlocked.Increment(ref counter);
        }

        private static void GetSetValueIL<T>(ILGenerator il, IEnumerable<PropertyInfo> propertyInfos, bool isGet)
        {
            var numberOfProperties = propertyInfos.Count();

            var labels = new Label[numberOfProperties];

            for (int i = 0; i < numberOfProperties; i++)
            {
                labels[i] = il.DefineLabel();
            }

            for (int i = 0; i < numberOfProperties; i++)
            {
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldstr, propertyInfos.ElementAt(i).Name);
                il.Emit(OpCodes.Beq, labels[i]);
            }

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);

            for (int i = 0; i < numberOfProperties; i++)
            {
                var property = propertyInfos.ElementAt(i);

                il.MarkLabel(labels[i]);           
                il.Emit(OpCodes.Ldarg_1);                

                if (isGet)
                {
                    var getAccessor = property.GetGetMethod();

                    il.EmitCall(OpCodes.Callvirt, getAccessor, null);

                    if (property.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, property.PropertyType);
                    }
                }
                else
                {
                    var setAccessor = property.GetSetMethod();

                    il.Emit(OpCodes.Ldarg_3);

                    if (property.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, property.PropertyType);
                    }

                    il.EmitCall(OpCodes.Callvirt, setAccessor, null);
                }

                il.Emit(OpCodes.Ret);
            }
        }
    }
}