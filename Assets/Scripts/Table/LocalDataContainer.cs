using UnityEngine;

namespace Table
{
    public class LocalDataContainer : Container<LocalDataContainer, LocalData>
    {
        public string GetLocalization(string key)
        {
            if (Datas.IsNullOrEmpty())
                return string.Empty;

            for (int i = 0; i < Datas.Length; ++i)
            {
                var data = Datas[i];
                if(data == null)
                    continue;

                if (!string.IsNullOrEmpty(data.Key) &&
                    data.Key == key)
                    return data.Id.GetLocalization();
            }

            return string.Empty;
        }
    }
}


