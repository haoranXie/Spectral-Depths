using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoadScreen : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip bgm;
    [SerializeField] public Canvas IntroCanvas;
    [SerializeField] public Animator animator;
    void Awake()
    {
        Application.targetFrameRate = 60;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play the animation
        animator.SetTrigger("UI");
        PlayWithFadeIn(bgm,5f);
        // Unsubscribe from the event to prevent multiple subscriptions
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
