using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class PlayerShootingSystem : ComponentSystem
{
    private EntityQuery gunQuery;
    private EntityQuery playerQuery;

    private float timer;

    protected override void OnCreateManager()
    {
        gunQuery = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerGunData>(),
            ComponentType.ReadOnly<ParticleSystem>(),
            ComponentType.ReadOnly<LineRenderer>(),
            ComponentType.ReadOnly<AudioSource>(),
            ComponentType.ReadOnly<Light>());
        playerQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<HealthData>());
    }

    protected override void OnUpdate()
    {
        var hp = playerQuery.ToComponentDataArray<HealthData>(Allocator.TempJob);
        if (hp.Length == 0)
        {
            hp.Dispose();
            return;
        }

        if (hp[0].Value <= 0)
        {
            hp.Dispose();
            return;
        }
        
        hp.Dispose();

        timer += Time.deltaTime;

        var timeBetweenBullets = SurvivalShooterBootstrap.Settings.TimeBetweenBullets;
        var effectsDisplayTime = SurvivalShooterBootstrap.Settings.GunEffectsDisplayTime;

        Entities.With(gunQuery).ForEach(
            (Entity entity, AudioSource audio, Light light, ParticleSystem particles, LineRenderer line) =>
            {
                if (Input.GetButton("Fire1") && timer > timeBetweenBullets)
                    Shoot(audio, light, particles, line);

                if (timer >= timeBetweenBullets * effectsDisplayTime)
                    DisableEffects(light, line);
            });
    }

    private void Shoot(AudioSource audio, Light light, ParticleSystem particles, LineRenderer line)
    {
        timer = 0f;

        audio.Play();

        light.enabled = true;

        particles.Stop();
        particles.Play();

        var go = audio.gameObject;
        var pos = go.transform.position;
        line.enabled = true;
        line.SetPosition(0, pos);

        var shootRay = new Ray
        {
            origin = pos,
            direction = go.transform.forward
        };

        RaycastHit shootHit;
        if (Physics.Raycast(shootRay, out shootHit, SurvivalShooterBootstrap.Settings.GunRange,
            LayerMask.GetMask("Shootable")))
        {
            var goEntity = shootHit.collider.gameObject.GetComponent<GameObjectEntity>();
            if (goEntity != null)
            {
                var hitEntity = shootHit.collider.gameObject.GetComponent<GameObjectEntity>().Entity;
                var entityManager = World.EntityManager;
                if (!entityManager.HasComponent<DamagedData>(hitEntity))
                    PostUpdateCommands.AddComponent(hitEntity, new DamagedData
                    {
                        Damage = SurvivalShooterBootstrap.Settings.DamagePerShot,
                        HitPoint = shootHit.point
                    });
            }

            line.SetPosition(1, shootHit.point);
        }
        else
        {
            line.SetPosition(1, shootRay.origin + shootRay.direction * SurvivalShooterBootstrap.Settings.GunRange);
        }
    }

    private void DisableEffects(Light light, LineRenderer line)
    {
        light.enabled = false;
        line.enabled = false;
    }
}
