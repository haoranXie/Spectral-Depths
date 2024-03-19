using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace COMMANDS
{
    public class CommandProcess
    {
        public Guid ID;
        public string processName;
        public Delegate command;
        public CoroutineWrapper runningProcess;
        public string[] args;

        public UnityEvent onTerminateAction;

        public CommandProcess(Guid iD, string processName, Delegate command, CoroutineWrapper runningProcess, string[] args, UnityEvent onTerminateAction = null)
        {
            ID = iD;
            this.processName = processName;
            this.command = command;
            this.runningProcess = runningProcess;
            this.args = args;
            this.onTerminateAction = onTerminateAction;
        }
    }
}