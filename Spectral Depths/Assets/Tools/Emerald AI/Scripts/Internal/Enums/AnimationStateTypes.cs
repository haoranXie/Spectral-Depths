namespace EmeraldAI
{
    [System.Flags]
    public enum AnimationStateTypes
    {
        None = 0,
        Idling = 1 << 1,
        Moving = 1 << 2,
        BackingUp = 1 << 3,
        TurningLeft = 1 << 4,
        TurningRight = 1 << 5,
        Attacking = 1 << 6,
        Strafing = 1 << 7,
        Blocking = 1 << 8,
        Dodging = 1 << 9,
        Recoiling = 1 << 10,
        Stunned = 1 << 11,
        GettingHit = 1 << 12,
        Equipping = 1 << 13,
        SwitchingWeapons = 1 << 14,
        Dead = 1 << 15,
        Emoting = 1 << 16,
        Everything = ~0,
    }
}