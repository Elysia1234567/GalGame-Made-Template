using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance {  get; private set; }

    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    public UI_ITEMS ui;

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i==0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();
        //Resolution[] resolutions =Screen.resolutions;
        LoadConfig();
    }

    private void LoadConfig()
    {
        if(File.Exists(VN_Configuration.filePath))
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath,encrypt:VN_Configuration.ENCRYPY);
        else
            VN_Configuration.activeConfig=new VN_Configuration();

        VN_Configuration.activeConfig.Load();

    }
    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        //VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel=panels.First(p=> p.name == panelName);

        if(panel == null)
        {
            Debug.LogWarning($"没有找到叫做{panelName}的菜单");
            return;
        }

        

        if(activePanel!=null && activePanel!=panel)
        {
            activePanel.SetActive(false);
        }
        panel.SetActive(true);
        activePanel = panel;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions=Screen.resolutions;
        List<string> options=new List<string>();

        for(int i = resolutions.Length-1; i >= 0;i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }

    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color button_selectedColor = new Color(1, 0.35f, 0, 1);
        private static Color button_unselectedColor = new Color(1f, 1f, 1f, 1);
        private static Color text_selectedColor = new Color(1, 1f, 0, 1);
        private static Color text_unselectedColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public static Color musicOnColor = new Color(1, 0.65f, 0, 1);
        public static Color musicOffColor = new Color(0.5f,0.5f,0.5f, 1);   
        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoReaderSpeed;

        [Header("Audio")]
        public Slider musicVolume;
        public Image musicFill;
        public Slider sfxVolume;
        public Image sfxFill;
        public Slider voicesVolume;
        public Image voicesFill;
        public Sprite mutedSymbol;
        public Sprite unmutedSymbol;
        public Image musicMute;
        public Image sfxMute;
        public Image voicesMute;

        public void SetButtonColors(Button A,Button B,bool selectedA)
        {
            A.GetComponent<Image>().color=selectedA?button_selectedColor:button_unselectedColor;
            B.GetComponent<Image>().color = !selectedA ? button_selectedColor : button_unselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA? text_selectedColor:text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color=!selectedA?text_selectedColor:text_unselectedColor;
        }
    }

    public void SetDisplayToFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen,ui.windowed,fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.display_resolution = resolution;
        }
        else
            Debug.LogError($"分辨率格式错误");
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue,ui.skippingStop,continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;

        if(DialogueSystem.instance!=null)
            DialogueSystem.instance.conversationManager.architect.speed = config.dialogueTextSpeed;
    }

    public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReadSpeed=ui.autoReaderSpeed.value;
        AutoReader autoReader=null;
        if (DialogueSystem.instance!=null)
        autoReader =DialogueSystem.instance.autoReader;
        
        if (autoReader != null)
            autoReader.speed = config.dialogueAutoReadSpeed;
    }

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value;
        AudioManager.instance.SetMusicVolume(config.musicVolume,config.musicMute);
        ui.musicFill.color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetSFXVolume()
    {
        config.sfxVolume=ui.sfxVolume.value;
        AudioManager.instance.SetSFXVolume(config.sfxVolume,config.sfxMute);
        ui.sfxFill.color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetVoicesVolume()
    {
        config.voicesVolume=ui.voicesVolume.value;
        AudioManager.instance.SetVoicesVolume(config.voicesVolume,config.voicesMute);
        ui.voicesFill.color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetMusicMute()
    {
        config.musicMute=!config.musicMute;
        ui.musicFill.color=config.musicMute?UI_ITEMS.musicOffColor:UI_ITEMS.musicOnColor;
        ui.musicMute.sprite=config.musicMute?ui.mutedSymbol:ui.unmutedSymbol;

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);
    }

    public void SetSFXMute()
    {
        config.sfxMute = !config.sfxMute;
        ui.sfxFill.color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;
        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);
    }

    public void SetVoicesMute()
    {
        config.voicesMute = !config.voicesMute;
        ui.voicesFill.color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.voicesMute.sprite = config.voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;
        AudioManager.instance.SetVoicesVolume(config.voicesVolume, config.voicesMute);
    }
}
