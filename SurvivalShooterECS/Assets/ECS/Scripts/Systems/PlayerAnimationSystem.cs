using Unity.Entities;
using UnityEngine;

public class PlayerAnimationSystem : ComponentSystem
{
    private ComponentGroup group;
    
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    protected override void OnCreateManager()
    {
        group = GetComponentGroup(
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.ReadOnly<Animator>());
    }

    protected override void OnUpdate()
    {
        Entities.With(group).ForEach(
            (Entity entity, Animator animator, ref PlayerInputData input) =>
            {
                var move = input.Move;
                animator.SetBool(IsWalkingHash, move.x != 0f || move.y != 0f);
            });
    }
}
