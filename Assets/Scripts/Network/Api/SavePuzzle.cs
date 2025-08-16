using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

using Info;

namespace Network.Api
{
    public class SavePuzzle : ApiRequest<SavePuzzle.Request, SavePuzzle.Response>
    {
        [Serializable]
        public class Request : Packet
        {
            public int PuzzleIndex = 0;
            public int ItemId = 0;
            public int  PositionIndex = 0;
            public int  RotationZ = 0;
        }
        
        [Serializable]
        public class Response : Packet
        {
            
        }
        
        public override UnityWebRequest Create(string url, bool isLocal, IApiResponse<SavePuzzle.Response> iApiResponse)
        {
            var fileName = GetFileName(Common.EJsonFile.Puzzle);
            var fullUrl = GetFullPath(url, fileName);
            
            Debug.Log(fullUrl);
            if (isLocal)
            {
                if (_requestPacket == null)
                    return null;
        
                // int itemId = _requestPacket.ItemId;
                // int itemCount = _requestPacket.ItemCount;
                
                // var inventory = InfoManager.Instance?.AddItem(itemId, itemCount);
                // bool isSuccess = inventory != null;
                // if (isSuccess)
                // {
                //     var jsonString = GetJsonString(inventory);
                //     SaveLocal(fullUrl, jsonString);
                // }
                //
                // iApiResponse?.OnResponse(
                //     new Response
                //     {
                //         ItemId = itemId,
                //         ItemCount = itemCount,
                //         
                //     }, isSuccess);
                
                return null;
            }
            
            return new UnityWebRequest(fullUrl, "POST");
        }
    }
}