using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

//Data container that holds all information pertaining to a speaker on a specific dialogue line
public class DLSpeakerData
{
    public string name, castName; //name: true name of character, castName: display name

    public string displayname => (castName != string.Empty ? castName : name); //determines whether to show cast name or normal name depending on if there is a cast name

    public Vector2 castPosition; //Position of character sprite
    public List<(int layer, string expression)> CastExpressions { get; set; } //List of information for character sprite location and expression information

    //Assigning the command identifiers as variables
    private const string NAMECAST_ID = " as ";
    private const string POSITIONCAST_ID = " at ";
    private const string EXPRESSIONCAST_ID = " [";
    private const char AXISDELIMITER = ':';
    private const char EXPRESSIONLAYER_JOINER = ',';
    private const char EXPRESSIONLAYER_DELIMITER = ':';

    //Constructor
    public DLSpeakerData(string rawSpeaker) //takes in arsed speakerdata information
    {
        string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}"; //commands pattern for speaker
        MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

        //Populate the data to avoid null references to values
        castName = "";
        castPosition = Vector2.zero;
        CastExpressions = new List<(int layer, string expression)>();

        //If no commands, speaker is just speaker
        if (matches.Count == 0 )
        {
            name = rawSpeaker;
            return;
        }

        //Otherwise, find the commands in the speaker segment of string
        int index = matches[0].Index; //location of first match, " as "
        name = rawSpeaker.Substring(0, index); //Getting the name data

        //Goes through every match and gets diplay name, position, and expression location
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            int startIndex = 0, endIndex = 0;

            //getting the location of display name in the string
            if (match.Value == NAMECAST_ID)
            {
                startIndex = match.Index + NAMECAST_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
            }
            //getting the location of character postion in the string and also its x y information
            else if (match.Value == POSITIONCAST_ID)
            {
                startIndex = match.Index + POSITIONCAST_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                string[] axis = castPos.Split(AXISDELIMITER, System.StringSplitOptions.RemoveEmptyEntries);

                float.TryParse(axis[0], out castPosition.x);

                 if (axis.Length > 1)
                    float.TryParse(axis[1], out castPosition.y);
            }
            //getting the location of expression in the string and its layer and expression name information
            else if (match.Value == EXPRESSIONCAST_ID)
            {
                startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER) //Split into array
                .Select(x =>
                { //Each item split for each other, turned into an integer, and name for expression
                    var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);
                    return (int.Parse(parts[0]), parts[1]);
                }).ToList();
            }
        }
    }
}
