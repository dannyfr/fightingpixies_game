using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using TMPro;
using DG.Tweening;

namespace NFTGame.VFX
{
    [AddComponentMenu(Utils.EditorMenu.VFX + nameof(VFXDamage))]
    public class VFXDamage : SerializedMonoBehaviour,IVFX,IDisposable
    {
        #region const
        const string grpPrefabs = "Prefabs";
        const string grpTween = "Tween";
        const string grpGizmos = "Giszmos";
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpPrefabs)]
        private TextMeshPro _textPrefab;

        [SerializeField,FoldoutGroup(grpPrefabs)]
        private string _textZeroDamage = "Miss";

        [SerializeField,FoldoutGroup(grpTween)]
        private float _offsetY = 1;

        [SerializeField,FoldoutGroup(grpTween)]
        private float _duration = 1;

        [SerializeField,FoldoutGroup(grpTween)]
        private float _radius = 0.5f;

        [SerializeField,FoldoutGroup(grpTween)]
        private Ease _easeText = Ease.Linear;

        [SerializeField,FoldoutGroup(grpTween)]
        private Ease _easeFade = Ease.OutBack;

        [SerializeField,FoldoutGroup(grpGizmos),ColorPalette]
        private Color _color = Color.red;
        #endregion

        #region private
        private List<TextMeshPro> _active;
        private Queue<TextMeshPro> _pull;
        private List<Tween> _tween;
        private bool _inited;
        #endregion

        #region methods
        public void Play(int damage){
            Show(damage);
        }
        #endregion

        #region IVFX
        public event Action<IVFX> onComplete;
        public bool isActive => !_tween.IsNullOrEmpty();
        public void Init(){
            if(_inited)
                return;

            _active = new List<TextMeshPro>();
            _pull   = new Queue<TextMeshPro>();
            _tween  = new List<Tween>();
            _textPrefab.gameObject.Hide();
            _inited = true;
        }
        public void Show(object param)
        {
            gameObject.Show();

            //Get from pull or create one
            var damage = ((Params.VFXDamageParams)param).damage;
                damage = Mathf.RoundToInt(damage);
                
            var text = _pull.Count > 0? _pull.Dequeue() : GameObject.Instantiate(_textPrefab);
                text.transform.SetParent(transform);
                text.transform.localPosition = UnityEngine.Random.insideUnitCircle * _radius;
                text.text = damage > 0 ? damage.ToString() : _textZeroDamage;
                text.gameObject.Show();

            //Tween Color
            var color  = text.color; color.a = 1;
            text.color = color;
            color = text.color; color.a = 0;
            text.DOColor(color,_duration).SetEase(_easeFade).SetAutoKill();

            //Tween Move
            var tween = text.transform.DOLocalMoveY(text.transform.localPosition.y + _offsetY,_duration).SetEase(_easeText).SetAutoKill();
                tween.onComplete = ()=>
                {
                    text.gameObject.Hide();
                    _active.Remove(text);
                    _pull.Enqueue(text);
                    _tween.Remove(tween);

                    if(!isActive){
                        onComplete?.Invoke(this);
                    }
                };

            _tween.Add(tween);
            _active.Add(text);
        }
        public void Hide()
        {
            gameObject.Hide();

            //Move from active to pull
            if(!_active.IsNullOrEmpty())
            {
                for (int i = 0; i < _active.Count; i++){
                    _pull.Enqueue(_active[i]);
                }

                _active.Clear();
            }
                
            //Hide and kill tween
            if(!_pull.IsNullOrEmpty())
            {
                foreach (var item in _pull)
                {
                    item.transform.DOKill();
                    item.gameObject.Hide();
                }
            }   
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            onComplete = null;
        }
        private void OnDrawGizmos() {
            #if UNITY_EDITOR
                UnityEditor.Handles.color = _color;
                UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.forward,_radius);
            #endif
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