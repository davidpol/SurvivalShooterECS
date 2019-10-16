using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
public class PlayerInputSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem barrier;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction shootAction;

    private float2 moveInput;
    private float2 lookInput;
    private float shootInput;

    protected override void OnCreate()
    {
        barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
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

    [BurstCompile]
    private struct PlayerInputJob : IJobForEach<PlayerInputData>
    {
        [ReadOnly]
        public float2 MoveInput;
        [ReadOnly]
        public float2 LookInput;
        [ReadOnly]
        public float ShootInput;

        public void Execute(ref PlayerInputData inputData)
        {
            inputData.Move = MoveInput;
            inputData.Look = LookInput;
            inputData.Shoot = ShootInput;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerInputJob
        {
            MoveInput = moveInput,
            LookInput = lookInput,
            ShootInput = shootInput
        };
        inputDeps = job.Schedule(this, inputDeps);
        barrier.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }
}
