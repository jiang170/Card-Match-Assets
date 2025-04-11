/* UV编辑器
 * 用于实现将Unity中的纹理图集中的单个纹理显示出来
 * 作者：江臻亲
 * 版本：1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class UV编辑器 : EditorWindow
{
    // 表示纹理图集预制对象
    public GameObject 纹理图集预制对象 = null;
    // 图集中的数据
    public AtlasData 图集中的数据 = null;
    // 默认选中项的索引
    public int PopupIndex = 0;

    [MenuItem("Window/UV编辑器")]
    static void Init()
    {
        // 显示窗口
        GetWindow(typeof(UV编辑器), false, "纹理图集UV编辑器");
    }

    // OnGUI每帧都会调用多次
    void OnGUI()
    {
        // 绘制纹理图集选择器
        GUILayout.Label("纹理图集选择器",EditorStyles.boldLabel);
        纹理图集预制对象=(GameObject)EditorGUILayout.ObjectField("纹理图集预制对象",纹理图集预制对象,typeof(GameObject),true);
        // 获取图集的Atlas Data组件
        try
        {
            图集中的数据 = 纹理图集预制对象.GetComponent<AtlasData>();
            // 弹出选择器，选择可用的纹理
            PopupIndex = EditorGUILayout.Popup(PopupIndex, 图集中的数据.TextureNames);
        }
        catch { }
        // 单击按钮后
        if (GUILayout.Button("显示此纹理在纹理图集上"))
        {
            // 设置网格对象的UV坐标
            if (Selection.gameObjects.Length > 0)
            {
                Debug.Log("Selection.gameObjects.Length>0");
                foreach (GameObject obj in Selection.gameObjects)
                {
                    // 确认是否为网格对象
                    if (obj.GetComponent<MeshFilter>())
                    {
                        Debug.Log("是网格对象");
                        UpdateUVs(obj, 图集中的数据.UVs[PopupIndex]);
                    }
                }
            }
        }
    }

    // 更新UV
    void UpdateUVs(GameObject MeshObject,Rect AtlasUVs,bool Reset = false)
    {
        // 获取Mesh Filter组件
        MeshFilter meshFilter = MeshObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] Vertices = mesh.vertices;
        Vector2[] UVs = new Vector2[Vertices.Length];
        // 矩形左下角的坐标
        UVs[0].x = (Reset) ? 0 : AtlasUVs.x;
        UVs[0].y = (Reset) ? 0 : AtlasUVs.y;
        // 右下角
        UVs[1].x = (Reset) ? 1 : AtlasUVs.x + AtlasUVs.width;
        UVs[1].y = (Reset) ? 0 : AtlasUVs.y;
        // 左上角
        UVs[2].x = (Reset) ? 0 : AtlasUVs.x;
        UVs[2].y = (Reset) ? 1 : AtlasUVs.y + AtlasUVs.height;
        // 右上角
        UVs[3].x = (Reset) ? 1 : AtlasUVs.x + AtlasUVs.width;
        UVs[3].y = (Reset) ? 1 : AtlasUVs.y + AtlasUVs.height;
        mesh.uv = UVs;
        mesh.vertices = Vertices;
    }

}
