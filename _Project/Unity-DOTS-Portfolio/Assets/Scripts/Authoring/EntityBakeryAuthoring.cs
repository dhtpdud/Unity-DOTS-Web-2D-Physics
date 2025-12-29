using Unity.Entities;
using UnityEngine;

public class EntityBakeryAuthoring : MonoBehaviour
{
    public GameObject mouseCollider;
    public class EntityBakeryAuthoringBaker : Baker<EntityBakeryAuthoring>
    {
        public override void Bake(EntityBakeryAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EntityBakeryComponent
            {
                mouseCollider = GetEntity(authoring.mouseCollider, TransformUsageFlags.Dynamic),
            });
        }
    }
}
