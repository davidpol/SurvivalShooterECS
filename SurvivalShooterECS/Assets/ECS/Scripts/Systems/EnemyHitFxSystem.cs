using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(EnemyHealthSystem))]
public class EnemyHitFxSystem : ComponentSystem
{
    private ComponentGroup group;
    
    protected override void OnCreateManager()
    {
        group = EntityManager.CreateComponentGroup(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<DamagedData>(),
            ComponentType.ReadOnly<AudioSource>());
    }

    protected override void OnUpdate()
    {
        Entities.With(group).ForEach(
            (Entity entity, AudioSource audio, ref DamagedData damagedData) =>
            {
                audio.Play();

                var go = audio.gameObject;
                var particles = go.GetComponentInChildren<ParticleSystem>();
                particles.transform.position = damagedData.HitPoint;
                particles.Play();
            });
    }
}
