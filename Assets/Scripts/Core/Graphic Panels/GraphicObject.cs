using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject
{
    private const string NAME_FORMAT = "Graphic - [{0}]";
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    private const string MATERIAL_FIELD_COLOR = "_Color";
    private const string MATERIAL_FIELD_MAINTEX = "_MainTex";
    private const string MATERIAL_FIELD_BLENDTEX = "_BlendTex";
    private const string MATERIAL_FIELD_BLEND = "_Blend";
    private const string MATERIAL_FIELD_ALPHA = "_Alpha";
    public RawImage renderer;

    public bool isVideo { get {  return video != null; } }
    public VideoPlayer video = null;
    public AudioSource audio=null;

    public string graphicPath = "";
    public string graphicName { get; private set; }

    private Coroutine co_fadingIn = null;
    private Coroutine co_fadingOut = null;

    public GraphicObject(GraphicLayer layer,string graphicPath,Texture tex)
    {
        this.graphicPath = graphicPath;

        GameObject ob=new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer=ob.AddComponent<RawImage>();

        graphicName=tex.name;

        InitGraphic();

        renderer.name=string.Format(NAME_FORMAT, graphicName);

        renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
    }

    private void InitGraphic()
    {
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect=renderer.GetComponent<RectTransform>();  
        rect.anchorMin=Vector2.zero;
        rect.anchorMax=Vector2.one;
        rect.offsetMin=Vector2.zero;
        rect.offsetMax=Vector2.one;

        renderer.material = GetTransitionMaterial();

        renderer.material.SetFloat(MATERIAL_FIELD_BLEND, 0);
        renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, 0);
    }

    private Material GetTransitionMaterial()
    {
        Material mat= Resources.Load<Material>(MATERIAL_PATH);

        if(mat!=null)
            return new Material(mat);

        return null;
    }

    GraphicPanelManager panelManager =>GraphicPanelManager.instance;

    public Coroutine FadeIn(float speed,Texture blend =null)
    {
        if(co_fadingOut!=null)
            panelManager.StopCoroutine(co_fadingOut);
        if(co_fadingIn!=null)
            return co_fadingIn;

        co_fadingIn=panelManager.StartCoroutine(Fading(1f,speed,blend));

        return co_fadingIn;
    }

    public Coroutine FadeOut(float speed,Texture blend =null)
    {
        if (co_fadingIn != null)
            panelManager.StopCoroutine(co_fadingIn);
        if (co_fadingOut != null)
            return co_fadingOut;

        co_fadingOut = panelManager.StartCoroutine(Fading(0f, speed, blend));

        return co_fadingOut;
    }

    private IEnumerator Fading(float target,float speed,Texture blend)
    {
        bool isBlending=blend!=null;
        bool fadingIn = target > 0;

        renderer.material.SetTexture(MATERIAL_FIELD_BLENDTEX, blend);
        renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, isBlending ? 1 : fadingIn ? 0 : 1);
        renderer.material.SetFloat(MATERIAL_FIELD_BLEND, isBlending ? fadingIn ? 0 : 1 : 1);

        string opacityParam = isBlending ? MATERIAL_FIELD_BLEND : MATERIAL_FIELD_ALPHA;

        while(renderer.material.GetFloat(opacityParam)!=target)
        {
            float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
            renderer.material.SetFloat(opacityParam, opacity);
            yield return null;
        }

        co_fadingIn = null;
        co_fadingOut=null;
    }
}
