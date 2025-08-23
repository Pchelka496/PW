using System;
using System.Collections.Generic;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Dmi.Scripts
{
    public enum EnumUpdateFrequency
    {
        EveryFrame = 1,
        Every2Frames = 2,
        Every4Frames = 4,
        Every10Frames = 10,
        Every60Frames = 60,
    }

    public interface IDistributedUpdatable
    {
        void DistributedUpdate();
    }

    public sealed class DistributedUpdateLoop : IDisposable
    {
        const int DEFAULT_CAPACITY = 32;

        readonly Dictionary<EnumUpdateFrequency, Bucket> _buckets = new();

        CancellationTokenSource _cts;
        CompositeDisposable _disposable = new();

        [Zenject.Inject]
        public void Construct()
        {
            _cts = new();

            foreach (EnumUpdateFrequency freq in Enum.GetValues(typeof(EnumUpdateFrequency)))
            {
                int f = (int)freq;
                _buckets[freq] = new Bucket(f, DEFAULT_CAPACITY);
            }

            RunLoop(_cts.Token).Forget();
        }

        public void Dispose()
        {
            ClearTokenSupport.ClearToken(ref _cts);
            _disposable.Dispose();
            foreach (var bucket in _buckets.Values)
                bucket.Clear();
        }

        public void Register(EnumUpdateFrequency frequency, IDistributedUpdatable updatable)
        {
            if (updatable == null) throw new ArgumentException();

            if (_buckets.TryGetValue(frequency, out var bucket))
                bucket.Add(updatable);
            else throw new ArgumentException($"Invalid update frequency [{updatable.GetType().Name}]");
        }

        public void Unregister(EnumUpdateFrequency frequency, IDistributedUpdatable updatable)
        {
            if (updatable == null) return;

            if (_buckets.TryGetValue(frequency, out var bucket))
                bucket.Remove(updatable);
        }

        private async UniTaskVoid RunLoop(CancellationToken token)
        {
            int frame = 0;

            while (!token.IsCancellationRequested)
            {
                foreach (var kvp in _buckets)
                {
                    try
                    {
                        kvp.Value.InvokeGroup(frame);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogException(new Exception(
                            $"[DistributedUpdateLoop] Error in Bucket (part of the loop). Frequency: {kvp.Key}", e));
                    }
                }

                frame++;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        public void RebalanceAll()
        {
            foreach (var bucket in _buckets.Values)
                bucket.Rebalance();
        }

        private class Bucket
        {
            readonly List<IDistributedUpdatable>[] _groups;
            readonly Dictionary<IDistributedUpdatable, (int groupIndex, int entryIndex)> _map;
            readonly int _frequency;

            public Bucket(int frequency, int initialCapacity)
            {
                _frequency = frequency;
                _groups = new List<IDistributedUpdatable>[frequency];
                for (int i = 0; i < frequency; i++)
                    _groups[i] = new List<IDistributedUpdatable>(initialCapacity / frequency);

                _map = new(initialCapacity);
            }

            public void Add(IDistributedUpdatable updatable)
            {
                if (_map.ContainsKey(updatable)) return;

                int bestGroup = 0;
                int minCount = _groups[0].Count;

                for (int i = 1; i < _frequency; i++)
                {
                    int count = _groups[i].Count;
                    if (count < minCount)
                    {
                        bestGroup = i;
                        minCount = count;
                    }
                }

                var group = _groups[bestGroup];
                group.Add(updatable);
                _map[updatable] = (bestGroup, group.Count - 1);
            }

            public void Remove(IDistributedUpdatable updatable)
            {
                if (!_map.TryGetValue(updatable, out var data)) return;

                var group = _groups[data.groupIndex];
                int lastIndex = group.Count - 1;

                if (data.entryIndex != lastIndex)
                {
                    var moved = group[lastIndex];
                    group[data.entryIndex] = moved;
                    _map[moved] = (data.groupIndex, data.entryIndex);
                }

                group.RemoveAt(lastIndex);
                _map.Remove(updatable);
            }

            public void InvokeGroup(int frame)
            {
                int groupIndex = frame % _frequency;
                var group = _groups[groupIndex];

                for (int i = group.Count - 1; i >= 0; i--)
                {
                    var target = group[i];
                    if (target == null)
                    {
                        RemoveNullAt(group, i);
                        continue;
                    }

                    try
                    {
                        target.DistributedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(
                            new Exception($"[DistributedUpdateLoop] Error in {target.GetType().Name}", e));
                    }
                }
            }

            public void Clear()
            {
                foreach (var group in _groups)
                    group.Clear();

                _map.Clear();
            }

            private void RemoveNullAt(List<IDistributedUpdatable> group, int index)
            {
                var lastIndex = group.Count - 1;

                if (index != lastIndex)
                {
                    var moved = group[lastIndex];
                    group[index] = moved;
                    _map[moved] = (_map[moved].groupIndex, index);
                }

                var nullTarget = group[lastIndex];
                group.RemoveAt(lastIndex);
                _map.Remove(nullTarget);
            }

            public void Rebalance()
            {
                List<IDistributedUpdatable> all = new(_map.Count);
                foreach (var group in _groups)
                    all.AddRange(group);

                foreach (var group in _groups)
                    group.Clear();

                _map.Clear();

                for (int i = 0; i < all.Count; i++)
                {
                    int groupIndex = i % _frequency;
                    var group = _groups[groupIndex];

                    group.Add(all[i]);
                    _map[all[i]] = (groupIndex, group.Count - 1);
                }
            }

            public bool IsEmpty => _map.Count == 0;

            public int Count => _map.Count;
        }
    }
}