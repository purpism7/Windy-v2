using Common;
using UnityEngine;

namespace UI.Puzzle
{
    public class PuzzlePoint : MonoBehaviour
    {
        public enum EPoint
        {
            None,
            
            First,
            Second,
            Third,
            
            Start,
            End,
        }

        [SerializeField] private EPoint ePoint = EPoint.None;
        [SerializeField] private EDirection eDirection = EDirection.None;

        public EPoint _EPoint => ePoint;
        public EDirection EDirection => eDirection;
    }
}

