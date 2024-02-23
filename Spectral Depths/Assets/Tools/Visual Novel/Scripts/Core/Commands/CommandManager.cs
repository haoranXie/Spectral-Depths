using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

//System responsible for validating and executing special commands
public class CommandManager : MonoBehaviour
{
    public static CommandManager instance { get; private set; }

    private CommandDatabase database;

    //Set everything up
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            database = new CommandDatabase();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMDDatabaseExtension))).ToArray();

            foreach(Type extension in extensionTypes)
            {
                MethodInfo extendMethod = extension.GetMethod("Extend");
                extendMethod.Invoke(null, new object[] { database });
            }
        }
        else
            DestroyImmediate(gameObject);
    }

    public void Execute(string commandName)
    {
        Delegate command = database.GetCommand(commandName);

        if (command != null)
            command.DynamicInvoke();
    }
}
