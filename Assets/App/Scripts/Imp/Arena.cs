using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame
{   
    [HideMonoScript,HideReferenceObjectPicker]
    public class Arena : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPlacement = "Placement";
        const string grpBattleGrid = "Battle Grid";
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpBattleGrid),Required]
        private SpriteRenderer _renderer;

        [SerializeField,FoldoutGroup(grpBattleGrid),Required]
        private ArenaBattleGrid _leftGrid,_rightGrid;
        #endregion
        
        #region property
        public ArenaBattleGrid leftGrid => _leftGrid;
        public ArenaBattleGrid rightGrid => _rightGrid;
        public Rect rect {
            get{
                return _renderer.GetRect();
            }
        }
        #endregion

        #region methods
        public void Init(){
           _leftGrid.Init();
           _rightGrid.Init(); 
        }
        public void Show(){
            gameObject.Show();
        }
        public void Hide(){
            gameObject.Hide();
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

