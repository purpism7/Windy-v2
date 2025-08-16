using UnityEngine;
using UnityEngine.UI;

using Common;
using Creator;
using Cysharp.Threading.Tasks;
using GameSystem;
using GameSystem.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using Table;
using UI.Puzzle;
using UnityEngine.EventSystems;
using static UI.Puzzle.PathFindPuzzleView;

namespace UI.Puzzle
{
    public interface IPathFindPuzzlePresenter : IPresenter<IPathFindPuzzlePresenter, IPathFindPuzzleView>, PuzzlePiece.IListener
    {
        IPathFindPuzzlePresenter SetPuzzleIndex(int index);
    }

    public class PathFindPuzzlePresenter : IPathFindPuzzlePresenter
    {
        private IPathFindPuzzleView _iPathFindPuzzleView = null;
        private IPathFindPuzzleService _iPathFindPuzzleService = null;
        
        private List<PuzzlePiece> _puzzlePieceList = null;
        private int _puzzleIndex = 0;
        private List<PuzzlePiecePosition> _puzzlePiecePositionList = null;

        private List<Transform> _debugTmList = new();


        public IPathFindPuzzlePresenter Initialize(IPathFindPuzzleView iView)
        {
            _iPathFindPuzzleView = iView;
            _iPathFindPuzzleService = new PathFindPuzzleService();

            _puzzlePieceList = new();
            _puzzlePieceList.Clear();

            return this;
        }

        async UniTask IPresenter<IPathFindPuzzlePresenter, IPathFindPuzzleView>.ActivateAsync()
        {
            await ApplyPuzzlePieceAsync();
        }

        async UniTask IPresenter<IPathFindPuzzlePresenter, IPathFindPuzzleView>.DeactivateAsync()
        {
            _debugTmList?.Clear();

            await UniTask.CompletedTask;
        }

        void IPresenter<IPathFindPuzzlePresenter, IPathFindPuzzleView>.ChainUpdate()
        {

#if UNITY_EDITOR
            if (!_debugTmList.IsNullOrEmpty())
            {
                for (int i = 0; i < _debugTmList.Count - 1; i++)
                {
                    if (_debugTmList[i] != null && _debugTmList[i + 1] != null)
                    {
                        Debug.DrawLine(_debugTmList[i].position, _debugTmList[i + 1].position, Color.green);
                    }
                }
            }
#endif

            if (!_puzzlePieceList.IsNullOrEmpty())
            {
                for (int i = 0; i < _puzzlePieceList.Count; ++i)
                {
                    _puzzlePieceList[i]?.ChainUpdate();
                }
            }
        }

        IPathFindPuzzlePresenter IPathFindPuzzlePresenter.SetPuzzleIndex(int index)
        {
            _puzzleIndex = index;
            return this;
        }
        
        private void AllDeactivatePuzzlePiece()
        {
            if (_puzzlePieceList == null)
                return;

            for (int i = 0; i < _puzzlePieceList.Count; ++i)
            {
                if (_puzzlePieceList[i].IsArranged)
                    continue;

                _puzzlePieceList[i].Deactivate();
            }
        }

        private async UniTask ApplyPuzzlePieceAsync()
        {
            AllDeactivatePuzzlePiece();

            var puzzleData = _iPathFindPuzzleView?.GetPuzzleData(_puzzleIndex);
            if (puzzleData == null) 
                return;

            var piecePositionTm = _iPathFindPuzzleView.PiecePositionVerticalLayoutGroup?.transform; 
            if (!piecePositionTm) 
                return;

            int childCount = piecePositionTm.childCount;
            
            ActivatePuzzlePiecePosition(piecePositionTm, childCount, puzzleData);
            
            for (int i = 0; i < puzzleData.PieceIndices?.Length; ++i)
            {
                if(childCount <= i)
                    continue;
            
                int index = puzzleData.PieceIndices[i];
                await ActivatePuzzlePieceAsync(index);
            }
            
            // // await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            // if (_iPathFindPuzzleService.IsPuzzleSolved(puzzleData, ref _debugTmList))
            //     PuzzleSolved();
        }

        private void ActivatePuzzlePiecePosition(Transform piecePositionTm, int childCount, PathFindPuzzleView.PuzzleData puzzleData)
        {
            if (!_puzzlePiecePositionList.IsNullOrEmpty())
                return;

            if(_puzzlePiecePositionList == null)
                _puzzlePiecePositionList = new();

            _puzzlePiecePositionList?.Clear();

            for (int i = 0; i < childCount; ++i)
            {
                var childTm = piecePositionTm.GetChild(i);
                if (!childTm)
                    continue;

                var puzzlePiecePosition = childTm.GetComponent<PuzzlePiecePosition>();
                if (puzzlePiecePosition == null)
                    continue;

                int pieceItemId = 0;
                if (puzzleData.PieceIndices?.Length > i)
                    pieceItemId = puzzleData.PieceIndices[i];

                puzzlePiecePosition.ApplySize(puzzleData.PieceSize)?
                    .ApplyPuzzlePieceItemId(pieceItemId);
                
                Extensions.SetActive(puzzlePiecePosition.RectTm, puzzleData.PieceIndices?.Length > i);

                _puzzlePiecePositionList?.Add(puzzlePiecePosition);
            }
        }
        
