using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEngine;
using MoreMountains.Tools;
using Cinemachine;

namespace SpectralDepths.TopDown{
    /// <summary>
    /// Used for RTS camera movement
    /// </summary>
    public class CameraSystem : MonoBehaviour
    {
        [Header("Assigned Camera")]
		[Tooltip("Cinemachine Virtual Camera")]
		public CinemachineVirtualCamera cinemachineVirtualCamera;
        [Header("Camera Movement Settings")]
		/// movespeed of camera
		[Tooltip("horizontal movespeed of camera")]
		public float MoveSpeed = 14f;
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
        private void Awake(){
            SetInputManager();
            _targetFieldOfView = cinemachineVirtualCamera.m_Lens.FieldOfView;
            _followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Update()
        {
            HandleInput();
            MoveCamera();
        }


		protected virtual void HandleInput()
		{
            if(UseKeyboardHorizontal){InputHorizontalMoveCamera();}
            if(UseEdgeScrolling){InputEdgeScrollCamera();}
            if(UseDragPanning){InputDragCamera();}
            if(UseZoomingFOV){InputCameraZoomFOV();}
            if(UseZoomingTransform){InputCameraZoomMoveForward();}
            if(UseZoomingTransformY){InputCameraZoomLowerY();}
            if(UseRotating){InputRotateCamera();}
        }

        private void MoveCamera()
        {
            _moveDir = transform.forward * _inputDir.z + transform.right * _inputDir.x;
            _moveDir = _moveDir.normalized;
            //Moves the camera in Input Direction mutliplied by MoveSpeed
            transform.position += _moveDir * MoveSpeed * Time.deltaTime;
            //Turns the camera based on Rotate Input Direction
            transform.eulerAngles += new Vector3 (0,_rotateDir*RotateSpeed*Time.deltaTime,0);
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
			if (_linkedInputManager.RotateCameraLeftButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
                _rotateDir = +1;
			}
			if (_linkedInputManager.RotateCameraLeftButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{   
                if(_rotateDir==+1f)
                {
                    _rotateDir = 0;    
                }
			}
			if (_linkedInputManager.RotateCameraRightButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
                _rotateDir = -1f;
            }
			if (_linkedInputManager.RotateCameraRightButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
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
            
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView,_targetFieldOfView,Time.deltaTime*ZoomSpeed);
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
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime*ZoomSpeed);
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _followOffset;
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
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime*ZoomSpeed);
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _followOffset;
        }

        public virtual void SetInputManager()
        {
            _linkedInputManager = null;
            InputManager foundInputManagers = FindObjectOfType(typeof(InputManager)) as InputManager;
            _linkedInputManager = foundInputManagers;
        }
    }
}