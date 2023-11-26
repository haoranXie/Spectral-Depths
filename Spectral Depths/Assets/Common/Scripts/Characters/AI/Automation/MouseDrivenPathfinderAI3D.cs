using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// A class to add on a CharacterPathfinder3D equipped character.
	/// It will allow you to click anywhere on screen, which will determine a new target and the character will pathfind its way to it
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Automation/MouseDrivenPathfinderAI3D")]
	public class MouseDrivenPathfinderAI3D : TopDownMonoBehaviour 
	{
		[Header("Testing")]
		/// the camera we'll use to determine the destination from
		[Tooltip("the camera we'll use to determine the destination from")]
		public Camera Cam;
		/// a gameobject used to show the destination
		[Tooltip("a gameobject used to show the destination")]
		public GameObject Destination;
		/// the layers to consider as ground (where character can walk on)
		[Tooltip("the layers to consider as ground (where the character can walk on)")]
		public LayerMask GroundLayerMasks = LayerManager.GroundLayerMask;
		/// Non-formation broken movement, use only for testing
		[Tooltip("Non-formation broken movement, use only for testing")]
		public bool NoRTS = false;
		public float stoppingDistance = 2f; // Distance to stop from the destination
		public float spacing = 3f; // Spacing between characters

		protected CharacterPathfinder3DWithVector _characterPathfinder3D;
		protected CharacterSelectable _characterSelectable;
		protected bool _destinationSet = false;
		protected Camera _mainCamera;
		//Used to determine whether the character can be selected if Selecetable is on

		/// <summary>
		/// On awake we create a plane to catch our ray
		/// </summary>
		protected virtual void Awake()
		{
			_mainCamera = Camera.main;
			_characterPathfinder3D = this.gameObject.GetComponent<CharacterPathfinder3DWithVector>();
			if(NoRTS){
				_characterSelectable = this.gameObject.GetComponent<CharacterSelectable>();
			}
		}

		/// <summary>
		/// On Update we look for a mouse click
		/// </summary>
		protected virtual void Update()
		{
			if(NoRTS) //If the selectable setting is turned on
			{
				if(_characterSelectable.selected) //If the character is selected
				{
					DetectMouse();
				}
			} 
		}

		/// <summary>
		/// If the mouse is clicked, we cast a ray and if that ray hits the plane we make it the pathfinding target
		/// </summary>
		protected virtual void DetectMouse()
		{
			if (Input.GetMouseButtonDown(1))
			{
				Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

				//float distance;
				RaycastHit distance;
				if (Physics.Raycast(ray, out distance, 50000.0f, GroundLayerMasks))
				{
					//Vector3 target = ray.GetPoint(distance);

					Vector3 target = distance.point;
					Destination.transform.position = target;
					_destinationSet = true;
					_characterPathfinder3D.SetNewDestination(Destination.transform.position);
				}
			}
		}
		public void UpdatePosition(Vector3 newPos){
			_destinationSet = true;
			_characterPathfinder3D.SetNewDestination(newPos);
		}
	}
}