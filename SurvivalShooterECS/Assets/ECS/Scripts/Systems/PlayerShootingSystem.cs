using Unity.Entities;
using UnityEngine;

public class PlayerShootingSystem : ComponentSystem
{
    private EntityQuery gunQuery;
    private EntityQuery playerQuery;

    private float timer;

    protected override void OnCreate()
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
            ComponentType.ReadOnly<PlayerInputData>(),
            ComponentType.ReadOnly<HealthData>());
    }

    protected override void OnUpdate()
    {
        var hasToExit = true;
        var playerShot = false;
        Entities.With(playerQuery).ForEach(
            (Entity entity, ref HealthData health, ref PlayerInputData input) =>
            {
                if (health.Value > 0)
                    hasToExit = false;

                if (input.Shoot > 0.0f)
                    playerShot = true;
            });

        if (hasToExit)
            return;

        timer += Time.deltaTime;

        var timeBetweenBullets = SurvivalShooterBootstrap.Settings.TimeBetweenBullets;
        var effectsDisplayTime = SurvivalShooterBootstrap.Settings.GunEffectsDisplayTime;

        Entities.With(gunQuery).ForEach(
            (Entity entity, AudioSource audio, Light light, ParticleSystem particles, LineRenderer line) =>
            {
                if (playerShot && timer > timeBetweenBullets)
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
