using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI;
using EmeraldAI.Utility;
using SpectralDepths.TopDown;
public class MoveToMousePosition : MonoBehaviour
{
    EmeraldSystem EmeraldComponent;
    Camera _mainCamera;
	[Tooltip("the layers to consider as ground (where the character can walk on)")]
	public LayerMask GroundLayerMasks = LayerManager.GroundLayerMask;
    private void Start()
    {
        EmeraldComponent = GetComponentInParent<EmeraldSystem>();
		_mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        DetectMouse();
    }

		protected virtual void DetectMouse()
		{
			if (Input.GetMouseButtonDown(1))
			{
				Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MousePosition);

				//float distance;
				RaycastHit distance;
				if (Physics.Raycast(ray, out distance, 50000.0f, GroundLayerMasks))
				{

                    if (EmeraldComponent != null)
                    {
                        EmeraldAPI.Movement.SetCustomDestination(EmeraldComponent, distance.point);
                    }
				}
			}
		}
}
