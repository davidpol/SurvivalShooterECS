using Unity.Entities;
using Unity.Mathematics;

public struct DamagedData : IComponentData
{
    public int Damage;
    public float3 HitPoint;
}
