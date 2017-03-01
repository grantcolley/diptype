using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace DevelopmentInProgress.DipType
{
    internal class DynamicTypeHelper<T>
    {
        // TODO: replace object with T
        private readonly Dictionary<string, Func<object, object>> getters;
        private readonly Dictionary<string, Action<object, object>> setters;

        public DynamicTypeHelper(Func<T> createInstance,
            Dictionary<string, Func<object, object>> getters,
            Dictionary<string, Action<object, object>> setters)
        {
            CreateInstance = createInstance;
            this.getters = getters;
            this.setters = setters;
        }

        internal Func<T> CreateInstance { get; private set; }

        internal void SetValue(T target, string fieldName, object value)
        {
            if (setters.ContainsKey(fieldName))
            {
                setters[fieldName](target, value);
                return;
            }
            
            throw new ArgumentOutOfRangeException(fieldName + " not supported.");
        }

        internal object GetValue(T target, string fieldName)
        {
            if (setters.ContainsKey(fieldName))
            {
                return getters[fieldName](target);
            }

            throw new ArgumentOutOfRangeException(fieldName + " not supported.");
        }
    }

    internal static class DynamicTypeHelper
    {
        private static readonly IDictionary<Type, object> cache = new ConcurrentDictionary<Type, object>();

        private static int counter;

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

        public static DynamicTypeHelper<T> CreateTypeHelper<T>(IEnumerable<PropertyInfo> propertyInfos) where T : class, new()
        {
            var getters = new Dictionary<string, Func<object, object>>();
            var setters = new Dictionary<string, Action<object, object>>();

            var createInstance = CreateInstance<T>();

            foreach (var propertyInfo in propertyInfos)
            {
                getters.Add(propertyInfo.Name, GetValue(propertyInfo));
                setters.Add(propertyInfo.Name, SetValue(propertyInfo));
            }

            return new DynamicTypeHelper<T>(createInstance, getters, setters);
        }

        private static Func<T> CreateInstance<T>() where T : class, new()
        {
            var t = typeof (T);
            var methodName = "CreateInstance_" + typeof (T).Name + "_" + GetNxtCounterValue();
            var dynMethod = new DynamicMethod(methodName, t, null, typeof (DynamicTypeHelper).Module);
            ILGenerator ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
            ilGen.Emit(OpCodes.Ret);
            return (Func<T>) dynMethod.CreateDelegate(typeof (Func<T>));
        }

        private static Func<object, string> GetValue(PropertyInfo propertyInfo)
        {
            var dynMethod = new DynamicMethod("GetValue_" + propertyInfo.Name, typeof(object), null, typeof(DynamicTypeHelper).Module);
            ILGenerator ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ret);
            return (Func<object, string>)dynMethod.CreateDelegate(typeof(Func<object, string>));
        }

        private static Action<object, object> SetValue(PropertyInfo propertyInfo)
        {
            var dynMethod = new DynamicMethod("SetValue_" + propertyInfo.Name, typeof(void), null, typeof(DynamicTypeHelper).Module);
            ILGenerator ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ret);
            return (Action<object, object>)dynMethod.CreateDelegate(typeof(Action<object, object>));
        }

        private static int GetNxtCounterValue()
        {
            return Interlocked.Increment(ref counter);
        }
    }
}
