using CHARACTERS;
using COMMANDS;
using DIALOGUE.LogicalLines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem=>DialogueSystem.instance;
        private Coroutine process=null;
        public bool isRunning => process!=null;

        public TextArchitect architect=null;
        private bool userPrompt = false;

        
        private LogicalLineManager logicalLineManager;

        public Conversation conversation =>(conversationQueue.IsEmpty()?null:conversationQueue.top);
        public int conversationProgress=>(conversationQueue.IsEmpty()?-1:conversationQueue.top.GetProgress());
        private ConversationQueue conversationQueue;
        public ConversationManager(TextArchitect architect)
        {
            this.architect=architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;

            
            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation)=>conversationQueue.Enqueue(conversation);
        public void EnqueuePriority(Conversation conversation)=>conversationQueue.EnqueuePriority(conversation);

        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }
        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();
            Enqueue(conversation);
            process=dialogueSystem.StartCoroutine(RunningConversation());

            return process;
        }

        public void StopConversation()
        {
            if(!isRunning)
                return;
            dialogueSystem.StopCoroutine(process);
            process=null;
        }
        IEnumerator RunningConversation()
        {
            while(!conversationQueue.IsEmpty()) 
            {
                Conversation currentConversation = conversation;

                if(currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }
                    

                DIALOGUE_LINE line = DialogueParser.Parse(rawLine);
                if(logicalLineManager.TryGetLogic(line,out Coroutine logic))
                {
                    //Debug.LogWarning("找到逻辑了");
                    yield return logic;
                }
                else
                {
                    if (line.hasDialogue)
                        yield return Line_RunDialogue(line);
                    if (line.hasCommands)
                        yield return Line_RunCommands(line);
                    //yield return new WaitForSeconds(1);
                    if (line.hasDialogue)
                    {
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcesses();
                    }
                }
                TryAdvanceConversation(currentConversation);
                    
            }
            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if(conversation!=conversationQueue.top)
                return;

            if(conversation.HasReachedEnd())
            {
                conversationQueue.Dequeue();
            }
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);
                
            if(!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();
            //Build dialogue
            yield return BuildLineSegments(line.dialogueData);

            //yield return WaitForUserInput();
            
        }

        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool characterMustBeCreated=(speakerData.makeCharacterEnter||speakerData.isCastingExpressions||speakerData.isCastingPosition);
            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);
            if (speakerData.makeCharacterEnter&&(!character.isVisible&&!character.isRevealing))
                character.Show();
            

            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayname));

            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            if(speakerData.isCastingPosition)
                character.SetPosition(speakerData.castPosition);

            if(speakerData.isCastingExpressions)
            {
                foreach(var ce in speakerData.CastExpressions)
                {
                    //Debug.LogWarning($"正在检测{ce.layer}和{ce.expression}");
                    character.OnReceiveCastingExpression(ce.layer,ce.expression);
                }

            }
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            List<DL_COMMAND_DATA.Command> commands=line.commandData.commands;

            foreach(DL_COMMAND_DATA.Command command in commands)
            {
                if(command.waitForCompletion||command.name=="wait")
                {
                    CoroutineWrapper cw=CommandManager.instance.Execute(command.name,command.arguments);
                    //Coroutine c = CommandManager.instance.Execute(command.name, command.arguments);
                    while(!cw.IsDone)
                    {
                        if(userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }
        
        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for(int i = 0;i<line.segments.Count;i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue,segment.appendText);
            }
        }

        public bool isWaitingOnAutoTimer { get; private set; } = false;
        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:    
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:       
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue,bool append=false)
        {
            dialogue =TagManager.Inject(dialogue);
            if(!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);
            while(architect.isBuilding)
            {
                if(userPrompt)
                {
                    if(!architect.hurryUp)
                        architect.hurryUp=true;
                    else
                        architect.ForceComplete();
                    userPrompt = false;
                }
                yield return null;
            }
        }
        
        IEnumerator WaitForUserInput()
        {
            dialogueSystem.prompt.Show();
            while(!userPrompt)
                yield return null;
            dialogueSystem.prompt.Hide();
            userPrompt = false;
        }
    }
}


