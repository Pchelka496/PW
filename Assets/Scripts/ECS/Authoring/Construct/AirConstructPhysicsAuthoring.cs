using System;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Construct
{
    public class AirConstructPhysicsAuthoring : MonoBehaviour
    {
        [SerializeField] AerodynamicPart[] _parts;

        private class Baker : Baker<AirConstructPhysicsAuthoring>
        {
            public override void Bake(AirConstructPhysicsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AirConstructPhysicsComponent());
            }
        }
    }

    public struct AirConstructPhysicsComponent : IComponentData, IDisposable
    {
        public void Dispose()
        {
        }
    }
}