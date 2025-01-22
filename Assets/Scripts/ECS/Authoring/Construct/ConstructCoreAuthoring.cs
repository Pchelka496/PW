using Unity.Entities;
using UnityEngine;
using Zenject;

namespace ECS.Authoring.Construct
{
    public class ConstructCoreAuthoring : MonoInstaller
    {
        [field: SerializeField] public Rigidbody Rb { get; private set; }
    
        private class Baker : Baker<ConstructCoreAuthoring>
        {
            public override void Bake(ConstructCoreAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ConstructCoreComponent());
                AddComponentObject(entity, authoring.Rb);
            }
        }
    }

    public struct ConstructCoreComponent : IComponentData
    {
        
    }
}