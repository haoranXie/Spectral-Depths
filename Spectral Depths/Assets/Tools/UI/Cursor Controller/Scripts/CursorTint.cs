

using UnityEngine;
using UnityEngine.UI;

namespace SlimUI.CursorControllerPro{
    public class CursorTint : MonoBehaviour{
        [TextArea]
        public string note = "Place this script on the root of a cursor style and it will check all the child objects that need to be tinted. Remember to use the 'TineBypass' script to avoid having an image component colored!";
        Transform[] allChildren;
        [Header("COLOR")]
        [Tooltip("The color of all child object elements in the cursor will use this tint. The 'alpha' value is overwritten, so setting that here won't affect the animation. NOTE: This tint is only used if the 'override tint' is 'unticked' on CursorControl!")]
        public Color tint;
        CursorController cursorController;
        [Tooltip("If true, the CursorControl tint override won't affect this cursor and you can choose individual colors and customization.")]
        public bool localTintOverride = false;

        public void Start(){
            allChildren = GetComponentsInChildren<Transform>();
            cursorController = transform.root.GetComponent<CursorController>();

            foreach (Transform child in allChildren){
                if(child.GetComponent<Image>() != null){
                    if(child.GetComponent<Image>().sprite != null && child.GetComponent<TintBypass>() == null){
                        if(cursorController.overrideTint){
                            child.GetComponent<Image>().color = cursorController.tint;
                        }else{
                            child.GetComponent<Image>().color = tint;
                        }
                    }
                }
            }
        }

        public void SetColor(Color tint){
            allChildren = GetComponentsInChildren<Transform>();
            cursorController = transform.root.GetComponent<CursorController>();

            foreach (Transform child in allChildren){
                if(child.GetComponent<Image>() != null){
                    if(child.GetComponent<Image>().sprite != null && child.GetComponent<TintBypass>() == null){
                        if(cursorController.overrideTint){
                            child.GetComponent<Image>().color = cursorController.tint;
                        }else{
                            child.GetComponent<Image>().color = tint;
                        }
                    }
                }
            }
        }
    }
}

