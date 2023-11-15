using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private const string START = "Start";

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    private bool isFirstUpdate = true;
    private void Update(){
        if(isFirstUpdate){
            isFirstUpdate=false;
            StartCoroutine(LoadLevel());
        }
    }
    IEnumerator LoadLevel(){
        //Play Animation
        transition.SetTrigger(START);
        yield return new WaitForSeconds(transitionTime);
        Loader.LoaderCallback();
    }
}
