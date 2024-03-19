using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TABuilder_Instant : TABuilder
{
    public override Coroutine Build()
    {
        architect.tmpro.color = architect.tmpro.color;
        architect.tmpro.text = architect.fullTargetText;
        architect.tmpro.ForceMeshUpdate();
        architect.tmpro.maxVisibleCharacters = architect.tmpro.textInfo.characterCount;

        OnComplete();

        return null;
    }
}
