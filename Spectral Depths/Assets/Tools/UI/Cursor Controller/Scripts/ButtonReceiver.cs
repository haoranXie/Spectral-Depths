

using UnityEngine;
using UnityEngine.EventSystems;

namespace SlimUI.CursorControllerPro{
    public class ButtonReceiver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
        GameObject controllerObj;
        CursorController cursorController;
        TooltipController tooltipController;

        [Header("TOOLTIP")]
        public bool hasTooltip = false;
        public string title = "Tooltip";
        public string body = "Tooltip information goes here.";

        void Start(){
            controllerObj = GameObject.Find("CursorControl");
            cursorController = controllerObj.GetComponent<CursorController>();
            tooltipController = controllerObj.GetComponent<TooltipController>();
        }

        public void OnPointerEnter(PointerEventData eventData){
            cursorController.FadeIn();
            cursorController.HoverSpeed();
            if(hasTooltip) cursorController.tooltipController.ShowTooltip(); tooltipController.UpdateTooltipText(title, body);
        }

        public void OnPointerExit(PointerEventData eventData){
            cursorController.FadeOut();
            cursorController.NormalSpeed();
            if(hasTooltip) cursorController.tooltipController.HideTooltip();
        }
    }
}