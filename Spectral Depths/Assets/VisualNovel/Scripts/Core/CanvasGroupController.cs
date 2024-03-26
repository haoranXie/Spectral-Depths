using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupController
{
    private const float DEFAULT_FADE_SPEED = 3f;

    private MonoBehaviour owner;
    private CanvasGroup rootCG;

    private Coroutine co_showing = null;
    private Coroutine co_hiding = null;
    public bool isShowing => co_showing != null;
    public bool isHiding => co_hiding != null;
    public bool isFading => isShowing || isHiding;

    public bool isVisible => co_showing != null || rootCG.alpha > 0;

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCG)
    {
        this.owner = owner;
        this.rootCG = rootCG;
    }

    public Coroutine Show(float speed = 1f, bool immediate = false)
    {
        if (isShowing)
            return co_showing;

        else if (isHiding)
        {
            DialogueSystem.instance.StopCoroutine(co_hiding);
            co_hiding = null;
        }

        co_showing = DialogueSystem.instance.StartCoroutine(Fading(1, speed, immediate));

        return co_showing;
    }

    public Coroutine Hide(float speed = 1f, bool immediate = false)
    {
        if (isHiding)
            return co_hiding;

        else if (isShowing)
        {
            DialogueSystem.instance.StopCoroutine(co_showing);
            co_showing = null;
        }

        co_hiding = DialogueSystem.instance.StartCoroutine(Fading(0, speed, immediate));

        return co_hiding;
    }

    private IEnumerator Fading(float alpha, float speed, bool immediate)
    {
        CanvasGroup cg = rootCG;

        if (immediate)
            cg.alpha = alpha;

        while (cg.alpha != alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED * speed);
            yield return null;
        }

        co_showing = null;
        co_hiding = null;
    }
}
