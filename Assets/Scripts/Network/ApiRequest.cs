using System;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

using Common;

namespace Network
{
    [Serializable]
    public class Packet
    {
            
    }
    
    public abstract class ApiRequest<TReq, TRes> where TReq : Packet where TRes : Packet, new()
    {
        protected TReq _requestPacket = null;

        public static T CreateRequest<T>(TReq packet = null) where T : new()
        {
            var request = new T();
            (request as ApiRequest<TReq, TRes>)?.SetRequestPacket(packet);
            
            return request;
        }
        
        public abstract UnityWebRequest Create(string url, bool isLocal = false, IApiResponse<TRes> iApiResponse = null);
        
        protected string GetFileName(EJsonFile eJsonFile)
        {
            return $"{eJsonFile}.json";
        }

        public byte[] GetJsonBytes(bool isLocal)
        {
            if (isLocal)
            {
                // return GetJsonBytes()
            }
            else
                return GetJsonBytes(_requestPacket);
        
            return null;
        }
        
        protected void SaveLocal(string path, string json)
        {
            System.IO.File.WriteAllText(path, json);
        }

        protected string GetJsonString(object obj)
        {
            string jsonData = JsonConvert.SerializeObject(obj);
            
            return jsonData;
        }
        
        private byte[] GetJsonBytes(object obj)
        {
            string jsonData = JsonConvert.SerializeObject(obj);
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(jsonData);

            return jsonBytes;
        }
        
        private void SetRequestPacket(TReq tPacket)
        {
            _requestPacket = tPacket;
        }

        protected string GetFullPath(string path, string fileName)
        {
            // var fileName = GetFileName(EJsonFile.Inventory);
            return Path.Combine(path, fileName);
        }
    }
}

