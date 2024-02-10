using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DLDialogueData
{
    public List<DIALOGUE_SEGMENT> segments; //Segments are segments of dialogue seperated by the commands
    private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}"; //String pattern to identify segments

    public bool hasDialogue => segments.Count > 0;//If we have segments we have dialogue

    //constructor
    public DLDialogueData(string rawDialogue) //Dialogue section ripped from string
    {
        segments = RipSegments(rawDialogue);
    }

    //Splitting dialogue into different segments based off of commands
    public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {
        List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
        MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern); //Finds the index of every match in string

        int lastIndex = 0;

        //Find the first or only segment in file
        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
        segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index)); //If no command matches returns dialogue string otherwise returns the first segment that from 0 to the index of the first command match
        segment.startSignal = DIALOGUE_SEGMENT.StartSignal.None;
        segment.signalDelay = 0;
        segments.Add(segment); //Adds this segment as first of our list of segments"

        //If no commands, there is nothing else to do so ends everything otherwise it preps for making next segment substring by defining its starting index
        if (matches.Count == 0)
            return segments;
        else
            lastIndex = matches[0].Index;

        //Loops to find each segment
        for(int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            segment = new DIALOGUE_SEGMENT();

            //Get the start signal for the segment
            string signalMatch = match.Value;//{A}
            signalMatch = signalMatch.Substring(1, match.Length - 2);//We only get the letter in brackets "A"
            string[] signalSplit = signalMatch.Split(' ');//Splits it up so we only get WA and WC not the time(s) after it

            segment.startSignal = (DIALOGUE_SEGMENT.StartSignal) Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper()); //Where the splitting happens

            //Get the signal delay
            if (signalSplit.Length > 1)//If we have more than one part in identifier meaning that there is a delay signal not just start signal
                float.TryParse(signalSplit[1], out segment.signalDelay);//Getting the second part of identifier, delay signal

            //Get the dialogue for the segment
            int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
            segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
            lastIndex = nextIndex;

            segments.Add(segment);
        }
        return segments;
    }

    public struct DIALOGUE_SEGMENT
    {
        public string dialogue;
        public StartSignal startSignal;
        public float signalDelay;
        public enum StartSignal { None, C, A, WA, WC }

        public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
    }
}
