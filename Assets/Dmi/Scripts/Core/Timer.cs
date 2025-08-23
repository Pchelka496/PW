using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts
{
    public class Timer : ITickable, IDisposable
    {
        public const int INCORRECT_CANCELLATION_KEY = -1;

        readonly List<TimerData> _timers = new(capacity: 20);
        int _activeCount = 0;
        
        public void Tick()
        {
            try
            {
                float scaledTime = Time.time;
                float unscaledTime = Time.unscaledTime;

                for (int i = 0; i < _timers.Count; i++)
                {
                    var timer = _timers[i];
                    if (timer.OnTimerFinished == null) continue;

                    float currentTime = timer.UseUnscaledTime ? unscaledTime : scaledTime;

                    if (timer.OnProgress != null)
                        timer.OnProgress(Mathf.Clamp01((currentTime - timer.StartTime) / timer.Duration));

                    if (timer.IsActivated(currentTime))
                    {
                        timer.OnTimerFinished.Invoke();
                        _timers[i] = new();
                        _activeCount--;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public int AddTimer(float duration, Action onFinished, Action<float> onProgress = null,
            bool unscaledTime = false)
        {
            float now = unscaledTime ? Time.unscaledTime : Time.time;
            float endTime = now + duration;
            bool ActivationCondition(float t) => t >= endTime;

            return AddTimer(new TimerData(now, duration, ActivationCondition, onFinished, onProgress, unscaledTime));
        }

        public int CallFromNonMainThread(Action onFinished)
        {
            return AddTimer(new TimerData(0, 0, t => true, onFinished, null, true));
        }

        private int AddTimer(TimerData timerData)
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                if (_timers[i].OnTimerFinished == null)
                {
                    _timers[i] = timerData;
                    _activeCount++;
                    return i;
                }
            }

            _timers.Add(timerData);
            _activeCount++;
            return _timers.Count - 1;
        }

        public void RemoveTimer(int index)
        {
            if (index < 0 || index >= _timers.Count || _timers[index].OnTimerFinished == null) return;

            _timers[index] = new();
            _activeCount--;
        }

        private readonly struct TimerData
        {
            public readonly float StartTime;
            public readonly float Duration;
            public readonly Func<float, bool> IsActivated;
            public readonly Action OnTimerFinished;
            public readonly Action<float> OnProgress;
            public readonly bool UseUnscaledTime;

            public TimerData(float startTime, float duration, Func<float, bool> isActivated,
                Action onTimerFinished, Action<float> onProgress, bool useUnscaledTime)
            {
                StartTime = startTime;
                Duration = duration;
                IsActivated = isActivated;
                OnTimerFinished = onTimerFinished;
                OnProgress = onProgress;
                UseUnscaledTime = useUnscaledTime;
            }
        }

        public void Dispose()
        {
            _timers.Clear();
        }
    }
}
