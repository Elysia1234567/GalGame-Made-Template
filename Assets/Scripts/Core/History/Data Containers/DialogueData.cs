using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace History
{
    [System.Serializable]
    public class DialogueData
    {
        public string currentDialogue = "";
        public string currentSpeaker = "";

        public string dialogueFont;
        public Color dialogueColor;
        public float dialogueScale;

        public string speakerFont;
        public Color speakerNameColor;
        public float speakerScale;

        public static DialogueData Capture()
        {
            DialogueData data = new DialogueData();

            var ds = DialogueSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText =ds.dialogueContainer.nameContainer.nameText;

            data.currentDialogue = dialogueText.text;
            data.dialogueFont=FilePaths.resources_font+dialogueText.font.name;
            data.dialogueColor =dialogueText.color;
            data.dialogueScale=dialogueText.fontSize;

            data.currentSpeaker = nameText.text;
            data.speakerFont =FilePaths.resources_font+nameText.font.name;
            data.speakerNameColor =nameText.color;
            data.speakerScale=nameText.fontSize;

            return data;
        }

        public static void Apply(DialogueData data)
        {
            var ds=DialogueSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText = ds.dialogueContainer.nameContainer.nameText;

            ds.conversationManager.architect.SetText(data.currentDialogue);
            //dialogueText.text = data.currentDialogue;
            dialogueText.color = data. dialogueColor;
            dialogueText.fontSize = data.dialogueScale;
            //dialogueText.maxVisibleCharacters=dialogueText.text.Length;
            //dialogueText.ForceMeshUpdate();
            //TMP_TextInfo textInfo = dialogueText.textInfo;
            //Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
            //for(int i = 0; i < vertexColors.Length; i++)
            //{
            //    vertexColors[i].a = 255;
            //}
            //TMP_CharacterInfo[] tMP_CharacterInfos = textInfo.characterInfo;
            //for(int i=0;i<tMP_CharacterInfos.Length;i++)
            //{
            //    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            //    if(!charInfo.isVisible) continue;
            //    for(int v=0;v<4;v++)
            //    {
            //        vertexColors[charInfo.vertexIndex + v].a = 255;
            //    }
            //}
            //dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            nameText.text = data.currentSpeaker;
            if (nameText.text != string.Empty)
                ds.dialogueContainer.nameContainer.Show();
            else
                ds.dialogueContainer.nameContainer.Hide();
            nameText.color=data.speakerNameColor;
            nameText.fontSize = data.speakerScale;

            if(data.dialogueFont!=dialogueText.font.name)
            {
                TMP_FontAsset fontAsset=HistoryCache.LoadFont(data.dialogueFont);
                if(fontAsset!=null) 
                    dialogueText.font = fontAsset;
            }
            if (data.speakerFont!=nameText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.speakerFont);
                if(fontAsset!=null)
                    nameText.font = fontAsset;
            }
        }
    }
}