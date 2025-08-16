using UnityEngine;

namespace Network
{
    public interface IApiResponse<T> where T : Packet, new()
    {
        void OnResponse(T data, bool isSuccess, string errorMessage = null);
    }
}

