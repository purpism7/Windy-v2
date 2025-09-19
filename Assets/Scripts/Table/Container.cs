using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Table
{
    public class JsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
        
            return JsonConvert.DeserializeObject<T>(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    
    public abstract class Container
    {
        public abstract void Initialize(object obj, string json);
    }
    
    public class Container<T, V> : Container where T : new()  where V : Data
    {
        private static T _instance = default(T);

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
        
        protected V[] _datas = null;

        // public override void Initialize(object obj, GameSystem.JsonData jsonData)
        public override void Initialize(object obj, string json)
        {
            try
            {
                _instance = (T)obj;
                
                 var settings = new JsonSerializerSettings()
                 {
                     NullValueHandling = NullValueHandling.Ignore,
                 };
                
                 var datas = Newtonsoft.Json.JsonConvert.DeserializeObject<V[]>(json, settings);
                 if (datas == null)
                     return;
                
                // Debug.Log(jsonData.Sheet + " = " + jsonData.Json);
                
                 _datas = datas;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }   
        }

        public V[] Datas
        {
            get
            {
                return _datas;
            }
        }

        public V GetData(int id)
        {
            if (_datas == null)
                return null;
        
            if (id <= 0)
                return null;
            
            foreach (var data in _datas)
            {
                if(data == null)
                    continue;
        
                if (data.Id == id)
                    return data;
            }
        
            return null;
        }
        
        public V[] GetDatas(int id)
        {
            if (_datas == null)
                return null;
        
            if (id <= 0)
                return null;
        
            var list = new List<V>();
            list.Clear();
            
            foreach (var data in _datas)
            {
                if(data == null)
                    continue;
        
                if (data.Id == id)
                {
                    list.Add(data);
                }
            }
        
            return list.ToArray();
        }

#if UNITY_EDITOR
        public bool Add(V data)
        {
            if (data == null)
                return false;

            if (GetData(data.Id) != null)
                return false;

            var list = _datas?.ToList();
            list?.Add(data);
            _datas = list.ToArray();

            return true;
        }

        public bool Insert(V data, int index)
        {
            if (data == null)
                return false;

            if (GetData(data.Id) != null)
                return false;

            var list = _datas?.ToList();
            list.Insert(index, data);
            _datas = list.ToArray();

            return true;
        }

        public bool Remove(int id)
        {
            if (id <= 0)
                return false;

            _datas = _datas?.Where(data => data.Id != id)?.ToArray();

            return true;
        }

        public void OrderBy()
        {
            _datas = _datas?.OrderBy(_ => _.Id)?.ToArray();
        }
#endif
    }
}

