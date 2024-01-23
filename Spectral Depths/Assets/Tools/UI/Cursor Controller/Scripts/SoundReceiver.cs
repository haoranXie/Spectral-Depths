

using UnityEngine;
using UnityEngine.EventSystems;

namespace SlimUI.CursorControllerPro{
    public class SoundReceiver : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
        GameObject soundControllerObj;
        SoundController soundController;

        [Header("SOUND BEHAVIORS")]
        public bool playHoverSound = true;
        public bool playExitSound = false;
        public bool playClickSound = true;

        AudioSource source {get{return GetComponent<AudioSource>();}}

        void Start(){
            gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        public void OnPointerEnter(PointerEventData eventData){
            if(soundControllerObj == null){
                soundControllerObj = GameObject.Find("CursorControl");
                soundController = soundControllerObj.GetComponent<SoundController>();
                GetComponent<AudioSource>().outputAudioMixerGroup = soundController.audioMixer;
            }

            if(playHoverSound && soundController.hoverSound != null){
                gameObject.GetComponent<AudioSource>().volume = soundController.vol;
                gameObject.GetComponent<AudioSource>().pitch = soundController.hoverPitch;
                source.PlayOneShot(soundController.hoverSound);
            }
        }

        public void OnPointerClick(PointerEventData eventData){
            if(playClickSound && soundController.clickSound != null){
                gameObject.GetComponent<AudioSource>().pitch = soundController.clickPitch;
                source.PlayOneShot(soundController.clickSound);
            }
        }

        public void OnPointerExit(PointerEventData eventData){
            if(playExitSound && soundController.exitSound != null){
                gameObject.GetComponent<AudioSource>().pitch = soundController.exitPitch;
                source.PlayOneShot(soundController.exitSound);
            }
        }
    }
}