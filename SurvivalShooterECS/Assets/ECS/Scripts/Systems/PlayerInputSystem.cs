using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.Exclude<DeadData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(query).ForEach(entity =>
        {
            var newInput = new PlayerInputData
            {
                Move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };
            PostUpdateCommands.SetComponent(entity, newInput);
        });
    }
}
