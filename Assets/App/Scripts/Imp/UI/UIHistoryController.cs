using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using Evesoft;
using System;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIHistoryController : SerializedMonoBehaviour, IEnhancedScrollerDelegate
    {
        #region const
        const string grpComponent = "Component";
        const string grpCellView = "CellView";

        #endregion

        #region private fields
        [SerializeField,BoxGroup(grpComponent)]
        private EnhancedScroller _scroller;

        [SerializeField,BoxGroup(grpCellView),LabelText("Prefab")]
        private UIHistoryCellView _cellViewPrefab;

        [SerializeField,BoxGroup(grpCellView),LabelText("Size")]
        private float _cellViewSize = 200;
        #endregion

        #region private
        private UIHistoryData[] _data;
        private GameManager _gameManager;
        private int _numberOfCellsPerRow;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _gameManager.historyManager.onPullHistoryonCompleted += OnGetHistoryCompleted;
            _gameManager.historyManager.onPullHistoryonFailed += OnGetHistoryFailed;
        }
        private void Start()
        {
            _numberOfCellsPerRow = _cellViewPrefab.count;
            _scroller.Delegate = this;
        }
        #endregion

        #region EnhancedScroller Handlers
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _data.IsNullOrEmpty()? 0 : Mathf.CeilToInt((float)_data.Length / (float)_numberOfCellsPerRow);
        }
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return _cellViewSize;
        }
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = scroller.GetCellViewAtDataIndex(dataIndex) as UIHistoryCellView;
            if(cellView.IsNull()){
                cellView = scroller.GetCellView(_cellViewPrefab) as UIHistoryCellView;
                cellView.Init(_gameManager);
                cellView.name = "Cell " + (dataIndex * _numberOfCellsPerRow).ToString() + " to " + ((dataIndex * _numberOfCellsPerRow) + _numberOfCellsPerRow - 1).ToString();
                cellView.SetData(_data[dataIndex * _numberOfCellsPerRow]);
            }
            return cellView;
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            if(_gameManager){
                _gameManager.historyManager.onPullHistoryonCompleted -= OnGetHistoryCompleted;
                _gameManager.historyManager.onPullHistoryonFailed    -= OnGetHistoryFailed;
            }
        }
        private void OnGetHistoryCompleted(HistoryManager hm)
        {
            if(!hm.history.IsNullOrEmpty()){
                if(_data.IsNull() || _data.Length  != hm.history.Length)
                    _data = new UIHistoryData[hm.history.Length];
                
                for (var i = 0; i < _data.Length; i++)
                {
                    if(_data[i].IsNull())
                        _data[i] = new UIHistoryData();
                    
                    _data[i].history = hm.history[i];
                }
            }else{
                _data = null;
            }

            _scroller.ReloadData();
        }
        private void OnGetHistoryFailed(HistoryManager manager, Exception ex)
        {
            _data = null;
            _scroller.ReloadData();
        }
        #endregion
    }
}
