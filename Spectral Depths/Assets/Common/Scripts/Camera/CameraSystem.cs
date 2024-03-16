using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using Cinemachine;
using UnityEngine.InputSystem.Controls;

namespace SpectralDepths.TopDown{
    /// <summary>
    /// Used for RTS camera movement
    /// </summary>
    public class CameraSystem : PLSingleton<CameraSystem>, PLEventListener<TopDownEngineEvent>
    {
		[Tooltip("Enable/Disable Use of this camera")]
        public bool UseRTSCamera = true;

        [Header("Cameras")]
		[Tooltip("Camera for RTS seciton")]
		public CinemachineVirtualCamera RTSCamera;
		[Tooltip("Camera for following player topdown action")]
		public CinemachineVirtualCamera PlayerCamera;
		[Header("Speed")]
		/// movespeed of camera
		[Tooltip("horizontal movespeed of camera")]
		public float MovementSpeed = 14f;
		/// the acceleration to apply to the current speed / 0f : no acceleration, instant full speed
		[Tooltip("the acceleration to apply to the current speed / 0f : no acceleration, instant full speed")]
		public float Acceleration = 10f;
		/// the deceleration to apply to the current speed / 0f : no deceleration, instant stop
		[Tooltip("the deceleration to apply to the current speed / 0f : no deceleration, instant stop")]
		public float Deceleration = 10f;
		/// whether or not to interpolate movement speed
		[Tooltip("whether or not to interpolate movement speed")]
		public bool InterpolateMovementSpeed = false;
		/// the speed threshold after which the character is not considered idle anymore
		[Tooltip("the speed threshold after which the character is not considered idle anymore")]
		public float IdleThreshold = 0.05f;

        [Header("Camera Movement Settings")]
		[Tooltip("rotatespeed of camera")]
		public float RotateSpeed = 75f;
		[Tooltip("DragPanSpeedMult of camera")]
		public float DragPanSpeedMult = 1f;
		[Tooltip("How far mouse has to be move to scroll (Higher means more sensitive)")]
        public int EdgeScrollSize = 5;
		[Tooltip("Scroll Speed")]
        public float ZoomSpeed = 5f;
		[Tooltip("Max FOV Scroll")]
        public float FieldOfViewMax = 80f;
		[Tooltip("Min FOV Scroll")]
        public float FieldOfViewMin = 30f;
		[Tooltip("Max Offset Scroll")]
        public float FollowOffsetMax = 30f;
		[Tooltip("Min Offset Scroll")]
        public float FollowOffsetMin = 10f;   
		[Tooltip("Max Offset.y Scroll")]
        public float FollowOffsetMaxY = 50f;
		[Tooltip("Min Offset.y Scroll")]
        public float FollowOffsetMinY = 10f;   
        [Header("Camera Systems")]
		[Tooltip("Restricted Camera Collider")]
        public Collider MovementBounds;

		[Tooltip("Enable/Disable Edge Scrolling")]
        public bool UseEdgeScrolling = true;
		[Tooltip("Enable/Disable Drag Panning")]
        public bool UseDragPanning = false;
		[Tooltip("Enable/Disable Zooming with FOV")]
        public bool UseZoomingFOV = true;
		[Tooltip("Enable/Disable Zooming with Transform")]
        public bool UseZoomingTransform = false;
		[Tooltip("Enable/Disable Zooming with TransformY")]
        public bool UseZoomingTransformY = false;
		[Tooltip("Enable/Disable Rotating")]
        public bool UseRotating = true;
		[Tooltip("Enable/Disable Keyboard Horizontal Movement")]
        public bool UseKeyboardHorizontal = true;

		private float _acceleration = 0f;
		private InputManager _linkedInputManager;
        private float _horizontalInput;
        private float _verticalInput;
        private Vector3 _inputDir;
        private Vector3 _moveDir;
        private Vector2 _lastMousePosition;
        private float _rotateDir;
        private bool _dragPanMoveActive;
        private float _targetFieldOfView;
        private Vector3 _followOffset;
		protected Vector3 _lerpedInput = Vector3.zero;
		protected Vector3 _movementVector;
		protected float _movementSpeed;
        protected Vector2 _currentInput;
        protected bool _canInput = true;


