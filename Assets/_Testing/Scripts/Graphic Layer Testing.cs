using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunningLayers());
    }

    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer = panel.GetLayer(0, true);

        yield return new WaitForSeconds(1);

        Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/hurricane");
        layer.SetTexture("Graphics/BG Images/2",blendingTexture:blendTex);

        yield return new WaitForSeconds(1);

        layer.SetVideo("Graphics/BG Videos/Fantasy Landscape");

        yield return new WaitForSeconds(1);

        layer.currentGraphic.FadeOut();

        yield return new WaitForSeconds(2);

        //layer.currentGraphic.renderer.material.SetColor("_Color", Color.red);
    }

    IEnumerator RunningLayers()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer0 = panel.GetLayer(0, true);
        GraphicLayer layer1= panel.GetLayer(1, true);

        layer0.SetVideo("Graphics/BG Videos/Nebula");
        layer1.SetTexture("Graphics/BG Images/Spaceshipinterior");

        GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
        GraphicLayer cineLayer=cinematic.GetLayer(0,true);

        Character Raelin = CharacterManager.instance.CreateCharacter("Raelin", true);
        yield return Raelin.Say("Let's take a look at a picture on the cinematic layer.");

        cineLayer.SetTexture("Graphics/Gallery/pup");

        yield return DialogueSystem.instance.Say("Narrator","We truly don't deserve dogs");

        cineLayer.Clear();

        yield return new WaitForSeconds(1);

        panel.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
