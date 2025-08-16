
using System;
using System.Numerics;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

using GameSystem;
using Table;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UI.Puzzle
{
    public class PuzzlePiece : Common.Component<PuzzlePiece.Param>, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        public new class Param : Common.Component.Param
        {
            public IListener IListener { get; private set; } = null;
            public int Index { get; private set; } = 0;
            public float Size { get; private set; } = 0;

            public Param(IListener iListener, int index, float size)
            {
                IListener = iListener;
                Index = index;
                Size = size;
            }
        }

        public interface IListener
        {
            void Move(int pieceIndex, int pieceItemId, bool isInsert);
            void Remove(int pieceIndex, bool isArranged);
            void Rotate();
        }

        [Serializable]
        public class PuzzlePieceData
        {
            [SerializeField] private int index = 0;
            [SerializeField] private int itemId = 0;
            [SerializeField] private RectTransform puzzlePieceTm = null;

            public int Index => index;
            public int ItemId => itemId;
            public Transform PuzzlePieceTm => puzzlePieceTm;
        }

        [SerializeField] private RectTransform puzzlePieceRootRectTm = null;
        [SerializeField] private Button btn = null;
        [SerializeField] private RectTransform disabledRectTm = null;
        [SerializeField] private PuzzlePieceData[] puzzlePieceDatas = null;
        
        private BoxCollider2D _boxCollider2D = null;
        private bool _isDrag = false;
        private bool _isRotating = false;
        private PuzzleGridCell _closetPuzzleGridCell = null;
        private int _puzzleGridCellIndex = 0;
        private int _itemId = 0;
        
        public PuzzlePoint[] PuzzlePoints { get; private set; } = null;
        public RectTransform RectTm { get; private set; } = null;
        public int Index => _param != null ? _param.Index : 0;
        
        public override async UniTask InitializeAsync()
        {
            RectTm = GetComponent<RectTransform>();
            _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
            
            btn?.onClick?.AddListener(OnClick);

            await UniTask.CompletedTask;
        }

        public override async UniTask ActivateAsync()
        {
            if (_param != null)
            {
                if(RectTm)
                    RectTm.sizeDelta = Vector2.one * _param.Size;   
                
                for (int i = 0; i < puzzlePieceDatas?.Length; ++i)
                {
                    var puzzlePieceData = puzzlePieceDatas[i];
                    if(puzzlePieceData == null)
                        continue;

                    if (puzzlePieceData.Index == _param.Index)
                    {
                        _itemId = puzzlePieceData.ItemId;
                        PuzzlePoints = puzzlePieceData.PuzzlePieceTm.GetComponentsInChildren<PuzzlePoint>();
                    }

                    Extensions.SetActive(puzzlePieceData.PuzzlePieceTm, puzzlePieceData.Index == _param.Index);
                }
            }

            var canUse = CanUsePuzzlePiece;
            Extensions.SetActive(disabledRectTm, !canUse);

            await UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void ChainUpdate()
        {
            if (!_isDrag)
                return;

            base.ChainUpdate();
            
            if (_boxCollider2D == null)
                return;
            
            var colliders = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0);
            if (colliders.IsNullOrEmpty())
                return;
            
            PuzzleGridCell closetPuzzleGridCell = null;
            PuzzleGridCell lowerIndexPuzzleGridCell = null;

            for (int i = 0; i < colliders.Length; ++i)
            {
                var collider = colliders[i];
                if(collider == null)
                    continue;
                
                if(_boxCollider2D == collider)
                    continue;
                
                var puzzleGridCell = collider.GetComponent<PuzzleGridCell>();
                if (puzzleGridCell != null)
                {
                    if (lowerIndexPuzzleGridCell == null ||
                        lowerIndexPuzzleGridCell.Index < puzzleGridCell.Index)
                        lowerIndexPuzzleGridCell = puzzleGridCell;
                }
                
                var dist = _boxCollider2D.Distance(collider);
                if (dist.isOverlapped)
                    closetPuzzleGridCell = puzzleGridCell;
            }

            if (closetPuzzleGridCell == null)
                closetPuzzleGridCell = lowerIndexPuzzleGridCell;
            
            if (closetPuzzleGridCell != null)
            {
                if (!closetPuzzleGridCell.ContainPuzzlePiece)
                {
                    _closetPuzzleGridCell?.AddPuzzlePieceItemId(0);
                    _closetPuzzleGridCell = closetPuzzleGridCell;
                }
            }
            else
            {
                _closetPuzzleGridCell?.AddPuzzlePieceItemId(0);
                _closetPuzzleGridCell = null;
            }
              
        }

        // 퍼즐 조각을 사용 할 수 있는지 여부.
        private bool CanUsePuzzlePiece
        {
            get
            {
                if (_puzzleGridCellIndex > 0)
                    return true;

                if (_param == null)
                    return false;

                var recipeDataContainer = RecipeDataContainer.Instance;
                if (recipeDataContainer == null)
                    return false ;

                return recipeDataContainer.HasRequiredIngredients(_itemId);
            }
        }

        // private void ApplyPuzzlePieceImage()
        // {
        //     if (_data == null)
        //         return;
        //     
        //     puzzlePieceImg?.SetActive(false);
        //     
        //     var itemData = ItemDataContainer.Instance?.GetData(_data.ItemId);
        //     if (itemData == null)
        //         return;
        //
        //     var sprite = AtlasManager.Instance?.GetSprite(EAtlasKey.UIItems, itemData.ImageName);
        //     if (sprite == null)
        //         return;
        //
        //     if(puzzlePieceImg != null)
        //         puzzlePieceImg.sprite = sprite;
        //     
        //     puzzlePieceImg?.SetActive(true);
        // }

        private void OnClick()
        {
            if (!CanUsePuzzlePiece)
                return;

            if (!puzzlePieceRootRectTm)
                return;

            if (_closetPuzzleGridCell == null)
                return;

            if (_isRotating)
                return;

            _isRotating = true;
            
            float currentZ = Mathf.Round(puzzlePieceRootRectTm.localEulerAngles.z / 90f) * 90f;
            float targetZ = (currentZ - 90f + 360f) % 360f;  // 0 → 270 → 180 → 90 → 0 ...
            Vector3 targetEuler = new Vector3(0, 0, targetZ);

            DOTween.Sequence()
                .Append(puzzlePieceRootRectTm.DOLocalRotate(targetEuler, 0.2f).SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    _isRotating = false;
                    _param?.IListener?.Rotate();
                });
        }

        public bool IsArranged
        {
            get
            {
                return _closetPuzzleGridCell != null;
            }
        }

        #region DragHandler
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if(!CanUsePuzzlePiece)
                return;

            _isDrag = true;

            transform.SetAsLastSibling(); // 드래그 시작 시 해당 오브젝트를 가장 위로 올림
        }
        
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!_isDrag)
                return;

            if(RectTm)
                RectTm.anchoredPosition += eventData.delta / UIManager.Instance.Canvas.scaleFactor;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!_isDrag)
                return;

            _isDrag = false;
            
            if (!RectTm)
                return;

            if (_closetPuzzleGridCell != null)
            {
                bool isInsert = _puzzleGridCellIndex <= 0;
               
                RectTm.ScreenPointToLocalPointInRectangle(_closetPuzzleGridCell.RectTm);
                _closetPuzzleGridCell.AddPuzzlePieceItemId(_param.Index);
                _puzzleGridCellIndex = _closetPuzzleGridCell.Index;

                _param?.IListener?.Move(_param.Index, _itemId, isInsert);

                return;
            }
            else
            {
                Deactivate();

                bool isArranged = _puzzleGridCellIndex > 0;
                _param?.IListener?.Remove(_param.Index, isArranged);

                _closetPuzzleGridCell = null;
                _puzzleGridCellIndex = 0;
            }
        }
        #endregion
    }
}

