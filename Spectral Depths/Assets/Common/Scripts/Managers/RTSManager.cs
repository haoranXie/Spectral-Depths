using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpectralDepths.Tools;
using UnityEngine.AI;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.Tracing;
using UnityEngine.Rendering;
using UnityEngine.Pool;
using System.Runtime.CompilerServices;
using EmeraldAI;

namespace SpectralDepths.TopDown

{
    /// <summary>
    /// Handles Unit Selection
    /// </summary>
    [AddComponentMenu("Spectral Depths/Managers/RTS Manager")]

    public class GameRTSController : MonoBehaviour, PLEventListener<TopDownEngineEvent>, PLEventListener<RTSEvent>
    {
        [PLInformation("The RTSManager is responsible for allowing the player to select units for canInput, formations, and RTS-related visual indicators",PLInformationAttribute.InformationType.Info,false)]
        [Header("RTS Mode")]
		[Tooltip("Turn on or off the player being able to RTS")]
		public bool RTSMode;
		[Tooltip("Turn on or off the player being able to RTS")]
		public bool OverworldMode;
		[Tooltip("Turn on or off special commands")]
		public bool CommandMode = true;
		[Header("Character Selection Settings")]
		[Tooltip("How far the player has to hold down the mouse before it counts as multi select")]
		public int PlayerMultiSelectThreshold = 40;
		[Tooltip("the layers to consider as selectable characters")]
		public LayerMask CharacterLayerMasks = LayerManager.PlayerSelectionLayerMask;
		[Tooltip("the layers to consider where the selection box should form upwards up (usually the ground)")]
		public LayerMask GroundLayerMasks = LayerManager.GroundLayerMask;
		[Tooltip("the color of the border of the selection box on UI")]
		public Color SelectionBoxBorderColor = new Color(0.8f,0.8f,0.95f);
		[Tooltip("the color of the selection box on UI")]
		public Color SelectionBoxColor = new Color(0.8f,0.8f,0.95f,0.25f);
		[Header("RTS Visual Indicators")]
		[Tooltip("Right Click Movement Indicator")]
        public GameObject MovementIndicator;
		[Tooltip("A-Left Click Attack-Movement Indicator")]
        public GameObject AttackMovementIndicator;
		[Tooltip("Default Mouse")]
        [SerializeField] private Texture2D _defualtCursor;
		[Tooltip("A-Left Click Attack-Movement Mouse Indicator")]
        [SerializeField] private Texture2D _defualtAttackCursor;
		[Header("RTS Audio Cues")]
		[Tooltip("Character Switch Sound")]
        [SerializeField] private AudioSource _charSwitchSound;
		[Tooltip("Command Sound")]
        [SerializeField] private AudioSource _commandSound;



        //Holdes all the selected Game Objects
        public Dictionary<int,GameObject> SelectedTable = new Dictionary<int, GameObject>();

        //All possible commands
        public enum Commands
        {
            Default,
            ForceAttack,
            ForceHold,
            ForcePatrol,
            ForceTakeCover
        }
        //Position for where player clicks mouse
        private Vector3 p1;
        //Position for where player lets go of mouse
        private Vector3 p2;
        //Whether or not player can select and control characters
        private bool canInput;
        //Used for single selection
        private RaycastHit hit;
        private MeshCollider selectionBox;
        private Mesh selectionMesh;
        //the corners of our 2d selection box
        private Vector2[] corners;
        //the verticies of our meshcollider
        private Vector3[] verts;
        private Vector3[] vecs;
        private Vector3 target;

        private Commands _curretCommand = Commands.Default;

        private bool dragSelect;

        private void Start()
        {
            dragSelect=false;
            canInput=true;
            Cursor.SetCursor(_defualtCursor, new Vector2(6,6), CursorMode.Auto);
        }

        private void Update(){
            if(RTSMode){ //Player is RTSing
                if(canInput) //Player can input (on/off depending on pausing)
                {
                    HandleInput();
                    IssueCommands();
                }
            }
        }

        private void SwitchToDefaultCommand()
        {
            Cursor.SetCursor(_defualtCursor, new Vector2(6,6), CursorMode.Auto);
            _curretCommand = Commands.Default;
            p1 = Input.mousePosition;
        }

