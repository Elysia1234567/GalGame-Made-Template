using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string MUSIC_VOLUME_PARAMETER_NAME = "MusicVolume";
    public const string SFX_VOLUME_PARAMETER_NAME = "SFXVolume";
    public const string VOICES_VOLUME_PARAMETER_NAME = "VoicesVolume";
    public const float MUTED_VOLUME_LEVEL = -80f;

    private const string SFX_PARENT_NAME = "SFX";
    public static readonly char[] SFX_NAME_FORMAT_CONTAINERS = new char[] { '[', ']' };
    private static string SFX_NAME_FORMAT = $"SFX - {SFX_NAME_FORMAT_CONTAINERS[0]}" + "{0}" + $"{SFX_NAME_FORMAT_CONTAINERS[1]}";
    
    public const float TRACK_TRANSITION_SPEED = 1f;
    public static AudioManager instance { get; private set; } 

    public Dictionary<int,AudioChannel> channels= new Dictionary<int,AudioChannel>();
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voicesMixer;

    public AnimationCurve audioFalloffCurve;

    private Transform sfxRoot;

    public AudioSource[] allSFX => sfxRoot.GetComponentsInChildren<AudioSource>();
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        

        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"找不到叫做{filePath}的音频");
            return null;
        }
        return PlaySoundEffect(clip,mixer,volume,pitch,loop,filePath);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false,string filePath = "")
    {
        string fileName =clip.name;
        if(filePath!=string.Empty)
            fileName= filePath;

        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT,fileName)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position=sfxRoot.position;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume=volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch=pitch;
        effectSource.loop=loop;
        effectSource.Play();

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    public AudioSource PlayVoice(string filePath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filePath,voicesMixer,volume,pitch,loop);
    }

    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip,voicesMixer,volume,pitch,loop);
    }

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();
        AudioSource[] sources=sfxRoot.GetComponentsInChildren<AudioSource>();

        foreach(var source in sources)
        {
            if(source.clip.name.ToLower()==soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public void StopAllSoundEffects()
    {
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach(var source in sources)
        {
            Destroy(source.gameObject) ;
        }
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public AudioTrack PlayTrack(string filePath,int channel=0,bool loop=true,float startingVolume=0f,float volumeCap=1f,float pitch = 1f)
    {
        AudioClip clip=Resources.Load<AudioClip>(filePath);
        if(clip == null )
        {
            Debug.LogError($"找不到在{filePath}的音乐文件");
            return null;
        }

        return PlayTrack(clip,channel,loop,startingVolume,volumeCap,pitch,filePath);
    }

    public bool IsPlayingSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach(var source in sources)
        {
            if(source.clip.name.ToLower()==soundName)
                return true;
        }

        return false;
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f,float pitch=1f,string filePath="")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
        AudioTrack track=audioChannel.PlayTrack(clip,loop,startingVolume,volumeCap,pitch,filePath);
        return null;
    }

    public AudioChannel TryGetChannel(int channelNumber,bool createIfDoesNotExist  =false)
    {
        AudioChannel channel = null;

        if(channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if(createIfDoesNotExist)
        {
            channel= new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }
        return null;
    }

    public void StopTrack(int channel)
    {
        AudioChannel c=TryGetChannel(channel,createIfDoesNotExist:false);

        if (c == null)
            return;
        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach(var channel in channels.Values)
        {
            if(channel.activeTrack!=null&&channel.activeTrack.name.ToLower()==trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public void StopAllTracks()
    {
        foreach(AudioChannel channel in channels.Values)
        {
            channel.StopTrack();
        }
    }

    public void SetMusicVolume(float volume,bool muted)
    {
        volume=muted?MUTED_VOLUME_LEVEL:audioFalloffCurve.Evaluate(volume);
        musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME,volume);
    }

    public void SetSFXVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
        sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, volume);
    }

    public void SetVoicesVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
        voicesMixer.audioMixer.SetFloat(VOICES_VOLUME_PARAMETER_NAME, volume);
    }
}
