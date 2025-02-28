using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VISUALNOVEL;

public class TestDialogueFiles : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextAsset fileToRead = null;
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
       // List<string> lines = FileManager.ReadTextAsset(fileToRead);
       string fullPath=AssetDatabase.GetAssetPath(fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath=fullPath.Substring(resourcesIndex+10);

        string filePath = Path.ChangeExtension(relativePath, null);

        LoadFile(filePath);
        //foreach (string line in lines)
        //{
        //    if(string.IsNullOrEmpty(line)) continue;
        //    Debug.Log($"Segmenting line '{line}'");
        //    DIALOGUE_LINE dlLine=DialogueParser.Parse(line);

        //    int i = 0;
        //    foreach(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
        //    {
        //        Debug.Log($"Segment [{i++}]='{segment.dialogue}'[signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $"{segment.signalDelay}" : $"")}");
        //    }
        //}

        //for (int i = 0; i < lines.Count; i++)
        //{
        //    string line = lines[i];
        //    if (string.IsNullOrEmpty(line)) continue;
        //    DIALOGUE_LINE dl = DialogueParser.Parse(line);
        //    Debug.Log($"测试名字{dl.speaker.name} 测试投射名 [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}] 测试位置 {dl.speaker.castPosition}");
        //    List<(int l, string ex)> expr = dl.speaker.CastExpressions;
        //    for (int c = 0; c < expr.Count; c++)
        //    {
        //        Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
        //    }
        //}

        //foreach (string line in lines)
        //{
        //    if(string.IsNullOrWhiteSpace(line)) continue;

        //    DIALOGUE_LINE dl=DialogueParser.Parse(line);

        //    for(int i=0;i<dl.commandData.commands.Count;i++)
        //    {
        //        DL_COMMAND_DATA.Command command = dl.commandData.commands[i];
        //        Debug.Log($"现在是第{i}条命令，命令的名字是{command.name},命令的参数是[{string.Join(", ", command.arguments)}]");
        //    }
        //}
        
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.DownArrow))
        //    DialogueSystem.instance.dialogueContainer.Hide();
        //else if(Input.GetKeyDown(KeyCode.UpArrow))
        //    DialogueSystem.instance.dialogueContainer.Show();
    }
    public void LoadFile(string filePath)
    {
        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);
        }
        catch
        {
            Debug.LogError($"路径为'Resources/{filePath}的文件不存在'");
            return;
        }
        DialogueSystem.instance.Say(lines, filePath);
    }
}
