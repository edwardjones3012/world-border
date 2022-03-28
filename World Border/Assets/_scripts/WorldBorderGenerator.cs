using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class WorldBorderGenerator
{
    public static void Generate(BorderProperties borderProperties)
    {
        CreateTag("World Border");
        RemoveOtherBorders();
        MeshData meshData = CreateMesh(borderProperties.Height, borderProperties.Radius);

        meshData.GameObject.transform.position = borderProperties.CenterVertical ? new Vector3(0, -borderProperties.Height / 2, 0) : Vector3.zero;

        string shaderName = borderProperties.Pixelated ? "Boundary Pixelated" : "Boundary";
        meshData.MeshRenderer.material = (Material)Resources.Load(shaderName);

        if (borderProperties.Pixelated) meshData.MeshRenderer.sharedMaterial.SetFloat("_Pixelation", borderProperties.Pixelation);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_Speed", borderProperties.Speed);
        meshData.MeshRenderer.sharedMaterial.SetColor("_NearColor", borderProperties.NearColor);
        meshData.MeshRenderer.sharedMaterial.SetColor("_FarColor", borderProperties.FarColor);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_MinDistance", borderProperties.MinDistance);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_MaxDistance", borderProperties.MaxDistance);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_NearPoint", borderProperties.NearPoint);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_LineWidth", borderProperties.LineWidth);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_HorizontalDistortion", borderProperties.HorizontalDistortion);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_VerticalDirection", (float)borderProperties.VerticalDirection);
        meshData.MeshRenderer.sharedMaterial.SetFloat("_HorizontalDirection", (float)borderProperties.HorizontalDirection);

    }

    private static MeshData CreateMesh(float height, float radius)
    {
        Mesh _mesh = new Mesh();
        _mesh.name = "World Border (Mesh)";
        _mesh.vertices = GenerateVertices(height, radius);
        _mesh.triangles = GenerateTriangles();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.SetUVs(0, GenerateUVs(height, radius));

        GameObject border = new GameObject("World Border");
        border.tag = "World Border";

        MeshFilter meshFilter = border.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = border.AddComponent<MeshRenderer>();
        meshFilter.mesh = _mesh;

        MeshCollider meshCollider = border.AddComponent(typeof(MeshCollider)) as MeshCollider;

        MeshData meshData = new MeshData();
        meshData.MeshCollider = meshCollider;
        meshData.Mesh = _mesh;
        meshData.MeshFilter = meshFilter;
        meshData.MeshRenderer = meshRenderer;
        meshData.GameObject = border;

        return meshData;
    }

    private static void CreateTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
        }

        tagManager.ApplyModifiedProperties();
    }

    private static void RemoveOtherBorders()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("World Border");
        foreach(GameObject border in borders)
        {
            GameObject.DestroyImmediate(border);
        }
    }

    public static void UpdateFloat(string property, float value)
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("World Border");

        if (borders.Length == 0) return;
        MeshRenderer meshRenderer = borders[0].GetComponent<MeshRenderer>();

        if (meshRenderer.sharedMaterial.HasProperty(property))
        {
            meshRenderer.sharedMaterial.SetFloat(property, value);
        }
    }

    public static void UpdateCenteredPosition(bool value)
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("World Border");

        if (borders.Length == 0) return;
        Transform t = borders[0].GetComponent<Transform>();
        int height = (int)PlayerPrefs.GetFloat("tempHeight", 12);
        t.position = value? new Vector3(0, -height/ 2, 0) : Vector3.zero;
    }

    public static void UpdateHeight(float newHeight)
    {
        int radius = (int)PlayerPrefs.GetFloat("radius", 5);

        CreateMesh(newHeight, radius);

        GameObject[] borders = GameObject.FindGameObjectsWithTag("World Border");
        if (borders.Length == 0) return;
        Transform t = borders[0].GetComponent<Transform>();

        PlayerPrefs.SetFloat("tempHeight", newHeight);
    }

    public static void UpdateShader(string newShader)
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("World Border");

        if (borders.Length == 0) return;
        MeshRenderer meshRenderer = borders[0].GetComponent<MeshRenderer>();
        meshRenderer.material = (Material)Resources.Load(newShader);
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

            new Vector2(0,0), 
            new Vector2(1,0), 
            new Vector2(0,1), 
            new Vector2(1,1),

            new Vector2(0,0),
            new Vector2(1,0), 
            new Vector2(0,1), 
            new Vector2(1,1),

            new Vector2(0,0), // 3
            new Vector2(1,0), // 4
            new Vector2(0,1), // 1
            new Vector2(1,1), // 2
        };

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
