using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    private ComponentGroup group;

    protected override void OnCreateManager()
    {
        group = GetComponentGroup(
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.Exclude<DeadData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(group).ForEach(entity =>
        {
            var newInput = new PlayerInputData
            {
                Move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };
            PostUpdateCommands.SetComponent(entity, newInput);
        });
    }
}
