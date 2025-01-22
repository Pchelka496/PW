using ECS.Authoring.Construct.Parts;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class EngineSystem : SystemBase
{
    public void OnCreate(ref SystemState state)
    {
    }

    protected override void OnUpdate()
    {
        foreach (var (engine, entity) in SystemAPI.Query<RefRO<EngineComponent>>().WithEntityAccess())
        {
            
        }
    }
}