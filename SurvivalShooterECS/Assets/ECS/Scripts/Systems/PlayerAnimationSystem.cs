using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class PlayerAnimationSystem : ComponentSystem
{
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, Animator animator, ref PlayerInputData input) =>
        {
            var move = input.Move;
            animator.SetBool(IsWalkingHash, move.x != 0f || move.y != 0f);
        });
    }
}
