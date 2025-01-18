using Unity.Entities;
using Zenject;

namespace ECS.Authoring.Construct
{
    public class ConstructCoreAuthoring : MonoInstaller
    {
        private class Baker : Baker<ConstructCoreAuthoring>
        {
            public override void Bake(ConstructCoreAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ConstructCore());
            }
        }
    }

    public struct ConstructCore : IComponentData
    {
    }
}