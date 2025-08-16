using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Network
{
    public class ApiClient : Common.Singleton<ApiClient>
    {
        [SerializeField] 
        private bool isLocal = true;
        
        private string _localUrl = string.Empty;
        private string _url = string.Empty;

        public string LocalFilePath
        {
            get { return Path.Combine(Application.persistentDataPath, "Info"); }
        } 

        protected override void Initialize()
        {
#if UNITY_ANDROID
            _localUrl = LocalFilePath;
#else
            _localUrl = $"file://{LocalFilePath}";
#endif

            _url = _localUrl;
        }
        
        public string[] LocalFilePaths
        {
            get
            {
                if (!Directory.Exists(LocalFilePath))
                    Directory.CreateDirectory(LocalFilePath);
                
                return Directory.GetFiles(LocalFilePath);
            }
        }
        
        public void RequestGet<TReq, TRes>(ApiRequest<TReq, TRes> request, IApiResponse<TRes> iApiResponse = null) where TReq : Packet where TRes : Packet, new()
        {
            StartCoroutine(CoRequestGet(request, iApiResponse));
        }
        
        private IEnumerator CoRequestGet<TReq, TRes>(ApiRequest<TReq, TRes> request, IApiResponse<TRes> iApiResponse = null) where TReq : Packet where TRes : Packet, new()
        {
            UnityWebRequest webRequest = request.Create(_url, isLocal, iApiResponse);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            
            bool isError = webRequest.result != UnityWebRequest.Result.Success;
            if (isError)
                OnResponse(null, false, iApiResponse, webRequest.error);
            else
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
            
                var data = JsonConvert.DeserializeObject<TRes>(webRequest.downloadHandler.text, settings);
                OnResponse(data, true);
            }
        }
        
        public void RequestPost<TReq, TRes>(ApiRequest<TReq, TRes> request, IApiResponse<TRes> iApiResponse = null) where TReq : Packet where TRes : Packet, new()
        {
            StartCoroutine(CoRequestPost(request, iApiResponse));
        }
        
        private IEnumerator CoRequestPost<TReq, TRes>(ApiRequest<TReq, TRes> request, IApiResponse<TRes> iApiResponse = null) where TReq : Packet where TRes : Packet, new()
        {
            if (request == null)
            {
                OnResponse(null, false, iApiResponse);
                yield break;
            }
            
            UnityWebRequest webRequest = request.Create(LocalFilePath, isLocal, iApiResponse);
            if (webRequest == null)
            {
                if(!isLocal)
                    OnResponse(null, false, iApiResponse);
                
                yield break;
            }
            
            webRequest.uploadHandler = new UploadHandlerRaw(request.GetJsonBytes(false));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
        
            bool isError = webRequest.result != UnityWebRequest.Result.Success;
            if (isError)
                OnResponse(null, false, iApiResponse, webRequest.error);
            else
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
            
                var data = JsonConvert.DeserializeObject<TRes>(webRequest.downloadHandler.text, settings);
                OnResponse(data, true, iApiResponse);
            }
        }

        private void OnResponse<T>(T data, bool isSuccess, IApiResponse<T> iApiResponse = null, string errorMessage = "") where T : Packet, new()
        {
            if (iApiResponse != null)
                iApiResponse.OnResponse(data, isSuccess, errorMessage);
            else
            {
                if (isSuccess)
                {
                
                }
                else
                    Debug.Log("Error = " + errorMessage);
            }
        }
    }
}

