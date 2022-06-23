namespace Game.Systems.Items
{
    [System.Flags]
    public enum ItemTag
	{
        HEALTH = 1,
        DEFENCE = 2,
        SPEED = 4,
        DAMAGE = 8,
        CRIT = 16,
        AOE = 32
    }
}
