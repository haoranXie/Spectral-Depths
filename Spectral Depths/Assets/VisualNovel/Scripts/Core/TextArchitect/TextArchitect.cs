using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using System;
using DIALOGUE;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    /// <summary>
    /// The assigned text component for this architect.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    /// <summary>
    /// The text built by this architect.
    /// </summary>
    public string currentText => tmpro.text;
    /// <summary>
    /// The current text that this architect is trying to build. This is excluding any pretext that might be assigned for appending text.
    /// </summary>
    public string targetText { get; private set; } = "";
    /// <summary>
    /// The full text that this architect is trying to display, including the pre text that may have existed before appending the new target text..
    /// </summary>
    public string fullTargetText => preText + targetText;

    /// <summary>
    /// The text that should exist prior to an appending build.
    /// </summary>
    public string preText { get; private set; } = "";

    /// <summary>
    /// The color that is rendering on this text architect's tmpro component.
    /// </summary>
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    /// <summary>
    /// How fast text building is determined by the speed
    /// </summary>
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    /// <summary>
    /// How many characters will be built per frame. When used with the fade technique, this instead just multiplies the speed.
    /// </summary>
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    /// <summary>
    /// if the architect is set to rush, it will display text much faster than normal.
    /// </summary>
    public bool hurryUp = false;

    /// <summary>
    /// A list of all TABuilders available to be used in the architect. TABuilders are what build the strings of text in their own unique ways depening on the selected build method.
    /// </summary>
    private Dictionary<string, Type> builders = new Dictionary<string, Type>();
    /// <summary>
    /// Stores the created Text Architect Builder for generating text here. Changes according to the build method.
    /// </summary>
    private TABuilder builder = null;
    private TABuilder.BuilderTypes _builderType;
    /// <summary>
    /// What type of builder is the architect using to reveal its text?
    /// </summary>
    public TABuilder.BuilderTypes builderType => _builderType;

    private Coroutine buildProcess = null;
    /// <summary>
    /// Is this architect building its text at the moment?
    /// </summary>
    public bool isBuilding => buildProcess != null;

    /// <summary>
    /// Create a text architect using this ui text object
    /// </summary>
    public TextArchitect(TextMeshProUGUI uiTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    { 
        tmpro_ui = uiTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    /// <summary>
    /// Create a text architect using this text object
    /// </summary>
    public TextArchitect(TextMeshPro worldTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    {
        tmpro_world = worldTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    /// <summary>
    /// Get all TABuilder classes and create a dictionary of possible TABuilders for this Text Architect to use.
    /// </summary>
    private void AddBuilderTypes()
    {
        builders = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TABuilder)))
            .ToDictionary(t => t.Name, t => t);
    }

    /// <summary>
    /// Update the TABuilder to the correct one given the build type.
    /// </summary>
    public void SetBuilderType(TABuilder.BuilderTypes builderType)
    {
        string name = TABuilder.CLASS_NAME_PREFIX + builderType.ToString();
        Type classType = builders[name];

        builder = Activator.CreateInstance(classType) as TABuilder;
        builder.architect = this;
        builder.onComplete += OnComplete;

        _builderType = builderType;
    }

    /// <summary>
    /// Build and display a string using this text.
    /// </summary>
    /// <param name="text"></param>
    public Coroutine Build(string text)
    {
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //My own
        tmpro_ui = DialogueSystem.instance.dialogueContainer.dialogueText;
        //
        //
        //
        //
        //
        //
        //
        //
        //
        preText = "";
        targetText = text;

        Stop();

        buildProcess = builder.Build();
        return buildProcess;
    }

    /// <summary>
    /// Append and build a string to what is already being displayed on this text
    /// </summary>
    /// <param name="text"></param>
    public Coroutine Append(string text)
    {
        preText = currentText;
        targetText = text;

        Stop();

        buildProcess = builder.Build();
        return buildProcess;
    }

    /// <summary>
    /// Immediately apply text to the object
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        //My Own
        preText = "";
        targetText = text;

        Stop();

        tmpro.text = targetText;
        builder.ForceComplete();
    }

    /// <summary>
    /// Stop building the text. This will not finish the text, but stop it where it is immediately.
    /// </summary>
    public void Stop()
    {
        if (isBuilding)
            tmpro.StopCoroutine(buildProcess);

        buildProcess = null;
    }

    /// <summary>
    /// Stop any active build process immediately and complete the text.
    /// </summary>
    public void ForceComplete()
    {
        if (isBuilding)
            builder.ForceComplete();

        Stop();

        OnComplete();
    }

    /// <summary>
    /// This is what happens when the text has finished completing.
    /// </summary>
    private void OnComplete()
    {
        hurryUp = false;
        buildProcess = null;
    }
}
