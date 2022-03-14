using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldBorderGenerator
{
    //private GameObject _border;
    //private Mesh _mesh;
    //private MeshFilter _meshFilter;
    //private MeshRenderer _meshRenderer;
    //private MeshCollider _meshCollider;

    //[SerializeField] private float _height;
    //[SerializeField] private float _radius;

    public static void Generate(BorderProperties borderProperties)
    {
        CreateMesh(borderProperties.Height, borderProperties.Radius);

        Mesh _mesh = new Mesh();
        _mesh.name = "World Border (Mesh)";
        _mesh.vertices = GenerateVertices(borderProperties.Height, borderProperties.Radius);
        _mesh.triangles = GenerateTriangles();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.SetUVs(0, GenerateUVs(borderProperties.Height, borderProperties.Radius));

        CreateGameObject();

        GameObject border = new GameObject("World Border");
        border.transform.position = Vector3.zero;

        MeshFilter meshFilter = border.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = border.AddComponent<MeshRenderer>();
        meshFilter.mesh = _mesh;

        MeshCollider meshCollider = border.AddComponent(typeof(MeshCollider)) as MeshCollider;

        string shaderName = borderProperties.Pixelated ? "Boundary Pixelated" : "Boundary";
        meshRenderer.material = (Material)Resources.Load(shaderName);

        if (borderProperties.Pixelated) meshRenderer.sharedMaterial.SetFloat("_Pixelation", borderProperties.Pixelation);
        meshRenderer.sharedMaterial.SetFloat("_Speed", borderProperties.Speed);
        meshRenderer.sharedMaterial.SetColor("_NearColor", borderProperties.NearColor);
        meshRenderer.sharedMaterial.SetColor("_FarColor", borderProperties.FarColor);
        meshRenderer.sharedMaterial.SetFloat("_MinDistance", borderProperties.MinDistance);
        meshRenderer.sharedMaterial.SetFloat("_MaxDistance", borderProperties.MaxDistance);
        meshRenderer.sharedMaterial.SetFloat("_NearPoint", borderProperties.NearPoint);
        meshRenderer.sharedMaterial.SetFloat("_LineWidth", borderProperties.LineWidth);
        meshRenderer.sharedMaterial.SetFloat("_HorizontalDistortion", borderProperties.HorizontalDistortion);
        meshRenderer.sharedMaterial.SetFloat("_VerticalDirection", (float)borderProperties.VerticalDirection);
        meshRenderer.sharedMaterial.SetFloat("_HorizontalDirection", (float)borderProperties.HorizontalDirection);

    }

    private static void CreateMesh(float _height, float _radius)
    {

    }

    private static void CreateGameObject()
    {

    }

    private static Vector2[] GenerateUVs(float height, float radius)
    {
        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0,1),
            new Vector2(0,1),
            new Vector2(0,1),
            new Vector2(0,1),

            new Vector2(0,1),
            new Vector2(0,1),
            new Vector2(0,1),
            new Vector2(0,1),

            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(0,1),

            new Vector2(0,0), // 3
            new Vector2(1,0), // 4
            new Vector2(0,1), // 1
            new Vector2(1,1), // 2

            new Vector2(0,0), // 3
            new Vector2(1,0), // 4
            new Vector2(0,1), // 1
            new Vector2(1,1), // 2

            new Vector2(0,0), // 3
            new Vector2(1,0), // 4
            new Vector2(0,1), // 1
            new Vector2(1,1), // 2
        };


        //float _heightFactor = _height / 5;

        List<Vector2> uvsScaled = new List<Vector2>();

        foreach (Vector2 uv in uvs)
        {
            float u = uv.x * radius * 2;
            float v = uv.y * height;
            
            Vector2 uvScaled = new Vector2(u, v);
            uvsScaled.Add(uvScaled);
        }

        return uvsScaled.ToArray();
    }

    private static Vector3[] GenerateVertices(float height, float radius)
    {
        return new Vector3[]
        {
            // replace 1 with size!!!!

            // replace 1 with size!!!!

            // replace 1 with size!!!!

            // replace 1 with size!!!!

            // bottom
            new Vector3(-radius, 0, radius),
            new Vector3(radius, 0, radius),
            new Vector3(radius, 0, -radius),
            new Vector3(-radius, 0, -radius),

            // top
            new Vector3(-radius, height, radius),
            new Vector3(radius, height, radius),
            new Vector3(radius, height, -radius),
            new Vector3(-radius, height, -radius),

            // left
            new Vector3(-radius, 0, radius), // bottom right
            new Vector3(-radius, 0, -radius), // bottom left
            new Vector3(-radius, height, radius),// top right ?
            new Vector3(-radius, height, -radius), // top left ?

            // right
            new Vector3(radius, 0, radius), // bottom left
            new Vector3(radius, 0, -radius), // bottom right
            new Vector3(radius, height, radius), // top left
            new Vector3(radius, height, -radius), // top right

            // front
            new Vector3(radius, 0, -radius), // bottom left
            new Vector3(-radius, 0, -radius), // bottom right
            new Vector3(radius, height, -radius), // top left
            new Vector3(-radius, height, -radius), // top right
            
            // back
            new Vector3(-radius, 0, radius), // bottom left
            new Vector3(radius, 0, radius), // bottom right
            new Vector3(-radius, height, radius), // top left
            new Vector3(radius, height, radius), // top right
        };
    }

    private static int[] GenerateTriangles()
    {
        return new int[]
        {
            //// left side
            //1, 2, 3,
            //0, 2, 1,

            //// right side
            //4, 5, 7,
            //6, 4, 7,

            //// front side
            //9, 10, 12,
            //10, 9, 12,

            //// back side
            //13, 14, 15,
            //15, 13, 15,

            //1,0,2,
            //2,0,3,
            //4,5,6,
            //4,6,7,

            //// outward facing walls
            //9,10,11,
            //8,10,9,
            //12,13,15,
            //14,12,15,
            //16,17,19,
            //18,16,19,
            //20,21,23,
            //22,20,23,

            // inward facing walls
            10,9,11,
            10,8,9,
            13,12,15,
            12,14,15,
            17,16,19,
            16,18,19,
            21,20,23,
            20,22,23
        };
    }
}
