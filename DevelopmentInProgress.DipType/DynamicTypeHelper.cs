using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace DevelopmentInProgress.DipType
{
    public class DynamicTypeHelper<T>
    {
        private readonly Dictionary<string, Func<T, object>> getters;
        private readonly Dictionary<string, Action<T, object>> setters;

        public DynamicTypeHelper(Func<T> createInstance,
            Dictionary<string, Func<T, object>> getters,
            Dictionary<string, Action<T, object>> setters,
            IEnumerable<string> supportedProperties)
        {
            this.getters = getters;
            this.setters = setters;
            CreateInstance = createInstance;
            SupportedProperties = supportedProperties;
        }

        public Func<T> CreateInstance { get; private set; }

        public IEnumerable<string> SupportedProperties { get; private set; }

        public void SetValue(T target, string fieldName, object value)
        {
            if (setters.ContainsKey(fieldName))
            {
                setters[fieldName](target, value);
                return;
            }
            
            throw new ArgumentOutOfRangeException(fieldName + " not supported.");
        }

        public object GetValue(T target, string fieldName)
        {
            if (setters.ContainsKey(fieldName))
            {
                return getters[fieldName](target);
            }

            throw new ArgumentOutOfRangeException(fieldName + " not supported.");
        }
    }

    public static class DynamicTypeHelper
    {
        internal static readonly IDictionary<Type, object> cache = new ConcurrentDictionary<Type, object>();

        private static int counter;

        public static DynamicTypeHelper<T> Get<T>() where T : class, new()
        {
            var t = typeof(T);

            if (cache.ContainsKey(t))
            {
                return (DynamicTypeHelper<T>)cache[t];
            }

            var propertyInfos = PropertyHelper.GetPropertyInfos<T>();
            return Get<T>(propertyInfos);
        }

        public static DynamicTypeHelper<T> Get<T>(IEnumerable<PropertyInfo> propertyInfos) where T : class, new()
        {
            var t = typeof(T);

            if (cache.ContainsKey(t))
            {
                return (DynamicTypeHelper<T>)cache[t];
            }

            var typeHelper = CreateTypeHelper<T>(propertyInfos);
            cache.Add(t, typeHelper);
            return typeHelper;
        }

        private static DynamicTypeHelper<T> CreateTypeHelper<T>(IEnumerable<PropertyInfo> propertyInfos) where T : class, new()
        {
            var capacity = propertyInfos.Count() - 1;
            var propertyNames = new List<string>();
            var getters = new Dictionary<string, Func<T, object>>(capacity);
            var setters = new Dictionary<string, Action<T, object>>(capacity);

            var createInstance = CreateInstance<T>();

            foreach (var propertyInfo in propertyInfos)
            {
                propertyNames.Add(propertyInfo.Name);
                getters.Add(propertyInfo.Name, GetValue<T>(propertyInfo));
                setters.Add(propertyInfo.Name, SetValue<T>(propertyInfo));
            }

            return new DynamicTypeHelper<T>(createInstance, getters, setters, propertyNames);
        }

        private static Func<T> CreateInstance<T>() where T : class, new()
        {
            var t = typeof (T);
            var methodName = "CreateInstance_" + typeof(T).Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, t, null, typeof (DynamicTypeHelper).Module);
            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ret);
            return (Func<T>) dynMethod.CreateDelegate(typeof (Func<T>));
        }

        private static Func<T, object> GetValue<T>(PropertyInfo propertyInfo)
        {
            var getAccessor = propertyInfo.GetGetMethod();
            var methodName = "GetValue_" + propertyInfo.Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, typeof (T), new Type[] {typeof (object)},
                typeof (DynamicTypeHelper).Module);
            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, getAccessor, null);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            il.Emit(OpCodes.Ret);
            return (Func<T, object>)dynMethod.CreateDelegate(typeof(Func<T, object>));
        }

        private static Action<T, object> SetValue<T>(PropertyInfo propertyInfo)
        {
            var setAccessor = propertyInfo.GetSetMethod();
            var methodName = "SetValue_" + propertyInfo.Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, typeof (void),
                new Type[] {typeof (T), typeof (object)}, typeof (DynamicTypeHelper).Module);
            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }
            il.EmitCall(OpCodes.Callvirt, setAccessor, null);
            il.Emit(OpCodes.Ret);
            return (Action<T, object>)dynMethod.CreateDelegate(typeof(Action<T, object>));
        }

        private static int GetNextCounterValue()
        {
            return Interlocked.Increment(ref counter);
        }
    }
}
