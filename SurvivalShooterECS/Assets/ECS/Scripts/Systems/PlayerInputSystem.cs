using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnitySampleAssets.CrossPlatformInput;

[DisableAutoCreation]
public class PlayerInputSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem barrier;

    protected override void OnCreate()
    {
        barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    private struct EnemyAttackJob : IJobForEachWithEntity<PlayerInputData>
    {
        public float2 InputData;

        public void Execute(Entity entity, int index, ref PlayerInputData inputData)
        {
            inputData.Move = InputData;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new EnemyAttackJob
        {
            InputData = new float2(
                CrossPlatformInputManager.GetAxisRaw("Horizontal"),
                CrossPlatformInputManager.GetAxisRaw("Vertical"))
        };
        inputDeps = job.Schedule(this, inputDeps);
        barrier.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }
}
