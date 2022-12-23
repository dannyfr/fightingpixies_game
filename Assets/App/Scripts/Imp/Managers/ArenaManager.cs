using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;
using NFTGame.Utils;


namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(ArenaManager))]
    public class ArenaManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpComponent),ListDrawerSettings(DraggableItems = false,Expanded = true)]
        private Arena[] _arena;
        #endregion

        #region private
        private GameManager _gameManager;

        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            for (var i = 0; i < _arena.Length; i++)
            {
                _arena[i].Init();
                _arena[i].Hide();
            }
            
            this.LogCompleted(nameof(Init));
        }
        
        public Arena GetDefault(){
            return GetArena(0);
        }
        public Arena GetArena(int index){
            if(index > _arena.Length - 1)
                return _arena.Last();

            return _arena[index];
        }
        public Arena GetRandomArena(){
            return _arena[UnityEngine.Random.Range(0,_arena.Length)];
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


