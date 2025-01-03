using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer = panel.GetLayer(0, true);

        yield return new WaitForSeconds(1);

        Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/hurricane");
        //layer.SetTexture("Graphics/BG Images/2",blendingTexture:blendTex);
        layer.SetVideo("Graphics/BG Videos/Fantasy Landscape");

        //layer.currentGraphic.renderer.material.SetColor("_Color", Color.red);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
