using System;

namespace Trackman
{
    /// <summary>
    /// Global event to be used by static classes only to clean-up static fields (caches/maps/state)
    /// Only need to subscribe to it once, typically in static constructor.
    /// </summary>
    public static class DisposeStatic
    {
        #region Events
        /// Event will be triggered when a product Main scene gets unloaded.
        public static event Action OnDisposeStatic;
        #endregion

        #region Fields
        public static void Dispose() => OnDisposeStatic?.Invoke();
        #endregion
    }
}
