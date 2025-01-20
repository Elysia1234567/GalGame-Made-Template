using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNManager : MonoBehaviour
{
    public static VNManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    public void LoadFile(string filePath)
    {
        List<string> lines=new List<string>();
        TextAsset file =Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);
        }
        catch 
        {
            Debug.LogError($"路径为'Resources/{filePath}的文件不存在'");
            return;
        }
        DialogueSystem.instance.Say(lines,filePath);
    }
}
