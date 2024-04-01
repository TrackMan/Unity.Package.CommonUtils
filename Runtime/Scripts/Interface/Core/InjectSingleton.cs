using System.Diagnostics;
using UnityEngine;

namespace Trackman
{
    [DebuggerStepThrough]
    public abstract class InjectSingleton<TClass, TInterface> : MonoBehaviour, ISingletonInjectable where TClass : MonoBehaviour, TInterface where TInterface : ISingletonInjectable
    {
        #region Fields
        static TClass instance;
        #endregion

        #region Properties
        public static TClass Instance => instance.OrNull() ?? (instance = FindAnyObjectByType<TClass>());
        public static TInterface I => Instance;
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            instance = (TClass)(object)this;
            instance.Register<TClass, TInterface>();
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