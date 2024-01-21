using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Trackman
{
    public static class Injector
    {
        #region Containers
        static Dictionary<Type, PropertyInfo[]> typeInjectProperties = new();
        static Dictionary<Type, List<(PropertyInfo property, MonoBehaviour target)>> injectMonoBehaviours = new();
        static Dictionary<Type, MonoBehaviour> singletonInjectable = new();
        static Dictionary<Type, IList> collections = new();
        #endregion

        #region Methods
        public static void Register<TClass, TInterface>(this TClass value) where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
        {
            Type monoType = typeof(TInterface);
            if (!singletonInjectable.TryGetValue(monoType, out MonoBehaviour singletonMono))
                singletonInjectable.Add(monoType, singletonMono = value.MonoBehavior);

            if (!injectMonoBehaviours.TryGetValue(monoType, out List<(PropertyInfo property, MonoBehaviour target)> targets)) return;
            foreach ((PropertyInfo property, MonoBehaviour monoBehaviour) in targets)
                property.SetValue(monoBehaviour, singletonMono);
        }
        public static void Unregister<TClass, TInterface>(this TClass value) where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
        {
            Type monoType = typeof(TInterface);
            if (singletonInjectable.ContainsKey(monoType))
                singletonInjectable.Remove(monoType);

            if (!injectMonoBehaviours.TryGetValue(monoType, out List<(PropertyInfo property, MonoBehaviour target)> targets)) return;
            foreach ((PropertyInfo property, MonoBehaviour monoBehaviour) in targets)
                property.SetValue(monoBehaviour, null);
        }
        public static void Inject<T>(this T value) where T : MonoBehaviour, IMonoBehaviourInjectable
        {
            Type monoType = value.GetType();
            if (!typeInjectProperties.TryGetValue(monoType, out PropertyInfo[] properties))
                typeInjectProperties[monoType] = properties = monoType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Where(x => x.GetCustomAttribute<InjectAttribute>() is not null).ToArray();

            foreach (PropertyInfo property in properties)
            {
                (PropertyInfo property, MonoBehaviour target) valueTuple = (property, value);
                if (injectMonoBehaviours.TryGetValue(property.PropertyType, out List<(PropertyInfo property, MonoBehaviour target)> targets))
                {
                    if (!targets.Contains(valueTuple))
                        targets.Add(valueTuple);
                }
                else
                {
                    injectMonoBehaviours.Add(property.PropertyType, new List<(PropertyInfo property, MonoBehaviour target)> { valueTuple });
                }

                if (singletonInjectable.TryGetValue(property.PropertyType, out MonoBehaviour singletonMono))
                {
                    property.SetValue(value, singletonMono);
                }
#if UNITY_EDITOR
                else if (!Application.isPlaying)
                {
                    singletonMono = MonoBehaviour.FindObjectsOfType<MonoBehaviour>().FirstOrDefault(x => x.GetType() == property.PropertyType);
                    if (singletonMono) property.SetValue(value, singletonMono);
                }
#endif
            }
        }
        public static void Eject<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourInjectable
        {
            Type monoType = mono.GetType();
            if (injectMonoBehaviours.ContainsKey(monoType))
                injectMonoBehaviours[monoType].RemoveAll(x => x.target == mono);
        }
        public static void Add<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourCollectionItem
        {
            Type monoType = mono.GetType();
            if (collections.TryGetValue(monoType, out IList value))
            {
                if (value.Contains(mono)) return;
            }
            else
            {
                value = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(monoType));
                collections.Add(monoType, value);
            }

            value.Add(mono);
        }
        public static void Remove<T>(this T mono) where T : MonoBehaviour, IMonoBehaviourCollectionItem
        {
            Type monoType = mono.GetType();
            if (collections.TryGetValue(monoType, out IList value) && value is List<T> list)
                list.Remove(mono);
        }
        public static IReadOnlyList<T> GetCollection<T>() where T : MonoBehaviour, IMonoBehaviourCollectionItem
        {
            Type monoType = typeof(T);
            if (!collections.TryGetValue(monoType, out IList value))
                collections.Add(monoType, value = new List<T>());

            return (IReadOnlyList<T>)value;
        }
        #endregion
    }
}