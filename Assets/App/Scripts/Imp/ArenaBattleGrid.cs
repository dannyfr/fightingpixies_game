using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class ArenaBattleGrid : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpGizmos = "Gizmos";
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpConfig),OnValueChanged(nameof(Init))]
        private Direction _direction;

        [SerializeField,FoldoutGroup(grpConfig),OnValueChanged(nameof(Init))]
        private int _count;

        [SerializeField,FoldoutGroup(grpConfig),OnValueChanged(nameof(Init))]
        private float _distance;

        [SerializeField,FoldoutGroup(grpGizmos),BoxGroup(grpGizmos + "/Color 1"),HideLabel,ColorPalette]
        private Color _color1 = Color.red;

        [SerializeField,FoldoutGroup(grpGizmos),BoxGroup(grpGizmos + "/Color 2"),HideLabel,ColorPalette]
        private Color _color2 = Color.red;

        [SerializeField,Range(0,1f),BoxGroup(grpGizmos + "/Offset"),HideLabel]
        private float _offset = 0.4f;

        [SerializeField,Range(0,1f),BoxGroup(grpGizmos + "/Radius"),HideLabel]
        private float _radius = 0.4f;
        #endregion

        #region property
        public Direction direction => _direction;
        public Vector3[] positions => _positions;
        #endregion

        #region private
        private Vector3[] _positions;
        private Vector3[] _gizmos = new Vector3[]{Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero};
        #endregion

        #region methods
        public void Init() {
            _positions = new Vector3[_count];
            var dir = _direction == Direction.Right ? 1 : -1;
            for (var i = 0; i < _count; i++)
            {
                _positions[i] = i==0 ? transform.position : new Vector3(transform.position.x + (i * dir * _distance),transform.position.y,transform.position.z);
            }
        }
        
        public int GetNearPositionIndex(Vector3 position){
            var nearDist = float.MaxValue;
            var index = 0;
            for (var i = 0; i < _positions.Length; i++)
            {
                var dist = Vector3.Distance(position,_positions[i]);
                if(dist < nearDist){
                    nearDist = dist;
                    index = i;
                }
            }

            return index;
        }
        public Vector3 GetPosition(int index){
            if(index > _positions.Length - 1)
                return _positions.Last();

            return _positions[index];
        }
        public Vector3 GetPositionByStep(Vector3 position,int step){
            step = Mathf.Abs(step);
            var index = GetNearPositionIndex(position);

            return index + step < _positions.Length-1 ? _positions[index + step] : _positions.Last();
        }
        #endregion

        #region Callbacks
        private void OnDrawGizmos() {
            if(_positions.IsNull())
                _positions = new Vector3[_count];

            if(!_positions.IsNullOrEmpty()){
                for (int i = 0; i < _positions.Length; i++)
                {
                    var dir = _direction == Direction.Right ? 1 : -1;
                    _positions[i] = i==0 ? transform.position : new Vector3(transform.position.x + (i * dir * _distance),transform.position.y,transform.position.z);
                    
                    Gizmos.color = _color1;

                    _gizmos[0].x = _positions[i].x - (_distance/2) + _offset;
                    _gizmos[0].y = _positions[i].y + (_distance/2) - _offset;
                    _gizmos[1].x =  _gizmos[0].x + _distance;
                    _gizmos[1].y = _gizmos[0].y;
                    _gizmos[2].x = _positions[i].x - (_distance/2) - _offset;
                    _gizmos[2].y = _positions[i].y - (_distance/2) + _offset;
                    _gizmos[3].x = _gizmos[2].x + _distance;
                    _gizmos[3].y = _gizmos[2].y;
                    _gizmos[0].z = _gizmos[1].z = _gizmos[2].z = _gizmos[3].z = _positions[i].z;

                    Gizmos.DrawLine(_gizmos[0],_gizmos[1]);
                    Gizmos.DrawLine(_gizmos[2],_gizmos[3]);
                    Gizmos.DrawLine(_gizmos[0],_gizmos[2]);
                    Gizmos.DrawLine(_gizmos[1],_gizmos[3]);

                    Gizmos.color = _color2;
                    Gizmos.DrawSphere(_positions[i],_radius);
                }
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