using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TABuilder_Fade : TABuilder
{
    private int pretextlength = 0;

    public override Coroutine Build()
    {
        Prepare();

        return architect.tmpro.StartCoroutine(Building());
    }

    public override void ForceComplete()
    {
        architect.tmpro.ForceMeshUpdate();
    }

    private void Prepare()
    {
        architect.tmpro.text = architect.preText;
        if (architect.preText != "")
        {
            architect.tmpro.ForceMeshUpdate();
            pretextlength = architect.tmpro.textInfo.characterCount;
        }
        else
            pretextlength = 0;

        architect.tmpro.text += architect.targetText;
        architect.tmpro.maxVisibleCharacters = int.MaxValue;
        architect.tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = architect.tmpro.textInfo;

        Color colorVisible = new Color(architect.textColor.r, architect.textColor.g, architect.textColor.b, 1);
        Color colorHidden = new Color(architect.textColor.r, architect.textColor.g, architect.textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            if (i < pretextlength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }

        architect.tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator Building()
    {
        int minRange = pretextlength;
        int maxRange = minRange + 1;

        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = architect.tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while(true)
        {
            float fadeSpeed = (architect.hurryUp ? architect.charactersPerCycle * 5 : architect.charactersPerCycle) * architect.speed * 4f;

            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minRange++;
            }

            architect.tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if(lastCharacterIsInvisible || alphas[maxRange - 1] > alphaThreshold)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (lastCharacterIsInvisible || alphas[maxRange - 1] >= 255)
                    break;
            }

            yield return null;
        }

        OnComplete();
    }
}
