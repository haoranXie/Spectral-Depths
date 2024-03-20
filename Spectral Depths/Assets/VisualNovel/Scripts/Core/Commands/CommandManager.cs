using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Events;
using CHARACTERS;

namespace COMMANDS
{
    public class CommandManager : MonoBehaviour
    {
        private const char SUB_COMMAND_IDENTIFIER = '.';
        public const string DATABASE_CHARACTERS_BASE = "characters";
        public const string DATABASE_CHARACTERS_SPRITE = "characters_sprite";
        public const string DATABASE_CHARACTERS_LIVE2D = "characters_live2D";
        public const string DATABASE_CHARACTERS_Model3D = "characters_model3D";

        public static CommandManager instance { get; private set; }

        private CommandDatabase database;
        private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();

        public List<CommandProcess> activeProcesses = new List<CommandProcess>();

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
            if (commandName.Contains(SUB_COMMAND_IDENTIFIER))
                return ExecuteSubCommand(commandName, args);

            Delegate command = database.GetCommand(commandName);

            if (command == null)
                return null;

            return StartProcess(commandName, command, args);
        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts = commandName.Split(SUB_COMMAND_IDENTIFIER);
            string databaseName = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
            string subComandName = parts.Last();

            if (subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subComandName);
                if (command != null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"No command called '{subComandName}' was found in sub database '{databaseName}'");
                    return null;
                }
            }

            string characterName = databaseName;
            //If we've made it here then we should try to run as a character command
            if (CharacterManager.instance.HasCharacter(characterName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subComandName, args);
            }

            Debug.LogError($"No sub database called '{databaseName}' exists! Command '{subComandName}' could not be run.");
            return null;
        }

        private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
        {
            Delegate command = null;

            CommandDatabase db = subDatabases[DATABASE_CHARACTERS_BASE];
            if (db.HasCommand(commandName))
            {
                command = db.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }

            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch (characterConfigData.characterType)
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

            if (command != null)
                return StartProcess(commandName, command, args);

            Debug.LogError($"Command Manager was unable to execute command '{commandName}' on character '{args[0]}'. The character name or command may be invalid.");
            return null;
        }

        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            System.Guid processID = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processID, commandName, command, null, args, null);
            activeProcesses.Add(cmd);

            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this, co);

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
                //Debug.Log($"Stop process '{c.command}({string.Join(',', c.args)}) [term={c.onTerminateAction}]'");

                if (c.runningProcess != null && !c.runningProcess.IsDone)
                    c.runningProcess.Stop();

                c.onTerminateAction?.Invoke();
            }

            activeProcesses.Clear();
        }

        private IEnumerator RunningProcess(CommandProcess process)
        {
            yield return WaitingForProcessToComplete(process.command, process.args);

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
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);

            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();

            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

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
            name = name.ToLower();

            if (subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"A database by the name of '{name}' already exists!");
                return db;
            }

            CommandDatabase newDatabase = new CommandDatabase();
            subDatabases.Add(name, newDatabase);

            return newDatabase;
        }
    }
}