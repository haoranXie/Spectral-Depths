using System.Collections;
using System.Collections.Generic;
using System.Media;
using SpectralDepths.Feedbacks;
using UnityEngine;

namespace SpectralDepths.TopDown
{
    public class ProximityActivatedDoor : MonoBehaviour
    {
		[Header("Requirements")]
		[Tooltip("Door Animator")]
        public Animator DoorAnimator;
		[Tooltip("Acceptable Layers")]
        public LayerMask AcceptableLayers;    
        private int charactersInside;
		[Tooltip("Audio player ")]
        public AudioSource AudioSource;  
		[Tooltip("Open Door Sound")]
        public AudioClip DoorOpenSound;    
		[Tooltip("Door Close Sound")]
        public AudioClip DoorCloseSound;   
        private BoxCollider _boxCollider;
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (( AcceptableLayers & (1 << other.gameObject.layer)) != 0)
            {
                charactersInside++;
                if (charactersInside == 1) // Only one character inside
                {
                    OpenDoor();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (( AcceptableLayers & (1 << other.gameObject.layer)) != 0)
            {
                charactersInside--;

                if (charactersInside == 0) // No characters inside
                {
                    CloseDoor();
                }
            }
        }

        private void OpenDoor()
        {
            DoorAnimator.SetTrigger("OpenDoor");
            if(DoorOpenSound!=null){AudioSource.PlayOneShot(DoorOpenSound);}
        }

        private void CloseDoor()
        {
            DoorAnimator.SetTrigger("CloseDoor");
            if(DoorCloseSound!=null){AudioSource.PlayOneShot(DoorCloseSound);}
        }
    }
}