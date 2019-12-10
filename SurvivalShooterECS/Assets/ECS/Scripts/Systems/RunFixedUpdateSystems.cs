using Unity.Entities;
using UnityEngine;

public class RunFixedUpdateSystems : MonoBehaviour
{
    private PlayerInputSystem playerInputSystem;
    private PlayerMovementSystem playerMovementSystem;
    private PlayerTurningSystem playerTurningSystem;
    private PlayerAnimationSystem playerAnimationSystem;
    private CameraFollowSystem cameraFollowSystem;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        playerInputSystem = world.GetOrCreateSystem<PlayerInputSystem>();
        playerMovementSystem = world.GetOrCreateSystem<PlayerMovementSystem>();
        playerTurningSystem = world.GetOrCreateSystem<PlayerTurningSystem>();
        playerAnimationSystem = world.GetOrCreateSystem<PlayerAnimationSystem>();
        cameraFollowSystem = world.GetOrCreateSystem<CameraFollowSystem>();
    }

    private void FixedUpdate()
    {
        playerInputSystem.Update();
        playerMovementSystem.Update();
        playerTurningSystem.Update();
        playerAnimationSystem.Update();
        cameraFollowSystem.Update();
    }
}
