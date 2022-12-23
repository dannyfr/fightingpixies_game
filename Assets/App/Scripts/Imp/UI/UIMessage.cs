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
    public class UIMessage : SerializedMonoBehaviour,IDisposable
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
        private UIButton _btnYes,_btnNo;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private TextMeshProUGUI _text;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private UIView _view;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private Image _icon;

        [SerializeField,Required,FoldoutGroup(grpComponenet)]
        private TextMeshProUGUI _title;
        #endregion

        #region private
        private bool _yes,_no,_isShowing;
        private GameManager _gameManager;
        #endregion

        #region methods
        private void Start()
        {
            _btnYes.OnClick.OnTrigger.Event.AddListener(()=> _yes = true);
            _btnNo.OnClick.OnTrigger.Event.AddListener(()=> _no = true);
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
        public async Task<bool> Show(string message,bool showNoBtn = true,string title = null)
        {
            try 
            {
                _title.text = title;
                _title.gameObject.SetActive(!title.IsNullOrEmpty());
                _icon.gameObject.SetActive(title.IsNullOrEmpty());

                _btnNo.gameObject.SetActive(showNoBtn);
                _yes = _no = false;
                _text.text = message;
                
                Show();
                await new WaitUntil(()=> _yes || _no);
                Hide();

                return _yes; 
            } 
            catch (Exception ex) 
            {
                ex.Message.LogError();
                return _yes; 
            }
        }       
        public void Hide()
        {
           _view.Hide();
        }    
        #endregion

        #region callback
        private void OnDestroy()
        {
            _yes =_no       = false;
            _isShowing      = false;
            _gameManager    = null;
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
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
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