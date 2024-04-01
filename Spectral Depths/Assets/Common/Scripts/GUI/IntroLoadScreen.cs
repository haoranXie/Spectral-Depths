using System.Collections;
using System.Collections.Generic;
using EmeraldAI;
using SpectralDepths.TopDown;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoadScreen : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip bgm;
    [SerializeField] public Canvas IntroCanvas;
    [SerializeField] public Animator animator;
    [SerializeField] public Transform initialPoint;
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        // Play the animation
        VNEvent.Trigger(VNEventTypes.DisableVNScene, null);
        animator.SetTrigger("UI");
        PlayWithFadeIn(bgm,5f);
        // Unsubscribe from the event to prevent multiple subscriptions
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CustomStart()
    {
        foreach(Character character in LevelManager.Instance.Players)
        {
            EmeraldSystem emeraldSystem = character.GetComponent<EmeraldSystem>();
            EmeraldAPI.Movement.SetCustomDestination(emeraldSystem, initialPoint.position);
        }
    }



    public void PlayWithFadeIn(AudioClip clip, float fadeInDuration)
    {
        StartCoroutine(FadeIn(clip, fadeInDuration));
    }
    IEnumerator FadeIn(AudioClip clip, float fadeInDuration)
    {
        // Fade in
        float timer = 0f;
        audioSource.clip = clip;
        audioSource.Play();
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
    }
}
