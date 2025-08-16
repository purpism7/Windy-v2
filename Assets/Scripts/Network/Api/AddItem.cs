using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

using Info;

namespace Network.Api
{
    public class AddItem : ApiRequest<AddItem.Request, AddItem.Response>
    {
        [Serializable]
        public class Request : Packet
        {
            public int ItemId = 0;
            public int ItemCount = 0;
        }
        
        [Serializable]
        public class Response : Packet
        {
            public int ItemId = 0;
            public int ItemCount = 0;
        }
        
        public override UnityWebRequest Create(string url, bool isLocal, IApiResponse<AddItem.Response> iApiResponse)
        {
            var fileName = GetFileName(Common.EJsonFile.Inventory);
            var fullUrl = GetFullPath(url, fileName);
            
            Debug.Log(fullUrl);
            if (isLocal)
            {
                if (_requestPacket == null)
                    return null;
        
                int itemId = _requestPacket.ItemId;
                int itemCount = _requestPacket.ItemCount;
                
                var inventory = InfoManager.Instance?.AddItem(itemId, itemCount);
                bool isSuccess = inventory != null;
                if (isSuccess)
                {
                    var jsonString = GetJsonString(inventory);
                    SaveLocal(fullUrl, jsonString);
                }
                
                iApiResponse?.OnResponse(
                    new Response
                    {
                        ItemId = itemId,
                        ItemCount = itemCount,
                        
                    }, isSuccess);
                
                return null;
            }
            
            return new UnityWebRequest(fullUrl, "POST");
        }
    }
}

