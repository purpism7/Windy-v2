using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;

namespace UI.Puzzle
{
    public class PuzzleGrid : MonoBehaviour
    {
        [SerializeField] private GameObject puzzleGridCellGameObj = null;

        [SerializeField] private int row = 0;
        [SerializeField] private int column = 0;
        [SerializeField] private float cellSize = 200f;

        private List<GameObject> _rowGameObjList = null;
        private List<PuzzleGridCell> _puzzleGridCellList = null;

        public IReadOnlyList<PuzzleGridCell> PuzzleGridCellList => _puzzleGridCellList;
        
        public async UniTask CreateAsync()
        {
            CreateRow();
            await UniTask.Yield();
            
            CreateColumn();
        }

        public void RemoveAllChild()
        {
            _puzzleGridCellList?.Clear();
            
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                if (transform.GetChild(i))
                {
                    transform.GetChild(i).RemoveAllChild();
                    transform.GetChild(i).Remove();
                }
            }
            
            _rowGameObjList?.Clear();
            
        }
        
        private void CreateRow()
        {
            if (_rowGameObjList == null)
            {
                _rowGameObjList = new();
                _rowGameObjList.Clear();
            }

            if (!_rowGameObjList.IsNullOrEmpty())
            {
                for (int i = 0; i < _rowGameObjList?.Count; ++i)
                {
                    _rowGameObjList[i].SetActive(false);
                }
            }
            
            for (int i = 0; i < row; ++i)
            {
                int index = i + 1;
                if (_rowGameObjList != null &&
                    _rowGameObjList?.Count > i)
                {
                    _rowGameObjList[i].name = $"{index}";
                    _rowGameObjList[i].transform.SetActive(true);
                    continue;
                }
                
                var rowGameObj = new GameObject($"{index}");
                if (rowGameObj)
                {
                    rowGameObj.transform.SetParent(transform);

                    var horizontalLayoutGroup = rowGameObj.transform.AddOrGetComponent<HorizontalLayoutGroup>();
                    if (horizontalLayoutGroup != null)
                    {
                        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                        horizontalLayoutGroup.childForceExpandWidth = true;
                        horizontalLayoutGroup.childForceExpandHeight = true;
                    }
                    
                    _rowGameObjList?.Add(rowGameObj);
                }
            }
        }

        private void CreateColumn()
        {
            if (_puzzleGridCellList == null)
                _puzzleGridCellList = new();
            
            _puzzleGridCellList?.Clear();
            
            int index = 1;
            for (int i = 0; i < _rowGameObjList.Count; ++i)
            {
                if(!_rowGameObjList[i].activeSelf)
                    continue;

                for (int j = 0; j < _rowGameObjList[i].transform.childCount; ++j)
                {
                    _rowGameObjList[i].transform.GetChild(j).transform.SetActive(false);
                }
                
                for (int j = 0; j < column; ++j)
                {
                    PuzzleGridCell puzzleGridCell = null;
                    if (_rowGameObjList[i].transform.childCount > j)
                        puzzleGridCell = _rowGameObjList[i].transform.GetChild(j)?.GetComponent<PuzzleGridCell>();
                    else
                    {
                        var gridCellGameObj = Instantiate(puzzleGridCellGameObj, _rowGameObjList[i].transform);
                        if (gridCellGameObj)
                            puzzleGridCell = gridCellGameObj.GetComponent<PuzzleGridCell>();
                    }

                    puzzleGridCell?.SetIndex(index)?.SetSize(cellSize);
                    puzzleGridCell?.transform.SetActive(true);
                    
                    _puzzleGridCellList?.Add(puzzleGridCell);

                    ++index;
                }
            }
        }

        public void RefreshIndex()
        {
            if (_puzzleGridCellList == null)
                _puzzleGridCellList = new();
            
            _puzzleGridCellList?.Clear();
            
            int index = 1;
            for (int i = 0; i < row; ++i)
            {
                var childTm = transform.GetChild(i);
                if (!childTm.gameObject.activeSelf)
                    continue;

                for (int j = 0; j < column; ++j)
                {
                    PuzzleGridCell puzzleGridCell = null;
                    if (childTm.childCount > j)
                        puzzleGridCell = childTm.GetChild(j)?.GetComponent<PuzzleGridCell>();

                    puzzleGridCell?.SetIndex(index);
                    
                    _puzzleGridCellList?.Add(puzzleGridCell);
                    
                    ++index;
                }
            }
        }

        public PuzzleGridCell GetPuzzleGridCell(int index)
        {
            for (int i = 0; i < _puzzleGridCellList.Count; ++i)
            {
                var puzzleGridCell = _puzzleGridCellList[i];
                if(puzzleGridCell == null)
                    continue;

                if (puzzleGridCell.Index == index)
                    return puzzleGridCell;
            }

            return null;
        }
    }
}

