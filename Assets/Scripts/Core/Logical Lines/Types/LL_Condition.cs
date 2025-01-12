using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Condtitions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        private const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string rawCondtiton =ExtractCondition(line.rawData.Trim());
            bool conditionResult=EvaluateCondition(rawCondtiton);

            Conversation currentConversation=DialogueSystem.instance.conversationManager.conversation;
            int currentProgress =DialogueSystem.instance.conversationManager.conversationProgress;

            EncapsulateData ifData = RipEncapsulationData(currentConversation, currentProgress, false);
            EncapsulateData elseData = new EncapsulateData();

            if(ifData.endingIndex+1<currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex+1].Trim();
                if(nextLine==ELSE)
                {
                    elseData=RipEncapsulationData(currentConversation,ifData.endingIndex+1,false);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(ifData.endingIndex);
            EncapsulateData selData=conditionResult?ifData:elseData;
            if(selData.lines.Count>0)
            {
                Conversation newConversation = new Conversation(selData.lines);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }
            
               
            yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0])+1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}