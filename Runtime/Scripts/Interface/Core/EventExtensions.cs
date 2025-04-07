using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Trackman
{
    [DebuggerStepThrough]
    public static class EventExtensions
    {
        #region Methods
        public static IEnumerable<T> FindEvents<T>(this MonoBehaviour _) where T : IEvent => FindEventsOrdered<T>();
        public static void ExecuteEvent<T>(this MonoBehaviour _, Action<T> execute) where T : IEvent
        {
            foreach (T target in FindEventsOrdered<T>())
            {
                if (target is MonoBehaviour { isActiveAndEnabled: true })
                    execute(target);
            }
        }
        public static void ExecuteEventValid<T>(this MonoBehaviour _, Action<T> execute) where T : IEvent
        {
            foreach (T target in FindEventsOrdered<T>())
            {
                if (target.CanExecute && target is MonoBehaviour { isActiveAndEnabled: true })
                    execute(target);
            }
        }
        public static async Task ExecuteEventAsync<T>(this MonoBehaviour _, Func<T, Task> execute) where T : IEvent
        {
            foreach (T target in FindEventsOrdered<T>())
            {
                if (target is MonoBehaviour { isActiveAndEnabled: true })
                    await execute(target);
            }
        }
        public static async Task<List<U>> ExecuteEventFuncAsync<T, U>(this MonoBehaviour _, Func<T, Task<U>> execute) where T : IEvent
        {
            T[] events = FindEventsOrdered<T>();
            List<U> results = new(events.Length);
            foreach (T target in events)
            {
                if (target is MonoBehaviour { isActiveAndEnabled: true })
                    results.Add(await execute(target));
            }
            return results;
        }
        public static async Task<U> ExecuteEventFuncAsync<T, U>(this MonoBehaviour _, Func<T, Task<U>> execute, Func<U, bool> predicate) where T : IEvent
        {
            U result = default;
            foreach (T target in FindEventsOrdered<T>())
            {
                if (target is MonoBehaviour { isActiveAndEnabled: true })
                {
                    U obj = await execute(target);
                    if (predicate(obj))
                        result = obj;
                }
            }
            return result;
        }
        #endregion

        #region Support Methods
        static T[] FindEventsOrdered<T>() where T : IEvent
        {
            T[] targets = ServiceLocator.FindInterfaces<T>();
            Array.Sort(targets, (x, y) => x.Order.CompareTo(y.Order));
            return targets;
        }
        #endregion
    }
}