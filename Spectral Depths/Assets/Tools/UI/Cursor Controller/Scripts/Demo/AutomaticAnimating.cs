

using UnityEngine;

public class AutomaticAnimating : MonoBehaviour {
	bool calledYet = false;

	void Start()
    {
        InvokeRepeating("Animating", 1.0f, 1f);
    }

	void Animating(){
		if(GetComponent<Animator>().GetBool("Fade") == false && calledYet == false){
			calledYet = true;
			GetComponent<Animator>().SetBool("Fade",true);
		}else if(GetComponent<Animator>().GetBool("Fade") == true && calledYet == false){
			calledYet = true;
			GetComponent<Animator>().SetBool("Fade",false);
		}
		calledYet = false;
	}
}
