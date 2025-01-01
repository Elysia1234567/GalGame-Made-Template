using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

        public bool HasCommand(string commandName) => database.ContainsKey(commandName);

        public void AddCommand(string commandName, Delegate command)
        {
            commandName = commandName.ToLower();
            if (!database.ContainsKey(commandName))
            {
                database.Add(commandName, command);
            }
            else
                Debug.LogError($"指令已经存在数据库中了'{commandName}'");
        }

        public Delegate GetCommand(string commandName)
        {
            commandName = commandName.ToLower();
            if (!database.ContainsKey(commandName))
            {
                Debug.LogError($"指令不存在数据库中'{commandName}'");
                return null;
            }

            return database[commandName];
        }
    }
}