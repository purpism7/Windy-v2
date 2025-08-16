using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public class Creator<T> where T : class, new()
    {
        private static T _instance = null;

        public static T Get
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                
                return _instance;
            }
        }
    }
}
