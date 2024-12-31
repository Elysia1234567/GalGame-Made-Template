using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class Character_Text : Character
    {
        public Character_Text(string name, CharacterConfigData config) : base(name, config, prefab:null) 
        {
            Debug.Log($"你创建了一个文本角色{name}");
        }
    }
}