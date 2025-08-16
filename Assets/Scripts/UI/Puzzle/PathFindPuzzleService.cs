using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

using Common;
using GameSystem;
using GameSystem.Event;
using Network;

using static UI.Puzzle.PathFindPuzzleView;

namespace UI.Puzzle
{
    public interface IPathFindPuzzleService
    {
        bool IsPuzzleSolved(PuzzleData puzzleData, ref List<Transform> debugTmList);

        void RequestRemoveItem(Dictionary<int, int> itemDic);
    }

    public class PathFindPuzzleService : IPathFindPuzzleService, IApiResponse<Network.Api.RemoveItem.Response>
    {
        private RaycastHit2D[] _hits = new RaycastHit2D[5];
        
        bool IPathFindPuzzleService.IsPuzzleSolved(PuzzleData puzzleData, ref List<Transform> debugTmList)
        {
            if (puzzleData == null ||
                puzzleData.PuzzleGrid == null)
                return false;

            if (puzzleData.PuzzleGrid.PuzzleGridCellList == null)
                return false;

            var uiCamera = UIManager.Instance?.UICamera;
            if (uiCamera == null)
                return false;

            var canvas = UIManager.Instance?.Canvas;
            if (canvas == null)
                return false;

            PuzzlePoint targetPuzzlePoint = puzzleData.StartPuzzlePoint;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();

            var direction = Vector3.zero;

#if UNITY_EDITOR
            debugTmList?.Clear();
            debugTmList?.Add(targetPuzzlePoint.transform);
#endif
            while (targetPuzzlePoint != null)
            {
                results.Clear();
                pointerData.position = uiCamera.WorldToScreenPoint(targetPuzzlePoint.transform.position);
                raycaster?.Raycast(pointerData, results);

                bool isConnected = false;
                PuzzlePiece puzzlePiece = null;

                foreach (var result in results)
                {
                    var puzzlePoint = result.gameObject.GetComponent<PuzzlePoint>();
                    if (puzzlePoint == null)
                        continue;

                    if (puzzlePoint == targetPuzzlePoint)
                        continue;

                    puzzlePiece = puzzlePoint.GetComponentInParent<PuzzlePiece>();
                    if (puzzlePiece == null)
                        continue;

                    targetPuzzlePoint = null;
                    for (int i = 0; i < puzzlePiece.PuzzlePoints.Length; ++i)
                    {
                        var puzzlePiecePoint = puzzlePiece.PuzzlePoints[i];
                        if (puzzlePiecePoint == null)
                            continue;

                        if (puzzlePiecePoint._EPoint == puzzlePoint._EPoint)
                            isConnected = true;
                        else
                        {
                            targetPuzzlePoint = puzzlePiecePoint;
                            direction = GetDirection(puzzlePiecePoint.transform, puzzlePiecePoint.EDirection);
                        }
                    }

                    break;
                }

                if (!isConnected)
                    break;

#if UNITY_EDITOR
                if (targetPuzzlePoint != null)
                    debugTmList?.Add(targetPuzzlePoint.transform);
#endif
            }

            if (targetPuzzlePoint != null)
            {
                var targetTm = targetPuzzlePoint.transform;

                int hitCount = Physics2D.RaycastNonAlloc(targetTm.position, direction, _hits, 1f, LayerMask.GetMask("UI"));
                if (hitCount > 0)
                {
                    foreach (var hit in _hits)
                    {
                        if (hit.collider == null)
                            continue;

                        var puzzlePoint = hit.collider.GetComponent<PuzzlePoint>();
                        if (puzzlePoint != null)
                        {
                            if (puzzlePoint._EPoint == PuzzlePoint.EPoint.End)
                            {
                                Debug.Log("Clear Puzzle");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private Vector3 GetDirection(Transform tm, EDirection eDirection)
        {
            switch (eDirection)
            {
                case EDirection.Right:
                    return tm.right;

                case EDirection.Left:
                    return tm.right * -1f;

                case EDirection.Up:
                    return tm.up;

                case EDirection.Down:
                    return tm.up * -1f;
            }

            return Vector3.zero;
        }

        void IPathFindPuzzleService.RequestRemoveItem(Dictionary<int, int> itemDic)
        {
            var request = Network.Api.RemoveItem.CreateRequest<Network.Api.RemoveItem>(
                new Network.Api.RemoveItem.Request
                {
                    ItemDic = itemDic,
                });

            ApiClient.Instance?.RequestPost(request, this);
        }

        void IApiResponse<Network.Api.RemoveItem.Response>.OnResponse(Network.Api.RemoveItem.Response data, bool isSuccess, string errorMessage)
        {
            EventDispatcher.Dispatch<Item>(new RemoveItem(data.ItemDic));
        }
    }
}

