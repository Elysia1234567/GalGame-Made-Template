using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config=> _config;

        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;
        private AutoReader autoReader;
        [SerializeField] private CanvasGroup mainCanvas;

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;

        public bool isRunningConversation => conversationManager.isRunning;

        public DialogueContinuePrompt prompt;
        private CanvasGroupController cgController;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }
        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager=new ConversationManager(architect);

            cgController=new CanvasGroupController(this,mainCanvas);
            dialogueContainer.Initialize();

            if (TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager);
        }

        public void OnUserPrompt_Next()
        {
            //Debug.LogWarning("点击到了");
            onUserPrompt_Next?.Invoke();

            if(autoReader != null&&autoReader.isOn)
                autoReader.Disable();
        }

        public void OnSystemPrompt_Next()
        {
            //Debug.LogWarning("点击到了");
            onUserPrompt_Next?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character=CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config=character!=null?character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }
        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            if(config.dialogueFontScale==1)
            dialogueContainer.SetDialogueFontSize(config.dialogueFontSize);
            else
            {
                float fontSize = this.config.defaultDialogueFontSize * config.dialogueFontScale;
                dialogueContainer.SetDialogueFontSize(fontSize);
            }
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            if(config.nameFontScale==1)
            dialogueContainer.nameContainer.SetNameFontSize(config.nameFontSize);
            else
            {
                float fontSize=this.config.defaultNameFontSize * config.nameFontScale;
                dialogueContainer.nameContainer.SetNameFontSize(fontSize);
            }
        }

        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
                HideSpeakerName();
        }
        public void HideSpeakerName()=>dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker,string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker}\"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public bool isVisible =>cgController.isVisible;

        public Coroutine Show(float speed=1f,bool immediate=false) =>cgController.Show(speed,immediate);
       

        public Coroutine Hide(float speed=1f,bool immediate=false) =>cgController.Hide(speed,immediate);
       
    }
}


