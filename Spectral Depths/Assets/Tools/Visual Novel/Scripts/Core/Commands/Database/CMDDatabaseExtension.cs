using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    //A base class used to extend the available commands in the CommandDatabase
    public abstract class CMDDatabaseExtension
    {
        public static void Extend(CommandDatabase database) { }
    }
}