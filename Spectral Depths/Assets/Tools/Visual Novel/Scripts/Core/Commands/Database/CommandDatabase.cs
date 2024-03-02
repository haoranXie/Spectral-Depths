using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    //A database of all commands that are available for the CommandManager to use
    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

        public bool HasCommand(string commandName) => database.ContainsKey(commandName);

        //Adding command to database
        public void AddCommand(string commandName, Delegate command)
        {
            if (!database.ContainsKey(commandName))
            {
                database.Add(commandName, command);
            }
            else
                Debug.LogError($"Command already exists in the database '{commandName}'");

        }

        //Getting a command from the database
        public Delegate GetCommand(string commandName)
        {
            if (!database.ContainsKey(commandName))
            {
                Debug.LogError($"Command '{commandName}' does not exist in the database");
                return null;
            }

            return database[commandName];
        }
    }
}