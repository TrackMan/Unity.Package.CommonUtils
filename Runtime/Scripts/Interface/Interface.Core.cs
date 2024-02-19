using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trackman
{
    public interface IMonoBehaviour
    {
        #region Properties
        MonoBehaviour MonoBehavior => this.As<MonoBehaviour>();
        Transform Transform => MonoBehavior.transform;
        GameObject GameObject => MonoBehavior.gameObject;
        #endregion
    }

    public interface IMonoBehaviourInjectable : IMonoBehaviour
    {
    }

    public interface IMonoBehaviourCollectionItem : IMonoBehaviour
    {
    }

    public interface ISingletonInjectable : IMonoBehaviour
    {
    }

    public interface IClassName
    {
        #region Properties
        string ClassName { get; }
        #endregion
    }

    public interface ISingletonInterface : IClassName, IMonoBehaviour
    {
        #region Properties
        string Status => string.Empty;
        bool Testable => false;
        #endregion

        #region Methods
        void InitializeForTests() => throw new NotSupportedException();
        #endregion
    }

    public interface IPrefs : IDictionary<string, object>
    {
        #region Methods
        bool Contains(string path);
        void Write(string path, object value);
        T Read<T>(string path, T defaultValue = default);
        #endregion
    }

    public interface IEvent
    {
        #region Properties
        int Order => 0;
        bool CanExecute => true;
        #endregion
    }


    public interface ITimer
    {
        #region Properties
        float Elapsed { get; }
        float Duration { get; set; }
        float Lerp { get; }
        bool Started { get; }
        bool Playing { get; }
        bool Pause { get; set; }
        bool Done { get; }
        float Acceleration { get; }
        bool Loop { get; }

        #pragma warning disable S100, TM0006, InconsistentNaming
        protected internal float elapsed { get; set; }
        protected internal float duration { get; set; }
        protected internal bool started { get; set; }
        protected internal bool pause { get; set; }
        protected internal bool done { get; set; }
        protected internal float acceleration { get; set; }
        protected internal bool loop { get; set; }
        #pragma warning restore S100, TM0006, InconsistentNaming
        #endregion

        #region Methods
        void Start(float duration) => Start(0, duration);
        void Start(float elapsed, float duration, float acceleration = 1.0f, bool loop = false);
        protected internal void StartDefault(float elapsed, float duration, float acceleration, bool loop)
        {
            started = true;
            done = false;
            pause = false;
            this.elapsed = elapsed;
            this.acceleration = acceleration;
            this.duration = duration;
            this.loop = loop;
        }
        void PreStart(float duration, float acceleration = 1.0f, bool loop = false) => Start(-Time.deltaTime, duration, acceleration, loop);
        void Cancel();
        protected internal void CancelDefault()
        {
            started = false;
            done = false;
            pause = false;
            elapsed = -1;
            acceleration = 1;
            duration = -1;
            loop = false;
        }
        void ChangeAcceleration(float acceleration);
        protected internal void ChangeAccelerationDefault(float acceleration)
        {
            this.acceleration = acceleration;
        }
        /// <summary>
        /// Moves timer by Time.deltaTime
        /// </summary>
        /// <returns>false if just finished, else true</returns>
        bool ChronoTrigger();
        protected internal bool ChronoTriggerDefault()
        {
            if (!Started) return !Done;
            float dt = Time.deltaTime * Acceleration;
            if (Pause) dt = 0;
            if (Elapsed >= Duration)
            {
                if (Loop) elapsed = 0;
                else done = true;
            }
            else if (Elapsed + dt > Duration)
            {
                if (Loop)
                {
                    elapsed = Elapsed + dt - Duration;
                }
                else
                {
                    elapsed = Duration;
                    done = true;
                }
            }
            else
            {
                elapsed += dt;
            }
            return !Done;
        }
        #endregion
    }
}