using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// 主要作用是使得对话可以加入很多函数
    /// 摆脱了原本字符串的限制，可以让一行的对话实现追加，清除，延时出现等多种效果
    /// 是DL_LINE的其中一部分
    /// </summary>
    public class DL_DIALOGUE_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        public List<DIALOGUE_SEGMENT> segments;
        private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*.?\d*\}";

        //public bool hasDialogue=>segments.Count > 0;
        public DL_DIALOGUE_DATA(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);

        }
        public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
        {
            List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

            int lastIndex = 0;
            DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
            segment.dialogue = matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index);
            segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
            segment.signalDelay = 0;
            //Debug.Log($"现在正在查找{segment}");
            segments.Add(segment);

            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DIALOGUE_SEGMENT();

                string signalMatch = match.Value;
                signalMatch = signalMatch.Substring(1, match.Value.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;
                segments.Add(segment);
            }
            return segments;
        }
        public struct DIALOGUE_SEGMENT
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;
            /// <summary>
            /// 因为有时候为了节省空间，本来分成好几行的会写在一起
            /// 这时为了区分每一节的内容要重新开始还是接着上一节开始
            /// 设置了以下方式
            /// NONE是第一部分或者只有一部分时使用
            /// C是清除，WC是延时清除，A是延续，WA是延时延续
            /// </summary>
            public enum StartSignal { NONE, C, A, WA, WC }

            public bool appendText => startSignal == StartSignal.A || startSignal == StartSignal.WA;
        }

    }
}