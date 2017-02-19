using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace DevelopmentInProgress.DipType
{
    public abstract class TypeHelperBase
    {
        public abstract object GetValue(object obj, string propertyName);
        public abstract void SetValue(object obj, string propertyName, object value);
    }

    public static class TypeHelper
    {
        private static AssemblyBuilder assembly;

        private static ModuleBuilder module;

        private static int counter;

        public static TypeHelperBase Create<T>()
        {
            if (assembly == null)
            {
                AssemblyName name = new AssemblyName("TypeHelper");
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                module = assembly.DefineDynamicModule(name.Name);
            }

            TypeAttributes attribs = typeof (TypeHelperBase).Attributes;

            TypeBuilder tb = module.DefineType("TypeHelper." + typeof (T).Name + "." + GetNextCounterValue(),
                (attribs | TypeAttributes.Sealed | TypeAttributes.Public) &
                ~(TypeAttributes.Abstract | TypeAttributes.NotPublic), typeof (TypeHelperBase));

            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Newobj, ctor);
            ctorIL.Emit(OpCodes.Ret);

            MethodInfo baseGetValue = typeof (TypeHelperBase).GetMethod("GetValue");

            MethodBuilder getValueBody = tb.DefineMethod(baseGetValue.Name,
                baseGetValue.Attributes & ~MethodAttributes.Abstract,
                typeof (object), new Type[] {typeof (object), typeof (string)});

            var getValueIL = getValueBody.GetILGenerator();

            IEnumerable<PropertyInfo> propertyInfos = GetPropertyInfos<T>();

            GetSetValueIL<T>(getValueIL, propertyInfos, true);

            tb.DefineMethodOverride(getValueBody, baseGetValue);

            MethodInfo baseSetValue = typeof (TypeHelperBase).GetMethod("SetValue");

            MethodBuilder setValueBody = tb.DefineMethod(baseSetValue.Name,
                baseSetValue.Attributes & ~MethodAttributes.Abstract,
                typeof (object), new Type[] {typeof (object), typeof (string)});

            var setValueIL = setValueBody.GetILGenerator();

            GetSetValueIL<T>(setValueIL, propertyInfos, false);

            tb.DefineMethodOverride(setValueBody, baseGetValue);

            var typeHelper = (TypeHelperBase) Activator.CreateInstance(tb.CreateType(), Type.EmptyTypes);

            return typeHelper;
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
                labels[0] = il.DefineLabel();
            }

            for (int i = 0; i < numberOfProperties; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, propertyInfos.ElementAt(i).Name);
                il.Emit(OpCodes.Beq, labels[i]);
            }

            il.Emit(OpCodes.Ldstr, typeof (T).Name + " doesn't have a property called ");
            il.Emit(OpCodes.Newobj, typeof (ArgumentOutOfRangeException).GetConstructor(new Type[] {typeof (string)}));
            il.Emit(OpCodes.Throw);

            for (int i = 0; i < numberOfProperties; i++)
            {
                var property = propertyInfos.ElementAt(i);

                il.MarkLabel(labels[i]);

                il.Emit(OpCodes.Ldarg_0);

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
                    var setAccessor = property.GetGetMethod();

                    il.Emit(OpCodes.Ldarg_1);

                    if (property.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox, property.PropertyType);
                    }

                    il.EmitCall(OpCodes.Callvirt, setAccessor, null);
                }

                il.Emit(OpCodes.Ret);
            }
        }

        private static IEnumerable<PropertyInfo> GetPropertyInfos<T>()
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

            if (propertyType != typeof (string)
                && (propertyType.IsClass
                    || propertyType.IsInterface
                    || propertyType.IsArray
                    || propertyType.GetInterfaces()
                        .Any(
                            i =>
                                (i.GetTypeInfo().Name.Equals(typeof (IEnumerable).Name)
                                 || (i.IsGenericType &&
                                     i.GetGenericTypeDefinition().Name.Equals(typeof (IEnumerable<>).Name))))))
            {
                return true;
            }

            return false;
        }
    }
}