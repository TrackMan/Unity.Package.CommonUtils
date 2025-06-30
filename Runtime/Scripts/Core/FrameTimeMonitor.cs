using System;
using Trackman;
using UnityEngine;

namespace TrackMan
{
    public sealed class FrameTimeMonitor : MonoBehaviour
    {
        const int maxSamplesCount = 5 * 60 * 60; // 5 minutes at 60 FPS

        #region Containers
        [Serializable]
        public sealed class ResultsInMilliseconds
        {
            #region Fields
            readonly TimeSpan duration;
            readonly int collectedFramesCount;
            readonly double average;
            readonly double median;
            readonly double max;
            readonly double min;
            readonly double frameTime99ThPercentile;
            #endregion

            #region Properties
            public TimeSpan Duration => duration;
            public int CollectedFramesCount => collectedFramesCount;
            public double Average => average;
            public double Median => median;
            public double Max => max;
            public double Min => min;
            public double FrameTime99ThPercentile => frameTime99ThPercentile;
            #endregion

            #region Constructors
            public ResultsInMilliseconds()
            {
                duration = TimeSpan.Zero;
            }
            public ResultsInMilliseconds(TimeSpan duration, int collectedFramesCount, double average, double median, double max, double min, double frameTime99ThPercentile)
            {
                this.duration = duration;
                this.collectedFramesCount = collectedFramesCount;
                this.average = average;
                this.median = median;
                this.max = max;
                this.min = min;
                this.frameTime99ThPercentile = frameTime99ThPercentile;
            }
            #endregion
        }
        #endregion

        #region Fields
        static readonly ResultsInMilliseconds emptyResults = new();
        static readonly float[] samples = new float[maxSamplesCount];
        static int collectedSamplesCount;
        static bool isRunning;
        static FrameTimeMonitor behaviour;
        static double startTime;
        #endregion

        #region Properties
        public static ResultsInMilliseconds LastResults { get; private set; } = emptyResults;
        #endregion

        #region Methods
        public static bool TryStartMonitoring()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("FrameTimeMonitor not started: it can only be used in play mode.");
                return false;
            }

            if (isRunning) return false;

            isRunning = true;
            collectedSamplesCount = 0;
            startTime = Time.realtimeSinceStartupAsDouble;

            GameObject gameObject = new("FrameTimeMonitor");
            behaviour = gameObject.AddComponent<FrameTimeMonitor>();
            behaviour.hideFlags = HideFlags.DontSave;
            DontDestroyOnLoad(gameObject);

            return true;
        }
        public static ResultsInMilliseconds StopMonitoring()
        {
            if (!isRunning) return LastResults;

            KillMonoBehaviour();
            ResultsInMilliseconds results = CalculateResults(startTime, Time.realtimeSinceStartupAsDouble);
            LastResults = results;
            isRunning = false;
            return results;
        }
        #endregion

        #region Base Methods
        void Update()
        {
            if (collectedSamplesCount >= maxSamplesCount)
            {
                KillMonoBehaviour();
                return;
            }

            samples[collectedSamplesCount++] = Time.deltaTime;
        }
        #endregion

        #region Support Methods
        static void KillMonoBehaviour()
        {
            if (behaviour)
            {
                behaviour.gameObject.DestroySafe();
                behaviour = null;
            }
        }
        static ResultsInMilliseconds CalculateResults(double startTime, double stopTime)
        {
            if (collectedSamplesCount == 0) return emptyResults;

            Array.Sort(samples, 0, collectedSamplesCount);
            ArraySegment<float> sortedSamples = new(samples, 0, collectedSamplesCount);

            double average = 0;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < sortedSamples.Count; i++)
            {
                float sample = sortedSamples[i];
                average += sample;
            }

            average /= sortedSamples.Count;

            return new ResultsInMilliseconds
            (
                duration: TimeSpan.FromSeconds(stopTime - startTime),
                collectedFramesCount: sortedSamples.Count,
                average: average,
                max: sortedSamples[^1],
                min: sortedSamples[0],
                median: CalculateMedian(sortedSamples),
                frameTime99ThPercentile: Calculate99ThPercentile(sortedSamples)
            );
        }
        static double CalculateMedian(ArraySegment<float> sortedSamples)
        {
            int count = sortedSamples.Count;

            if (count % 2 == 0) return (sortedSamples[count / 2 - 1] + sortedSamples[count / 2]) / 2.0;

            return sortedSamples[count / 2];
        }
        static double Calculate99ThPercentile(ArraySegment<float> sorted)
        {
            const double p = 0.99;

            if (sorted.Count == 1) return sorted[0];

            double pos = p * (sorted.Count - 1);
            int lower = (int)pos;
            int upper = lower + 1;
            double fraction = pos - lower;

            return fraction < Mathf.Epsilon
                ? sorted[lower]
                : sorted[lower] + (sorted[upper] - sorted[lower]) * fraction;
        }
        #endregion
    }
}