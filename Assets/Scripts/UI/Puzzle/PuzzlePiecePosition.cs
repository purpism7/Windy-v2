using UnityEngine;

namespace UI.Puzzle
{
    public class PuzzlePiecePosition : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTm = null;

        public RectTransform RectTm => rectTm;

        public int _puzzlePieceItemId = 0;

        public PuzzlePiecePosition ApplySize(float size)
        {
            if(rectTm)
                rectTm.sizeDelta = new Vector2(size, size);
            
            return this;
        }

        public PuzzlePiecePosition ApplyPuzzlePieceItemId(int itemId)
        {
            _puzzlePieceItemId = itemId;
            return this;
        }       
    }
}

