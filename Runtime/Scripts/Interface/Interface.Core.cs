using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trackman
{
    public interface IMonoBehaviour
    {
        #region Properties
        MonoBehaviour MonoBehavior => this.As<MonoBehaviour>();
        string Name => MonoBehavior.name;
        Transform Transform => MonoBehavior.transform;
        GameObject GameObject => MonoBehavior.gameObject;
        #endregion
    }

    public interface IMonoBehaviourInjectable : IMonoBehaviour { }

    public interface IMonoBehaviourCollectionItem : IMonoBehaviour { }

    public interface ISingletonInjectable : IMonoBehaviour { }

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

        float ElapsedValue { get; protected set; }
        float DurationValue { get; set; }
        bool StartedValue { get; protected set; }
        bool PauseValue { get; set; }
        bool DoneValue { get; protected set; }
        float AccelerationValue { get; protected set; }
        bool LoopValue { get; protected set; }
        #endregion

        #region Methods
        void Start(float duration) => Start(0, duration);
        void Start(float elapsed, float duration, float acceleration = 1.0f, bool loop = false);
        void StartDefault(float elapsed, float duration, float acceleration, bool loop)
        {
            StartedValue = true;
            DoneValue = false;
            PauseValue = false;
            ElapsedValue = elapsed;
            AccelerationValue = acceleration;
            DurationValue = duration;
            LoopValue = loop;
        }
        void PreStart(float duration, float acceleration = 1.0f, bool loop = false) => Start(-Time.deltaTime, duration, acceleration, loop);
        void Cancel();
        void CancelDefault()
        {
            StartedValue = false;
            DoneValue = false;
            PauseValue = false;
            ElapsedValue = -1;
            AccelerationValue = 1;
            DurationValue = -1;
            LoopValue = false;
        }
        void ChangeAcceleration(float acceleration);
        void ChangeAccelerationDefault(float acceleration)
        {
            AccelerationValue = acceleration;
        }
        /// <summary>
        /// Moves timer by Time.deltaTime
        /// </summary>
        /// <returns>false if just finished, else true</returns>
        bool ChronoTrigger();
        bool ChronoTriggerDefault()
        {
            if (!Started) return !Done;

            float dt = Time.deltaTime * Acceleration;
            if (Pause) dt = 0;
            if (Elapsed >= Duration)
            {
                if (Loop) ElapsedValue = 0;
                else DoneValue = true;
            }
            else if (Elapsed + dt > Duration)
            {
                if (Loop)
                {
                    ElapsedValue = Elapsed + dt - Duration;
                }
                else
                {
                    ElapsedValue = Duration;
                    DoneValue = true;
                }
            }
            else
            {
                ElapsedValue += dt;
            }

            return !Done;
        }
        #endregion
    }
}