using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Network.Api
{
    public class GetInventory : ApiRequest<Packet, GetInventory.Response>
    {
        [Serializable]
        public class Response : Packet
        {
       
        }
        
        public override UnityWebRequest Create(string url, bool isLocal, IApiResponse<Response> iApiResponse)
        {
            var fileName = GetFileName(Common.EJsonFile.Inventory);
            var fullUrl = GetFullPath(url, fileName);
            
            return UnityWebRequest.Get(fullUrl);
        }
    }
}
