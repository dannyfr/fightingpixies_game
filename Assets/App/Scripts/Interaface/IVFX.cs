using System;
using UnityEngine;

namespace NFTGame
{
    public interface IVFX
    {
        event Action<IVFX> onComplete;
        GameObject gameObject{get;}
        Transform transform{get;}
        bool isActive{get;}
        void Init();
        void Show(object param = null);
        void Hide();
    }
}