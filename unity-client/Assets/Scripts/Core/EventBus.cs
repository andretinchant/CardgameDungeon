using System;
using System.Collections.Generic;

namespace CardgameDungeon.Unity.Core
{
    /// <summary>
    /// Static event bus for decoupled communication between Unity systems.
    /// Thread-safe. Supports generic event types via Subscribe, Unsubscribe, and Publish.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();
        private static readonly object _lock = new object();

        /// <summary>
        /// Subscribe a handler to events of type T.
        /// </summary>
        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var type = typeof(T);
                if (!_handlers.TryGetValue(type, out var list))
                {
                    list = new List<Delegate>();
                    _handlers[type] = list;
                }

                if (!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
        }

        /// <summary>
        /// Unsubscribe a handler from events of type T.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var type = typeof(T);
                if (_handlers.TryGetValue(type, out var list))
                {
                    list.Remove(handler);
                    if (list.Count == 0)
                    {
                        _handlers.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event of type T to all subscribed handlers.
        /// Handlers are invoked synchronously in subscription order.
        /// </summary>
        public static void Publish<T>(T eventData)
        {
            Delegate[] snapshot;

            lock (_lock)
            {
                var type = typeof(T);
                if (!_handlers.TryGetValue(type, out var list) || list.Count == 0)
                    return;

                snapshot = list.ToArray();
            }

            foreach (var handler in snapshot)
            {
                try
                {
                    ((Action<T>)handler).Invoke(eventData);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
        }

        /// <summary>
        /// Remove all subscriptions. Useful for cleanup between scenes.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _handlers.Clear();
            }
        }
    }
}
