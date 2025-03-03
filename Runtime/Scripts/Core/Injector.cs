using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Trackman
{
    public static class Injector
    {
        const BindingFlags nonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        #region Containers
        static readonly Dictionary<Type, PropertyInfo[]> injectTypeProperties = new();
        static readonly Dictionary<Type, List<(PropertyInfo property, MonoBehaviour target)>> injectTargets = new();
        static readonly Dictionary<Type, MonoBehaviour> singletonInjectable = new();
        static readonly Dictionary<Type, IList> collections = new();
        #endregion

        #region Constructors
        static Injector() => DisposeStatic.OnDisposeStatic += Clear;
        static void Clear()
        {
            injectTypeProperties.Clear();
            injectTargets.Clear();
            singletonInjectable.Clear();
            collections.Clear();
        }
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
#if UNITY_EDITOR
            void UpdateCollections()
            {
                foreach ((Type type, IList collection) in collections)
                {
                    collection.Clear();
                    foreach (Object value in Object.FindObjectsByType(type, FindObjectsSortMode.None))
                        collection.Add(value);
                }
            }

            if (!Application.isPlaying)
                UnityEditor.EditorApplication.hierarchyChanged += UpdateCollections; // Keep track of all collections in editor
#endif
            Clear();
        }
        #endregion

        #region Methods
        public static void Register<TClass, TInterface>(this TClass value) where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
        {
            static void AddType(Type type, MonoBehaviour target)
            {
                if (!singletonInjectable.TryGetValue(type, out MonoBehaviour singletonMono))
                    singletonInjectable.Add(type, singletonMono = target);

                if (injectTargets.TryGetValue(type, out List<(PropertyInfo property, MonoBehaviour target)> targets))
                    foreach ((PropertyInfo property, MonoBehaviour monoBehaviour) in targets)
                        property.SetValue(monoBehaviour, singletonMono);
            }

            Type classType = typeof(TClass);
            Type interfaceType = typeof(TInterface);

            AddType(classType, value.MonoBehavior);
            AddType(interfaceType, value.MonoBehavior);
        }
        public static void Unregister<TClass, TInterface>(this TClass value) where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
        {
            static void RemoveType(Type type)
            {
                if (singletonInjectable.ContainsKey(type))
                    singletonInjectable.Remove(type);

                if (injectTargets.TryGetValue(type, out List<(PropertyInfo property, MonoBehaviour target)> targets))
                    foreach ((PropertyInfo property, MonoBehaviour monoBehaviour) in targets)
                        property.SetValue(monoBehaviour, default);
            }

            Type classType = typeof(TClass);
            Type interfaceType = typeof(TInterface);

            RemoveType(classType);
            RemoveType(interfaceType);
        }
        public static void Inject<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourInjectable
        {
            void EnsureTargets(PropertyInfo property, MonoBehaviour target)
            {
                (PropertyInfo property, MonoBehaviour target) value = (property, target);

                if (injectTargets.TryGetValue(property.PropertyType, out List<(PropertyInfo property, MonoBehaviour target)> targets))
                {
                    if (!targets.Contains(value)) targets.Add(value);
                }
                else
                {
                    injectTargets.Add(property.PropertyType, new List<(PropertyInfo property, MonoBehaviour target)> { value });
                }
            }
            void InjectSingleton(PropertyInfo property)
            {
                if (singletonInjectable.TryGetValue(property.PropertyType, out MonoBehaviour singletonMono)) property.SetValue(mono, singletonMono);
                else
                {
                    if (typeof(MonoBehaviour).IsAssignableFrom(property.PropertyType)) // Attempt to find singleton instance for OnEnable
                    {
                        MonoBehaviour firstSingleton = (MonoBehaviour)MonoBehaviour.FindFirstObjectByType(property.PropertyType);
                        if (firstSingleton) property.SetValue(mono, firstSingleton);
                    }
                }
            }
            void InjectCollection(PropertyInfo property)
            {
                Type[] arguments = property.PropertyType.GetGenericArguments();
                if (arguments.Length == 0) return;

                Type elementType = arguments[0];

                if (collections.TryGetValue(elementType, out IList collection))
                {
                }
                else if (elementType.IsInterface && collections.FirstOrDefault(x => elementType.IsAssignableFrom(x.Key)) is { } keyValuePair)
                {
                    collection = keyValuePair.Value;
                }
                else
                {
                    collections.Add(elementType, collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)));
                }

                property.SetValue(mono, collection);
            }

            Type monoType = mono.GetType();
            if (!injectTypeProperties.TryGetValue(monoType, out PropertyInfo[] properties))
                injectTypeProperties[monoType] = properties = monoType.GetProperties(nonPublic).Where(x => x.GetCustomAttribute<InjectAttribute>() is not null).ToArray();

            foreach (PropertyInfo property in properties)
            {
                EnsureTargets(property, mono);
                if (typeof(ISingletonInjectable).IsAssignableFrom(property.PropertyType)) InjectSingleton(property);
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) InjectCollection(property);
            }
        }
        public static void Eject<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourInjectable
        {
            Type monoType = mono.GetType();
            if (injectTargets.TryGetValue(monoType, out List<(PropertyInfo property, MonoBehaviour target)> targets))
                targets.RemoveAll(x => x.target == mono);
        }
        public static void Add<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourCollectionItem
        {
            Type monoType = mono.GetType();
            if (collections.TryGetValue(monoType, out IList collection))
            {
                if (collection.Contains(mono)) return;
            }
            else
            {
                collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(monoType));
                collections.Add(monoType, collection);
            }

            collection.Add(mono);
        }
        public static void Remove<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourCollectionItem
        {
            Type monoType = mono.GetType();
            if (collections.TryGetValue(monoType, out IList value) && value is List<T> collection)
                collection.Remove(mono);
        }
        #endregion
    }
}