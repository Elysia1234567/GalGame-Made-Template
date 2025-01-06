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
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            database.AddCommand("showui", new Func<string[],IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[],IEnumerator>(HideDialogueSystem));

            database.AddCommand("showdb", new Func<IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<IEnumerator>(HideDialogueBox));
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