using System;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using Sirenix.OdinInspector;
using TMPro;
using Evesoft;


namespace NFTGame.UI
{
    [HideMonoScript]
    public class UIHistoryCellView : EnhancedScrollerCellView
    {
        #region const
        const string grpConfig = "Config";
        const string grpRequired = "Required";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpConfig)]
        private Color _colorName,_colorWin,_colorLose;

        [SerializeField,BoxGroup(grpRequired)]
        private TextMeshProUGUI _date,_desciption;
        #endregion

        #region property
        public int count => 1;
        #endregion

        #region private
        private UIHistoryData _data;
        private GameManager _gameManager;
        private bool _isRefreshing;
        private bool _isRequiredRefresh;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
        }
        public void SetData(UIHistoryData data)
        {
            _data = data;
            Refresh();
        }
        public void Refresh(){
            if(_isRefreshing){
                _isRequiredRefresh = true;
                return;
            }
                
            _isRefreshing = true;
            _date.text  = _data.history.date.ToString("dd MMM yyyy H:mm");;
            var address = _gameManager.blockChainApi.account;
            var tokenId   = default(string);
            var opponent  = default(string);
            var win       = _data.history.IsWon(address);
            _data.history.GetTokenAndOpponent(address,out tokenId,out opponent);

            if(tokenId.IsNullOrEmpty() || opponent.IsNullOrEmpty()){
                _desciption.text = "-";
                return;
            }

            RefreshDetail(win,tokenId,opponent);

            _isRefreshing = false;
            if(_isRequiredRefresh){
                _isRequiredRefresh = false;
                Refresh();
            }else{
                RefreshNameAsync(win,new string[]{tokenId,opponent});
            }
        }
        
        private async void RefreshNameAsync(bool win,string[] tokenIds){
            (var ex,var names) = await _gameManager.webApi.GetCharactersNameAsync(tokenIds,true);
            if(!ex.IsNull())
                return;
            
            if(names.IsNullOrEmpty() || names.Length < 2)
                return;

            RefreshDetail(win,names[0],names[1]);
        }
        private void RefreshDetail(bool win,string userName,string opponent){
            var isWin       = win;
            var status      = isWin?"won":"lost";
            var colorName   = ColorUtility.ToHtmlStringRGB(_colorName);
            var colorStatus = ColorUtility.ToHtmlStringRGB(isWin?_colorWin:_colorLose);
            _desciption.text = $"Your Pixies Fighter, <color=#{colorName}>{userName}</color> has <color=#{colorStatus}>{status}</color> a battle against <color=#{colorName}>{opponent}</color>";
        }
        #endregion
    }
}