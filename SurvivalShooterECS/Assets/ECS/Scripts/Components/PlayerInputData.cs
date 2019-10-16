using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInputData : IComponentData
{
    public float2 Move;
    public float2 Look;
    public float Shoot;
}
