using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
/// <summary>
/// 用来装载名字的容器，主要是方便单独为名字实现特殊效果
/// </summary>

namespace DIALOGUE
{
    [System.Serializable]
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI nameText;
        // Start is called before the first frame update

        public void Show(string nameToShow = "")
        {
            root.SetActive(true);
            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetNameColor(Color color) => nameText.color = color;

        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
    }
}


