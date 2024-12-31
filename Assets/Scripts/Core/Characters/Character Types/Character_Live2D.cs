using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Expression;
using System.Linq;

namespace CHARACTERS
{
    
    public class Character_Live2D :Character
    {
        public const float DEFAULT_TRANSITION_SPEED = 3f;
        public const int CHARACTER_SORTING_DEPTH_SIZE = 250;
        private CubismRenderController renderController;
        private CubismExpressionController expressionController;
        private Animator motionAnimator;

        private List<CubismRenderController> oldRenderers=new List<CubismRenderController>();

        private float xScale;
        public override bool isVisible 
        { 
            get => isRevealing||renderController.Opacity>0;
            set => renderController.Opacity=value?1:0; 
        }
        public Character_Live2D(string name,CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name,config, prefab)
        {
            Debug.Log($"你创建了一个Live2D角色{name}");

            motionAnimator=animator.transform.GetChild(0).GetComponentInChildren<Animator>();
            renderController=motionAnimator.GetComponent<CubismRenderController>();
            expressionController=motionAnimator.GetComponentInChildren<CubismExpressionController>();

            xScale=renderController.transform.localScale.x;
        }

        public void SetMotion(string animationName)
        {
            motionAnimator.Play(animationName);
        }

        public void SetExpression(int expressionIndex)
        {
            expressionController.CurrentExpressionIndex = expressionIndex;
        }

        public void SetExpression(string expressionName)
        {
            expressionController.CurrentExpressionIndex=GetExpressionIndexByName(expressionName);
        }

        private int GetExpressionIndexByName(string expressionName)
        {
            expressionName=expressionName.ToLower();
            for(int i=0;i<expressionController.ExpressionsList.CubismExpressionObjects.Length;i++)
            {
                CubismExpressionData expr = expressionController.ExpressionsList.CubismExpressionObjects[i];
                if (expr.name.Split('.')[0] == expressionName) return i;
            }
            return -1;
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show? 1.0f : 0.0f;
            while(renderController.Opacity!=targetAlpha)
            {
                renderController.Opacity = Mathf.MoveTowards(renderController.Opacity, targetAlpha, DEFAULT_TRANSITION_SPEED);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);
            foreach(CubismRenderer renderer in renderController.Renderers)
            {
                renderer.Color = color;
            }
        }

        public override IEnumerator ChangingColor(float speed)
        {
            if(!isChangingColor)
                yield return ChangingColorL2D(speed);
            co_changingColor = null;
        }

        public override IEnumerator Highlighting(float speedMultiplier)
        {
            Color targetColor = displayColor;

            yield return ChangingColorL2D(speedMultiplier);

            co_highlighting = null;
        }

        private IEnumerator ChangingColorL2D(float speed)
        {
            CubismRenderer[] renderers=renderController.Renderers;
            Color startColor = renderers[0].Color;

            float colorPercent = 0;

            while(colorPercent!=1)
            {
                colorPercent=Mathf.Clamp01(colorPercent+(DEFAULT_TRANSITION_SPEED*speed*Time.deltaTime));
                Color currentColor=Color.Lerp(startColor, displayColor, colorPercent);

                foreach(CubismRenderer renderer in renderController.Renderers)
                {
                    renderer.Color = currentColor;
                }
                yield return null;
            }
        }

        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            GameObject newLive2DCharacter = CreateNewCharacterController();
            newLive2DCharacter.transform.localScale = new Vector3(1, newLive2DCharacter.transform.localScale.y, newLive2DCharacter.transform.localScale.z);
            renderController.Opacity = 0;
            float transitionSpeed=DEFAULT_TRANSITION_SPEED*speedMultiplier*Time.deltaTime;

            while(renderController.Opacity<1||oldRenderers.Any(r=>r.Opacity>0))
            {
                renderController.Opacity=Mathf.MoveTowards(renderController.Opacity, 1, transitionSpeed);  
                foreach(CubismRenderController oldRenderer in oldRenderers) 
                    oldRenderer.Opacity=Mathf.MoveTowards(oldRenderer.Opacity, 0, transitionSpeed);
                yield return null;
            }

            foreach(CubismRenderController r in oldRenderers)
                Object.Destroy(r.gameObject);

            oldRenderers.Clear();

            co_flipping = null;
        }

        private GameObject CreateNewCharacterController()
        {
            oldRenderers.Add(renderController);

            GameObject newLive2DCharacter = Object.Instantiate(renderController.gameObject,renderController.transform.parent);
            newLive2DCharacter.name = name;
            renderController = newLive2DCharacter.GetComponent<CubismRenderController>();
            expressionController=newLive2DCharacter.GetComponent<CubismExpressionController>();
            motionAnimator = newLive2DCharacter.GetComponent<Animator>();

            return newLive2DCharacter;
        }

        public override void OnSort(int sortingIndex)
        {
            renderController.SortingOrder = sortingIndex * CHARACTER_SORTING_DEPTH_SIZE;
        }
    }
}