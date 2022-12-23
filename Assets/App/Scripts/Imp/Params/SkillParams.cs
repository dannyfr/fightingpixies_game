namespace NFTGame.Params
{
    public struct SkillParams{
        public Character attacker;
        public Character victim;
        public ArenaBattleGrid attackerGrid;
        public ArenaBattleGrid victimGrid;
        public float attackerDamage;
        public bool isVictimDeath;   
    }
}