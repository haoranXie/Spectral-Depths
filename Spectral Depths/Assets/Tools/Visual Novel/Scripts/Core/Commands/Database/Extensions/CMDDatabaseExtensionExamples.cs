using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDDatabaseExtensionExamples : CMDDatabaseExtension
{
    new public static void Extend(CommandDatabase database)
    {
        //Add command with no parameters
        database.AddCommand("print", new Action(PrintDefaultMessage));
    }

    private static void PrintDefaultMessage()
    {
        Debug.Log("Printing a default message to console.");
    }
}
