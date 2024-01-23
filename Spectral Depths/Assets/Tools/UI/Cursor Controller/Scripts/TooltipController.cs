

using UnityEngine;
using UnityEngine.UI;

namespace SlimUI.CursorControllerPro{
    public class TooltipController : MonoBehaviour{
        CursorController cursorController;
        [Header("TOOLTIPS")]
        [Tooltip("The Player 1 Rect Transform holding the position of the tool tips.")]
        public RectTransform tooltipRect;
        public Text tooltipTitle1;
        public Text tooltipBody1;
        [Tooltip("The Player 2 Rect Transform holding the position of the tool tips.")]
        public RectTransform tooltipRect2;
        public Text tooltipTitle2;
        public Text tooltipBody2;
        [Tooltip("The Player 3 Rect Transform holding the position of the tool tips.")]
        public RectTransform tooltipRect3;
        public Text tooltipTitle3;
        public Text tooltipBody3;
        [Tooltip("The Player 4 Rect Transform holding the position of the tool tips.")]
        public RectTransform tooltipRect4;
        public Text tooltipTitle4;
        public Text tooltipBody4;
        [Tooltip("The 'Width' of the Tooltip game object. This is how Console Cursors determines the size boundaries at runtime.")]
        public int toolTipWidth = 770;
        [Tooltip("The 'Height' of the Tooltip game object. This is how Console Cursors determines the size boundaries at runtime.")]
        public int toolTipHeight = 245;
        [Tooltip("The Pos X of the game object 'container' that is used to re-position the tooltip so it always stays visible on the screen. Adjusting this value will adjust how far left or right the tooltip positions itself when re-aligning.")]
        public int toolTipOffsetX = 468;
        [Tooltip("The Pos Y of the game object 'container' that is used to re-position the tooltip so it always stays visible on the screen. Adjusting this value will adjust how far left or right the tooltip positions itself when re-aligning.")]
        public int toolTipOffsetY = -206;
        [HideInInspector]
        public bool tooFarRight = false;
        [HideInInspector]
        public bool tooFarLeft = false;
        [Range(0.0f, 0.2f)] public float toolTipSmoothing = 0.07f;
        Vector3 toolTipV = Vector3.zero;
        Vector3 toolTipPosition = new Vector3(0,0,0);
        Vector3 toolTipPosition2 = new Vector3(0,0,0);
        Vector3 toolTipPosition3 = new Vector3(0,0,0);
        Vector3 toolTipPosition4 = new Vector3(0,0,0);
        [Tooltip("The delay before the tooltip appears over a button")]
        public float popUpDelay = 0.35f;
        [HideInInspector]
        public float timer = 0.0f;
        [HideInInspector]
        public bool countTimer = false;

        void Start(){
            cursorController = GetComponent<CursorController>();
        }

        public void ToolTipPopUpDelay(){
            if (timer < popUpDelay && countTimer){
                timer += Time.deltaTime;
            }else if (timer >= popUpDelay){
                if (cursorController.currentPlayerActive == 0) tooltipRect.GetComponent<Animator>().SetBool("Show", true);
                else if (cursorController.currentPlayerActive == 1) tooltipRect2.GetComponent<Animator>().SetBool("Show", true);
                else if (cursorController.currentPlayerActive == 2) tooltipRect3.GetComponent<Animator>().SetBool("Show", true);
                else if (cursorController.currentPlayerActive == 3) tooltipRect4.GetComponent<Animator>().SetBool("Show", true);
                countTimer = false;
            }
        }

        public void ToolTipPositions(){
            if(cursorController.currentPlayerActive == 0) tooltipRect.transform.localPosition = Vector3.SmoothDamp(tooltipRect.transform.localPosition, toolTipPosition, ref toolTipV, toolTipSmoothing);
            if(cursorController.currentPlayerActive == 1) tooltipRect2.transform.localPosition = Vector3.SmoothDamp(tooltipRect2.transform.localPosition, toolTipPosition, ref toolTipV, toolTipSmoothing);
            if(cursorController.currentPlayerActive == 2) tooltipRect3.transform.localPosition = Vector3.SmoothDamp(tooltipRect3.transform.localPosition, toolTipPosition, ref toolTipV, toolTipSmoothing);
            if(cursorController.currentPlayerActive == 3) tooltipRect4.transform.localPosition = Vector3.SmoothDamp(tooltipRect4.transform.localPosition, toolTipPosition, ref toolTipV, toolTipSmoothing);
        }

        public void ToolTipBoundaries(RectTransform rect){
            if(rect.anchoredPosition.x <= cursorController.xMin + (toolTipWidth + 70)){ // Too Far left
                tooFarLeft = true;
                if(rect.anchoredPosition.y <= cursorController.yMin + (toolTipHeight + 55)){
                    toolTipPosition = new Vector3(toolTipOffsetX, -toolTipOffsetY, 100);
                }else{
                    toolTipPosition = new Vector3(toolTipOffsetX, toolTipOffsetY, 100);
                }
            }else{ tooFarLeft = false;}
            if(rect.anchoredPosition.x >= cursorController.xMax - (toolTipWidth + 70)){ // Too Far Right
                tooFarRight = true;
                if(rect.anchoredPosition.y <= cursorController.yMin + (toolTipHeight + 55)){
                    toolTipPosition = new Vector3(-toolTipOffsetX, -toolTipOffsetY, 100);
                }else{
                    toolTipPosition = new Vector3(-toolTipOffsetX, toolTipOffsetY, 100);
                }
            }else{ tooFarRight = false;}
            if(rect.anchoredPosition.y <= cursorController.yMin + (toolTipHeight + 55)){
                if(!tooFarRight)toolTipPosition = new Vector3(toolTipOffsetX, -toolTipOffsetY, 100);
            }else{
                if(!tooFarRight)toolTipPosition = new Vector3(toolTipOffsetX, toolTipOffsetY, 100);
            }
        }

        public void UpdateTooltipText(string title, string body){
            if(cursorController.currentPlayerActive == 0) tooltipTitle1.text = title; tooltipBody1.text = body;
            if(cursorController.currentPlayerActive == 1) tooltipTitle2.text = title; tooltipBody2.text = body;
            if(cursorController.currentPlayerActive == 2) tooltipTitle3.text = title; tooltipBody3.text = body;
            if(cursorController.currentPlayerActive == 3) tooltipTitle4.text = title; tooltipBody4.text = body;
        }

        public void ShowTooltip(){
            if(cursorController.canMoveCursor) countTimer = true;
        }

        public void HideTooltip(){
            if(tooltipRect.GetComponent<Animator>().runtimeAnimatorController!=null) tooltipRect.GetComponent<Animator>().SetBool("Show",false);
            if(cursorController.currentPlayerActive == 1) tooltipRect2.GetComponent<Animator>().SetBool("Show",false);
            if(cursorController.currentPlayerActive == 2)  tooltipRect3.GetComponent<Animator>().SetBool("Show",false);
            if(cursorController.currentPlayerActive == 3)  tooltipRect4.GetComponent<Animator>().SetBool("Show",false);
            countTimer = false;
            timer = 0;
        }
    }
}
