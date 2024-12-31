using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name)=> CharacterManager.instance.CreateCharacter(name);
        void Start()
        {
            //Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            //Character Stella = CharacterManager.instance.CreateCharacter("Stella");
            //Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            //--------------------------����Model3D----------------------------------
            Character_Model3D Maria=CreateCharacter("Maria") as Character_Model3D;

            //Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
            //----------------------------����Live2D-----------------------------------
            //Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;
            //Character_Live2D Rice = CreateCharacter("Rice") as Character_Live2D;

            ////Raelin.SetPosition(new Vector2(0, 0));
            //Mao.SetPosition(new Vector2(1, 0));
            //Rice.SetPosition(new Vector2(0.5f, 0));

            //yield return new WaitForSeconds(1);
            //Mao.SetExpression(5);
            //Mao.SetMotion("Rabbit");

            //Rice.SetMotion("MasterBark");
            //Rice.TransitionColor(Color.blue);

            //----------------------------------�ܻ����---------------------------------------------------------
            //Character_Sprite Raelin = CharacterManager.instance.CreateCharacter("Raelin") as Character_Sprite;
            //Character_Sprite Guard = CharacterManager.instance.CreateCharacter("Generic") as Character_Sprite;
            //yield return Raelin.UnHighlight();
            //Raelin.SetPosition(new Vector2(0, 0));
            //Guard.SetPosition(new Vector2(1, 0));

            //yield return new WaitForSeconds(1);

            //Guard.Animate("Hop");
            //yield return Guard.Say("�٣��ⲻ�ǽ�ʿ������");
            //yield return Guard.UnHighlight();

            //yield return Raelin.Highlight();
            //Raelin.FaceRight();
            //Raelin.TransitionSprite(Raelin.GetSprite("A2"));
            //Raelin.TransitionSprite(Raelin.GetSprite("A_Shocked"), layer: 1);
            //Raelin.MoveToPosition(new Vector2(0.1f, 0));
            //Raelin.Animate("Shiver", true);
            //yield return Raelin.Say("��ð���������������");
            //yield return Raelin.UnHighlight();

            //yield return Guard.Highlight();
            //yield return Guard.Say("��ƣ���ƿ��ʿ������");
            //yield return Guard.UnHighlight();

            //yield return Raelin.Highlight();
            //Raelin.Animate("Shiver", false);
            //Raelin.TransitionColor(Color.red);
            //Raelin.TransitionSprite(Raelin.GetSprite("B2"));
            //Raelin.TransitionSprite(Raelin.GetSprite("B_Blush"), layer: 1);
            //yield return Raelin.Say("���๾�๾�๾�๾��...............");
            //yield return Raelin.TransitionColor(Color.white);
            //Raelin.Animate("Hop");
            //Raelin.TransitionSprite(Raelin.GetSprite("B1"));
            //Raelin.TransitionSprite(Raelin.GetSprite("B_Laugh"), layer: 1);
            //yield return Raelin.Say("�����ö���");
            //------------------------------------�ָ���--------------------------------------------

            //yield return Raelin.Show();
            //yield return Guard.Show();
            //yield return new WaitForSeconds(1f);
            ////Raelin.SetPriority(1);
            //CharacterManager.instance.SortCharacters(new string[] {"Generic","Raelin"});
            //yield return new WaitForSeconds(1f);
            //Sprite body = Raelin.GetSprite("B2");
            //Sprite face = Raelin.GetSprite("B_Blush");

            //yield return Raelin.TransitionSprite(face, 1);
            //Raelin.TransitionSprite(body, 0);
            //yield return Raelin.Say("��ã����ǽ�ʿ����");
            //yield return Raelin.Flip();
            //yield return new WaitForSeconds(1);
            //yield return Raelin.UnHighlight();
            //yield return new WaitForSeconds(1);
            //yield return Raelin.TransitionColor(Color.red);
            //yield return new WaitForSeconds(1);
            //yield return Raelin.Highlight();
            //yield return Raelin.MoveToPosition(Vector2.one);
            //yield return Raelin.FaceLeft(0.3f);
            //yield return Raelin.Say("��Զδ��ն��");
            //yield return Raelin.TransitionColor(Color.white);
            //yield return Raelin.Hide();

            //---------------------------�ƶ�����-----------------------------------------
            //Character guard1 = CreateCharacter("Guard1 as Generic");
            //Character guard2 = CreateCharacter("Guard2 as Generic");
            //Character guard3 = CreateCharacter("Guard3 as Generic");

            //guard1.SetPosition(Vector2.zero);
            //guard2.SetPosition(new Vector2(0.5f,0.5f));
            //guard3.SetPosition(Vector2.one);

            //guard1.Show();

            //yield return guard1.MoveToPosition(Vector2.one);
            //yield return guard1.MoveToPosition(Vector2.zero) ;


            //guard2.Show();
            //guard3.Show();
            yield return null;
            //--------------------------�Ի�����--------------------------------------------
            //yield return new WaitForSeconds(1);
            //Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");
            //yield return new WaitForSeconds(1);
            //yield return Raelin.Hide();
            //yield return new WaitForSeconds(0.5f);
            //yield return Raelin.Show();
            //yield return Raelin.Say("��ã����ǽ�ʿ����");
            //Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            //Character Ben = CharacterManager.instance.CreateCharacter("Benjamin");
            //List<string> lines = new List<string>()
            //{
            //    "Hi, there",
            //    "My name is Elen",
            //    "What's your name?",
            //    "Oh,{wa 1} that's very nice."
            //};
            //yield return Elen.Say(lines);

            //Elen.SetNameColor(Color.red);
            //Elen.SetDialogueColor(Color.green);
            //Elen.SetNameFont(tempFont);
            //Elen.SetDialogueFont(tempFont);

            //yield return Elen.Say(lines);

            //Elen.ResetConfigurationData();

            //yield return Elen.Say(lines);
            //lines = new List<string>()
            //{
            //    "I am Adam",
            //    "More lines{c}Here."
            //};
            //yield return Adam.Say(lines);

            //yield return Ben.Say("This is a line that I want to say.{a} It is a simple line.");

            //Debug.Log("Finished");

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}