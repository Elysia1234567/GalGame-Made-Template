using System.Collections;
using UnityEngine;
using TMPro;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;//用于二维空间的Text
    private TextMeshPro tmpro_world;//用于三维空间的Text
    public TMP_Text tmpro=>tmpro_ui!=null?tmpro_ui:tmpro_world;//看你怎么想用二维还是三维

    public string currentText=> tmpro.text;//现在已经显示的文本
    public string targetText { get; private set; } = "";//接下要加上的文本
    public string preText { get; private set; } = "";
    private int preTextLength = 0;

    public string fullTargetText=>preText+targetText;

    public enum BuildMethod
    {
        instant,//立即显示
        typewriter,//打字机
        fade//褪色
    }
    public BuildMethod buildMethod =BuildMethod.typewriter;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float speed { get { return baseSpeed * speedMultiplier; }set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int charactersPerCycle {  get { return speed<=2f?characterMultiplier:speed<=2.5f?characterMultiplier*2:characterMultiplier*3; }}//每一次显示的字符个数
    private int characterMultiplier = 1;

    public bool hurryUp = false;
    public bool forceEnd = false;

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }
    /// <summary>
    ///构筑对话的方法 ,似乎可以和方法一样直接调用，调用之前要先设置好更新方法，执行顺序是Build->Building->Prepare->对应的构建方法->OnComplete
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Build(string text)
    {
        preText = "";
        targetText= text;
        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    /// <summary>
    /// 将文本添加到已有的文本构筑器中
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;
        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess=null;

    public bool isBuilding=>buildProcess!=null;

    public void Stop()
    {
        if (!isBuilding)
            return;
        tmpro.StopCoroutine(buildProcess);
        buildProcess=null;
    }

    IEnumerator Building()
    {
        Prepare();
        
        switch(buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter(); 
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
        forceEnd = false;
    }

    public void ForceComplete()
    {
        tmpro.ForceMeshUpdate();
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters=tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                forceEnd = true;
                break;
        }
    }

    private void Prepare()
    {
        switch(buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;

        }
    }
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text=fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters=tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        tmpro.color=tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text=preText; 

        if(preText!="")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }
        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if(preText!="")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
            preTextLength = 0;
        tmpro.text+=targetText;
        tmpro.maxVisibleCharacters=int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo=tmpro.textInfo;

        Color colorVisable=new Color(textColor.r,textColor.g,textColor.b,1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i = 0;i<textInfo.characterCount;i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if(!charInfo.isVisible ) continue;
            if(i<preTextLength)
            {
                for(int v=0;v<4;v++)
                    vertexColors[charInfo.vertexIndex+v]=colorVisable;
            }
            else
            {
                for(int v=0;v<4;v++)
                    vertexColors[charInfo.vertexIndex+v]=colorHidden;
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters<tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;
            yield return new WaitForSeconds(0.015f/speed);
        }
    }

    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;
        int maxRange = minRange+1;

        byte alphaThreshold = 15;
        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas=new float[textInfo.characterCount];

        while(true&&!forceEnd)
        {
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 4f;

            for(int i=minRange; i<maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if(!charInfo.isVisible) continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i],255, fadeSpeed);

                for(int v=0;v<4;v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];
                if (alphas[i] >= 255)
                    minRange++;
            }
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange-1].isVisible;
            if (alphas[maxRange-1]>alphaThreshold||lastCharacterIsInvisible)//这边用maxRange-1是因为minRange已经++了
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }
            yield return new WaitForEndOfFrame();
            
        }
    }

    public void SetText(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        tmpro.text=targetText;
        ForceComplete();
        
    }

    
}
