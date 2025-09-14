using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

using Creator;
using GameSystem;
using GameSystem.Event;
using Table;
using UI.Puzzle;
using EventHandler = GameSystem.Event;

namespace UI.Puzzle
{
    public interface IPathFindPuzzleView : IView
    {
        RectTransform PieceRootRectTm { get; }
        VerticalLayoutGroup PiecePositionVerticalLayoutGroup { get; }

        PathFindPuzzleView.PuzzleData GetPuzzleData(int index);
    }

    public class PathFindPuzzleView : BaseView<PathFindPuzzleView.Param>, 
        IPathFindPuzzleView,
        InputManager.IKeyListener
    {
        public new class Param : BaseView<Param>.Param
        {
            public int PuzzleIndex { get; private set; } = 0;
            
            public Param(int puzzleIndex)
            {
                PuzzleIndex = puzzleIndex;
            }
        }
        
        [Serializable]
        public class PuzzleData
        {
            [SerializeField] private PuzzleGrid puzzleGrid = null;
            [SerializeField] private int[] pieceIndices = null;
            [SerializeField] private float pieceSize = 0;
            [SerializeField] private PuzzlePoint startPuzzlePoint = null;
            // [SerializeField] private PuzzlePoint endPuzzlePoint = null;

            public PuzzleGrid PuzzleGrid => puzzleGrid;
            public int[] PieceIndices => pieceIndices;
            public float PieceSize => pieceSize;
            public PuzzlePoint StartPuzzlePoint => startPuzzlePoint;
            // public PuzzlePoint EndPuzzlePoint => endPuzzlePoint;
        }

        [SerializeField] private VerticalLayoutGroup piecePositionVerticalLayoutGroup = null;
        [SerializeField] private RectTransform pieceRootRectTm = null;
        [SerializeField] private PuzzleData[] puzzleDatas = null;
        
        private IPathFindPuzzlePresenter _iPathFindPuzzlePresenter = null;
        private int _puzzleIndex = -1;
        
        #region IPathFindPuzzleView
        RectTransform IPathFindPuzzleView.PieceRootRectTm => pieceRootRectTm;
        VerticalLayoutGroup IPathFindPuzzleView.PiecePositionVerticalLayoutGroup => piecePositionVerticalLayoutGroup;

        PuzzleData IPathFindPuzzleView.GetPuzzleData(int index)
        {
            if (puzzleDatas != null && puzzleDatas.Length > index)
                return puzzleDatas[index];

            return null;
        }

        #endregion

        public override void CreatePresenter()
        {
            _iPathFindPuzzlePresenter = new PathFindPuzzlePresenter()
                .Initialize(this);
        }

        public override async UniTask InitializeAsync()
        {
            Manager.Get<IInput>()?.AddListener(this);

            await UniTask.CompletedTask;
        }

        public override async UniTask ActivateAsync()
        {
            int puzzleIndex = _param != null ? _param.PuzzleIndex : 0;
            puzzleIndex -= 1;
            if (_param != null && 
                _puzzleIndex != puzzleIndex)
            {
                AllDeactivatePuzzleRootRectTm();
                ActivatePuzzleRootRectTm(puzzleIndex);
            }
            
            // await _iPathFindPuzzlePresenter
            //     .SetPuzzleIndex(puzzleIndex)
            //     .ActivateAsync();
            
            Manager.Get<IInputLocker>()?.Lock(EInputLock.Axis);
        }

        public override async UniTask AfterActivateAsync()
        {
            await base.AfterActivateAsync();
            
            await _iPathFindPuzzlePresenter
                .SetPuzzleIndex(_puzzleIndex)
                .ActivateAsync();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _iPathFindPuzzlePresenter?.DeactivateAsync();

            Manager.Get<IInputLocker>()?.Lock(EInputLock.None);
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            _iPathFindPuzzlePresenter?.ChainUpdate();
        }

        private void AllDeactivatePuzzleRootRectTm()
        {
            if (puzzleDatas.IsNullOrEmpty())
                return;

            for (int i = 0; i < puzzleDatas.Length; ++i)
            {
                var puzzleData = puzzleDatas[i];
                if(puzzleData == null)
                    continue;
                
                puzzleData.PuzzleGrid?.SetActive(false);
            }
        }
        
        private void ActivatePuzzleRootRectTm(int index)
        {
            if (puzzleDatas?.Length > index)
            {
                for (int i = 0; i < puzzleDatas?.Length; ++i)
                {
                    var puzzleData = puzzleDatas[i];
                    if (puzzleData == null)
                        return;

                    if (i == index)
                        puzzleData.PuzzleGrid?.RefreshIndex();
                    
                    puzzleData.PuzzleGrid?.SetActive(i == index);
                }
            
                _puzzleIndex = index;
            }
        }
        
        #region InputManager.IKeyListener
        void InputManager.IKeyListener.OnKeyDown(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Escape)
            {
                Deactivate();
                return;
            }
        }
        
        void InputManager.IKeyListener.OnKey(KeyCode keyCode)
        {
           
        }
        #endregion
    }
}

