using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class CameraFollowSystem : SystemBase
{
    private bool firstFrame = true;
    private Vector3 offset;

    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((Entity entity, Transform transform, ref PlayerInputData data) =>
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return;

            var go = transform.gameObject;
            var playerPos = go.transform.position;

            if (firstFrame)
            {
                offset = mainCamera.transform.position - playerPos;
                firstFrame = false;
            }

            var smoothing = SurvivalShooterBootstrap.Settings.CamSmoothing;
            var dt = Time.DeltaTime;
            var targetCamPos = playerPos + offset;
            mainCamera.transform.position =
                Vector3.Lerp(mainCamera.transform.position, targetCamPos, smoothing * dt);
        }).Run();
    }
}
