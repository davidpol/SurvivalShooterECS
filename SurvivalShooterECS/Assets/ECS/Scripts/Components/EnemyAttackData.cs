using Unity.Entities;

public struct EnemyAttackData : IComponentData
{
    public float Timer;
    public float Frequency;
    public int Damage;
    public Entity Source;
    public Entity Target;
}
