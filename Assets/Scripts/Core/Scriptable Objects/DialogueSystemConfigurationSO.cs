using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName ="Dialogue System Configuration",menuName ="Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        public CharacterConfigSO characterConfigurationAsset;

        public Color defaultTextColor=Color.white;
        public TMP_FontAsset defaultFont;

        public float defaultDialogueFontSize = 36;
        public float defaultNameFontSize = 38;
    }
}