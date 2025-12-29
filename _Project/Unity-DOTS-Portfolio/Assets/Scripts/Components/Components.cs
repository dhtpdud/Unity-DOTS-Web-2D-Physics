
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct DragableTag : IComponentData
{

}
public struct EntityBakeryComponent : IComponentData
{
    public Entity mouseCollider;
}
public struct SpawnerComponent : IComponentData
{
    public Entity targetEntity;
    public int maxCount;
    public int spawnedCount;

    public float spawnIntervalSec;
    public float currentSec;
    public int batchCount;

    public float3 minPos;
    public float3 maxPos;

    public bool isRandomSize;
    public float minSize;
    public float maxSize;
}
public struct GameManagerSingletonComponent : IComponentData
{
    public struct DragingEntityInfo
    {
        readonly public Entity entity;
        readonly public RigidBody rigidbody;
        readonly public ColliderKey colliderKey;
        readonly public Unity.Physics.Material material;

        public DragingEntityInfo(Entity entity, RigidBody rigidbody, ColliderKey colliderKey, Unity.Physics.Material material)
        {
            this.entity = entity;
            this.rigidbody = rigidbody;
            this.colliderKey = colliderKey;
            this.material = material;
        }
    }
    public DragingEntityInfo dragingEntityInfo;

    public UnityEngine.Ray ScreenPointToRayOfMainCam;
    public float2 ScreenToWorldPointMainCam;

    public float dragPower;
    public float dragDamping;

    public float physicMaxVelocity;
}
[BurstCompile]
public struct RandomComponent : IComponentData
{
    public bool isUpdate;
    public Random Random;
    [BurstCompile]
    public void UpdateSeed()
    {
        if (!isUpdate)
        {
            Random = new Random((uint)Random.NextInt(int.MinValue, int.MaxValue));
            isUpdate = true;
        }
    }
}