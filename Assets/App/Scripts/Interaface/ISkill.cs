using System;
using System.Threading.Tasks;

namespace NFTGame
{
    //public delegate void OnAttackCallback(Character attacker,Character victim,float damage);

    public interface ISkill
    {
        string id {get;}
        string name{get;}
        int minDamage{get;}
        int maxDamage{get;}
        float minCoolDown{get;}
        float maxCoolDown{get;}

        event Action<BattleAttack> onAttack;
        void Init(GameManager gameManager);
        Task Use(Params.SkillParams param);
        float GetDamage(CharacterStats From,CharacterStats To);
    }
}