using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
public class PlayerInputSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction shootAction;

    private float2 moveInput;
    private float2 lookInput;
    private float shootInput;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        moveAction = new InputAction("move", binding: "<Gamepad>/rightStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.performed += context =>
        {
            moveInput = context.ReadValue<Vector2>();
        };
        moveAction.canceled += context =>
        {
            moveInput = context.ReadValue<Vector2>();
        };
        moveAction.Enable();

        lookAction = new InputAction("look", binding: "<Mouse>/position");
        lookAction.performed += context =>
        {
            lookInput = context.ReadValue<Vector2>();
        };
        lookAction.canceled += context =>
        {
            lookInput = context.ReadValue<Vector2>();
        };
        lookAction.Enable();

        shootAction = new InputAction("shoot", binding: "<Mouse>/leftButton");
        shootAction.performed += context =>
        {
            shootInput = context.ReadValue<float>();
        };
        shootAction.canceled += context =>
        {
            shootInput = context.ReadValue<float>();
        };
        shootAction.Enable();
    }

    protected override void OnStopRunning()
    {
        shootAction.Disable();
        lookAction.Disable();
        moveAction.Disable();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var moveInputCopy = moveInput;
        var lookInputCopy = lookInput;
        var shootInputCopy = shootInput;

        var jobHandle = Entities
            .WithReadOnly(moveInputCopy)
            .WithReadOnly(lookInputCopy)
            .WithReadOnly(shootInputCopy)
            .ForEach((Entity entity, ref PlayerInputData inputData) =>
        {
            inputData.Move = moveInputCopy;
            inputData.Look = lookInputCopy;
            inputData.Shoot = shootInputCopy;
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
