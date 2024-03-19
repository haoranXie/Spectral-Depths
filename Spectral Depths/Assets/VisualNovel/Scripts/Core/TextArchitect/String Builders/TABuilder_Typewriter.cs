using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TABuilder_Typewriter : TABuilder
{
    public override Coroutine Build()
    {
        Prepare();
        return architect.tmpro.StartCoroutine(Building());
    }

    public override void ForceComplete()
    {
        architect.tmpro.maxVisibleCharacters = architect.tmpro.textInfo.characterCount;
    }

    private void Prepare()
    {
        architect.tmpro.color = architect.tmpro.color;
        architect.tmpro.maxVisibleCharacters = 0;
        architect.tmpro.text = architect.preText;

        if (architect.preText != "")
        {
            architect.tmpro.ForceMeshUpdate();
            architect.tmpro.maxVisibleCharacters = architect.tmpro.textInfo.characterCount;
        }

        architect.tmpro.text += architect.targetText;
        architect.tmpro.ForceMeshUpdate();
    }

    private IEnumerator Building()
    {
        while (architect.tmpro.maxVisibleCharacters < architect.tmpro.textInfo.characterCount)
        {
            architect.tmpro.maxVisibleCharacters += architect.hurryUp ? architect.charactersPerCycle * 2 : architect.charactersPerCycle;

            yield return new WaitForSeconds(0.015f / (architect.hurryUp ? architect.speed * 5 : architect.speed));
        }

        OnComplete();
    }
}
