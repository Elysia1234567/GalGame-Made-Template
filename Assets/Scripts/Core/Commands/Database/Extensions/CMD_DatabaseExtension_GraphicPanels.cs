using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_GraphicPanels : CMD_DatabaseExtension
    {
        private static string[] PARAM_PANEL = new string[] { "-p", "-panel" };
        private static string[] PARAM_LAYER = new string[] { "-l", "-layer" };
        private static string[] PARAM_MEDIA = new string[] { "-m", "-media" };
        private static string[] PARAM_GRAPHIC = new string[] { "-g", "-graphic" };
        private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };
        private static string[] PARAM_USEVIDEOAUDIO = new string[] { "-aud", "-audio" };

        private const string HOME_DIRECTORY_SYMBOL = "~/";
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia",new Func<string[],IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }

        private static IEnumerator SetLayerMedia(string[] data)
        {
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";
            bool useAudio=false;

            string pathToGraphic = "";
            UnityEngine.Object graphic = null;
            Texture blendTex = null;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel =GraphicPanelManager.instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"找不到叫做{panelName}的面板");
                yield break;
            }

            parameters.TryGetValue(PARAM_LAYER, out layer,defaultValue:0);

            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            parameters.TryGetValue(PARAM_IMMEDIATE,out immediate,defaultValue:false);

            if(!immediate)
                parameters.TryGetValue(PARAM_SPEED,out transitionSpeed,defaultValue:1);

            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio,defaultValue:false);

            pathToGraphic = FilePaths.GetPathToResource(FilePaths.resources_backgroundImages, mediaName);
            graphic = Resources.Load<Texture>(pathToGraphic);

            if(graphic == null )
            {
                pathToGraphic=FilePaths.GetPathToResource(FilePaths.resources_backgroundVideos,mediaName);
                graphic=Resources.Load<VideoClip>(pathToGraphic);
            }

            if(graphic ==null)
            {
                Debug.LogError($"找不到叫做{mediaName}的图片或视频");
                yield break;
            }

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            GraphicLayer graphicLayer = panel.GetLayer(layer, createIfDoesNotExist: true);

            if (graphic is Texture)
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetTexture(graphic as Texture, filePath: pathToGraphic, immediate: true); });
                yield return graphicLayer.SetTexture(graphic as  Texture,transitionSpeed,blendTex,filePath:pathToGraphic,immediate);
            }
            else
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer.SetVideo(graphic as VideoClip, filePath: pathToGraphic, immediate:true); });
                yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed,useAudio, blendTex, filePath:pathToGraphic, immediate);
            }
        }

        private static IEnumerator ClearLayerMedia(string[] data)
        {
            string panelName = "";
            int layer = 0;
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";

            Texture blendTex = null;

            var parameters =ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel=GraphicPanelManager.instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"没有找到叫做{panelName}的面板");
                yield break ;
            }
            parameters.TryGetValue(PARAM_LAYER, out layer,defaultValue:-1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate,defaultValue:false);

            if(!immediate)
            {
                parameters.TryGetValue(PARAM_SPEED,out transitionSpeed,defaultValue:1);
            }

            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            if(layer==-1)
                panel.Clear(transitionSpeed,blendTex,immediate);
            else
            {
                GraphicLayer graphicLayer=panel.GetLayer(layer);
                if(graphicLayer == null)
                {
                    Debug.LogError($"找不到层数为{layer}的层");
                    yield break;
                }

                graphicLayer.Clear(transitionSpeed,blendTex,immediate);
            }
        }

        private static string GetPathToGraphic(string defaultPath,string graphicName)
        {
            if(graphicName.StartsWith(HOME_DIRECTORY_SYMBOL))
            {
                return graphicName.Substring(HOME_DIRECTORY_SYMBOL.Length);
            }
            return defaultPath + graphicName;
        }
    }
}