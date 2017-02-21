using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("DevelopmentInProgress.DipType.Test")]

namespace DevelopmentInProgress.DipType
{
    public abstract class TypeHelperBase
    {
        public abstract object New();
        public abstract object GetValue(object obj, string propertyName);
        public abstract void SetValue(object obj, string propertyName, object value);
    }

    public static class TypeHelper
    {
        private static AssemblyBuilder assemblyBuilder;

        private static ModuleBuilder moduleBuilder;

        private static int counter;

        public static TypeHelperBase Create<T>()
        {
            if (assemblyBuilder == null)
            {
                assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("TypeHelperAssembly"), AssemblyBuilderAccess.Run);

                moduleBuilder = assemblyBuilder.DefineDynamicModule("TypeHelperModule");
            }

            TypeAttributes attribs = typeof(TypeHelperBase).Attributes;
            attribs = (attribs | TypeAttributes.Sealed | TypeAttributes.Public) &
                      ~(TypeAttributes.Abstract | TypeAttributes.NotPublic);

            TypeBuilder typeBuilder = moduleBuilder.DefineType("TypeHelper." + typeof(T).Name + "_" + GetNextCounterValue(),
                attribs, typeof(TypeHelperBase));

            MethodInfo baseNew = typeof(TypeHelperBase).GetMethod("New");

            MethodBuilder newBody = typeBuilder.DefineMethod(baseNew.Name, baseNew.Attributes, baseNew.ReturnType,
                Type.EmptyTypes);

            ConstructorInfo ctor = typeof(T).GetConstructor(Type.EmptyTypes);

            ILGenerator newIl = newBody.GetILGenerator();

            newIl.Emit(OpCodes.Newobj, ctor);

            newIl.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(newBody, baseNew);

            IEnumerable<PropertyInfo> propertyInfos = GetPropertyInfos<T>();

            MethodInfo baseGetValue = typeof(TypeHelperBase).GetMethod("GetValue");

            MethodBuilder getValueBody = typeBuilder.DefineMethod(baseGetValue.Name,
                baseGetValue.Attributes & ~MethodAttributes.Abstract,
                typeof(object), new Type[] { typeof(object), typeof(string) });

            ILGenerator getValueIL = getValueBody.GetILGenerator();

            GetSetValueIL<T>(getValueIL, propertyInfos, true);

            typeBuilder.DefineMethodOverride(getValueBody, baseGetValue);

            MethodInfo baseSetValue = typeof(TypeHelperBase).GetMethod("SetValue");

            MethodBuilder setValueBody = typeBuilder.DefineMethod(baseSetValue.Name,
                baseSetValue.Attributes & ~MethodAttributes.Abstract,
                typeof(void), new Type[] { typeof(object), typeof(string), typeof(object) });

            ILGenerator setValueIL = setValueBody.GetILGenerator();

            GetSetValueIL<T>(setValueIL, propertyInfos, false);

            typeBuilder.DefineMethodOverride(setValueBody, baseSetValue);

            TypeHelperBase typeHelper = (TypeHelperBase)Activator.CreateInstance(typeBuilder.CreateType(), Type.EmptyTypes);

            return typeHelper;
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

            //il.Emit(OpCodes.Ldstr, typeof(T).Name + " doesn't have a property called ");
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);

            for (int i = 0; i < numberOfProperties; i++)
            {
                var property = propertyInfos.ElementAt(i);

                il.MarkLabel(labels[i]);

                if (isGet)
                {
                    var getAccessor = property.GetGetMethod();

                    il.Emit(OpCodes.Ldarg_1);

                    il.EmitCall(OpCodes.Callvirt, getAccessor, null);

                    if (property.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, property.PropertyType);
                    }
                }
                else
                {
                    var setAccessor = property.GetGetMethod();

                    il.Emit(OpCodes.Ldarg_2);

                    if (property.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox, property.PropertyType);
                    }

                    il.EmitCall(OpCodes.Callvirt, setAccessor, null);
                }

                il.Emit(OpCodes.Ret);
            }
        }
    }
}