using AYellowpaper.SerializedCollections;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float nameFontSize;
        public float dialogueFontSize;

        public float nameFontScale = 1f;
        public float dialogueFontScale=1f;

        [SerializedDictionary("Path / ID","Sprite")]
        public SerializedDictionary<string,Sprite> sprites =new SerializedDictionary<string, Sprite> ();
        public CharacterConfigData Copy()
        {
            CharacterConfigData result= new CharacterConfigData();

            result.name=name;
            result.alias=alias;
            result.characterType=characterType;
            result.nameFont=nameFont;
            result.dialogueFont=dialogueFont;

            result.nameColor=new Color(nameColor.r, nameColor.g, nameColor.b,nameColor.a);//因为颜色直接赋值会指向同一个引用，所以要建个新的
            result.dialogueColor=new Color(dialogueColor.r,dialogueColor.g,dialogueColor.b,dialogueColor.a);

            result.dialogueFontSize=nameFontSize;
            result.nameFontSize=nameFontSize;

            result.dialogueFontScale = dialogueFontScale;
            result.nameFontScale=nameFontScale;
            return result;
        }

        private static Color defaultColor=>DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont=>DialogueSystem.instance.config.defaultFont;

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;
                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;

                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);//因为颜色直接赋值会指向同一个引用，所以要建个新的
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.dialogueFontSize=DialogueSystem.instance.config.defaultDialogueFontSize;
                result.nameFontSize = DialogueSystem.instance.config.defaultNameFontSize;

                result.dialogueFontScale = 1f;
                result.nameFontScale = 1f;

                return result;
            }
        }
    }
}