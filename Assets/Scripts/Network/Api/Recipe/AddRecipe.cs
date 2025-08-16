using System;
using System.IO;
using Info;
using UnityEngine;
using UnityEngine.Networking;

namespace Network.Api.Recipe
{
    public class AddRecipe : ApiRequest<AddRecipe.Request, AddRecipe.Response>
    {
        [Serializable]
        public class Request : Packet
        {
            public int[] RecipeIds = null;
        }
        
        [Serializable]
        public class Response : Packet
        {
            public int[] RecipeIds = null;
        }
        
        public override UnityWebRequest Create(string url, bool isLocal, IApiResponse<AddRecipe.Response> iApiResponse)
        {
            var fileName = GetFileName(Common.EJsonFile.Recipe);
            var fullUrl = GetFullPath(url, fileName);
            
            Debug.Log(fullUrl);
            if (isLocal)
            {
                if (_requestPacket == null)
                    return null;
        
                var recipeIds = _requestPacket.RecipeIds;
                
                var recipe = InfoManager.Get<Info.Recipe>()?.Add(recipeIds);
                var jsonString = GetJsonString(recipe);
        
                SaveLocal(fullUrl, jsonString);
                
                iApiResponse?.OnResponse(
                    new Response
                    {
                        RecipeIds = recipeIds,
                        
                    }, true);
                
                return null;
            }
            
            return new UnityWebRequest(fullUrl, "POST");
        }
    }
}