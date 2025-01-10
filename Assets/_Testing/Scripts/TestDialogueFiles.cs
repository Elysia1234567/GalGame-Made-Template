using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        List<string> lines = FileManager.ReadTextAsset(fileToRead);
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
        DialogueSystem.instance.Say(lines);
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.DownArrow))
        //    DialogueSystem.instance.dialogueContainer.Hide();
        //else if(Input.GetKeyDown(KeyCode.UpArrow))
        //    DialogueSystem.instance.dialogueContainer.Show();
    }
}
