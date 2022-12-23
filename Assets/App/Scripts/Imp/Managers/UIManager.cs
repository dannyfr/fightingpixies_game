using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using NFTGame.UI;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(UIManager))]
    public class UIManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpRuntime = "Runtime";
        const string grpComponent = "Component";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpComponent),Required]
        private UIAuth _uiAuth;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIMainMenu _uiMainMenu;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIReward _uiReward;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIRewardPreview _uiRewardPreview;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UICreator _uiCreator;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UISetting _uiSetting;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIHistory _uiHistory;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIClaim _uiClaim;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIHeader _uiHeader;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIPixies _uiPixies;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIPixiesDetails _uiPixiesDetails;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIMatchMaking _uiMatchMaking;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIPreBattle _uiPreBattle;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIBattle _uiBattle;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIVictory _uiVictory;

        [SerializeField,BoxGroup(grpComponent),Required]
        public  UIDefeat _uiDefeat; 

        [SerializeField,BoxGroup(grpComponent),Required]
        private UILoading _uiLoading;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UIMessage _uiMessage;

        [SerializeField,BoxGroup(grpComponent),Required]
        private UISC _uiSC;
        #endregion

        #region property
        public UIAuth uiAuth => _uiAuth;
        public UICreator uiCreator => _uiCreator;
        public UIReward uiReward => _uiReward;
        public UIRewardPreview uiRewardPreview => _uiRewardPreview;
        public UI.UIMainMenu uiMainMenu => _uiMainMenu;
        public UISetting uiSetting => _uiSetting;
        public UIHistory uIHistory => _uiHistory;
        public UIPixies uiPixies =>_uiPixies;
        public UIPixiesDetails uiPixiesDetails =>_uiPixiesDetails;
        public UIMatchMaking uiMatchMaking => _uiMatchMaking;
        public UIPreBattle uiPreBattle => _uiPreBattle;
        public UIBattle uiBattle => _uiBattle;
        public UIVictory uiVictory => _uiVictory;
        public UIDefeat uiDefeat => _uiDefeat;
        public UILoading uiLoading => _uiLoading;
        public UIMessage uiMessage => _uiMessage;
        #endregion

        #region private
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager    = gameManager;
            _uiAuth.Init(gameManager);
            _uiReward.Init(gameManager);
            _uiRewardPreview.Init(gameManager);
            _uiCreator.Init(gameManager);
            _uiMainMenu.Init(gameManager);
            _uiSetting.Init(gameManager);
            _uiHistory.Init(gameManager);
            _uiClaim.Init(gameManager);
            _uiHeader.Init(gameManager);
            _uiPixies.Init(gameManager);
            _uiPixiesDetails.Init(gameManager);
            _uiMatchMaking.Init(gameManager);
            _uiPreBattle.Init(gameManager);
            _uiBattle.Init(gameManager);
            _uiLoading.Init(gameManager);
            _uiMessage.Init(gameManager);
            _uiVictory.Init(gameManager);
            _uiDefeat.Init(gameManager);
            _uiSC.Init(gameManager);
               
            this.LogCompleted(nameof(Init));
        }    
        #endregion

        #region callback
        private void OnDestroy() {
            _uiCreator = null;
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


