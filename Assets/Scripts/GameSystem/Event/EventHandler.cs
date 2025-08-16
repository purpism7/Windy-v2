using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Event
{
    public class CharacterStatusChangeEventParam : EventParam
    {

    }

    public interface IEventHandler
    {
        void Add(System.Action action);
    }

    public class EventHandler<T> : IEventHandler where T : EventParam
    {
        void IEventHandler.Add(Action action)
        {
            //EventDispatcher.Register<T>();
        }
    }

    public class EventDispatcher
    {
        private readonly static Dictionary<Type, Delegate> _eventHandlers = new();
        // private static Dictionary<System.Type, System.Action<T>> _eventActionDic = null;

        public static void Register<T>(Action<T> action) where T : EventParam
        {
            
            Debug.Log(typeof(T));
            
            if (_eventHandlers.TryGetValue(typeof(T), out var handler))
                _eventHandlers[typeof(T)] = (Action<T>)handler + action;
            else
                _eventHandlers[typeof(T)] = action;
        }

        //public static void Remove<T>(Action<T> action) where T : Event.Data
        //{
        //    // lock (_lockObj)
        //    {
        //        if (_eventHandlers.TryGetValue(typeof(T), out var handler))
        //        {
        //            var updated = (Action<T>)handler - action;
        //            if (updated == null)
        //                _eventHandlers.Remove(typeof(T));
        //            else
        //                _eventHandlers[typeof(T)] = updated;
        //        }
        //    }
        //}

        public static void Dispatch<T>(T param) where T : EventParam
        {
            if (param == null) 
                return;

            if (_eventHandlers.TryGetValue(typeof(T), out var handler))
                ((Action<T>)handler)?.Invoke(param);
        }
    }
}

