using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CHARACTERS
{
    //Э��Ҳһ��ύ���������ܣ���Ϊ�������̳�Mono
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string , Character> characters = new Dictionary<string , Character>();//Ŀ¼һ�㶼��洢�ڹ�������

        private CharacterConfigSO config=>DialogueSystem.instance.config.characterConfigurationAsset;

        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";
        public string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";

        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";

        [SerializeField] private RectTransform _characterpanel = null;
        [SerializeField] private RectTransform _characterpanel_live2D = null;
        [SerializeField] private Transform _characterpanel_model3D = null;
        public RectTransform characterPanel=> _characterpanel;
        public RectTransform characterPanelLive2D => _characterpanel_live2D;
        public Transform characterPanelModel3D => _characterpanel_model3D;

        private void Awake()
        {
            instance = this;
        }

        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        public Character GetCharacter(string characterName,bool createIfDoesNotExist=false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if(createIfDoesNotExist)
                return CreateCharacter(characterName);
            return null;
        }

        public Character CreateCharacter(string characterName)
        {
            if(characters.ContainsKey(characterName.ToLower()))
            {
                Debug.Log($"��ɫ{characterName}�Ѿ�����");
                return null;
            }
            CHARACTER_INFO info =GetCharacterInfo(characterName);

            Character character=CreateCharacterFromInfo(info);

            characters.Add(info.name.ToLower(), character);
            return character;
        }

       

        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result=new CHARACTER_INFO();

            string[] nameData=characterName.Split(CHARACTER_CASTING_ID,System.StringSplitOptions.RemoveEmptyEntries);

            result.name=nameData[0];
            result.castingName=nameData.Length > 1 ? nameData[1] :result.name;
            result.config=config.GetConfig(result.castingName);

            result.prefab=GetPrefabForCharacter(result.castingName);
            result.rootCharacterFolder=FormatCharacterPath(characterRootPathFormat,result.castingName);
            return result;
        }

        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath=FormatCharacterPath(characterPrefabPathFormat,characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        public string FormatCharacterPath(string path,string characterName)=>path.Replace(CHARACTER_NAME_ID,characterName);

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            switch(info.config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name,info.config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, info.config,info.prefab,info.rootCharacterFolder);
                case Character.CharacterType.Live2D:
                    return new Character_Live2D(info.name, info.config, info.prefab,info.rootCharacterFolder);
                case Character.CharacterType.Model3D:
                    return new Character_Model3D(info.name, info.config, info.prefab,info.rootCharacterFolder);
                default:
                    return null;
            }
        }

        /// <summary>
        /// ͨ���ַ�������ָ����ɫ��ʾ˳��
        /// </summary>
        /// <param name="characterNames"></param>
        public void SortCharacters(string[] characterNames)
        {
            List<Character> sortedCharacters=new List<Character>();

            sortedCharacters=characterNames
                .Select(name=>GetCharacter(name))
                .Where(character=>character!=null)
                .ToList();

            List<Character> remainingCharacters=characters.Values
                .Except(sortedCharacters)
                .OrderBy(character=>character.priority)
                .ToList();

            sortedCharacters.Reverse();
            //��ÿ����ɫ�����ȼ���������
            int startingPriority=remainingCharacters.Count>0?remainingCharacters.Max(c=>c.priority) : 0;
            for(int i=0; i<sortedCharacters.Count; i++)
            {
                Character character = sortedCharacters[i];
                character.SetPriority(startingPriority+i+1,autoSortCharactersOnUI:false);
            }

           
            List<Character> allCharacters=remainingCharacters.Concat(sortedCharacters).ToList();
            
            SortCharacters(allCharacters);
        }

        /// <summary>
        /// ���ݽ�ɫ�����ȼ�����ɫ����
        /// </summary>
        public void SortCharacters()
        {
            List<Character> activeCharacters=characters.Values.Where(c=>c.root.gameObject.activeInHierarchy&&c.isVisible).ToList();
            List<Character> inactiveCharacters=characters.Values.Except(activeCharacters).ToList();

            activeCharacters.Sort((a,b)=>a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inactiveCharacters);

            SortCharacters(activeCharacters);
        }
        /// <summary>
        /// ����Ļ�ϸı��ɫ��˳��
        /// </summary>
        /// <param name="charactersSortingOrder"></param>
        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach(Character character in charactersSortingOrder)
            {
                Debug.Log($"{character.name}�����ȼ���{character.priority}");
                character.root.SetSiblingIndex(i++);
                character.OnSort(i);
            }
        }

        public int GetCharacterCountFromCharacterType(Character.CharacterType characterType)
        {
            int count = 0;
            foreach(var c in characters.Values)
            {
                if(c.config.characterType == characterType)
                    count++;
            }
            return count;
        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";
            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }
    }
}