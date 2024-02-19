using UnityEngine;
using System;
using System.Diagnostics;

namespace Trackman
{
    [Serializable]
    [DebuggerStepThrough]
    public class Timer : ITimer
    {
        #region Fields
        bool ITimer.StartedValue { get; set; }
        bool ITimer.DoneValue { get; set; }
        bool ITimer.PauseValue { get; set; }

        float ITimer.ElapsedValue { get; set; } = -1;
        float ITimer.AccelerationValue { get; set; } = 1;
        float ITimer.DurationValue { get; set; } = -1;
        bool ITimer.LoopValue { get; set; }
        #endregion

        #region Properties
        ITimer I => this.As<ITimer>();
        public float Elapsed => I.ElapsedValue;
        public float Duration { get => I.DurationValue; set => I.DurationValue = value; }
        public float Lerp => I.ElapsedValue / I.DurationValue;
        public bool Started => I.StartedValue;
        public bool Playing => I.StartedValue && !I.DoneValue;
        public bool Pause { get => I.StartedValue && I.PauseValue; set => I.PauseValue = I.StartedValue && value; }
        public bool Done => I.StartedValue && I.DoneValue;
        public float Acceleration => I.AccelerationValue;
        public bool Loop => I.LoopValue;
        #endregion

        #region Methods
        public void Start(float duration) => Start(0, duration);
        public void Start(float elapsed, float duration, float acceleration = 1.0f, bool loop = false) => I.StartDefault(elapsed, duration, acceleration, loop);
        public void PreStart(float duration, float acceleration = 1.0f, bool loop = false) => Start(-Time.deltaTime, duration, acceleration, loop);
        public void Cancel() => I.CancelDefault();
        public void ChangeAcceleration(float acceleration) => I.ChangeAccelerationDefault(acceleration);
        public bool ChronoTrigger() => I.ChronoTriggerDefault();
        #endregion
    }
}