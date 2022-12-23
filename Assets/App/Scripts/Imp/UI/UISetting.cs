using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;
using DarkTonic.MasterAudio;
using UnityEngine.UI;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UISetting : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,BoxGroup(grpConfig),SoundGroup]
        private string _tickSfx;

        [SerializeField,BoxGroup(grpConfig),SoundGroup]
        private string[] _bgm,_sfx;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;
        
        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textVersion;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Slider _bgmSlider,_sfxSlider;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private float[] _initSfxVolume,_initBgmVolume;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _textVersion.text = $"Version {Application.version}";
              
            _initBgmVolume = new float[_bgm.Length];
            for (var i = 0; i < _initBgmVolume.Length; i++)
            {
                _initBgmVolume[i] = MasterAudio.GetGroupVolume(_bgm[i]);
            }

            _initSfxVolume = new float[_sfx.Length];
            for (var i = 0; i < _initSfxVolume.Length; i++)
            {
                _initSfxVolume[i] = MasterAudio.GetGroupVolume(_sfx[i]);
            }
        
            _bgmSlider.value = _bgmSlider.maxValue;
            _sfxSlider.value = _sfxSlider.maxValue;

            _bgmSlider.onValueChanged.AddListener(OnBGMValueChange);
            _sfxSlider.onValueChanged.AddListener(OnSFXValueChange);
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
        #endregion
       
        #region Callback
        private void OnDestroy()
        { 
           
        }
        private void OnVisibilityChange(float visibility)
        {
            if (visibility > 0 && !_isShowing)
            {
                OnShow();
                _isShowing = true;
            }
            else if(_isShowing && visibility == 0)
            {
                _isShowing = false;
                OnHide();
            }
        }
        private void OnShow()
        {
            
        }
        private void OnHide()
        {

        }       
        private void OnSFXValueChange(float value)
        {
            MasterAudio.PlaySound(_tickSfx);

            for (var i = 0; i < _sfx.Length; i++)
            {
                MasterAudio.SetGroupVolume(_sfx[i],value/_sfxSlider.maxValue * _initSfxVolume[i]);
            }
        }
        private void OnBGMValueChange(float value)
        {
            MasterAudio.PlaySound(_tickSfx);

            for (var i = 0; i < _bgm.Length; i++)
            {
                MasterAudio.SetGroupVolume(_bgm[i],value/_bgmSlider.maxValue * _initBgmVolume[i]);
            }
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