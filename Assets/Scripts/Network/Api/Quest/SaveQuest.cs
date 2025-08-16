using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Network.Api.Quest
{
    public class SaveQuest : ApiRequest<SaveQuest.Request, Packet>
    {
        [Serializable]
        public class Request : Packet
        {
            public int QuestGroup = 0;
            public int QuestStep = 0;
        }
        
        public override UnityWebRequest Create(string url, bool isLocal = false, IApiResponse<Packet> iApiResponse = null)
        {
            var fileName = GetFileName(Common.EJsonFile.Quest);
            var fullUrl = GetFullPath(url, fileName);
            
            Debug.Log(fullUrl);
            if (isLocal)
            {
                if (_requestPacket == null)
                    return null;

                var quest = InfoManager.Instance?.SaveQuest(_requestPacket.QuestGroup, _requestPacket.QuestStep);
                var jsonString = GetJsonString(quest);

                SaveLocal(fullUrl, jsonString);
                
                return null;
            }
            
            return new UnityWebRequest(fullUrl, "POST");
        }
    }
}

