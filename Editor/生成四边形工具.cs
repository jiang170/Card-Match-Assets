using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// 四边形生成窗口
public class 生成四边形工具 : ScriptableWizard
{
    // 四边形的各种属性
    public bool 添加BoxCollider = true;
    public string 四边形名 = "Quad";
    public string 对象名 = "Plane_Object";
    public string 资源文件夹 = "Assets";
    public float Width = 1.0f;
    public float Height = 1.0f;
    public enum 轴点
    {
        左上,
        中上,
        右上,
        右中,
        右下,
        中下,
        左下,
        左中,
        中间,
        自定义
    }
    public 轴点 轴点位置 = 轴点.中间;
    public float 轴点X = 0.5f;
    public float 轴点Y = 0.5f;

    [MenuItem("GameObject/2D Object/Sprites/四边形")]
    // 生成窗口
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("四边形",
            typeof(生成四边形工具), "生成");
        Debug.Log("工具加载成功！");
    }

    void OnWizardCreate()
    {
        Vector3[] Vertices = new Vector3[4];
        // 左下角的顶点
        Vertices[0].x = -轴点X;
        Vertices[0].y = -轴点Y;
        // 右下角的顶点
        Vertices[1].x = Vertices[0].x + Width;
        Vertices[1].y = Vertices[0].y;
        // 左上角的顶点
        Vertices[2].x = Vertices[0].x;
        Vertices[2].y = Vertices[0].y + Height;
        // 右上角的顶点
        Vertices[3].x = Vertices[0].x + Width;
        Vertices[3].y = Vertices[0].y + Height;
        // 创建UV
        Vector2[] UVs = new Vector2[4];
        UVs[0].x = 0.0f;
        UVs[0].y = 0.0f;
        UVs[1].x = 1.0f;
        UVs[1].y = 0.0f;
        UVs[2].x = 0.0f;
        UVs[2].y = 1.0f;
        UVs[3].x = 1.0f;
        UVs[3].y = 1.0f;
        // 创建三角形
        int[] Triangles = new int[6];
        Triangles[0] = 3;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 2;
        Triangles[4] = 1;
        Triangles[5] = 0;
        // 创建四边形
        Mesh quadMesh = new Mesh();
        quadMesh.name = 四边形名;
        quadMesh.vertices = Vertices;
        quadMesh.uv = UVs;
        quadMesh.triangles = Triangles;
        quadMesh.RecalculateNormals();
        // 创建并命名 GameObject
        GameObject plane = new GameObject(对象名);
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));
        // 赋予四边形mesh filter组件
        meshFilter.sharedMesh = quadMesh;
        quadMesh.RecalculateBounds();
        // 赋予四边形box collider组件
        if (添加BoxCollider)
            plane.AddComponent(typeof(BoxCollider));
        // 保存在 Assets 面板
        AssetDatabase.CreateAsset(quadMesh, $"{资源文件夹}/{四边形名}.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("生成成功!");
        // 关闭窗口
        Close();
    }

    void OnInspectorUpdate()
    {
        switch (轴点位置)
        {
            case 轴点.左上:
                轴点X = 0;
                轴点Y = Height;
                break;
            case 轴点.中上:
                轴点X = Width * 0.5f;
                轴点Y = Height;
                break;
            case 轴点.右上:
                轴点X = Width;
                轴点Y = Height;
                break;
            case 轴点.右中:
                轴点X = Width;
                轴点Y = Height * 0.5f;
                break;
            case 轴点.右下:
                轴点X = Width;
                轴点Y = 0;
                break;
            case 轴点.中下:
                轴点X = Width * 0.5f;
                轴点Y = 0;
                break;
            case 轴点.左下:
                轴点X = 0;
                轴点Y = 0;
                break;
            case 轴点.左中:
                轴点X = 0;
                轴点Y = Height * 0.5f;
                break;
            case 轴点.中间:
                轴点X = Width * 0.5f;
                轴点Y = Height * 0.5f;
                break;
            case 轴点.自定义:
                break;
            default:
                break;
        }
    }

    void OnEnable()
    {
        GetFolderSelection();
    }

    void GetFolderSelection()
    {
        if (Selection.objects != null && Selection.objects.Length == 1)
        {
            资源文件夹 = AssetDatabase.GetAssetPath(Selection.objects[0]);            
        }
    }
}
