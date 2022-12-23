using System;
using System.Threading.Tasks;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;
using TMPro;

namespace NFTGame.Dummy.Wallet
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class Wallet : SerializedMonoBehaviour,IDisposable
    {
            #region const
            const string grpConfig = "Config";
            const string grpRuntime = "Runtime";
            const string grpComponenet = "Component";
            #endregion

            #region Field
            [SerializeField,Required,FoldoutGroup(grpComponenet)]
            private UIView _view;

            [SerializeField,Required,FoldoutGroup(grpComponenet)]
            private UIButton _btnYes,_btnNo;
            
            [SerializeField,Required,FoldoutGroup(grpComponenet)]
            private TextMeshProUGUI _textMessage;
            
            #endregion

            #region private
            private bool _yes,_no,_isShowing;
            #endregion

            #region methods
            private void Start()
            {
                _btnYes.OnClick.OnTrigger.Event.AddListener(()=> _yes = true);
                _btnNo.OnClick.OnTrigger.Event.AddListener(()=> _no = true);
            }
            public void Show()
            {
                if(_view.IsVisible)
                    return;

                _view.Show();
            }
            public async Task<bool> Show(string message)
            {
                _textMessage.text = message;
                _yes = _no = false;
                
                Show();
                await new WaitUntil(()=> _yes || _no);
                Hide();

                return _yes; 
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

