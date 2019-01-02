using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    private ComponentGroup inputGroup;

    protected override void OnCreateManager()
    {
        inputGroup = GetComponentGroup(
            ComponentType.Create<PlayerInputData>(),
            ComponentType.Subtractive<DeadData>());
    }

    protected override void OnUpdate()
    {
        var input = inputGroup.GetComponentDataArray<PlayerInputData>();
        for (var i = 0; i < input.Length; i++)
        {
            var newInput = new PlayerInputData
            {
                Move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };
            input[i] = newInput;
        }
    }
}
