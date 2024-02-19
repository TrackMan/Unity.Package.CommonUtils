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
        bool ITimer.started { get; set; }
        bool ITimer.done { get; set; }
        bool ITimer.pause { get; set; }

        float ITimer.elapsed { get; set; } = -1;
        float ITimer.acceleration { get; set; } = 1;
        float ITimer.duration { get; set; } = -1;
        bool ITimer.loop { get; set; }
        #endregion

        #region Properties
        ITimer I => this.As<ITimer>();
        public float Elapsed => I.elapsed;
        public float Duration { get => I.duration; set => I.duration = value; }
        public float Lerp => I.elapsed / I.duration;
        public bool Started => I.started;
        public bool Playing => I.started && !I.done;
        public bool Pause { get => I.started && I.pause; set => I.pause = I.started && value; }
        public bool Done => I.started && I.done;
        public float Acceleration => I.acceleration;
        public bool Loop => I.loop;
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