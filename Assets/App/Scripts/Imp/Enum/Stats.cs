using System;

namespace NFTGame
{
    [Flags]
    public enum Stats
    {
        HP      = 1,
        ATTACK  = 2,
        DEFENSE = 4,
        SPEED   = 8
    }
}