        protected override void Awake(){
            base.Awake();
            SetInputManager();
            _targetFieldOfView = RTSCamera.m_Lens.FieldOfView;
            _followOffset = RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Update()
        {
            if(UseRTSCamera)
            {
                HandleInput();
                MoveCamera();
            }
        }

        public void SwapToRTSCamera(Transform initialPosition)
        {
            transform.position=initialPosition.position;
            PlayerCamera.Priority=0;
            RTSCamera.Priority=1;
        }

        public void SwapToPlayerCamera(Character playerToFollow)
        {
			PLCameraEvent.Trigger(PLCameraEventTypes.SetTargetCharacter, playerToFollow);
			PLCameraEvent.Trigger(PLCameraEventTypes.StartFollowing);
            RTSCamera.Priority=0;
            PlayerCamera.Priority=1;
        }


		protected virtual void HandleInput()
		{
            if(!_canInput) return;
            if(UseKeyboardHorizontal){InputHorizontalMoveCamera();}
            if(UseDragPanning){InputDragCamera();}
            if(UseZoomingFOV){InputCameraZoomFOV();}
            if(UseZoomingTransform){InputCameraZoomMoveForward();}
            if(UseZoomingTransformY){InputCameraZoomLowerY();}
            if(UseRotating){InputRotateCamera();}
            if(UseEdgeScrolling){InputEdgeScrollCamera();}
        }

        private void MoveCamera()
        {
            SetMovement();
            /*
            _moveDir = transform.forward * _inputDir.z + transform.right * _inputDir.x;
            _moveDir = _moveDir.normalized;

            //Moves the camera in Input Direction mutliplied by MoveSpeed
            transform.eulerAngles += new Vector3 (0,_rotateDir*RotateSpeed*Time.deltaTime,0);
            Vector3 targetPosition = transform.position + _moveDir * MovementSpeed * Time.deltaTime;
            if(MovementBounds!=null)
            {
                if(!MovementBounds.bounds.Contains(targetPosition))
                {
                    return;
                }
            }
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.9f); // You can adjust the interpolation factor (0.1f) for smoother or quicker movement
            //Turns the camera based on Rotate Input Direction
            */
        }
		protected virtual void SetMovement()
		{
			_movementVector = Vector3.zero;
			_currentInput = Vector2.zero;

            _moveDir = transform.forward * _inputDir.z + transform.right * _inputDir.x;
            _currentInput = _moveDir;
            _moveDir = _moveDir.normalized;
			float interpolationSpeed = 1f;
            
			_lerpedInput = _moveDir;

            if (_moveDir.magnitude == 0)
            {
                _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
                _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
                interpolationSpeed = Deceleration;
            }
            else
            {
                _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                _lerpedInput = Vector3.ClampMagnitude(_moveDir, _acceleration);
                interpolationSpeed = Acceleration;
            }
				
			
			_movementVector.x = _lerpedInput.x;
			_movementVector.y = 0f;
			_movementVector.z = _lerpedInput.z;

			if (InterpolateMovementSpeed)
			{
				_movementSpeed = Mathf.Lerp(_movementSpeed,  MovementSpeed, interpolationSpeed * Time.deltaTime);
			}
			else
			{
				_movementSpeed = MovementSpeed;
			}

			_movementVector *= _movementSpeed;

			if (_movementVector.magnitude > MovementSpeed)
			{
				_movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
			}

			if ((_currentInput.magnitude <= IdleThreshold) && (_movementVector.magnitude < IdleThreshold))
			{
				_movementVector = Vector3.zero;
			}
            Vector3 targetPosition = transform.position + _movementVector *Time.deltaTime;
            if(MovementBounds!=null)
            {
                if(!MovementBounds.bounds.Contains(targetPosition))
                {
                    return;
                }
            }
            // Update the position of the object
            transform.position = targetPosition;
		} 
        /// <summary>
        /// Sets camera movement on x and z axis based on input
        /// </summary>
        private void InputHorizontalMoveCamera()
        {
			if (_linkedInputManager == null) { return; }
			_inputDir.x = _linkedInputManager.PrimaryMovement.x;
			_inputDir.z = _linkedInputManager.PrimaryMovement.y;
        }
        /// <summary>
        /// Sets camera rotation based on input
        /// </summary>

        private void InputRotateCamera()
        {
			if (_linkedInputManager.RotateCameraLeftButton.State.CurrentState == PLInput.ButtonStates.ButtonDown)
			{
                _rotateDir = +1;
			}
			if (_linkedInputManager.RotateCameraLeftButton.State.CurrentState == PLInput.ButtonStates.ButtonUp)
			{   
                if(_rotateDir==+1f)
                {
                    _rotateDir = 0;    
                }
			}
			if (_linkedInputManager.RotateCameraRightButton.State.CurrentState == PLInput.ButtonStates.ButtonDown)
			{
                _rotateDir = -1f;
            }
			if (_linkedInputManager.RotateCameraRightButton.State.CurrentState == PLInput.ButtonStates.ButtonUp)
			{     
                if(_rotateDir==-1f)   
                {
                    _rotateDir = 0;
                }       
			}
        }
        /// <summary>
        /// Actives/deactivates whether player is dragging camera
        /// </summary>
        private void InputDragCamera()
        {
            if(Input.GetMouseButtonDown(2))
            {
                _dragPanMoveActive = true;
                _lastMousePosition = Input.mousePosition;
            }
            if(Input.GetMouseButtonUp(2))
            {
                _dragPanMoveActive = false;
            }
            if(_dragPanMoveActive)
            {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - _lastMousePosition;
                
                _inputDir.x = mouseMovementDelta.x * DragPanSpeedMult;
                _inputDir.z = mouseMovementDelta.y * DragPanSpeedMult;
                _lastMousePosition = Input.mousePosition;
            }
        }

        /// <summary>
        /// Moves the camera horizontally based on how close mouse position is to edge of screen
        /// </summary>
        private void InputEdgeScrollCamera()
        {
            //Mouse is at Left Side
            if(Input.mousePosition.x<EdgeScrollSize) _inputDir.x = -1f; 
            //Mouse is at Bottom Side
            if(Input.mousePosition.y<EdgeScrollSize) _inputDir.z = -1f;
            //Mouse is at Right Side
            if(Input.mousePosition.x>Screen.width - EdgeScrollSize) _inputDir.x = +1f; 
            //Mouse is at Top Side
            if(Input.mousePosition.y>Screen.height - EdgeScrollSize) _inputDir.z = +1f;
        }
        /// <summary>
        /// Moves camera zoom via FOV
        /// </summary>
        private void InputCameraZoomFOV()
        {
            if(Input.mouseScrollDelta.y>0){
                _targetFieldOfView -= 5;
            }
            if(Input.mouseScrollDelta.y<0){
                _targetFieldOfView += 5;
            }

            _targetFieldOfView = Mathf.Clamp(_targetFieldOfView,FieldOfViewMin,FieldOfViewMax);
            
            RTSCamera.m_Lens.FieldOfView = Mathf.Lerp(RTSCamera.m_Lens.FieldOfView,_targetFieldOfView,Time.deltaTime*ZoomSpeed);
        }

        private void InputCameraZoomMoveForward()
        {
            Vector3 zoomDir = _followOffset.normalized;
            float zoomAmount = 2f;
            if(Input.mouseScrollDelta.y>0){
                _followOffset-=zoomDir*zoomAmount;
            }
            if(Input.mouseScrollDelta.y<0){
                _followOffset+=zoomDir*zoomAmount;
            }
            if(_followOffset.magnitude<FollowOffsetMin)
            {
                _followOffset = zoomDir * FollowOffsetMin;
            }
            if(_followOffset.magnitude>FollowOffsetMax)
            {
                _followOffset = zoomDir * FollowOffsetMax;
            }
            RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime*ZoomSpeed);
            RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _followOffset;
        }

        private void InputCameraZoomLowerY()
        {
            float zoomAmount = 2f;
            if(Input.mouseScrollDelta.y>0){
                _followOffset.y-=zoomAmount;
            }
            if(Input.mouseScrollDelta.y<0){
                _followOffset.y+=zoomAmount;
            }
            _followOffset.y = Mathf.Clamp(_followOffset.y, FollowOffsetMinY,FollowOffsetMaxY);
            RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime*ZoomSpeed);
            RTSCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _followOffset;
        }

        public virtual void SetInputManager()
        {
            _linkedInputManager = null;
            InputManager foundInputManagers = FindObjectOfType(typeof(InputManager)) as InputManager;
            _linkedInputManager = foundInputManagers;
        }
        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.Pause:
                    _canInput = false;
                    break;
                case TopDownEngineEventTypes.UnPause:
                    _canInput = true;
                    break;
                case TopDownEngineEventTypes.RTSOn:
                    _canInput = true;
                    break;
                case TopDownEngineEventTypes.RTSOff:
                    _canInput = false;
                    break;
            }
                    
        }
		/// <summary>
		/// OnDisable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<TopDownEngineEvent> ();

		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<TopDownEngineEvent> ();
        }
    }
}