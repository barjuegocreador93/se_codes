public enum MyTransmitTarget
{
    None = 0,
    Owner = 1 << 0,
    Ally = 1 << 1,
    Neutral = 1 << 2,
    Enemy = 1 << 3,
    Everyone = Owner | Ally | Neutral | Enemy,
    Default = Owner | Ally,
}