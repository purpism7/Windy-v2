using UnityEngine;

using System;
using System.Collections.Generic;

namespace GameSystem.Event
{

    //public interface IEventHandler
    //{
    //    void Add(System.Action action);
    //}

    //public class EventHandler<T> : IEventHandler where T : EventParam
    //{
    //    void IEventHandler.Add(Action action)
    //    {
    //        //EventDispatcher.Register<T>();
    //    }
    //}

    public class EventDispatcher
    {
        private readonly static Dictionary<Type, Delegate> _eventHandlers = new();

        public static void Register<T>(Action<T> action) where T : EventParam
        {
            
            Debug.Log(typeof(T));
            
            if (_eventHandlers.TryGetValue(typeof(T), out var handler))
                _eventHandlers[typeof(T)] = (Action<T>)handler + action;
            else
                _eventHandlers[typeof(T)] = action;
        }

        public static void Unregister<T>(Action<T> action) where T : EventParam
        {
            if (_eventHandlers.TryGetValue(typeof(T), out var handler))
            {
                var newHandler = (Action<T>)handler - action;
                if (newHandler == null)
                    _eventHandlers.Remove(typeof(T));
                else
                    _eventHandlers[typeof(T)] = newHandler;
            }
        }

        public static void Dispatch<T>(T param) where T : EventParam
        {
            if (param == null) 
                return;

            if (_eventHandlers.TryGetValue(typeof(T), out var handler))
                ((Action<T>)handler)?.Invoke(param);
        }
    }
}

