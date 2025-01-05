using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunningTwo());
    }
    Character CreateCharacter(string name)=>CharacterManager.instance.CreateCharacter(name);
    IEnumerator Running()
    {
        Character_Sprite Raelin =CreateCharacter("Raelin") as Character_Sprite;
        Raelin.Show();

        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlaySoundEffect("Audio/SFX/thunder_strong_01");

        yield return new WaitForSeconds(1f);
        Raelin.Animate("Hop");
        Raelin.TransitionSprite(Raelin.GetSprite("A2"));
        Raelin.TransitionSprite(Raelin.GetSprite("A_Shocked"), 1);
        Raelin.Say("Yikes!");
    }

    IEnumerator RunningOne()
    {
        Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
        Raelin.Show();

       

        AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic",loop:true);

        yield return Raelin.Say("I'm going to turn off the radio");
        AudioManager.instance.StopSoundEffect("RadioStatic");

        Raelin.Say("It's off now!");
    }

    IEnumerator RunningTwo()
    {
        yield return new WaitForSeconds(1);

        Character_Sprite Raelin =CreateCharacter("Raelin") as Character_Sprite;
        Raelin.Show();

        yield return DialogueSystem.instance.Say("Narrator", "Can we see you ship?");

        GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/5");
        AudioManager.instance.PlayTrack("Audio/Music/Happy",startingVolume:0.5f);
        AudioManager.instance.PlayVoice("Audio/Voices/exclamation");

        Raelin.SetSprite(Raelin.GetSprite("A2"), 0);
        Raelin.SetSprite(Raelin.GetSprite("A_Blush"), 1);
        Raelin.MoveToPosition(new Vector2(0.7f, 0), speed: 0.5f);
        yield return Raelin.Say("Yes,of course!");
        yield return null;
    }

}
