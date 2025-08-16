
using UnityEngine;

namespace Table
{
    public class TalkDataContainer : Container<TalkDataContainer, TalkData>
    {
        public TalkData GetData(int npcId, int questGroup, int questStep)
        {
            if (Datas.IsNullOrEmpty())
                return null;

            for (int i = 0; i < Datas.Length; ++i)
            {
                var data = Datas[i];
                if(data == null)
                    continue;

                if (data.NpcId == npcId &&
                    data.QuestGroup == questGroup &&
                    data.QuestStep == questStep)
                    return data;
            }

            return null;
        }
    }
}

