using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    //System that handles parsing functions to convert strings into DialogueLines
    public class DialogueParser
    {
        private const string commandRegexPattern = "\\w*[^\\s]\\("; // A word of any length(w*) so long white space(s) does not(^) follow it and then a parenthathese ('(')

        //Calls RipContent method that parses string into three sections
        //@return three new strings speaker, dialogue, command
        public static DialogueLine Parse(string rawLine)
        {
            Debug.Log($"Parsing line - '{rawLine}'");

            (string speaker, string dialogue, string commands) = RipContent(rawLine);

            Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}'\nCommands = '{commands}'");

            return new DialogueLine(speaker, dialogue, commands);
        }

        //Parses string into three sections
        //@return three sections(string) speaker, dialogue, command
        private static (string, string, string) RipContent(string rawLine)
        {
            //Identifying Dialogue Section
            string speaker = "", dialogue = "", commands = "";

            int dialogueStart = -1; //start of dialogue section
            int dialogueEnd = -1; //end of dialogue section
            bool isEscaped = false; //Should the next quotation be counted

            for (int i = 0; i < rawLine.Length; i++) //Finds start and end of dialogue
            {
                char current = rawLine[i];
                if (current == '\\')
                    isEscaped = !isEscaped;
                else if (current == '"' && !isEscaped)
                {
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                else
                    isEscaped = false;
            }

            //Indentifying Command Pattern
            Regex commandRegex = new Regex(commandRegexPattern); //Making new string pattern assigned to commandRegex
            Match match = commandRegex.Match(rawLine); //Matches rawline with pattern of commandRegex
            int commandStart = -1;
            if(match.Success)
            {
                commandStart = match.Index;

                if(dialogueStart == -1 && dialogueEnd == -1) //If no dialogue in line
                    return ("", "", rawLine.Trim()); //return empty speaker, empty dialogue, trimed rawline(command)(rawline with no space)
            }

            //If we are here then we either have dialogue or a multi word argument in a command. Figure out which it is(quotation marks detected)
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd)) //If it is dialogue
            {
                //We know that we have valid dialogue
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if (commandStart != -1)
                    commands = rawLine.Substring(commandStart).Trim();
            }
            else if (commandStart != -1 && dialogueStart > commandStart)
                commands = rawLine;
            else
                speaker = rawLine;
            return (speaker, dialogue, commands);
        }
    }
}