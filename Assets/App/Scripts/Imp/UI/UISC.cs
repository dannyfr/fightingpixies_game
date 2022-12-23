using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UISC : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpRuntime = "Runtime";
        const string grpComponenet = "Component";
        #endregion

        #region Field
        [SerializeField,FoldoutGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private UIView _view;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private UIButton _btnCall,_btnUniqSC,_btnBattleSC;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private TextMeshProUGUI _contract;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private TMP_InputField _inputMethod,_inputParams;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private BlockChain.BlockChainApi _blockChainAPI;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private string _address;
        private string _abi;
        #endregion

        #region methods
        private void Awake() {
            _blockChainAPI.onInited += OnBlockChainInited;
        }
        private void Start()
        {
            _btnCall.OnClick.OnTrigger.Event.AddListener(Call);
            _btnBattleSC.OnClick.OnTrigger.Event.AddListener(ChangeToBattleSC);
            _btnUniqSC.OnClick.OnTrigger.Event.AddListener(ChangeToUniqSC);
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
        }
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        public void Show()
        {
            if(_view.IsVisible)
                return;

            if(_showByViewName)
            {
                Doozy.Engine.GameEventMessage.SendEvent(_view.ViewName); 
            }
            else
            {
                _view.Show();
            }  
        }
        public void Hide()
        {
           _view.Hide();
        }    
        public void Refresh(){
            _contract.text = _address;
            _inputMethod.text = null;
            _inputParams.text = null;
        }
        #endregion

        #region Buttons
        private async void Call()
        {
            if(_inputMethod.text.IsNullOrEmpty())
                return;

            var args = _inputParams.text.SplitBy();
            var parameter = "";
            for (var i = 0; i < args.Length; i++)
            {
                var separator = (i < args.Length - 1) ?",":"";
                parameter += $"\"{args[i]}\"{separator}";
            }
            parameter = $"[{parameter}]";

            await _blockChainAPI.WriteContract(_address,_abi,_inputMethod.text,parameter);
        }
        private void ChangeToUniqSC()
        {
            _address = _blockChainAPI.config.uniqSC.address;
            _abi     = _blockChainAPI.config.uniqSC.abi;
            _btnBattleSC.gameObject.SetActive(true);
            _btnUniqSC.gameObject.SetActive(false);
            Refresh();
        }

        private void ChangeToBattleSC()
        {
            _address = _blockChainAPI.config.battleSC.address;
            _abi     = _blockChainAPI.config.battleSC.abi;
            _btnBattleSC.gameObject.SetActive(false);
            _btnUniqSC.gameObject.SetActive(true);
            Refresh();
        }
        #endregion

        #region callback
        private void OnDestroy() {
            if(_blockChainAPI){
                 _blockChainAPI.onInited -= OnBlockChainInited;
            }
        }
        private void OnVisibilityChange(float visibility)
        {
            if(visibility > 0 && !_isShowing)
            {
                OnShow();
                _isShowing = true;
            } 
            else if(_isShowing && visibility == 0)
            {
                _isShowing = false;
                //OnHide();
            }   
        }
        private void OnShow()
        {
            Refresh();
        }
        private void OnBlockChainInited(IBlockChainAPI api)
        {
            ChangeToUniqSC();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }
        #endregion
    }
}