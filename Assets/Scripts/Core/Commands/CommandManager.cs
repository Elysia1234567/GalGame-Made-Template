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
        
        public static CommandManager instance { get; private set; }
        //private static Coroutine process = null;
        //public static bool isRunningProcess => process != null;
        private CommandDatabase database;

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
            Delegate command = database.GetCommand(commandName);

            if (command == null)
                return null;

            return StartProcess(commandName, command, args);


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
                command.DynamicInvoke(args[0]);
            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);
            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();
            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args[0]);
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
    }
}