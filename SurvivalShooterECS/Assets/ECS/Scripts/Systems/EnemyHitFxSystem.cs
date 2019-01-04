using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(EnemyHealthSystem))]
public class EnemyHitFxSystem : ComponentSystem
{
    private ComponentGroup enemyGroup;
    
    protected override void OnCreateManager()
    {
        enemyGroup = EntityManager.CreateComponentGroup(
            ComponentType.Create<Transform>(),
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<DamagedData>(),
            ComponentType.Create<AudioSource>());
    }

    protected override void OnUpdate()
    {
        var go = enemyGroup.GetGameObjectArray();
        var audioSource = enemyGroup.GetComponentArray<AudioSource>();
        var damaged = enemyGroup.GetComponentDataArray<DamagedData>();
        for (var i = 0; i < go.Length; ++i)
        {
            audioSource[i].Play();

            var particles = go[i].GetComponentInChildren<ParticleSystem>();
            particles.transform.position = damaged[i].HitPoint;
            particles.Play();
        }
    }
}
