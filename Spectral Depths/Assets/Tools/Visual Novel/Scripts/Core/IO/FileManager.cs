using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Handles saving, loading, and encryption of files in the project
public class FileManager
{
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = true)
    {
        if (!filePath.StartsWith('/'))//When run, it takes in the name of the file which does not start with / so it appends it to the end of the file path /Tools/Visual Novel/GameData/
            filePath = FilePaths.root + filePath;

        List<string> lines = new List<string>();//line, list of strings
        try //If something goes wrong in try, it runs catch
        {
            using (StreamReader sr = new StreamReader(filePath))//StreamReader sr reads lines of text files
            {
                while(!sr.EndOfStream)//So long there are still lines to read...
                {
                    string line = sr.ReadLine();//Reads the strings in list "line"
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))//Controls when to add lines or empty lines(true = add lines, false = don't add lines)
                        lines.Add(line);
                }
            }
        }
        catch(FileNotFoundException ex)
        {
            Debug.LogError($"File not found: '{ex.FileName}'");
        }

        return lines;
    }


    public static List<string> ReadTextAsset(string filePath, bool includeBlankLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if(asset == null)
        {
            Debug.LogError($"Asset not found: '{filePath}'");
            return null;
        }

        return ReadTextAsset(asset, includeBlankLines);
    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using(StringReader sr = new StringReader(asset.text))
        {
            while(sr.Peek() > -1)
            {
                string line = sr.ReadLine();//Reads the strings in list "line"
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))//Controls when to add lines or empty lines(true = add lines, false = don't add lines)
                        lines.Add(line);
            }
        }
        return lines;
    }
}