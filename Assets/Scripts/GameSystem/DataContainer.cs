
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class DataContainer : MonoBehaviour
    {
        public async UniTask InitializeAsync()
        {
            await LoadAsync();
        }
        
        private async UniTask LoadAsync()
        {
            // await UniTask.Defer(async () => await AddressableManager.Instance.LoadAssetAsync<TextAsset>("Table",
             await AddressableManager.Instance.LoadAssetAsync<TextAsset>("Table",
                (result) =>
                {
                    var typeName = $"{nameof(Table)}.{result.name}{nameof(DataContainer)}";
                    var type = System.Type.GetType(typeName);

                    if (type != null)
                    {
                        var obj = System.Activator.CreateInstance(type);
                        var container = obj as Table.Container;
                        
                        // result.text.Decrypy
                        var resText = result.text;
                        Debug.Log("resText = " + resText);
                        container?.Initialize(container, resText);
                    }

                    // endLoad = true;
                });
        }
    }
}

