using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    public static class LogicalLineUtils
    {
       public static class Encapsulation
       {
            public struct EncapsulateData
            {
                public List<string> lines;
                public int startingIndex;
                public int endingIndex;
            }

            private const char ENCAPSULATION_START = '{';
            private const char ENCAPSULATION_END = '}';

            public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
            public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);

            public static EncapsulateData RipEncapsulationData(Conversation conversation,int startingIndex,bool ripHeaderAndEncapsualators=false)
            {
                int encapsulationDepth = 0;
                EncapsulateData data = new EncapsulateData { lines = new List<string>(), endingIndex = 0 };
                for (int i = startingIndex; i < conversation.Count; i++)
                {
                    string line = conversation.GetLines()[i];
                    if(ripHeaderAndEncapsualators||(encapsulationDepth>0&&!IsEncapsulationEnd(line)))
                        data.lines.Add(line);

                    if (IsEncapsulationStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if (IsEncapsulationEnd(line))
                    {
                        encapsulationDepth--;
                        if (encapsulationDepth == 0)
                        {
                            data.endingIndex = i;
                            break;
                        }
                    }
                }
                return data;
            }
        }
    }
}