using ComponentsAndTags;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Camera
{
    public partial class CameraController : SystemBase
    {
        protected override void OnUpdate()
        {
            Entity brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            float brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            
            CameraSingleton camera = CameraSingleton.Instance;
            if (camera == null) return;

            float positionFactor = (float)SystemAPI.Time.ElapsedTime * camera.Speed;
            float height = camera.HeightAtScale(brainScale);
            float radius = camera.RadiusAtScale(brainScale);

            camera.transform.position = new Vector3(
                Mathf.Cos(positionFactor) * radius,
                height,
                Mathf.Sin(positionFactor) * radius);
            
            camera.transform.LookAt(Vector3.zero, Vector3.up);
        }
    }
}