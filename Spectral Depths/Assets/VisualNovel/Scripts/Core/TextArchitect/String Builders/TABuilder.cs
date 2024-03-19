using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TABuilder
{
    //A direct reference to the text architect that is using this builder.
    public TextArchitect architect = null;

    //Call logic whenever the build process for the builder is complete. The text architect subscribes to this event to reset itself at the end of the build process.
    public delegate void TA_Event();
    public event TA_Event onComplete;

    /// <summary>
    /// This is the string/text that precedes each builder type. Every TABuilder has a name preceded by this prefix that names the class. Instant = PREFIX + Instant. Fade = PREFIX + Fade
    /// </summary>
    public const string CLASS_NAME_PREFIX = "TABuilder_";

    /// <summary>
    /// Different methods that are available to this architect to render text with.
    /// </summary>
    public enum BuilderTypes
    {
        Instant,
        Typewriter,
        Fade
    }

    public virtual Coroutine Build() => null;

    public virtual void ForceComplete()
    {

    }

    protected void OnComplete() => onComplete?.Invoke();
}
