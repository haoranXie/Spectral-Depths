

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlimUI.CursorControllerPro;

namespace SlimUI.CursorControllerPro{
    public class Demo : MonoBehaviour{
        public CursorController controller;

        [Header("MULTI-CURSOR WINDOW")]
        public GameObject player1Block;
        public GameObject player2Block;
        public GameObject player3Block;
        public GameObject player4Block;

        public void ChangeTooltipSmoothing(float smoothAmount){
            controller.tooltipController.toolTipSmoothing = smoothAmount;
        }

        public void ChangeParallaxStrength(float strength){
            controller.parallaxStrength = strength;
        }

        public void ChangeToolTipDelay(float delayAmount){
            controller.tooltipController.popUpDelay = delayAmount;
        }

        public void DisableAllCursors(){
            controller.cursorRect.gameObject.SetActive(false);
            controller.cursorRect2.gameObject.SetActive(false);
            controller.cursorRect3.gameObject.SetActive(false);
            controller.cursorRect4.gameObject.SetActive(false);
            controller.cursorObjectPlayer1.SetActive(false);
            controller.cursorObjectPlayer2.SetActive(false);
            controller.cursorObjectPlayer3.SetActive(false);
            controller.cursorObjectPlayer4.SetActive(false);
        }
        
        public void SelectNextPlayer(){
            DisableAllCursors();
            DisablePlayerBlocks();
            if(controller.currentPlayerActive == 0){
                controller.ChangeActivePlayer(1);
                player2Block.SetActive(true);
            }else if(controller.currentPlayerActive == 1){
                controller.ChangeActivePlayer(2);
                player3Block.SetActive(true);
            }else if(controller.currentPlayerActive == 2){
                controller.ChangeActivePlayer(3);
                player4Block.SetActive(true);
            }else if(controller.currentPlayerActive == 3){
                controller.ChangeActivePlayer(0);
                player1Block.SetActive(true);
            }
        }

        void DisablePlayerBlocks(){
            player1Block.SetActive(false);
            player2Block.SetActive(false);
            player3Block.SetActive(false);
            player4Block.SetActive(false);
        }


        public void EnableMultiCursors(){
            DisablePlayerBlocks();
            DisableAllCursors();

            controller.ChangeActivePlayer(0);
            player1Block.SetActive(true);
        }

        public void ChangeCursorTints(int tintColorIndex){
            if(tintColorIndex == 0){
                controller.tint = new Color(1,1,1);
            }else if(tintColorIndex == 1){
                controller.tint = new Color(.38f,.74f,1f);
            }else if(tintColorIndex == 2){
                controller.tint = new Color(1f,.69f,.29f);
            }else if(tintColorIndex == 3){
                controller.tint = new Color(1,.26f,.26f);
            }else if(tintColorIndex == 4){
                controller.tint = new Color(.65f,.31f,1);
            }else if(tintColorIndex == 5){
                controller.tint = new Color(.32f,.9f,.34f);
            }

            controller.cursorObjectPlayer1.SetActive(false);
            controller.cursorObjectPlayer1.SetActive(true);
            controller.cursorObjectPlayer1.GetComponent<CursorTint>().SetColor(controller.tint);
        }

        public void LoadOnlineDocumentation(){
            Application.OpenURL("http://cursorcontrollerpro.slimui.com/documentation/");
        }
    }
}
