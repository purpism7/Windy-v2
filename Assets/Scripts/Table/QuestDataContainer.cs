using GameSystem;
using UnityEngine;

using Table;

namespace Table
{
    public class QuestDataContainer : Container<QuestDataContainer, QuestData>
    {
        public override void Initialize(object obj, string json)
        {
            base.Initialize(obj, json);
        }

        public QuestData GetData(int group, int step)
        {
            var datas = Datas;
            if (datas.IsNullOrEmpty())
                return null;

            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                if (data == null)
                    continue;

                if (data.Group == group &&
                    data.Step == step)
                    return data;
            }

            return null;
        }
    }
}

