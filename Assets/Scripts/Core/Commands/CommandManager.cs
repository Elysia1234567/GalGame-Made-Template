using CHARACTERS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace COMMANDS
{
    public class CommandManager : MonoBehaviour
    {
        private const char SUB_COMMAN_IDENTIFIER = '.';
        public const string DATABASE_CHARACTERS_BASE = "characters";
        public const string DATABASE_CHARACTERS_SPRITE = "characters_sprite";
        public const string DATABASE_CHARACTERS_LIVE2D = "characters_live2D";
        public const string DATABASE_CHARACTERS_Model3D = "characters_model3D";
        public static CommandManager instance { get; private set; }
        //private static Coroutine process = null;
        //public static bool isRunningProcess => process != null;
        private CommandDatabase database;
        private Dictionary<string,CommandDatabase> subDatabases= new Dictionary<string,CommandDatabase>();

        private List<CommandProcess> activeProcesses=new List<CommandProcess>();//建这个主要是为了能同时监视多个进程
        private CommandProcess topProcess => activeProcesses.Last();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();

                foreach (Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] { database });
                }
            }
            else
                DestroyImmediate(gameObject);
        }

        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if(commandName.Contains(SUB_COMMAN_IDENTIFIER))
            {
                return ExecuteSubCommand(commandName, args);
            }
            Delegate command = database.GetCommand(commandName);

            if (command == null)
                return null;

            return StartProcess(commandName, command, args);


        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts = commandName.Split(SUB_COMMAN_IDENTIFIER);
            string databaseName=string.Join(SUB_COMMAN_IDENTIFIER, parts.Take(parts.Length-1));
            string subCommandName = parts.Last();


            if(subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
                if(command!=null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"数据库{databaseName}中没有叫做{subCommandName}的命令");
                    return null;
                }
            }
            string characterName = databaseName;
            if(CharacterManager.instance.HasCharacter(characterName))
            {
                //Debug.LogWarning("进入了这个部分");
                List<string> newArgs=new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();
                return ExecuteCharacterCommand(subCommandName, args);
            }
            Debug.LogError($"没有叫做{databaseName}的数据库");
            return null;
        }

        private CoroutineWrapper ExecuteCharacterCommand(string commandName, string[] args)
        {
            Delegate command=null;
            Debug.LogWarning($"指令名字是{commandName}");
            CommandDatabase db = subDatabases[DATABASE_CHARACTERS_BASE];
            if(db.HasCommand(commandName))
            {
                command=db.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }
            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch(characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    db = subDatabases[DATABASE_CHARACTERS_SPRITE];
                    break;
                case Character.CharacterType.Live2D:
                    db = subDatabases[DATABASE_CHARACTERS_LIVE2D];
                    break;
                case Character.CharacterType.Model3D:
                    db = subDatabases[DATABASE_CHARACTERS_Model3D];
                    break;

            }
            command = db.GetCommand(commandName);

            if(command!=null)
                return StartProcess(commandName, command, args);
            Debug.LogError($"没有叫做{commandName}的命令");
            return null;
        }

        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            //StopCurrentProcess();
            Guid precessID=Guid.NewGuid();
            CommandProcess cmd=new CommandProcess(precessID, commandName, command,null,args,null);
            activeProcesses.Add(cmd);

            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this,co);
            return cmd.runningProcess;
        }

        public void StopCurrentProcess()
        {
            if (topProcess != null)
                KillProcess(topProcess);
        }

        public void StopAllProcesses()
        {
            foreach (var c in activeProcesses)
            {
                if(c.runningProcess != null&&!c.runningProcess.IsDone)
                    c.runningProcess.Stop();
                //Debug.LogWarning($"目前进程是{c}的终止方法是{c.onTerminateAction}");
                c.onTerminateAction?.Invoke();
            }
        }

        private IEnumerator RunningProcess(CommandProcess process)
        {
            yield return WaitingForProcessToComplete(process.command,process.args);

            KillProcess(process);
        }

        public void KillProcess(CommandProcess cmd)
        {
            activeProcesses.Remove(cmd);

            if (cmd.runningProcess != null && !cmd.runningProcess.IsDone)
                cmd.runningProcess.Stop();
            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if (command is Action)
                command.DynamicInvoke();
            else if (command is Action<string>)
                command.DynamicInvoke(args.Length==0?string.Empty:args[0]);
            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);
            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();
            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length==0?string.Empty: args[0]);
            else if (command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
        }

        public void AddTerminationActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;

            if (process == null)
                return;

            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }

        public CommandDatabase CreateSubDatabase(string name)
        {
            name=name.ToLower();
            if(subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"{name}数据库已经存在");
                return db;
            }
            CommandDatabase newDatabase = new CommandDatabase();
            subDatabases.Add(name, newDatabase);

            return newDatabase;
        }
    }
}