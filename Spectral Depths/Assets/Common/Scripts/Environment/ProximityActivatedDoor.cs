using System.Collections;
using System.Collections.Generic;
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
		[Tooltip("Open Door Feedback")]
        public PLF_Player DoorOpenFeedback;    
		[Tooltip("Door Close Feedback")]
        public PLF_Player DoorCloseFeedback;   
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
            if(DoorOpenFeedback!=null){DoorOpenFeedback.PlayFeedbacks();}
        }

        private void CloseDoor()
        {
            DoorAnimator.SetTrigger("CloseDoor");
            if(DoorCloseFeedback!=null){DoorCloseFeedback.PlayFeedbacks();}
        }
    }
}