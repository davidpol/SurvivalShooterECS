using Unity.Entities;
using UnityEngine;

public class PlayerShootingSystem : ComponentSystem
{
    private ComponentGroup gunGroup;
    private ComponentGroup playerGroup;

    private float timer;

    protected override void OnCreateManager()
    {
        gunGroup = GetComponentGroup(
            typeof(Transform),
            ComponentType.Create<PlayerGunData>(),
            ComponentType.Create<ParticleSystem>(),
            ComponentType.Create<LineRenderer>(),
            ComponentType.Create<AudioSource>(),
            ComponentType.Create<Light>());
        playerGroup = GetComponentGroup(
            typeof(PlayerData),
            typeof(HealthData));
    }

    protected override void OnUpdate()
    {
        var hp = playerGroup.GetComponentDataArray<HealthData>();
        if (hp.Length == 0)
            return;
        
        if (hp[0].Value <= 0)
            return;

        timer += Time.deltaTime;

        var timeBetweenBullets = SurvivalShooterBootstrap.Settings.TimeBetweenBullets;
        var effectsDisplayTime = SurvivalShooterBootstrap.Settings.GunEffectsDisplayTime;

        var gun = gunGroup.GetGameObjectArray();
        for (var i = 0; i < gun.Length; i++)
        {
            if (Input.GetButton("Fire1") && timer > timeBetweenBullets)
                Shoot(i);

            if (timer >= timeBetweenBullets * effectsDisplayTime)
                DisableEffects(i);
        }
    }

    private void Shoot(int i)
    {
        timer = 0f;

        var audioSource = gunGroup.GetComponentArray<AudioSource>();
        audioSource[i].Play();

        var light = gunGroup.GetComponentArray<Light>();
        light[i].enabled = true;

        var particleSystem = gunGroup.GetComponentArray<ParticleSystem>();
        particleSystem[i].Stop();
        particleSystem[i].Play();

        var go = gunGroup.GetGameObjectArray();
        var lineRenderer = gunGroup.GetComponentArray<LineRenderer>();
        lineRenderer[i].enabled = true;
        lineRenderer[i].SetPosition(0, go[i].transform.position);

        var shootRay = new Ray
        {
            origin = go[i].transform.position,
            direction = go[i].transform.forward
        };

        RaycastHit shootHit;
        if (Physics.Raycast(shootRay, out shootHit, SurvivalShooterBootstrap.Settings.GunRange,
            LayerMask.GetMask("Shootable")))
        {
            var goEntity = shootHit.collider.gameObject.GetComponent<GameObjectEntity>();
            if (goEntity != null)
            {
                var hitEntity = shootHit.collider.gameObject.GetComponent<GameObjectEntity>().Entity;
                var entityManager = World.GetExistingManager<EntityManager>();
                entityManager.AddComponentData(hitEntity, new DamagedData { Damage = SurvivalShooterBootstrap.Settings.DamagePerShot, HitPoint = shootHit.point });
            }

            lineRenderer[i].SetPosition(1, shootHit.point);
        }
        else
        {
            lineRenderer[i].SetPosition(1, shootRay.origin + shootRay.direction * SurvivalShooterBootstrap.Settings.GunRange);
        }
    }

    private void DisableEffects(int i)
    {
        var lineRenderer = gunGroup.GetComponentArray<LineRenderer>();
        lineRenderer[i].enabled = false;
        
        var light = gunGroup.GetComponentArray<Light>();
        light[i].enabled = false;
    }
}