        private bool _commandAttackButtonIsPressed = false;
        private void HandleInput()
        {
			if (InputManager.Instance.CommandAttackMoveButton.State.CurrentState == PLInput.ButtonStates.ButtonDown && !_commandAttackButtonIsPressed)
			{
                switch(_curretCommand)
                {
                    case(Commands.Default):
                        if(!dragSelect)
                        {
                            Cursor.SetCursor(_defualtAttackCursor, new Vector2(6,6), CursorMode.Auto);
                            _curretCommand = Commands.ForceAttack;
                            _commandAttackButtonIsPressed = true; 
                        }
                        break;
                    case(Commands.ForceAttack):
                        Cursor.SetCursor(_defualtCursor, new Vector2(6,6), CursorMode.Auto);
                        _curretCommand = Commands.Default;
                        _commandAttackButtonIsPressed = true;  
                        break;  
                }
            }
			if (InputManager.Instance.CommandAttackMoveButton.State.CurrentState == PLInput.ButtonStates.ButtonUp)
			{     
                _commandAttackButtonIsPressed = false;
			}
        }
        bool selectingButtonDown = false;
        /// <summary>
        /// Allows the player to select characters
        /// </summary>
        private void Selecting(){
            if(Input.GetMouseButtonDown(0))
            {
                p1 = Input.mousePosition;
                selectingButtonDown=true;
            }
            if(Input.GetMouseButton(0)&&selectingButtonDown)
            {
                if((p1-Input.mousePosition).magnitude>PlayerMultiSelectThreshold){
                    dragSelect=true;
                }
            }
            if(Input.GetMouseButtonUp(0)&&selectingButtonDown)
            {
                if(dragSelect==false){ //single Select
                    Ray ray = Camera.main.ScreenPointToRay(p1);
                    if(Physics.Raycast(ray,out hit,50000.0f,CharacterLayerMasks))
                    {
                        if(Input.GetKey(KeyCode.LeftShift)) //inclusive select
                        {
                            AddSelected(hit.transform.gameObject);
                        } else //exclusive select
                        {
                            DeselectAll();
                            AddSelected(hit.transform.gameObject);
                        }
                    }
                    else //if we didn't select select anything
                    {
                        if(Input.GetKey(KeyCode.LeftShift)){
                            //do nothing
                        }
                        else{
                            DeselectAll();
                        }
                    }
                }
                else //multi select 
                {
                    verts = new Vector3[4];
                    vecs = new Vector3[4];
                    int i = 0;
                    p2=Input.mousePosition;
                    corners = getBoundingBox(p1,p2);
                    foreach(Vector2 corner in corners)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(corner);
                        if(Physics.Raycast(ray,out hit,50000.0f,GroundLayerMasks))
                        {
                            verts[i] = new Vector3(hit.point.x,hit.point.y,hit.point.z);
                            vecs[i] = ray.origin - hit.point;
                        }
                        i++;
                    }

                    //generate the mesh
                    selectionMesh = GenerateSelectionMesh(verts,vecs);

                    selectionBox = gameObject.AddComponent<MeshCollider>();
                    selectionBox.sharedMesh = selectionMesh;
                    selectionBox.convex = true;
                    selectionBox.isTrigger = true;
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAll();
                    }
                    Destroy(selectionBox, 0.02f);
                } // end multi select
                selectingButtonDown=false;
                dragSelect=false;
            }
        }

        /// <summary>
        /// Uses an enum statemachine to issue various commands
        /// </summary>
        protected virtual void IssueCommands()
        {
            switch(_curretCommand)
            {
                case Commands.Default:
                    Selecting();
                    DetectMovement();
                    break;                   
                case Commands.ForceAttack:
                    ForceAttack();
                    break;
                case Commands.ForcePatrol:
                    break;
                case Commands.ForceHold:
                    break;          
                case Commands.ForceTakeCover:
                    ForceTakeCover();
                    break;
                
            }
        }


        protected virtual void ForceTakeCover()
        {

        }

        /// <summary>
        /// Moves every selected character to right clicked point of mouse
        /// </summary>
		protected virtual void DetectMovement()
		{
			if (Input.GetMouseButtonDown(1))
			{
				Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
				//float distance;
				RaycastHit distance;
				if (Physics.Raycast(ray, out distance, 50000.0f, GroundLayerMasks))
				{
					target=distance.point;
                    Instantiate(MovementIndicator,distance.point,Quaternion.identity);
                    RTSEvent.Trigger(RTSEventTypes.CommandForceMove,null,SelectedTable);
                    SetPositionsCircle();
				}
			}
		}
        /// <summary>
        /// Force Attack
        /// </summary>
		protected virtual void ForceAttack()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
				//float distance;
				RaycastHit distance;
				if (Physics.Raycast(ray, out distance, 50000.0f, GroundLayerMasks))
				{
					target=distance.point;
                    Instantiate(AttackMovementIndicator,distance.point,Quaternion.identity);
                    RTSEvent.Trigger(RTSEventTypes.CommandForceAttack,null,SelectedTable);
                    SetPositionsCircle();
				}
                SwitchToDefaultCommand();
			}
		}
        /// <summary>
        /// Grabs the MouseDrivenPathFinderAI3D component and assigns each unit a unique position
        /// </summary>
        public void SetPositionsCircle(){
            float[] distanceBetweenEachCharacter = {2f,4f,6f};
            int[] distanceBetweenEachRing = {1,2,3};

            List<Vector3> targetPositionList = GetPositionListAround(target,distanceBetweenEachCharacter,distanceBetweenEachRing);
            int targetPositionIndex = 0;

            foreach(KeyValuePair<int,GameObject> character in SelectedTable)
            {
                //character.Value.GetComponent<Character>().FindAbility<MouseDrivenPathfinderAI3D>().UpdatePosition(targetPositionList[targetPositionIndex]);
                EmeraldAPI.Movement.SetCustomDestination(character.Value.GetComponent<EmeraldSystem>(),targetPositionList[targetPositionIndex]);
                targetPositionIndex = (targetPositionIndex + 1) % targetPositionList.Count;
            }  
        }

        /// <summary>
        /// Layers circle formations together to create large filled circle formation
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="ringDistanceArray"></param>
        /// <param name="ringPositionCountArray"></param>
        /// <returns></returns>
        private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
        {
            List<Vector3> positionList = new List<Vector3>();
            positionList.Add(startPosition);
            for(int i = 0; i<ringDistanceArray.Length;i++)
            {
                positionList.AddRange(GetPositionListAround(startPosition,ringDistanceArray[i],ringPositionCountArray[i]));
            }
            return positionList;
        }
        /// <summary>
        /// Creates a circular formation for characters to take
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="distance"></param>
        /// <param name="positionCount"></param>
        /// <returns></returns>

        private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
        {
            List<Vector3> positionList = new List<Vector3>();
            for(int i = 0; i<positionCount;i++)
            {
                float angle = i *(360f/positionCount);
                Vector3 dir = ApplyRotationToVector(new Vector3(1,0),angle);
                Vector3 position = startPosition + dir *distance;
                positionList.Add(position);
            }
            return positionList;
        }

        private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0,0,angle) * vec;
        }
        /// <summary>
        /// Adds newly selected object to dictionary and fires RTSEvent
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddSelected(GameObject gameObject){
            GameObject character = FindCharacterComponentInParent(gameObject);
            int  id = character.GetInstanceID();
            if(!(SelectedTable.ContainsKey(id))){
                SelectedTable.Add(id,character);
            }
            RTSEvent.Trigger(RTSEventTypes.PlayerSelected, null, SelectedTable);
        }

        /// <summary>
        /// Removes a specific object from dictionary using it's ID and fires RTSEvent
        /// </summary>
        /// <param name="gameObject"></param>

        public void Deselect(int id){
            SelectedTable.Remove(id);
            RTSEvent.Trigger(RTSEventTypes.PlayerSelected, null, SelectedTable);

        }
        /// <summary>
        /// Removes all objects from selection dictionary and fires RTSEvent
        /// </summary>
        /// <param name="gameObject"></param>

        public void DeselectAll(){
            SelectedTable.Clear();
            RTSEvent.Trigger(RTSEventTypes.PlayerSelected,null, SelectedTable);
        }
        /// <summary>
        /// Draws rectangle on screen when mutliselecting
        /// </summary>
        private void OnGUI()
        {
            if(dragSelect==true){
                var rect = GetScreenRect(p1,Input.mousePosition);
                DrawScreenRect(rect,SelectionBoxColor);
                DrawScreenRectBorder(rect,2,SelectionBoxBorderColor);
            }
        }

        //Create a bounding box (4 corners in order) from the start and end mouse position
        private Vector2[] getBoundingBox(Vector2 p1,Vector2 p2)
        {
            // Min and Max to get 2 corners of rectangle regardless of drag direction.
            var bottomLeft = Vector3.Min(p1, p2);
            var topRight = Vector3.Max(p1, p2);

            // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
            Vector2[] corners =
            {
                new Vector2(bottomLeft.x, topRight.y),
                new Vector2(topRight.x, topRight.y),
                new Vector2(bottomLeft.x, bottomLeft.y),
                new Vector2(topRight.x, bottomLeft.y)
            };
            return corners;

        }

        //Generate a mesh from the 4 bottom points
        private Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
        {
            Vector3[] verts = new Vector3[8];
            int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

            for(int i = 0; i < 4; i++)
            {
                verts[i] = corners[i];
            }

            for(int j = 4; j < 8; j++)
            {
                verts[j] = corners[j - 4] + vecs[j - 4];
            }

            Mesh selectionMesh = new Mesh();
            selectionMesh.vertices = verts;
            selectionMesh.triangles = tris;

            return selectionMesh;
        }

        //Triggers Everytime an object enters our selection cube
        void OnTriggerEnter(Collider other)
        {
            if ((CharacterLayerMasks & (1 << other.gameObject.layer)) != 0){
                GameObject character = FindCharacterComponentInParent(other.gameObject);
                if(character){
                    AddSelected(character);
                }
            }
        }

        private GameObject FindCharacterComponentInParent(GameObject childObject)
        {
            if(childObject.GetComponent<Character>()!=null)
            {
                return childObject;
            }
            // Traverse the hierarchy upwards
            Transform parentTransform = childObject.transform.parent;
            // Check if there is a parent and if it has a Character component
            while (parentTransform != null)
            {
                Character characterComponent = parentTransform.GetComponent<Character>();

                // If a Character component is found, return it
                if (characterComponent != null)
                {
                    return parentTransform.gameObject;
                }

                // Move up to the next parent
                parentTransform = parentTransform.parent;
            }

            // If no Character component is found in the hierarchy, return null
            return null;
        }
        
        public Texture2D BoxSelectionTexture
         {
            get
            {
                if (_boxSelectionTexture == null)
                {
                    _boxSelectionTexture = new Texture2D(1, 1);
                    _boxSelectionTexture.SetPixel(0, 0, Color.white);
                    _boxSelectionTexture.Apply();
                }
                return _boxSelectionTexture;
            }
        }
        private Texture2D _boxSelectionTexture;

        public void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, BoxSelectionTexture);
            GUI.color = Color.white;
        }

        public void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        /// <summary>
        /// Prevents player from making RTS inputs depending on pauses
        /// </summary>
        /// <param name="engineEvent"></param>
        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.Pause:
                    if(RTSMode)
                    {
                        canInput=false;
                    }
                    break;
                case TopDownEngineEventTypes.UnPause:
                    if(RTSMode)
                    {
                        canInput=true;
                    }
                    break;
                case TopDownEngineEventTypes.RTSOn:
                    RTSMode=true;
                    break;
                case TopDownEngineEventTypes.RTSOff:
                    RTSMode=false;
                    break;
            }
                    
        }

        public virtual void OnMMEvent(RTSEvent rtsEvent)
        {
            switch(rtsEvent.EventType)
            {
                case RTSEventTypes.SelectionDisabled:
                    Deselect(rtsEvent.OriginCharacter.gameObject.GetInstanceID());
                    break;
                case RTSEventTypes.SwitchToRTS:
                    //_charSwitchSound.Play();
                    break;
                case RTSEventTypes.SwitchToPlayer:
                    //_charSwitchSound.Play();
                    break;
            }
        }

		/// <summary>
		/// OnDisable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<TopDownEngineEvent> ();
			this.PLEventStartListening<RTSEvent> ();

		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<TopDownEngineEvent> ();
			this.PLEventStopListening<RTSEvent> ();
        }
    }
}