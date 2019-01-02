using Unity.Entities;
using UnityEngine;

public class PlayerAnimationSystem : ComponentSystem
{
    private ComponentGroup inputGroup;
    
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    protected override void OnCreateManager()
    {
        inputGroup = GetComponentGroup(
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.Create<Animator>());
    }

    protected override void OnUpdate()
    {
        var input = inputGroup.GetComponentDataArray<PlayerInputData>();
        var animator = inputGroup.GetComponentArray<Animator>();
        for (var i = 0; i < input.Length; i++)
        {
            var move = input[i].Move;
            animator[i].SetBool(IsWalkingHash, move.x != 0f || move.y != 0f);
        }
    }
}
