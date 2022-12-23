using UnityEngine;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using Sirenix.OdinInspector;

namespace NFTGame.UI
{
    public class UIPixiesCellView : EnhancedScrollerCellView
    {
        #region fields
        [SerializeField,ListDrawerSettings(Expanded = true,DraggableItems = false)]
        private UIPixiesRowCellView[] _rowCellViews;
        #endregion

        #region property
        public int count => _rowCellViews.Length;
        #endregion

        #region private
        private GameManager _gameManager;
        private bool _inited;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            if(_inited)
                return;

            _gameManager = gameManager;
            
            for (var i = 0; i < _rowCellViews.Length; i++){
                _rowCellViews[i].Init(gameManager);
            }
                
            _inited = true;
        }
        public void SetData(UIPixiesData[] data, int startingIndex)
        {
            for (var i = 0; i < _rowCellViews.Length; i++)
            {
                _rowCellViews[i].SetData(startingIndex + i < data.Length ? data[startingIndex + i] : null);
            }
        }
        public void Refresh(int index = -1){
            if(index < 0){
                for (var i = 0; i < _rowCellViews.Length; i++)
                {
                    _rowCellViews[i].Refresh();
                }
            }else if(index < _rowCellViews.Length){
                _rowCellViews[index].Refresh();
            }
        }
        #endregion
    }
}