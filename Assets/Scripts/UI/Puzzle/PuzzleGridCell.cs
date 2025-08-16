using Creature;
using UnityEngine;

namespace UI.Puzzle
{
    public class PuzzleGridCell : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTm = null;
        [SerializeField] private BoxCollider2D boxCollider2D = null;
        
        public int _puzzlePieceItemId = 0;
        public int _index = 0;

        public int Index => _index;
        public RectTransform RectTm => rectTm;
        
        // public int PuzzleItemId = _puzzlePieceItemId

        public PuzzleGridCell SetIndex(int index)
        {
            _index = index;
            name = $"{index}";
            
            return this;
        }
        public PuzzleGridCell SetSize(float cellSize)
        {
            if(rectTm)
                rectTm.sizeDelta = new Vector2(cellSize, cellSize);
            
            if (boxCollider2D != null)
                boxCollider2D.size = new Vector2(cellSize, cellSize);
            
            return this;
        }

        public void AddPuzzlePieceItemId(int itemId)
        {
            _puzzlePieceItemId = itemId;
        }

        public bool ContainPuzzlePiece
        {
            get
            {
                return _puzzlePieceItemId != 0;
            }
        }
    }
}

