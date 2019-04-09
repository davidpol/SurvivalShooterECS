using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class PlayerAnimationSystem : ComponentSystem
{
    private EntityQuery query;
    
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.ReadOnly<Animator>());
    }

    protected override void OnUpdate()
    {
        Entities.With(query).ForEach(
            (Entity entity, Animator animator, ref PlayerInputData input) =>
            {
                var move = input.Move;
                animator.SetBool(IsWalkingHash, move.x != 0f || move.y != 0f);
            });
    }
}
