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

            EncapsulateData ifData = RipEncapsulationData(currentConversation, currentProgress, false,parentStartingIndex:currentConversation.fileStartIndex);
            EncapsulateData elseData = new EncapsulateData();

            if(ifData.endingIndex+1<currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex+1].Trim();
                if(nextLine==ELSE)
                {
                    elseData=RipEncapsulationData(currentConversation,ifData.endingIndex+1,false,parentStartingIndex:currentConversation.fileStartIndex);
                    
                }
            }

            currentConversation.SetProgress(elseData.isNull? ifData.endingIndex:elseData.endingIndex);
            EncapsulateData selData=conditionResult?ifData:elseData;
            if(!selData.isNull&&selData.lines.Count>0)
            {
                selData.startingIndex += 2;
                selData.endingIndex -= 1 ;
                Conversation newConversation = new Conversation(selData.lines,file:currentConversation.file,fileStartIndex:selData.startingIndex,fileEndIndex:selData.endingIndex);
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