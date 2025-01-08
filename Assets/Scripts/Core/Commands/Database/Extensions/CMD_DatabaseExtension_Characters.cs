using CHARACTERS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
    {
        private static string[] PARAM_ENABLE => new string[] { "-e", "-enable" };
        private static string[] PARAM_ANIMATE => new string[] { "-a", "-animate" };
        private static string[] PARAM_STATE => new string[] { "-st", "-state" };
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_ONLY => new string[] { "-o", "-only" };
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        private static string[] PARAM_SMOOTH => new string[] { "-sm", "-smooth" };
        private static string[] PARAM_SPRITE => new string[] { "-s", "-sprite" };
        private static string[] PARAM_LAYER => new string[] { "-l", "-layer" };
        private static string PARAM_XPOS => "-x";
        private static string PARAM_YPOS => "-y";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("sort", new Action<string[]>(Sort));
            database.AddCommand("highlight", new Func<string[],IEnumerator>(HighlightAll));
            database.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnHighlightAll));

            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_BASE);
            baseCommands.AddCommand("move",new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("show",new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide",new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
            baseCommands.AddCommand("setposition",new Action<string[]>(SetPosition));
            baseCommands.AddCommand("setColor",new Func<string[],IEnumerator>(SetColor));
            baseCommands.AddCommand("highlight",new Func<string[],IEnumerator>(Highlight));
            baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnHighlight));
            baseCommands.AddCommand("faceright", new Func<string[], IEnumerator>(FaceRight));
            baseCommands.AddCommand("faceleft", new Func<string[], IEnumerator>(FaceLeft));
            baseCommands.AddCommand("animate", new Action<string[]>(Animate));
            CommandDatabase spriteCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_SPRITE);
            spriteCommands.AddCommand("setsprite",new Func<string[],IEnumerator>(SetSprite));
        }

        private static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];
            Character character =CharacterManager.instance.GetCharacter(characterName);

            if(character == null) 
                yield break;

            float x = 0, y = 0;
            float speed = 1;
            bool smooth = false;
            bool immediate = false;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_XPOS, out x);

            parameters.TryGetValue(PARAM_YPOS,out y);

            parameters.TryGetValue(PARAM_SPEED,out speed,defaultValue:1);

            parameters.TryGetValue(PARAM_SMOOTH,out smooth,defaultValue:false);

            parameters.TryGetValue(PARAM_IMMEDIATE,out immediate,defaultValue:false);

            Vector2 position=new Vector2(x, y);

            if(immediate)
                character.SetPosition(position);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => {  character?.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }
              
        }

        public static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate=false;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_ENABLE,out enable,defaultValue:false);
            parameters.TryGetValue(PARAM_IMMEDIATE,out immediate,defaultValue:false);

            Character character= CharacterManager.instance.CreateCharacter(characterName);

            if(!enable)
            {
                return;
            }
            if(immediate)
                character.isVisible = true;
            else
                character.Show();
        }

        private static void Sort(string[] data)
        {
            CharacterManager.instance.SortCharacters(data);
        }

        public static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters= new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach(string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if(character != null)
                {
                    characters.Add(character);
                }
            }
            if(characters.Count ==0)
                yield break;
            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate,defaultValue:false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            foreach(Character character in characters)
            {
                if(immediate)
                    character.isVisible = true;
                else
                    character.Show(speed);

            }
            if(!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach(Character character in characters)
                        character.isVisible = true;
                });
                while(characters.Any(c=>c.isRevealing))
                    yield return null;
            }
        }

        public static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            if (characters.Count == 0)
                yield break;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = false;
                else
                    character.Hide(speed);

            }
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = false;
                });
                while (characters.Any(c => c.isHiding))
                    yield return null;
            }
        }

        public static IEnumerator HighlightAll(string[] data)
        {
            List<Character> characters =new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters=new List<Character>();

            for(int i=0;i<data.Length;i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i],createIfDoesNotExist:false);
                if(character != null)
                {
                    characters.Add(character);
                }
            }
            if(characters.Count==0)
                yield break;
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_ONLY,out handleUnspecifiedCharacters, defaultValue: true);

            foreach (Character character in characters)
                character.Highlight(immediate: immediate);
            if(handleUnspecifiedCharacters)
            {
                foreach(Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;
                    unspecifiedCharacters.Add(character);
                    character.UnHighlight(immediate: immediate);
                }
            }
            if(!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach(var character in characters)
                        character.Highlight(immediate: true);
                    if (!handleUnspecifiedCharacters) return;
                    foreach(var character in unspecifiedCharacters)
                        character.UnHighlight(immediate:true);
                });
                while (characters.Any(c => c.isHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(c => c.isUnHighlighting)))
                    yield return null;
            }
        }

        public static IEnumerator UnHighlightAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            for (int i = 0; i < data.Length; i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            if (characters.Count == 0)
                yield break;
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_ONLY, out handleUnspecifiedCharacters, defaultValue: true);

            foreach (Character character in characters)
                character.UnHighlight(immediate: immediate);
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;
                    unspecifiedCharacters.Add(character);
                    character.Highlight(immediate: immediate);
                }
            }
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.UnHighlight(immediate: true);
                    if (!handleUnspecifiedCharacters) return;
                    foreach (var character in unspecifiedCharacters)
                        character.Highlight(immediate: true);
                });
                while (characters.Any(c => c.isUnHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(c => c.isHighlighting)))
                    yield return null;
            }
        }

        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);
            if(character ==null)
                yield break;
            bool immediate =false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = true;
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if(character!=null) character.isVisible = true; });
                yield return character.Show();
            }
        }

        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);
            if (character == null)
                yield break;
            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = false;
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });
                yield return character.Show();
            }
        }

        public static void SetPriority(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0],createIfDoesNotExist:false);
            int priority;

            if (character == null || data.Length < 2)
                return;
            if (!int.TryParse(data[1], out priority))
                priority = 0;
            character.SetPriority(priority);
        }

        public static IEnumerator SetColor(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            string colorName;
            float speed;
            bool immediate;

            if(character== null || data.Length < 2)
                yield break;
            var parameters = ConvertDataToParameters(data,startingIndex:1);

            parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName);
            bool specifiedSpeed=parameters.TryGetValue(PARAM_SPEED,out speed,defaultValue:1f);
            if(!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE,out immediate,defaultValue:true);
            else
                immediate = false;
            Color color =Color.white;
            color = color.GetColorFromName(colorName);

            if(immediate)
                character.SetColor(color);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(()=> { character?.SetColor(color); });
                character.TransitionColor(color, speed);

            }
            yield break;
        }

        public static void SetPosition(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0],createIfDoesNotExist: false);
            float x = 0,y = 0;

            if(character==null || data.Length < 2)
                return;

            var parameters=ConvertDataToParameters(data,startingIndex:1);

            parameters.TryGetValue(PARAM_XPOS, out x, defaultValue: 0);
            parameters.TryGetValue(PARAM_YPOS,out y, defaultValue: 0);

            character.SetPosition(new Vector2(x,y));
        }

        public static IEnumerator Highlight(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0],createIfDoesNotExist: false) as Character; 

            if(character==null)
                yield break;

            bool immediate = false;

            var parameters=ConvertDataToParameters(data,startingIndex:1);

            parameters.TryGetValue(PARAM_IMMEDIATE,out immediate,defaultValue:false);

            if (immediate)
                character.Highlight(immediate: true);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate:true); });
                yield return character.Highlight();
            }
        }

        public static IEnumerator UnHighlight(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;

            if (character == null)
                yield break;

            bool immediate = false;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            if (immediate)
                character.UnHighlight(immediate: true);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.UnHighlight(immediate: true); });
                yield return character.UnHighlight();
            }
        }

        public static IEnumerator FaceLeft(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;
            if (character == null)
                yield break;
            float speed = 1f;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return character.FaceLeft(speed, immediate);
        }

        public static IEnumerator FaceRight(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0],createIfDoesNotExist: false) as Character;
            if(character == null)
                yield break;
            float speed = 1f;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE,out  immediate, defaultValue: false);

            yield return character.FaceRight(speed, immediate);
        }

        public static void Animate(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            if (character == null)
                 return;
            string animateName;
            bool state;
            var parameters=ConvertDataToParameters(data, startingIndex: 1);

            if(!parameters.TryGetValue(PARAM_ANIMATE, out animateName))
            {
                Debug.LogError("没有动画名字参数");
                return;
            }
            if(parameters.TryGetValue(PARAM_STATE, out state))
            {
                character.Animate(animateName,state);
            }
            else
            {
                character.Animate(animateName);
            }
            return;

        }

        public static IEnumerator SetSprite(string[] data)
        {
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0],createIfDoesNotExist:false) as Character_Sprite;
            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            if(character ==null||data.Length<2)
                yield break;

            var parameters=ConvertDataToParameters(data, startingIndex:1);
            parameters.TryGetValue(PARAM_SPRITE, out spriteName);
            parameters.TryGetValue(PARAM_LAYER, out layer,defaultValue:0);

            bool specifiedSpeed=parameters.TryGetValue(PARAM_SPEED, out speed,defaultValue:0.1f);

            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);
            Sprite sprite=character.GetSprite(spriteName);

            if(sprite == null)
                yield break;

            if(immediate)
            {
                character.SetSprite(sprite, layer);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetSprite(sprite, layer); });
                yield return character.TransitionSprite(sprite,layer,speed);    
            }
        }
    }
}