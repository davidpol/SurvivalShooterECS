using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(EnemyHealthSystem))]
public class EnemyHitFxSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EnemyData>().ForEach(
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
