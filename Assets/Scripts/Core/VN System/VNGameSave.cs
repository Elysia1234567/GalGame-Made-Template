using DIALOGUE;
using History;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VISUALNOVEL
{
    [System.Serializable]
    public class VNGameSave
    {
        public static VNGameSave activeFile = null;

        public const string FILE_TYPE = ".vns";
        public const string SCREENSHOT_FILE_TYPE = ".jpg";
        public const bool ENCRYPT_FILES = false;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";

        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

        public string playerName;
        public int slotNumber = 1;

        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLogs;

        public void Save()
        {
            activeState = HistoryState.Capture();
            historyLogs=HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData();

            string saveJSON =JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON);

        }

        public void Load()
        {
            if(activeState!=null)
                activeState.Load();
            HistoryManager.instance.history = historyLogs.ToList();
            HistoryManager.instance.logManager.Clear();
            HistoryManager.instance.logManager.Rebuild();

            SetConversationData();

            DialogueSystem.instance.prompt.Hide();
        }

        private void SetConversationData()
        {
            for(int i = 0; i < activeConversations.Length; i++)
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                    if(fullData!=null&&fullData.conversation!=null&&fullData.conversation.Count>0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                        if(compressedData!=null&&compressedData.fileName!=string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);
                            int count = compressedData.endIndex - compressedData.startIndex;
                            
                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count+1).ToList();
                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        }
                        else
                        {
                            Debug.LogError($"未知格式，无法加载对话");
                        }
                    }
                    if(conversation!=null&&conversation.GetLines().Count>0)
                    {
                        if (i == 0)
                            DialogueSystem.instance.conversationManager.StartConversation(conversation);
                        else
                            DialogueSystem.instance.conversationManager.Enqueue(conversation);
                    }
                }
                catch(System.Exception e) 
                {
                    Debug.LogError($"读取存档时出现错误{e}");
                    continue;
                }
            }
        }

        private string[] GetConversationData()
        {
            List<string> retData= new List<string>();
            var conversations=DialogueSystem.instance.conversationManager.GetConversationQueue();

            for(int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new VN_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                else
                {
                    var fullData = new VN_ConversationData();
                    fullData.conversation=conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }
                retData.Add(data);
            }
            return retData.ToArray();
        }
    }
}