        private PuzzlePiecePosition GetPuzzlePiecePosition(int pieceItemId)
        {
            if (_puzzlePiecePositionList.IsNullOrEmpty())
                return null;

            return _puzzlePiecePositionList.FirstOrDefault(position => position != null && position._puzzlePieceItemId == pieceItemId);
        }

        private async UniTask<PuzzlePiece> CreatePuzzlePieceAsync(PuzzlePiece.Param puzzlePieceParam)
        {
            var puzzlePiece = await UICreator<PuzzlePiece, PuzzlePiece.Param>.Get
                 .SetRootTm(_iPathFindPuzzleView?.PieceRootRectTm)
                 .SetParam(puzzlePieceParam)
                 .CreateAsync();

             if (puzzlePiece == null)
                 return null;
             
             _puzzlePieceList?.Add(puzzlePiece);
             
             return puzzlePiece;
        }

        private async UniTask ActivatePuzzlePieceAsync(int index)
        {
            var puzzleData = _iPathFindPuzzleView?.GetPuzzleData(_puzzleIndex);
            if (puzzleData == null)
                return;

            var puzzlePiecePosition = GetPuzzlePiecePosition(index);
            if (puzzlePiecePosition == null)
                return;

            var puzzlePieceParam = new PuzzlePiece.Param(this, index, puzzleData.PieceSize);
            
            PuzzlePiece puzzlePiece = null;
            
            for(int i = 0; i < _puzzlePieceList?.Count; ++i)
            {
                puzzlePiece = _puzzlePieceList[i];
                if (puzzlePiece == null)
                    continue;

                if (puzzlePiece.Index == index &&
                    !puzzlePiece.IsActivate &&
                    !puzzlePiece.IsArranged)
                    break;

                puzzlePiece = null;
            }
            //var puzzlePiece = _puzzlePieceList?.Find(piece => piece != null && !piece.IsActivate && !piece.IsArranged);
            if (puzzlePiece == null)
                puzzlePiece = await CreatePuzzlePieceAsync(puzzlePieceParam);
            else
                await puzzlePiece.ActivateWithParamAsync(puzzlePieceParam);
            // puzzlePiece?.Activate(puzzlePieceParam);

            // await UniTask.Yield();
           
            //await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            // if (isDelay)
            //     await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(puzzlePiece.RectTm);
            // Canvas.ForceUpdateCanvases();
            
            Extensions.ScreenPointToLocalPointInRectangle(puzzlePiece.RectTm, puzzlePiecePosition.RectTm);
            
            Debug.Log("Piece ");
        }

        private void PuzzleSolved()
        {
            _puzzlePiecePositionList?.Clear();

            EventDispatcher.Dispatch<Quest>(new PathFindPuzzle().WithIsClear(true));
            _iPathFindPuzzleView?.Deactivate();

            CutscenePlayer.Play(_puzzleIndex,
                () =>
                {
                    Debug.Log("Finished Cutscene");
                    MainManager.Instance?.NavMeshSurface?.BuildNavMesh();
                });
        }
        
        #region PuzzlePiece.IListener
        void PuzzlePiece.IListener.Move(int pieceIndex, int pieceItemId, bool isInsert)
        {
            if (isInsert)
            {
                var recipeData = RecipeDataContainer.Instance?.GetDataByResultItemId(pieceItemId);
                if (recipeData == null)
                    return;

                Dictionary<int, int> itemDic = new();

                for (int i = 0; i < recipeData.MaterialItemIds.Length; ++i)
                {
                    int itemId = recipeData.MaterialItemIds[i];
                    int itemCount = recipeData.MaterialItemCounts[i];

                    if (itemDic.ContainsKey(itemId))
                        itemDic[itemId] += itemCount;
                    else
                        itemDic.Add(itemId, itemCount);
                }

                _iPathFindPuzzleService?.RequestRemoveItem(itemDic);

                ActivatePuzzlePieceAsync(pieceIndex).Forget();
            }

            var puzzleData = _iPathFindPuzzleView?.GetPuzzleData(_puzzleIndex);
            if (puzzleData != null)
            {
                if (_iPathFindPuzzleService.IsPuzzleSolved(puzzleData, ref _debugTmList))
                    PuzzleSolved();
            }
        }
        
        void PuzzlePiece.IListener.Remove(int pieceIndex, bool isArranged)
        {
            if(!isArranged)
                ActivatePuzzlePieceAsync(pieceIndex).Forget();

            var puzzleData = _iPathFindPuzzleView?.GetPuzzleData(_puzzleIndex);
            if (puzzleData != null)
            {
                if (_iPathFindPuzzleService.IsPuzzleSolved(puzzleData, ref _debugTmList))
                    PuzzleSolved();
            }
        }

        void PuzzlePiece.IListener.Rotate()
        {
            var puzzleData = _iPathFindPuzzleView?.GetPuzzleData(_puzzleIndex);
            if (puzzleData != null)
            {
                if (_iPathFindPuzzleService.IsPuzzleSolved(puzzleData, ref _debugTmList))
                    PuzzleSolved();
            }
        }
        #endregion
    }
    
    
}

