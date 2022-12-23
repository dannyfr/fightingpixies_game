using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using Evesoft;
using DarkTonic.MasterAudio;

namespace NFTGame.Skill
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigSkill + nameof(BasicAttack), fileName = nameof(BasicAttack))]
    public class BasicAttack : SerializedScriptableObject, ISkill
    {
        #region const
        const string grpMove = "Move To Target";
        const string grpDamage = "Damage";
        const string grpSlash = "Slash";
        const string grpHit = "Hit";
        const string grpBlink = "Blink";
        const string grpCamShake = "Cam Shake";
        const string grpBackToPos = "Back To Position";
        const string grpDeath = "Death";
        const string grpCoolDown = "CoolDown";
        #endregion

        #region cooldown
        [SerializeField,BoxGroup(grpCoolDown)]
        private float _minCoolDown;
        [SerializeField,BoxGroup(grpCoolDown)]
        private float _maxCoolDown;
        #endregion

        #region move
        [SerializeField,BoxGroup(grpMove)]
        private Ease _moveEase = Ease.Linear;

        [SerializeField,BoxGroup(grpMove)]
        private float _moveDuration = 1;

        [SerializeField,BoxGroup(grpMove)]
        private float _enemyMoveOffset = 0.5f;

        [SerializeField,BoxGroup(grpMove),SoundGroup]
        private string _moveSFXGroup;

        [SerializeField,BoxGroup(grpMove)]
        private float _moveSFXdelay;
        #endregion


        #region slash
        [SerializeField,BoxGroup(grpSlash)]
        private Params.VFXVariantParams _vfxSlash = new Params.VFXVariantParams(VFXType.Slash);
        
        [SerializeField,BoxGroup(grpSlash)]
        private float _slashDelay = 0.1f;

        [SerializeField,BoxGroup(grpSlash),SoundGroup]
        private string _slashSFXGroup;

        [SerializeField,BoxGroup(grpSlash)]
        private float _slashSFXdelay;
        #endregion


        #region hit
        [SerializeField,BoxGroup(grpHit)]
        private Params.VFXVariantParams _vfxHit = new Params.VFXVariantParams(VFXType.Hit);

        [SerializeField,BoxGroup(grpHit)]
        private float _hitDelay = 0.5f;

        [SerializeField,BoxGroup(grpHit),SoundGroup]
        private string _hitSFXGroup;

        [SerializeField,BoxGroup(grpHit)]
        private float _hitSFXdelay;
        #endregion
        
        #region damage
        [SerializeField,BoxGroup(grpDamage)]
        private Params.VFXVariantParams _vfxDamage = new Params.VFXVariantParams(VFXType.Damage);

        [SerializeField,BoxGroup(grpDamage)]
        private int _minDamage = 10;

        [SerializeField,BoxGroup(grpDamage)]
        private int _maxDamage = 100;

        [SerializeField,BoxGroup(grpDamage)]
        private float _damageOffset = 0;
        #endregion

        #region camshake
        [SerializeField,BoxGroup(grpCamShake)]
        private Params.VFXVariantParams _vfxCamShake = new Params.VFXVariantParams(VFXType.CamShake);
        #endregion

        #region blink
        [SerializeField,BoxGroup(grpBlink)]
        private Params.VFXVariantParams _vfxBlinking = new Params.VFXVariantParams(VFXType.Blinking);
        #endregion

        #region back to pos
        [SerializeField,BoxGroup(grpBackToPos)]
        private Ease _backEase = Ease.Linear;

        [SerializeField,BoxGroup(grpBackToPos)]
        private float _backDuration = 1;
        #endregion

        #region death
        [SerializeField,BoxGroup(grpDeath)]
        private Params.VFXVariantParams _vfxDeath = new Params.VFXVariantParams(VFXType.Death);

        [SerializeField,BoxGroup(grpDeath)]
        private Vector2 _vfxDeathOffset;
        #endregion


        #region private
        private GameManager _gameManager;
        private IVFX _damage;
        #endregion

        #region ISkill
        [ShowInInspector,BoxGroup(nameof(id)),HideLabel,PropertyOrder(-1)]
        public string id => name;
        public int minDamage => _minDamage;
        public int maxDamage => _maxDamage;
        public float minCoolDown => _minCoolDown;
        public float maxCoolDown => _maxCoolDown;

        //public event Action<Character, Character, float> onAttack;
        public event Action<BattleAttack> onAttack;
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
        }
        public async Task Use(Params.SkillParams param)
        {
            try
            {   
                _damage = (_damage.IsNull() || !_damage.isActive) ? _gameManager.vfxManager.GetVFX(_vfxDamage) : _damage;
                
                var fromPrevPos = param.attacker.transform.position;
                var toPrevPos   = param.victim.transform.position;
    
                //TweenMove
                var tween = param.attacker.transform.DOMove(param.victimGrid.GetPositionByStep(param.victim.transform.position,1),_moveDuration).SetEase(_moveEase).SetAutoKill();
                    tween = param.victim.transform.DOMove(param.victim.transform.position + (Vector3.right * _enemyMoveOffset * (param.victim.direction == Direction.Left? -1 : 1)),_moveDuration).SetEase(_moveEase).SetAutoKill();
                MasterAudio.PlaySound(sType:_moveSFXGroup,delaySoundTime:_moveSFXdelay);

                await new WaitForSeconds(_moveDuration - _slashDelay);

                //Slash Effect
                var slash = _gameManager.vfxManager.GetVFX(_vfxSlash);
                slash.transform.position = param.victim.GetHitPosition();
                slash.transform.rotation = Quaternion.identity;  
                var scale = slash.transform.localScale;
                scale.x   = param.attacker.direction == Direction.Right ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                slash.transform.localScale = scale;
                slash.Show();
                MasterAudio.PlaySound(sType:_slashSFXGroup,delaySoundTime:_slashSFXdelay);
                
                //Hit Effect
                await new WaitForSeconds(_hitDelay);
                var hit   = _gameManager.vfxManager.GetVFX(_vfxHit);
                hit.transform.position = param.victim.GetHitPosition();
                hit.Show();
                MasterAudio.PlaySound(sType:_hitSFXGroup,delaySoundTime:_hitSFXdelay);

                //Damage effect
                _damage.transform.position = param.victim.GetDamagePosition(Vector2.up * _damageOffset);
                _damage.Show(new Params.VFXDamageParams(param.attackerDamage));
                onAttack?.Invoke(new BattleAttack(param.attacker,param.victim,param.attackerDamage));
               
                //CamShake Effect
                var camShake = _gameManager.vfxManager.GetVFX(_vfxCamShake);
                camShake.Show();

                //Blink effect
                var blink = _gameManager.vfxManager.GetVFX(_vfxBlinking);
                blink.Show(new Params.VFXBlinkingParams(param.victim,1));
                await new WaitForSeconds(0.5f);
                
                //Back to Prev position
                var complete = false;
                tween = param.attacker.transform.DOMove(fromPrevPos,_backDuration).SetEase(_backEase).SetAutoKill();

                //Death
                if(param.isVictimDeath){
                    var death = _gameManager.vfxManager.GetVFX(_vfxDeath);
                    death.transform.position = param.victim.transform.position + (Vector3)_vfxDeathOffset;
                    death.Show();
                    param.victim.Hide();
                }else{
                    tween = param.victim.transform.DOMove(toPrevPos,_backDuration).SetEase(_backEase).SetAutoKill();
                }

                tween.onComplete = ()=>{complete = true;};
                await new WaitUntil(()=> complete);
            }
            catch (System.Exception ex)
            {
                ex.Message.LogError();
            }
        }
        public float GetDamage(CharacterStats from, CharacterStats to)
        {
            var dmg = Mathf.Clamp(from.attack - to.defense,_minDamage,_maxDamage);
            return dmg;
        }
        #endregion
    }
}