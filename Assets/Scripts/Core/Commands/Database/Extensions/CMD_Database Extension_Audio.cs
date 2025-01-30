using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
        private static string[] PARAM_VOLUME = new string[] { "-v", "-vol","-volume" };
        private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
        private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

        private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_START_VOLUME = new string[] { "-sv", "-startvolume" };
        private static string[] PARAM_SONG = new string[] { "-so", "-song" };
        private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx",new Action<string>(StopSFX));
            database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            database.AddCommand("stopvoice", new Action<string>(StopSFX));
            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambience", new Action<string[]>(PlayAmbience) );
            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySFX(string[] data)
        {
            string filepath;
            float volume, pitch;
            bool loop;

            var parameters=ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SFX, out filepath);

            parameters.TryGetValue(PARAM_VOLUME, out volume,defaultValue:1f);

            parameters.TryGetValue(PARAM_PITCH, out pitch,defaultValue:1f);

            parameters.TryGetValue(PARAM_LOOP, out loop,defaultValue:false);

            string resourcesPath = FilePaths.GetPathToResource(FilePaths.resources_sfx,filepath);
            AudioClip sound = Resources.Load<AudioClip>(resourcesPath);

            if(sound == null)
            {
                Debug.LogWarning($"无法加载{filepath}");
                return;
            }

           
            AudioManager.instance.PlaySoundEffect(sound,volume:volume, pitch:pitch,loop:loop,filePath:resourcesPath);
        }

        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }

        private static void PlayVoice(string[] data)
        {
            string filepath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SFX, out filepath);

            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);

            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);

            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, filepath));

            if (sound == null)
            {
                Debug.LogError($"找不到路径为{filepath}的音频文件");
                return;
            }
               
            AudioManager.instance.PlayVoice(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void PlaySong(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SONG, out filepath);
            filepath=FilePaths.GetPathToResource(FilePaths.resources_music, filepath);

            parameters.TryGetValue(PARAM_CHANNEL,out channel,defaultValue:1);

            PlayTrack(filepath, channel,parameters);
        }

        private static void PlayAmbience(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_AMBIENCE, out filepath);
            filepath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filepath);

            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
        {
            bool loop;
            bool immediate;
            float volumeCap;
            float startVolume;
            float pitch;

            parameters.TryGetValue(PARAM_VOLUME, out volumeCap,defaultValue:1f);

            parameters.TryGetValue(PARAM_START_VOLUME, out startVolume,defaultValue:0f);

            parameters.TryGetValue(PARAM_PITCH, out pitch,defaultValue:1f);

            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

            parameters.TryGetValue(PARAM_IMMEDIATE,out immediate, defaultValue: false);

            AudioClip sound =Resources.Load<AudioClip>(filepath);

            if(sound == null )
            {
                Debug.LogError($"没有找到路径为{filepath}的音频文件");
                return;
            }

            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }

        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else 
                StopTrack(data);
        }

        private static void StopAmbience(string data)
        {
            if(data==string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }
    }

}