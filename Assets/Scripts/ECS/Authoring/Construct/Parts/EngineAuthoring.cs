using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Authoring.Construct.Parts
{
    public class EngineAuthoring : MonoBehaviour
    {
        [field: SerializeField] public float Thrust { get; private set; } = 10f;
        [field: SerializeField] public Vector3 Direction { get; private set; } = Vector3.forward;
        [field: SerializeField] public Rigidbody Rb { get; private set; }

        private class Baker : Baker<EngineAuthoring>
        {
            public override void Bake(EngineAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new EngineComponent
                {
                    Thrust = authoring.Thrust,
                    Direction = authoring.Direction
                });
                
            }
        }
    }

    public struct EngineComponent : IComponentData
    {
        public float Thrust;
        public float3 Direction;
    }
}