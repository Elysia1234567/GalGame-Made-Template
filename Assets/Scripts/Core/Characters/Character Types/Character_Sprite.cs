using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TreeEditor.TextureAtlas;

namespace CHARACTERS
{
    public class Character_Sprite : Character
    {
        private const string SPRITE_RENDERER_PARENT_MAME = "Renderers";
        private const string SPRITESHEET_DEFAULT_SHEETNAME = "Default";
        private const char SPRITESHEET_TEX_SPRITE_DELIMITTER = '-';
        private CanvasGroup rootCG=>root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetDirectory = "";
        public override bool isVisible
        {
            get { return isRevealing || rootCG.alpha == 1; }
            set { rootCG.alpha = value ? 1 : 0; }
        }
        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab,string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCG.alpha = ENABLE_ON_START?1:0;
            artAssetDirectory = rootAssetsFolder+"/Images";
            GetLayers();
            Debug.Log($"�㴴����һ�������ɫ{name}");
        }

        private void GetLayers()
        {
            Transform rendererRoot=animator.transform.Find(SPRITE_RENDERER_PARENT_MAME);
            if (rendererRoot == null)
                return;
            for(int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child=rendererRoot.transform.GetChild(i);

                Image rendererImage=child.GetComponentInChildren<Image>();
                if(rendererImage != null )
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite,int layer=0)
        {
            layers[layer].SetSprite(sprite);
        }

        public Sprite GetSprite(string spriteName)
        {
            if(config.characterType==CharacterType.SpriteSheet)
            {
                string[] data = spriteName.Split(SPRITESHEET_TEX_SPRITE_DELIMITTER);
                Sprite[] spriteArray = new Sprite[0];

                if(data.Length==2)
                {
                    string textureName = data[0];
                    spriteName = data[1];
                     spriteArray = Resources.LoadAll<Sprite>($"{artAssetDirectory}/{textureName}");

                   
                }
                else
                {
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}");
                   
                }
                if (spriteArray.Length == 0)
                    Debug.LogWarning($"��ɫ{name}û�н���'{SPRITESHEET_DEFAULT_SHEETNAME}'���ʲ�");
                return Array.Find(spriteArray, sprite => sprite.name == spriteName);

            }
            else
            {
                return Resources.Load<Sprite>($"{artAssetDirectory}/{spriteName}");
            }
        }

        public Coroutine TransitionSprite(Sprite sprite,int layer=0,float speed=1)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self=rootCG;

            while(self.alpha!=targetAlpha)
            {
                self.alpha=Mathf.MoveTowards(self.alpha,targetAlpha,3f*Time.deltaTime);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            color = displayColor;
            foreach(CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        public override IEnumerator ChangingColor(float speed)
        {
            foreach(CharacterSpriteLayer layer in layers)
                layer.TransitionColor(displayColor, speed);

            yield return null;

            while(layers.Any(l=>l.isChangingColor))
            {
                yield return null;
            }

            co_changingColor = null;
        }

        public override IEnumerator Highlighting(float speedMultiplier)
        {
            Color targetColor = displayColor;

            foreach(CharacterSpriteLayer layer in layers)
                layer.TransitionColor(targetColor,speedMultiplier);
            yield return null;

            while(layers.Any(l=>l.isChangingColor))
            { yield return null;}
            co_highlighting = null;
        }

        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            foreach(CharacterSpriteLayer layer in layers)
            {
                if(faceLeft)
                    layer.FaceLeft(speedMultiplier,immediate);
                else
                    layer.FaceRight(speedMultiplier,immediate);
            }
            yield return null;

            while(layers.Any(l=>l.isFlipping))
            { yield return null;}

            co_flipping = null;
        }

    }
}