using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(GameManager))]
    public class GameManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        const string grpTest = "Test";
        #endregion

        #region field
        [BoxGroup(grpConfig),SerializeField]
        private Environment _environment;

        [BoxGroup(grpComponent),SerializeField,Required]
        private IWebAPI _webApi;

        [BoxGroup(grpComponent),SerializeField,Required]
        private IBlockChainAPI _blockChainApi;

        [BoxGroup(grpComponent),SerializeField,Required]
        private CameraManager _cameraManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private SoundManager _soundManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private AuthManager _authManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private RewardManager _rewardManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private HistoryManager _historyManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private AppDataManager _appDataManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private MatchMakingManager _matchMakingManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private CharacterManager _characterManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private ArenaManager _arenaManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private VFXManager _vfxManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private SkillManager _skillManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private BattleManager _battleManager;

        [BoxGroup(grpComponent),SerializeField,Required]
        private UIManager _uiManager;

        [BoxGroup(grpTest),SerializeField,Required]
        private bool _enableLog = true;
        #endregion

        #region property
        public Environment environment => _environment;
        public CameraManager cameraManager => _cameraManager;
        public SoundManager soundManager => _soundManager;
        public RewardManager rewardManager => _rewardManager;
        public HistoryManager  historyManager => _historyManager;
        public AuthManager authManager => _authManager;
        public IWebAPI webApi => _webApi;
        public IBlockChainAPI blockChainApi => _blockChainApi;
        public AppDataManager appDataManager => _appDataManager;
        public MatchMakingManager matchMakingManager => _matchMakingManager;
        public CharacterManager characterManager => _characterManager;
        public ArenaManager arenaManager => _arenaManager;
        public VFXManager vfxManager => _vfxManager;
        public SkillManager skillManager => _skillManager;
        public BattleManager battleManager => _battleManager;
        public UIManager uiManager => _uiManager;
        #endregion

        #region methods
        private void Start()
        {
            Init();
        }
        public void Init()
        {
            DebugExtend.debugable = _enableLog;

            _webApi.Init(this);
            _blockChainApi.Init(this);
            _cameraManager.Init(this);
            _soundManager.Init(this);
            _rewardManager.Init(this);
            _historyManager.Init(this);
            _authManager.Init(this);
            _appDataManager.Init(this);
            _matchMakingManager.Init(this);
            _characterManager.Init(this);
            _arenaManager.Init(this);
            _vfxManager.Init(this);
            _skillManager.Init(this);
            _battleManager.Init(this);
            _uiManager.Init(this);

            this.LogCompleted(nameof(Init));
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