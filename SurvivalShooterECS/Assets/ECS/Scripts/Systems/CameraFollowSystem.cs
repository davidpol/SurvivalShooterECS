using Unity.Entities;
using UnityEngine;

public class CameraFollowSystem : ComponentSystem
{
    private ComponentGroup playerInputGroup;
    
    private bool firstFrame = true;
    private Vector3 offset;

    protected override void OnCreateManager()
    {
        playerInputGroup = GetComponentGroup(typeof(PlayerInputData), typeof(Transform));
    }

    protected override void OnUpdate()
    {
        var playerInput = playerInputGroup.GetComponentDataArray<PlayerInputData>();
        if (playerInput.Length == 0)
            return;
        
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;
        
        var go = playerInputGroup.GetGameObjectArray();
        var playerPos = go[0].transform.position;

        if (firstFrame)
        {
            offset = mainCamera.transform.position - playerPos;
            firstFrame = false;
        }

        var smoothing = SurvivalShooterBootstrap.Settings.CamSmoothing;
        var dt = Time.deltaTime;
        var targetCamPos = playerPos + offset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, smoothing * dt);
    }
}
