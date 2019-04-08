using Unity.Entities;
using UnityEngine;

public class CameraFollowSystem : ComponentSystem
{
    private EntityQuery query;
    
    private bool firstFrame = true;
    private Vector3 offset;

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerInputData>());
    }

    protected override void OnUpdate()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;
        
        Entities.With(query).ForEach(
            (Entity entity, Transform transform, ref PlayerInputData data) =>
            {
                var go = transform.gameObject;
                var playerPos = go.transform.position;

                if (firstFrame)
                {
                    offset = mainCamera.transform.position - playerPos;
                    firstFrame = false;
                }

                var smoothing = SurvivalShooterBootstrap.Settings.CamSmoothing;
                var dt = Time.deltaTime;
                var targetCamPos = playerPos + offset;
                mainCamera.transform.position =
                    Vector3.Lerp(mainCamera.transform.position, targetCamPos, smoothing * dt);
            });
    }
}
