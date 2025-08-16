using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking; 

namespace Network.Api
{
    public class RemoveItem : ApiRequest<RemoveItem.Request, RemoveItem.Response>
    {
        [Serializable]
        public class Request : Packet
        {
            public Dictionary<int, int> ItemDic = null;
            public System.Action<System.Action> CompletedAction = null; 
        }
        
        [Serializable]
        public class Response : Packet
        {
            public Dictionary<int, int> ItemDic = null;
            public System.Action<System.Action> CompletedAction = null;
        }
        
        public override UnityWebRequest Create(string url, bool isLocal, IApiResponse<RemoveItem.Response> iApiResponse)
        {
            var fileName = GetFileName(Common.EJsonFile.Inventory);
            var fullUrl = GetFullPath(url, fileName);
            
            Debug.Log(fullUrl);
            if (isLocal)
            {
                if (_requestPacket == null)
                    return null;

                var itemDic = _requestPacket.ItemDic;
                var inventory = InfoManager.Instance?.RemoveItem(itemDic);
                bool isSuccess = inventory != null;
                if (isSuccess)
                {
                    var jsonString = GetJsonString(inventory);
                    SaveLocal(fullUrl, jsonString);
                }
                
                iApiResponse?.OnResponse(
                    new Response
                    {
                        ItemDic = itemDic,
                        CompletedAction = _requestPacket.CompletedAction,
                        
                    }, isSuccess);
                
                return null;
            }
            
            return new UnityWebRequest(fullUrl, "POST");
        }
    }
}