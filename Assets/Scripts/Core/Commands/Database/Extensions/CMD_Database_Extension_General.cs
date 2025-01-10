using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_Database_Extension_General : CMD_DatabaseExtension
    {
       
        private static readonly string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static readonly string[] PARAM_SPEED = new string[] { "-s","-spd", "-speed" };
        private static readonly string[] PARAM_FILEPATH = new string[] { "-f", "-file", "filepath" };
        private static readonly string[] PARAM_ENQUEUE = new string[] { "-e", "-enqueue" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            database.AddCommand("showui", new Func<string[],IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[],IEnumerator>(HideDialogueSystem));

            database.AddCommand("showdb", new Func<IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<IEnumerator>(HideDialogueBox));

            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
        }

        private static void LoadNewDialogueFile(string[] data)
        {
            string fileName =string.Empty;
            bool enqueue = false;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue,defaultValue:false);

            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file=Resources.Load<TextAsset>(filePath);

            if(file==null)
            {
                Debug.LogWarning($"路径为{filePath}的文件无法加载，请确保路径输入正确");
                return;
            }

            List<string> lines=FileManager.ReadTextAsset(file,includeBlankLines:true);
            Conversation newConversation = new Conversation(lines);

            if(enqueue)
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.instance.conversationManager.StartConversation(newConversation);
        }

        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static IEnumerator ShowDialogueBox()
        {
            yield return DialogueSystem.instance.dialogueContainer.Show();
        }

        private static IEnumerator HideDialogueBox()
        {
            yield return DialogueSystem.instance.dialogueContainer.Hide();
        }

        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate,defaultValue:false);
            yield return DialogueSystem.instance.Show(speed,immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            yield return DialogueSystem.instance.Hide(speed,immediate);
        }
    }
}