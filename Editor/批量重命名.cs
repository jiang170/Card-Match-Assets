using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 批量重命名窗口
public class 批量重命名 : ScriptableWizard
{
    public string 名称前缀 = "Object_";
    public int 初始数字 = 1;
    public int 递增值 = 1;
    [MenuItem("Edit/批量重命名...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("批量重命名", typeof(批量重命名), "重命名");
    }

    // 窗口出现时此函数将会被调用
    private void OnEnable()
    {
        UpdateNumber();
    }

    // 窗口中的选择发生改变时此函数被调用
    private void OnSelectionChange()
    {
        UpdateNumber();
    }

    // 更新选择的对象的个数
    private void UpdateNumber()
    {
        helpString = "";
        if (Selection.objects != null)
        {
            helpString = "选择的对象的数量：" + Selection.objects.Length;
        }
    }

    // 实现重命名
    private void OnWizardCreate()
    {
        // 如果一个对象都没选
        if (Selection.objects == null)
        {
            return; // 直接返回
        }
        // 输入的初始数
        int PostFix = 初始数字;
        // 循环命名
        foreach (Object o in Selection.objects)
        {
            o.name = 名称前缀 + PostFix;
            PostFix += 递增值;
        }
    }
}
