using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    public class LL_Choice : ILogicalLine
    {
        public string keyword => "choice";
       
        private const char CHOICE_IDENTIFIER = '-';

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            var currentConversation = DialogueSystem.instance.conversationManager.conversation;
            var progress =DialogueSystem.instance.conversationManager.conversationProgress;
            EncapsulateData data = RipEncapsulationData(currentConversation,progress,ripHeaderAndEncapsualators:true);
            List<Choice> choices = GetChoicesFromData(data);

            string title = line.dialogueData.rawData;
            ChoicePanel panel = ChoicePanel.instance;
            string[] choiceTitles=choices.Select(c=>c.title).ToArray();

            panel.Show(title,choiceTitles);

            while(panel.isWaitingOnUserChoice)
                yield return null;
            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines);
            DialogueSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex);
            DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            
        }

        private List<Choice> GetChoicesFromData(EncapsulateData data)
        {
            List<Choice> choices=new List<Choice>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                title = string.Empty,
                resultLines = new List<string>()
            };

            foreach(var line in data.lines.Skip(1))
            {
                //Debug.LogWarning("这是一行对话"+line);
                if(IsChoiceStart(line)&&encapsulationDepth==1)
                {
                    //Debug.LogWarning(line + "进入了有选项标识");
                    if (!isFirstChoice)
                    {
                        choices.Add(choice);
                        choice = new Choice
                        {
                            title = string.Empty,
                            resultLines = new List<string>()
                        };
                    }
                    choice.title =line.Trim().Substring(1);
                    isFirstChoice = false;
                    continue;

                }
                AddLineToResults(line,ref choice,ref encapsulationDepth);
            }
            if(!choices.Contains(choice))
                choices.Add(choice) ;

            return choices;
        }

        private void AddLineToResults(string line,ref Choice choice,ref int encapsulationDepth)
        {
            line.Trim();

            if(IsEncapsulationStart(line))
            {
                if(encapsulationDepth>0)
                    choice.resultLines.Add(line);
                encapsulationDepth++;
                return;
            }

            if(IsEncapsulationEnd(line))
            {
                encapsulationDepth--;

                if (encapsulationDepth > 0)
                    choice.resultLines.Add(line);

                return;
            }
            choice.resultLines.Add(line);
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker&&line.speakerData.name.ToLower()==keyword);
        }

        

     
        private bool IsChoiceStart(string line)=>line.Trim().StartsWith(CHOICE_IDENTIFIER);

        

        private struct Choice
        {
            public string title;
            public List<string> resultLines;
        }

    }
}