using System.Collections;
using UnityEngine;
using TMPro;

//Builds and reveals text on screen
public class TextArchitect
{
    //Text displayed as part of user interface(UI)
    private TextMeshProUGUI tmpro_ui;
    //Text displayed as part of world(3D environment)
    private TextMeshPro tmpro_world;
    //If we have ui space, we will use ui text, otherwise use world text
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    //The current text on screen
    public string currentText => tmpro.text;
    //What specific text currently trying to build
    //Privately changeable but publicly accessible
    public string targetText {get; private set;} = "";
    //The part of the target text that has already been built
    public string preText {get; private set;} = "";
    private int preTextLength = 0;
    //Full string of text that should be built
    public string fullTargetText => preText + targetText;

    //Controls how text is displayed
    public enum BuildMethod{instant, typewriter, fade}
    public BuildMethod buildMethod = BuildMethod.typewriter;
    
    //Text color changer
    public Color textColor {get{return tmpro.color;} set {tmpro.color = value;}}

    //Text speed changer
    public float speed{get{return baseSpeed * speedMultiplier;} set {speedMultiplier = value;}}
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;
    
    //How speed changes when click on screen
    public int charactersPerCycle{get{return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2: characterMultiplier * 3;}}
    private int characterMultiplier = 1;

    public bool hurryUp = false;

    //Core

    //Constructors: Run automatically when you make a new TextArchitect object(text), it initializes the object by assigning the value of the parameter to a variable of the text architect class
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    //Building an entirely new string(text)
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    //Append text to whats already in the text architect
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    //Stops new text from building if already building
    public void Stop()
    {
        if (!isBuilding)
        {
            return;
        }

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    //Building the text based off of method type(instant, typewritter)
    IEnumerator Building()
    {
        Prepare();

        switch(buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }
        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    public void ForceComplete()
    {
        switch(buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                break;
        }

        Stop();
        OnComplete();
    }

    private void Prepare()
    {
        //variable not datatype
        switch(buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private void Prepare_Fade()
    {

    }

    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5: charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    private IEnumerator Build_Fade()
    {
        yield return null;
    }
}
