using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager instance { get; private set; }

        private void Awake()
        {
            instance = this;

            VNDatabaseLinkSetup linkSetup=GetComponent<VNDatabaseLinkSetup>();
            linkSetup.SetupExternalLinks();
        }
        public void LoadFile(string filePath)
        {
            List<string> lines = new List<string>();
            TextAsset file = Resources.Load<TextAsset>(filePath);

            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"·��Ϊ'Resources/{filePath}���ļ�������'");
                return;
            }
            DialogueSystem.instance.Say(lines, filePath);
        }
    }
}