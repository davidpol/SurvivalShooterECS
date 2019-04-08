using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(EnemyHealthSystem))]
public class EnemyHitFxSystem : ComponentSystem
{
    private EntityQuery query;
    
    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<DamagedData>(),
            ComponentType.ReadOnly<AudioSource>());
    }

    protected override void OnUpdate()
    {
        Entities.With(query).ForEach(
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
