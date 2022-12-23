using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using NFTGame.Utils;
using System.Threading.Tasks;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(BattleManager))]
    public class BattleManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region events
        public event Action<BattleManager> onPrepareBattleCompleted;
        public event Action<BattleManager,Exception> onPrepareBattleFailed;
        public event Action<BattleManager,Character[]> onBattleStart;
        public event Action<BattleManager,BattleAttack> onBattleAttack;
        public event Action<BattleManager,BattleResult> onBattleEnd; 
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpConfig)]
        private float _delay = 2;

        [BoxGroup(grpComponent),SerializeField,Required]
        private Transform _parent;
        #endregion

        #region property
        [ShowInInspector,BoxGroup(nameof(simulationData)),HideLabel]
        public BattleSimulationData simulationData => _simulationData;

        [ShowInInspector]
        public Character[] characters => _characters;
        #endregion

        #region private
        private GameManager _gameManager;
        private bool _isSimulating;
        private BattleSimulationData _simulationData;
        private Character[] _characters;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _characters  = new Character[2];
            this.LogCompleted(nameof(Init));
        }   
        public async void Simulate(Arena arena = null){
            if(_isSimulating || _characters.IsNullOrEmpty() ||_characters[0].IsNull() || _characters[1].IsNull() || _simulationData.IsNull())
                return;
            
            $"Battle Simulation turn : {_simulationData.steps.Length}, win : {_simulationData.winID}".Log();
            
            //Random Area
            if(arena.IsNull())
                arena = _gameManager.arenaManager.GetRandomArena();

            _isSimulating = true;
            onBattleStart?.Invoke(this,_characters);
            arena.Show();

            //FitCamera
            _gameManager.cameraManager.CropFit(arena.rect);
            
            //char1 left
            _characters[0].transform.position = arena.leftGrid.GetPosition(0);
            _characters[0].SetFaceDirection(arena.leftGrid.direction);
            _characters[0].HideFrame();
            _characters[0].ShowStat();
            _characters[0].Show();

            //char2 right
            _characters[1].transform.position = arena.rightGrid.GetPosition(0);
            _characters[1].SetFaceDirection(arena.rightGrid.direction);
            _characters[1].HideFrame();
            _characters[1].ShowStat();
            _characters[1].Show();

            if(_delay > 0)
                await new WaitForSeconds(_delay);

            var onSkillAttack = default(Action<BattleAttack>);
                onSkillAttack = (battleAttack)=>
                {
                    onBattleAttack?.Invoke(this,battleAttack);
                };

            var skillParam = new Params.SkillParams();

            for (int i = 0; i < _simulationData.steps.Length; i++)
            {
                $"Simulated {i}".Log();
                var skill = _gameManager.skillManager.GetSkill(_simulationData.steps[i].skillID);
                skillParam.attacker       = _simulationData.steps[i].charID == _characters[0].data.id ? _characters[0] : _characters[1];
                skillParam.victim         = skillParam.attacker == _characters[0] ? _characters[1] : _characters[0];
                skillParam.attackerGrid   = skillParam.attacker == _characters[0]? arena.leftGrid  : arena.rightGrid;
                skillParam.victimGrid     = skillParam.attacker == _characters[0]? arena.rightGrid : arena.leftGrid;
                skillParam.attackerDamage = _simulationData.steps[i].damage;
                
                //set layer
                skillParam.attacker.BringForward();

                if(i == _simulationData.steps.Length-1)
                    skillParam.isVictimDeath = true;

                skill.onAttack += onSkillAttack;
                await skill.Use(skillParam);
                skill.onAttack -= onSkillAttack;

                //set layer
                skillParam.attacker.BringBackward();
                
                if(_simulationData.steps[i].coldown > 0)
                    await new WaitForSeconds(_simulationData.steps[i].coldown);
            }

            //Hide Area
            arena.Hide();

            //Hide Character
            for (var i = 0; i < _characters.Length; i++){
                _characters[i]?.Hide();
            }
            
            _isSimulating = false;
            "Simulation ended".Log();

            var win      = _simulationData.winID  == _gameManager.characterManager.mainCharacter.data.id;
            var loseChar = _simulationData.loseID == _characters[0].data.id ? _characters[0] : _characters[1];
            var result = new BattleResult(win,loseChar);
            onBattleEnd?.Invoke(this,result);
        }      
        #endregion

        #region async
        public async void PrepareBattleAsync(string battleId){
            _characters?.Dispose();
            _simulationData?.Dispose();
            _simulationData = null;
            var ex = default(Exception);

            do
            {
                //Get Battle Simulation
                (ex,_simulationData) = await _gameManager.webApi.GetBattleSimulationAsync(battleId,true);
    
                if(_simulationData.IsNull()){
                    await new WaitForSeconds(5);
                }

            } while (_simulationData.IsNull());

            //Get Characters
            (ex,_characters) = await _gameManager.webApi.GetCharactersAsync(new string[]{_simulationData.char1ID,_simulationData.char2ID},true);
            if(!ex.IsNull()){
                onPrepareBattleFailed?.Invoke(this,ex);
                return;
            }

            //Switch position
            if(!_characters.IsNullOrEmpty()){
                if(_characters[1].data.id == _simulationData.char2ID){
                    var tmp_switch = _characters[1];
                    _characters[1] = _characters[0];
                    _characters[0] = tmp_switch;
                    tmp_switch = null;
                }
            }

            //Setup Characters
            for (var i = 0; i < _characters.Length; i++)
            {
                _characters[i]?.Hide();
                _characters[i]?.transform.SetParent(_parent);
            }

            onPrepareBattleCompleted?.Invoke(this);
        }    
        #endregion
        
        #region callbacks
        private void OnDestroy() {
            onPrepareBattleCompleted = null;
            onPrepareBattleFailed = null;
            onBattleStart  = null;
            onBattleAttack = null;
            onBattleEnd    = null;
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

