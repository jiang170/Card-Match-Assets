using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class 纹理图集 : ScriptableWizard
{
    public Texture2D[] Textures;
    public string 纹理名 = "Atlas_Texture";
    public int 间隔 = 4;
    [MenuItem("GameObject/2D Object/Sprites/纹理图集")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("生成纹理图集", typeof(纹理图集));
    }

    void OnEnable()
    {
        // 创建一个新的纹理列表
        List<Texture2D> TextureList = new List<Texture2D>();
        // 遍历所选纹理并添加到列表中
        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            foreach (var obj in EditorUtility.CollectDeepHierarchy(Selection.objects))
            {
                Texture2D tex = obj as Texture2D;  // 得到被选中的对象
                if (tex != null)
                {
                    TextureList.Add(tex);
                }
            }
        }
        // 检查列表中是否有纹理
        if (TextureList.Count > 0)
        {
            Textures = new Texture2D[TextureList.Count];
            for (int i = 0; i < TextureList.Count; i++)
            {
                Textures[i] = TextureList[i];
            }
        }
    }

    void OnWizardCreate()
    {
        GenerateAtlas();
    }

    public void ConfigureForAtlas(string TexturePath)
    {
        // 1--获取指定路径下的纹理
        TextureImporter TexImport = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
        TexImport.textureType = TextureImporterType.Default;
        // 2--修改此纹理的设置
        TextureImporterSettings tiSettings = new();
        TexImport.ReadTextureSettings(tiSettings);
        tiSettings.mipmapEnabled = false;
        tiSettings.readable = true;
        tiSettings.wrapMode = TextureWrapMode.Clamp;
        TexImport.maxTextureSize = 4096;
        tiSettings.filterMode = FilterMode.Point;
        tiSettings.npotScale = TextureImporterNPOTScale.None;
        // 3--重新导入纹理
        TexImport.SetTextureSettings(tiSettings);
        AssetDatabase.ImportAsset(TexturePath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }

    public void GenerateAtlas()
    {
        // 生成图集对象
        GameObject AtlasObject = new("obj");
        AtlasData AtlasComp = AtlasObject.AddComponent<AtlasData>();
        AtlasComp.TextureNames = new string[Textures.Length];
        // 使用循环配置每一个要加入到图集的纹理
        for (int i = 0; i < Textures.Length; i++)
        {
            string TexturePath = AssetDatabase.GetAssetPath(Textures[i]);
            // 修改纹理的设置
            ConfigureForAtlas(TexturePath);
            // 将所有纹理的名字都加入到数组
            AtlasComp.TextureNames[i] = TexturePath;
            Debug.Log(i.ToString());
        }
        // 生成纹理图集
        Texture2D tex = new(1, 1, TextureFormat.ARGB32, false);
        AtlasComp.UVs = tex.PackTextures(Textures, 间隔, 4096);
        // 生成资源路径
        string AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + 纹理名 + ".png");
        // 把纹理图集保存成文件
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(AssetPath, bytes);
        bytes = null;
        // 删除生成的纹理图集
        DestroyImmediate(tex);
        // 加入到 AssetDatabase中
        AssetDatabase.ImportAsset(AssetPath);
        // 获取导入的纹理
        AtlasComp.AtlasTexture = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Texture2D)) as Texture2D;
        // 配置纹理图集
        ConfigureForAtlas(AssetDatabase.GetAssetPath(AtlasComp.AtlasTexture));
        AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/atlasdata_" + 纹理名 + ".prefab");
        // 保存为 Prefab
        UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(AtlasObject, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // 删除临时 GameObject
        DestroyImmediate(AtlasObject);
    }
}
