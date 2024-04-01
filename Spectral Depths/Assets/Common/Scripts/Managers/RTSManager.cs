using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpectralDepths.Tools;
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

    public class GameRTSController : PLSingleton<GameRTSController>, PLEventListener<TopDownEngineEvent>, PLEventListener<RTSEvent>
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
		[Tooltip("Enemy and Ground layer masks for attack move command")]
		public LayerMask EnemyAndGroundLayerMasks = LayerManager.GroundLayerMask;
		[Tooltip("the color of the border of the selection box on UI")]
		public Color SelectionBoxBorderColor = new Color(0.8f,0.8f,0.95f);
		[Tooltip("the color of the selection box on UI")]
		public Color SelectionBoxColor = new Color(0.8f,0.8f,0.95f,0.25f);
		[Header("RTS Visual Indicators")]
		[Tooltip("Right Click Movement Click Indicator")]
        public GameObject MovementClickIndicator;
		[Tooltip("Line between Movement Click Indicator")]
        public GameObject MovementEdgeIndicator;
		[Tooltip("Point between Edge Indicator")]
        public GameObject MovementVertexIndicator;
        
		[Tooltip("A-Left Click Attack-Movement Indicator")]
        public GameObject AttackMovementClickIndicator;
		[Tooltip("Line between Attack Movement Click Indicator")]
        public GameObject AttackEdgeIndicator;
		[Tooltip("Point between Edge Indicator")]
        public GameObject AttackVertexIndicator;
		[Tooltip("Indicator for attacking a specific target")]
        public GameObject AttackTargetIndicator;
		[Tooltip("Hold Position Indicator")]
        public GameObject HoldPositionIndicator;
		[Tooltip("Line between Hold Movement Click Indicator")]
        public GameObject HoldEdgeIndicator;
		[Tooltip("Point between Edge Indicator")]
        public GameObject HoldVertexIndicator;
		[Tooltip("Default Mouse")]
        [SerializeField] public Texture2D DefaultCursor;
		[Tooltip("A-Left Click Attack-Movement Mouse Indicator")]
        [SerializeField] public Texture2D DefaultAttackCursor;
		[Tooltip("S-Left Click Hold-Movement Mouse Indicator")]
        [SerializeField] public Texture2D DefaultHoldCursor;
		[Header("RTS Audio Cues")]
		[Tooltip("Character Switch Sound")]
        [SerializeField] private AudioSource _charSwitchSound;
		[Tooltip("Command Sound")]
        [SerializeField] private AudioSource _commandSound;
        [HideInInspector] public PLSimpleObjectPooler MovementClickIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler AttackMovementClickIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler MovementVertexIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler MovementEdgeIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler AttackVertexIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler AttackEdgeIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler AttackTargetIndicatorPool;

        [HideInInspector] public PLSimpleObjectPooler HoldPositionIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler HoldVertexIndicatorPool;
        [HideInInspector] public PLSimpleObjectPooler HoldEdgeIndicatorPool;
        [HideInInspector] public bool NeverHideIndicators = false;

        //Holdes all the selected Game Objects
        public Dictionary<int,Character> SelectedTable = new Dictionary<int, Character>();

        //All possible commands
        public enum Commands
        {
            Default,
            ForceAttack,
            ForceHold
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
            Cursor.SetCursor(DefaultCursor, new Vector2(6,6), CursorMode.Auto);
            PLSimpleObjectPooler[] pLSimpleObjectPoolers = GetComponents<PLSimpleObjectPooler>();
            foreach(PLSimpleObjectPooler plSimpleObjectPool in pLSimpleObjectPoolers)
            {
                if(plSimpleObjectPool.GameObjectToPool == MovementClickIndicator)
                {
                    MovementClickIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == AttackMovementClickIndicator)
                {
                    AttackMovementClickIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == MovementVertexIndicator)
                {
                    MovementVertexIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == MovementEdgeIndicator)
                {
                    MovementEdgeIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == AttackEdgeIndicator)
                {
                    AttackEdgeIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == AttackVertexIndicator)
                {
                    AttackVertexIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == AttackTargetIndicator)
                {
                    AttackTargetIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == HoldPositionIndicator)
                {
                    HoldPositionIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == HoldEdgeIndicator)
                {
                    HoldEdgeIndicatorPool = plSimpleObjectPool;
                }
                else if(plSimpleObjectPool.GameObjectToPool == HoldVertexIndicator)
                {
                    HoldVertexIndicatorPool = plSimpleObjectPool;
                }
            }
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

        public void SwitchToDefaultCommand()
        {
            if(_curretCommand!=Commands.Default)
            {
                Cursor.SetCursor(DefaultCursor, new Vector2(6,6), CursorMode.Auto);
                _curretCommand = Commands.Default;
                p1 = Input.mousePosition;
            }
        }
        bool allowedToClickAttackButton = true;
        bool allowedToClickHoldButton = true;
        private void HandleInput()
        {
            if(dragSelect) return;
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                foreach (Character character in LevelManager.Instance.Players) AddSelected(character.gameObject);
            }
			if (InputManager.Instance.CommandAttackMoveButton.State.CurrentState == PLInput.ButtonStates.ButtonDown && allowedToClickAttackButton && SelectedTable.Count > 0)
			{
                if(_curretCommand != Commands.ForceAttack)
                {
                    allowedToClickAttackButton = false;
                    Cursor.SetCursor(DefaultAttackCursor, new Vector2(6,6), CursorMode.Auto);
                    _curretCommand = Commands.ForceAttack;  
                }
                //Pressing the attack button again cancels it
                else
                {  
                    SwitchToDefaultCommand();
                } 
            }
			else if (InputManager.Instance.CommandAttackMoveButton.State.CurrentState == PLInput.ButtonStates.ButtonUp && !allowedToClickAttackButton)
			{     
                allowedToClickAttackButton = true;
			}

			if (Input.GetKeyDown(KeyCode.S) && allowedToClickHoldButton && SelectedTable.Count > 0)
			{
                if(_curretCommand != Commands.ForceAttack)
                {
                    allowedToClickHoldButton = false;
                    Cursor.SetCursor(DefaultHoldCursor, new Vector2(6,6), CursorMode.Auto);
                    _curretCommand = Commands.ForceHold;  
                }
                //Pressing the attack button again cancels it
                else
                {  
                    SwitchToDefaultCommand();
                } 
            }
			else if (Input.GetKeyUp(KeyCode.S) && !allowedToClickHoldButton)
			{     
                allowedToClickHoldButton = true;
			}


            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                SwitchToDefaultCommand();
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
                    DetectMovement();
                    break;
                case Commands.ForceHold:
                    ForceHold();
                    DetectMovement();
                    break;          
                
            }
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
                    SwitchToDefaultCommand();
					target=distance.point;
                    GameObject movementClickIndicator = MovementClickIndicatorPool.GetPooledGameObject();
                    movementClickIndicator.transform.position = distance.point;
                    movementClickIndicator.transform.rotation = Quaternion.identity;
                    movementClickIndicator.gameObject.SetActive(true);
                    SetOrderPositions(MovementVertexIndicatorPool);
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
				if (Physics.Raycast(ray, out distance, 50000.0f, EnemyAndGroundLayerMasks))
				{
                    target=distance.point;
                    ICombat enemy = distance.collider.gameObject.GetComponentInParent<ICombat>();
                    //If the player directly clicked on an enemy
                    if(enemy!=null)
                    {
                        SetOrderPositions(AttackTargetIndicatorPool, enemy.GetTransform());
                    }
                    //Else if the player clicked on the ground
                    else
                    {
                        SetOrderPositions(AttackVertexIndicatorPool);                
                    }
				}
			}
		}
        /// <summary>
        /// Force Hold
        /// </summary>
		protected virtual void ForceHold()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
				//float distance;
				RaycastHit distance;
				if (Physics.Raycast(ray, out distance, 50000.0f, GroundLayerMasks))
				{
                    target=distance.point;
                    SetOrderPositions(HoldVertexIndicatorPool);                
				}
			}
		}

        public void SetOrderPositions(PLSimpleObjectPooler orderVertexIndicatorPool, Transform targetCharacter = null)
        {
            foreach(KeyValuePair<int,Character> character in SelectedTable)
            {
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    AddOrder(character.Value.EmeraldComponent, target, orderVertexIndicatorPool);
                    EmeraldAPI.Movement.EnterOrderedMode(character.Value.EmeraldComponent);
                }
                else
                {
                    RemoveOrders(character.Value.EmeraldComponent);
                    if(character.Value.EmeraldComponent.m_NavMeshAgent.isActiveAndEnabled)
                    {
                        AddOrder(character.Value.EmeraldComponent, target, orderVertexIndicatorPool);
                        EmeraldAPI.Movement.EnterOrderedMode(character.Value.EmeraldComponent);  
                        SwitchToDefaultCommand();               
                    }
                }
            }
        }

        /// <summary>
        /// Assigns each unit a unique position along a line from where the player clicks
        /// </summary>
        public void SetPositionsLine(PLSimpleObjectPooler orderIndicatorPool){
            float[] distanceBetweenEachCharacter = {2f,4f,6f};
            int[] distanceBetweenEachRing = {1,2,3};

            List<Vector3> targetPositionList = GetPositionListAround(target,distanceBetweenEachCharacter,distanceBetweenEachRing);
            int targetPositionIndex = 0;

            foreach(KeyValuePair<int,Character> character in SelectedTable)
            {
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    AddOrder(character.Value.EmeraldComponent, targetPositionList[targetPositionIndex], orderIndicatorPool);
                }
                else
                {
                    RemoveOrders(character.Value.EmeraldComponent);
                    AddOrder(character.Value.EmeraldComponent, targetPositionList[targetPositionIndex], orderIndicatorPool);
                    EmeraldAPI.Movement.EnterOrderedMode(character.Value.EmeraldComponent);
                }
                //character.Value.GetComponent<Character>().FindAbility<MouseDrivenPathfinderAI3D>().UpdatePosition(targetPositionList[targetPositionIndex]);
                //EmeraldAPI.Movement.SetCustomDestination(character.Value.GetComponent<EmeraldSystem>(),targetPositionList[targetPositionIndex]);
                targetPositionIndex = (targetPositionIndex + 1) % targetPositionList.Count;
            }  
        }
        /// <summary>
        /// Sets and shows MovementEdge and MovementVertex indicators for specific character
        /// </summary>
        /// <param name="emeraldSystem"></param>
        public void AddOrder(EmeraldSystem emeraldSystem, Vector3 indicatorPosition, PLSimpleObjectPooler orderIndicatorPool, Transform targetToFollow = null)
        {
            if(emeraldSystem.MovementComponent.MovementEdgeIndicator==null) emeraldSystem.MovementComponent.MovementEdgeIndicator = MovementEdgeIndicatorPool.GetPooledGameObject();
            if(emeraldSystem.MovementComponent.OrderedWaypointsList==null) emeraldSystem.MovementComponent.OrderedWaypointsList = new List<Transform>();
            if(emeraldSystem.MovementComponent.MovementLineIndicator==null) emeraldSystem.MovementComponent.MovementLineIndicator = emeraldSystem.MovementComponent.MovementEdgeIndicator.GetComponent<LineRenderer>();
            //if(emeraldSystem.MovementComponent.OrderedWaypointsList.Count == 1 ) return;

            emeraldSystem.MovementComponent.MovementLineIndicator.positionCount = emeraldSystem.MovementComponent.OrderedWaypointsList.Count+1;

            emeraldSystem.MovementComponent.MovementEdgeIndicator.gameObject.SetActive(true);

            GameObject orderVertexIndicator = orderIndicatorPool.GetPooledGameObject();
            //Vector3 newPosition = emeraldSystem.MovementComponent.OrderedWaypointsList[emeraldSystem.MovementComponent.OrderedWaypointsList.Count-1];
            Vector3 newPosition = indicatorPosition;
            if(targetToFollow != null) {newPosition = targetToFollow.position;}
            newPosition.y += (float)0.1;
            orderVertexIndicator.transform.position = newPosition;
            if(orderVertexIndicator.GetComponent<Order>()) orderVertexIndicator.GetComponent<Order>().TargetCharacter = targetToFollow;

            emeraldSystem.MovementComponent.MovementLineIndicator.SetPosition(emeraldSystem.MovementComponent.OrderedWaypointsList.Count,newPosition);
            //We create an extra vertex if we have two vertices in order to form line which requires 3 verticies at least
            if(emeraldSystem.MovementComponent.OrderedWaypointsList.Count==1)
            {
                emeraldSystem.MovementComponent.MovementLineIndicator.positionCount = 3;
                newPosition.y -= (float)2;
                emeraldSystem.MovementComponent.MovementLineIndicator.SetPosition(2,newPosition);
            }
            /*
            if(emeraldSystem.MovementComponent.OrderedWaypointsList.Count > 1 )  emeraldSystem.MovementComponent.MovementLineIndicator.SetPosition(emeraldSystem.MovementComponent.OrderedWaypointsList.Count-1,newPosition);
            if(emeraldSystem.MovementComponent.OrderedWaypointsList.Count==2)
            {
                newPosition = emeraldSystem.MovementComponent.OrderedWaypointsList[emeraldSystem.MovementComponent.OrderedWaypointsList.Count-2].position;
                emeraldSystem.MovementComponent.MovementLineIndicator.SetPosition(emeraldSystem.MovementComponent.OrderedWaypointsList.Count-2,newPosition);
            } 
            */
            
            orderVertexIndicator.gameObject.SetActive(true);
            emeraldSystem.MovementComponent.OrderedWaypointsList.Add(orderVertexIndicator.transform);
    
        }
        /// <summary>
        /// Shows MovementEdge and MovementVertex indicators for specific character
        /// </summary>
        /// <param name="emeraldSystem"></param>
        public void ShowIndicators(EmeraldSystem emeraldSystem)
        {
            if(emeraldSystem.MovementComponent.OrderedWaypointsList!=null)
            {
                foreach(Transform transform in emeraldSystem.MovementComponent.OrderedWaypointsList)
                {
                    foreach(Renderer renderer in transform.gameObject.GetComponentsInChildren<Renderer>()){renderer.enabled=true;}
                }
            } 
            if(emeraldSystem.MovementComponent.MovementEdgeIndicator!=null)
            {
                emeraldSystem.MovementComponent.MovementEdgeIndicator.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
        /// <summary>
        /// Hides MovementEdge and MovementVertex indicators for specific character
        /// </summary>
        public void HideIndicators(EmeraldSystem emeraldSystem)
        {
            if(NeverHideIndicators) return;
            if(emeraldSystem.MovementComponent.OrderedWaypointsList!=null)
            {
                foreach(Transform transform in emeraldSystem.MovementComponent.OrderedWaypointsList)
                {   
                    foreach(Renderer renderer in transform.gameObject.GetComponentsInChildren<Renderer>()){renderer.enabled=false;}
                }
            } 
            if(emeraldSystem.MovementComponent.MovementEdgeIndicator!=null)
            {
                emeraldSystem.MovementComponent.MovementEdgeIndicator.gameObject.GetComponent<Renderer>().enabled = false;
            } 
        }
        public void RemoveOrders(EmeraldSystem emeraldSystem)
        {
            if(emeraldSystem.MovementComponent.OrderedWaypointsList!=null)
            {
                foreach(Transform transform in emeraldSystem.MovementComponent.OrderedWaypointsList)
                {
                    transform.gameObject.SetActive(false);
                    foreach(Renderer renderer in transform.gameObject.GetComponentsInChildren<Renderer>()){renderer.enabled=true;}
                }
                emeraldSystem.MovementComponent.OrderedWaypointsList = null;
            } 
            if(emeraldSystem.MovementComponent.MovementEdgeIndicator!=null)
            {
                emeraldSystem.MovementComponent.MovementEdgeIndicator.SetActive(false);
                emeraldSystem.MovementComponent.MovementEdgeIndicator.gameObject.GetComponent<Renderer>().enabled = true;
                emeraldSystem.MovementComponent.MovementLineIndicator = null;
                emeraldSystem.MovementComponent.MovementEdgeIndicator = null;
            } 
        }

        /// <summary>
        /// Hides all indicators
        /// </summary>
        public void HideAllIndicators()
        {
            foreach(KeyValuePair<int,Character> character in SelectedTable)
            {
                HideIndicators(character.Value.EmeraldComponent);
            } 
        }
        /// <summary>
        /// removes all indicators
        /// </summary>
        public void RemoveAllIndicators()
        {
            foreach(KeyValuePair<int,Character> character in SelectedTable)
            {
                RemoveOrders(character.Value.EmeraldComponent);
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
            Character character = FindCharacterComponentInParent(gameObject);
            int id = character.GetInstanceID();
            if(!(SelectedTable.ContainsKey(id))){
                SelectedTable.Add(id,character);
            }
            ShowIndicators(SelectedTable[id].EmeraldComponent);
            RTSEvent.Trigger(RTSEventTypes.PlayerSelected, null, SelectedTable);
        }

        /// <summary>
        /// Removes a specific object from dictionary using it's ID and fires RTSEvent
        /// </summary>
        /// <param name="gameObject"></param>

        public void Deselect(int id){
            if(!SelectedTable.ContainsKey(id)){return;}
            HideIndicators(SelectedTable[id].EmeraldComponent);
            SelectedTable.Remove(id);
            RTSEvent.Trigger(RTSEventTypes.PlayerSelected, null, SelectedTable);
        }
        /// <summary>
        /// Removes all objects from selection dictionary and fires RTSEvent
        /// </summary>
        /// <param name="gameObject"></param>

        public void DeselectAll(){
            HideAllIndicators();
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

        public void ShowAllCharacterIndicators()
        {
            foreach (Character character in LevelManager.Instance.Players)
            {
                ShowIndicators(character.GetComponent<EmeraldSystem>());
            }
        }

        public void HideAllCharacterIndicators()
        {
            foreach (Character character in LevelManager.Instance.Players)
            {
                if(!SelectedTable.ContainsValue(character))
                {
                    HideIndicators(character.GetComponent<EmeraldSystem>());
                }
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
                AddSelected(other.gameObject);
            }
        }

        private Character FindCharacterComponentInParent(GameObject childObject)
        {
            if(childObject.GetComponent<Character>()!=null)
            {
                return childObject.GetComponent<Character>();
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
                    return characterComponent;
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
                case TopDownEngineEventTypes.ActiveCinematicMode:
                    if(RTSMode)
                    {
                        canInput=false;
                    }
                    break;
                case TopDownEngineEventTypes.TurnOffCinematicMode:
                    if(RTSMode)
                    {
                        canInput=true;
                    }
                    break;
                case TopDownEngineEventTypes.SwitchToGameMode:
                    if(RTSMode)
                    {
                        canInput=true;
                    }
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
                case RTSEventTypes.UnselectedEveryone:
                    DeselectAll();
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