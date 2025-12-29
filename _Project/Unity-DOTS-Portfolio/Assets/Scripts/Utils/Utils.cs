using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using Unity.Physics;
using UnityEditor;
using UnityEngine;

namespace OSY
{
    public static class Utils
    {
        public static Dictionary<int, string> hashMemory = new Dictionary<int, string>();
        public static string stringSpace = " ";
        public static string stringPoint = ".";
        public static void KeepAlive(params object[] items) => GC.KeepAlive(items);
        public static void ToFloat3(this float2 target, ref float3 convert)
        {
            convert.x = target.x;
            convert.y = target.y;
        }
        public static void ToFloat2(this float3 target, ref float2 convert)
        {
            convert.x = target.x;
            convert.y = target.y;
        }
        public static float3 ToFloat3(this float2 target)
        {
            return math.float3(target.x, target.y, 0);
        }
        public static float2 ToFloat2(this float3 target)
        {
            return math.float2(target.x, target.y);
        }
        public static float3 ToFloat3(this Vector2 target)
        {
            return math.float3(target.x, target.y, 0);
        }
        public static float2 ToFloat2(this Vector3 target)
        {
            return math.float2(target.x, target.y);
        }

        public static string GetRandomHexNumber(int digits) // string 기준으로 최대 길이 (1byte = string 두글자)
        {
            System.Random random = new System.Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        public static Unity.Physics.Material GetMaterial(RigidBody rigidBody, ColliderKey colliderKey)
        {
            Unity.Physics.Material material;
            unsafe
            {
                Unity.Physics.Collider* colliderPointer = (Unity.Physics.Collider*)rigidBody.Collider.GetUnsafePtr();
                ChildCollider childCollider;
                colliderPointer->GetLeaf(colliderKey, out childCollider);
                ConvexCollider* childColliderPointer = (ConvexCollider*)childCollider.Collider;
                material = childColliderPointer->Material;
            }
            return material;
        }
        public static void SetMaterial(RigidBody rigidBody, Unity.Physics.Material material, ColliderKey colliderKey)
        {
            unsafe
            {
                Unity.Physics.Collider* colliderPointer = (Unity.Physics.Collider*)rigidBody.Collider.GetUnsafePtr();
                ChildCollider childCollider;
                colliderPointer->GetLeaf(colliderKey, out childCollider);
                ConvexCollider* childColliderPointer = (ConvexCollider*)childCollider.Collider;
                childColliderPointer->Material = material;
            }
        }
    }
}