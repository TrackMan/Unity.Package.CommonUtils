using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Trackman
{
    [DebuggerStepThrough]
    public abstract class InjectSingleton<TClass, TInterface> : MonoBehaviour, ISingletonInjectable where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
    {
        #region Fields
        static TClass instance;
        #endregion

        #region Properties
        // ReSharper disable once Unity.NoNullCoalescing
        public static TClass Instance
        {
            get
            {
                if (instance) return instance;

                TClass newInstance = FindAnyObjectByType<TClass>(FindObjectsInactive.Exclude);
                Debug.Log(newInstance);
                if (newInstance && newInstance.enabled) return instance = newInstance;

                return default;
            }
        }
        public static TInterface I => Instance;
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            if (enabled)
            {
                instance = (TClass)(object)this;
                instance.Register<TClass, TInterface>();
            }
        }
        protected virtual void OnEnable()
        {
            instance = (TClass)(object)this;
            instance.Register<TClass, TInterface>();
        }
        protected virtual void OnDisable()
        {
            instance.Unregister<TClass, TInterface>();
            instance = default;
        }
        #endregion
    }
}