

using UnityEngine;

namespace SlimUI.CursorControllerPro{
    public class ParallaxWindow : MonoBehaviour{
        [Header("CursorControl Scene Object")]
        public CursorController cursorController;
        [Tooltip("If you're using a prefab, the cursorController variable will automatically assign itself. In some cases, a window will be spawned at runtime and needs to find the cursor controller parent and assign it automatically.")]
        public bool autoFindController = false;
        RectTransform cursorRect;
        Vector2 center = new Vector2(0,0);
        RectTransform movingCanvas;

        void Start(){
            if(autoFindController){
                cursorController = GameObject.Find("CursorControl").GetComponent<CursorController>();
            }

            movingCanvas = GetComponent<RectTransform>();
            cursorRect = cursorController.cursorRect;
        }

        void Update(){
            if(cursorRect != null){
                movingCanvas.anchoredPosition = cursorRect.anchoredPosition * -cursorController.parallaxStrength;
            }else{
                Debug.LogWarning("Cursor Rect is missing! Cannot Parallax Window.");
            }
        }
    }
}