using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueParser
    {
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";
        public static DIALOGUE_LINE Parse(string rawLine)
        {
            Debug.Log($"Parsing line - '{rawLine}'");

            (string speaker,string dialogue,string commands)=RipContent(rawLine);

            Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}'\nCommands = '{commands}'");

            return new DIALOGUE_LINE(speaker,dialogue,commands);
        }

        private static (string,string,string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";

            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;
            for(int i = 0;i<rawLine.Length;i++)
            {
                char current = rawLine[i];
                if (current == '\\')
                    isEscaped = !isEscaped;
                else if ((current== '"'&&!isEscaped))
                {
                    Debug.LogWarning("进入了对话");
                    if(dialogueStart==-1)
                        dialogueStart = i;
                    else if(dialogueEnd==-1)
                        dialogueEnd = i;
                }
                else
                    isEscaped = false ;
            }
            //Debug.Log(rawLine.Substring(dialogueStart+1,dialogueEnd-dialogueStart-1));
            Regex commandRegex=new Regex(commandRegexPattern);
            MatchCollection matches=commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach(Match match in matches)
            {
                //Debug.LogWarning($"确认命令的匹配,{match.Index},对话的开头{dialogueStart},对话的结尾{dialogueEnd}");
                if (match.Index<dialogueStart||match.Index>dialogueEnd)//处理命令的起始位置，以及判断是否只有命令
                {
                    //Debug.LogWarning("进入了吗");
                    commandStart = match.Index;
                    if (commandStart != -1 && dialogueStart == -1 && dialogueEnd == -1)
                    {
                        //Debug.LogWarning($"应该全为命令");
                        return ("", "", rawLine.Trim());
                    }
                    //Debug.LogWarning($"{commandStart}");
                    break;
                }
               
                   
            }
           
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))//有对话
            {
                speaker=rawLine.Substring(0,dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if(commandStart != -1)
                    commands=rawLine.Substring(commandStart).Trim();
            }
            else if (commandStart != -1 && dialogueStart > commandStart)//只有命令
                commands = rawLine;
            else//没有找到匹配项
            {
                //Debug.LogWarning($"进了没有匹配项");
                dialogue = rawLine;
            }
                
            return (speaker, dialogue, commands);
        }
    }
}


