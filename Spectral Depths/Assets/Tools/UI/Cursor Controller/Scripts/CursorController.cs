

using UnityEngine;
using UnityEngine.EventSystems;
using SlimUI.CursorControllerPro.InputSystem;

namespace SlimUI.CursorControllerPro {

    public class CursorController : PointerInputModule {
        public static CursorController Instance { get; private set; }
        public static GeneralSettings InstanceGeneralSettings { get { return Instance._generalSettings;} }

        public enum eInputState{MouseKeyboard, Controller};
        private eInputState m_State = eInputState.MouseKeyboard;

        GeneralSettings _generalSettings;
        GeneralSettings generalSettingsInstance;

        IInputProvider inputProvider;

        Object[] CursorObjectsList;
        GameObject currentActiveCursorPlayer1;
        GameObject currentActiveCursorPlayer2;
        GameObject currentActiveCursorPlayer3;
        GameObject currentActiveCursorPlayer4;
        GameObject currentActiveCursor;

        [Header("PLAYER CURSORS")]
        public string startingCursor;
        public string startingCursorP2;
        public string startingCursorP3;
        public string startingCursorP4;
        public enum Players {One, Two, Three, Four};
        public Players players;


        [Tooltip("The axis name in the Input Manager")]
        public string horizontalAxis = "Horizontal";
        public string horizontalAxis2 = "Horizontal2";
        public string horizontalAxis3 = "Horizontal3";
        public string horizontalAxis4 = "Horizontal4";
        [Tooltip("The axis name in the Input Manager")]
        public string verticalAxis = "Vertical";
        public string verticalAxis2 = "Vertical2";
        public string verticalAxis3 = "Vertical3";
        public string verticalAxis4 = "Vertical4";

        [HideInInspector]
        public bool canMoveCursor = false;
        [Header("CURSOR CONTROLS")]
        [Tooltip("If enabled, the game will start with Mouse Input instead of a handheld controller.")]
        public bool startInDesktopMode = true;
        public bool startEnabled = true;
        bool usingDesktopCursor = false;
        [Space]
        [HideInInspector]
        public int currentPlayerActive = 0;
        [HideInInspector]
        public int playerCount = 1;
        [HideInInspector]
        public bool player2Exists = false;
        [HideInInspector]
        public bool player3Exists = false;
        [HideInInspector]
        public bool player4Exists = false;
        [HideInInspector]
        public GameObject cursorObjectPlayer1;
        [HideInInspector]
        public GameObject cursorObjectPlayer2;
        [HideInInspector]
        public GameObject cursorObjectPlayer3;
        [HideInInspector]
        public GameObject cursorObjectPlayer4;
        [Space]
        [Tooltip("The camera in your scene being used as the Main Camera")]
        public Camera cameraMain;
        Canvas selfCanvas;
        string currentXAxis;
        string currentYAxis;
        [Tooltip("If you want to switch between Mouse Input and Hand-Held Controller Input, add a game object with a Standalone Input Module attached and call it through the function.")]
        [HideInInspector]
        public EventSystem mouseEventSystem;
        [Tooltip("The rect UI elements functioning as the CURSOR")]
        [HideInInspector]
        public RectTransform cursorRect;
        [HideInInspector]
        public RectTransform cursorRect2;
        [HideInInspector]
        public RectTransform cursorRect3;
        [HideInInspector]
        public RectTransform cursorRect4;
        [Tooltip("The Rect Transform whose boundaries will be used to calculate edge of screen")]
        RectTransform boundaries;

        private float lastClickTime = 0.0f;


        [Tooltip("The speed that the button repeat presses when holding down the Submit action.")]
        [Range(0.1f, 0.5f)]
        public float pressHoldRepeatTime = 0.25f;
        [Tooltip("Is the user allow to hold a button down to repeat presses.")]
        public bool allowHoldRepeat = true;
        [HideInInspector]
        public bool canRepeatSubmit = true;
        [HideInInspector]
        public Animator fade;
        [HideInInspector]
        public Animator fade2;
        [HideInInspector]
        public Animator fade3;
        [HideInInspector]
        public Animator fade4;

        public static bool cursorVisible = true;
        bool visibilityState;
        public static bool usingDesktopCursorStaticValue = false;
        public static bool currentlyUsingDesktop = false; // if loading another scene, automatically set correct control input on start
        bool hasSwitchedToVirtualMouse = false;
        bool hasSwitchedToController = false;

        GameObject mouseInputModule;

        [Header("EDGE BOUNDS")]
        [Tooltip("Right from the LEFT of the screen limiting cursor movement. To move the offset right, use positive values (Ex. 100)")]
        public float leftOffset = 0.0f;
        [Tooltip("Right from the RIGHT of the screen limiting cursor movement. To move the offset left, use negative values (Ex. -100)")]
        public float rightOffset = 0.0f;
        [Tooltip("Offset from BOTTOM of screen limiting cursor movement. To move the offset up, use positive values (Ex. 100)")]
        public float bottomOffset = 0.0f;
        [Tooltip("Offset from TOP of screen limiting cursor movement. To move the offset down, use negative values (Ex. -100)")]
        public float topOffset = 0.0f;
        [HideInInspector]
        public float xMin = 0.0f;
        [HideInInspector]
        public float xMax = 0.0f;
        [HideInInspector]
        public float yMin = 0.0f;
        [HideInInspector]
        public float yMax = 0.0f;

        [HideInInspector]
        public float xMovement, yMovement;
    
        private Vector2 screenVec;
        public PointerEventData pointer;

        [HideInInspector]
        public TooltipController tooltipController;

        [Header("COLORS")]
        [Tooltip("If true, the tint color on this CursorControl prefab will override the tint set on each cursor prefab, unless the image component has a 'TintBypass' component attached.")]
        public bool overrideTint = false;
        public Color tint = new Color(1,1,1,1);

        [Header("PARALLAX")]
        [Tooltip("The speed at which RectTransform objects that have the ParallaxWindow' script component move with the cursor.")]
        [Range(0.0f, 0.25f)] public float parallaxStrength = 0.05f;

        [Header("DEMO STUFF")]
        [Tooltip("If enabled, certain actions with the controller will print Debug.Log() for.")]
        public bool debugLogging = false;

        protected override void Awake(){
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }else{
                Destroy(gameObject);
            }
        }

        protected override void Start(){
            inputProvider = transform.Find("InputSystemProvider").GetComponent<IInputProvider>();
            CursorObjectsList = Resources.LoadAll("SlimUI/Prefabs/Cursors", typeof(Object));
            _generalSettings = Resources.Load<GeneralSettings>("SlimUI/Settings/GeneralSettings");
            generalSettingsInstance = Object.Instantiate(_generalSettings);

            visibilityState = cursorVisible;

            if(startEnabled){
                canMoveCursor = true;
            }else{
                canMoveCursor = false;
            }

            StartingCursor();
            currentPlayerActive = 0;
            currentXAxis = horizontalAxis;
            currentYAxis = verticalAxis;

            tooltipController = GetComponent<TooltipController>();
            selfCanvas = GetComponent<Canvas>();
            boundaries = transform.GetComponent<RectTransform>();
            mouseInputModule = transform.Find("InputSystemProvider").gameObject;
            mouseEventSystem = transform.Find("InputSystemProvider").GetComponent<EventSystem>();
            cursorRect = transform.Find("cursorRect1").GetComponent<RectTransform>();
            cursorRect2 = transform.Find("cursorRect2").GetComponent<RectTransform>();
            cursorRect3 = transform.Find("cursorRect3").GetComponent<RectTransform>();
            cursorRect4 = transform.Find("cursorRect4").GetComponent<RectTransform>();

            xMin = (boundaries.rect.width / 2 * -1) + leftOffset;
            xMax = (boundaries.rect.width / 2) + rightOffset;
            yMin = (boundaries.rect.height / 2 * -1) + bottomOffset;
            yMax = (boundaries.rect.height / 2) + topOffset;

            base.Start();
            pointer = new PointerEventData(eventSystem);

            cursorObjectPlayer1 = currentActiveCursorPlayer1;
            cursorObjectPlayer2 = currentActiveCursorPlayer2;
            cursorObjectPlayer3 = currentActiveCursorPlayer3;
            cursorObjectPlayer4 = currentActiveCursorPlayer4;

            // Fade for Cursor Animations set to currently selected cursor
            fade = cursorObjectPlayer1.GetComponent<Animator>();
            fade2 = cursorObjectPlayer2.GetComponent<Animator>();
            fade3 = cursorObjectPlayer3.GetComponent<Animator>();
            fade4 = cursorObjectPlayer4.GetComponent<Animator>();

            // Set starting cursors to Inspector Strings Assigned
            ChangeCursor(startingCursor);
            ChangeCursorMulti(startingCursorP2, 1);
            ChangeCursorMulti(startingCursorP3, 2);
            ChangeCursorMulti(startingCursorP4, 3);

            generalSettingsInstance.tempHspeed = generalSettingsInstance.horizontalSpeed;
            generalSettingsInstance.tempVspeed = generalSettingsInstance.verticalSpeed;

            usingDesktopCursorStaticValue = startInDesktopMode;
            usingDesktopCursor = startInDesktopMode;
            currentlyUsingDesktop = usingDesktopCursorStaticValue;
            CheckPlayerCount();

            if (currentlyUsingDesktop){
                SwitchToMouse();
            }else{
                SwitchToController();
            }
        }

        private void CheckPlayerCount(){
            if (players == Players.One){
                playerCount = 1;
            }else if (players == Players.Two){
                playerCount = 2;
                player2Exists = true;
            }else if (players == Players.Three){
                playerCount = 3;
                player2Exists = true;
                player3Exists = true;
            }else if (players == Players.Four){
                playerCount = 4;
                player2Exists = true;
                player3Exists = true;
                player4Exists = true;
            }
        }

        private void StartingCursor(){
            for(int i = 0; i < CursorObjectsList.Length; i++){
                if(CursorObjectsList[i].name == startingCursor){
                    Destroy(currentActiveCursorPlayer1);
                    var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                    cursorInstance.transform.SetParent(cursorRect);
                    cursorInstance.SetActive(true);
                    cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition = 
                    new Vector3(cursorInstance.GetComponent<RectTransform>().anchoredPosition.x,cursorInstance.GetComponent<RectTransform>().anchoredPosition.y,0);
                    currentActiveCursorPlayer1 = cursorInstance;
                }
            }

            for(int i = 0; i < CursorObjectsList.Length; i++){
                if(CursorObjectsList[i].name == startingCursorP2){
                    Destroy(currentActiveCursorPlayer2);
                    var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                    cursorInstance.transform.SetParent(cursorRect2);
                    cursorInstance.SetActive(true);
                    cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                    currentActiveCursorPlayer2 = cursorInstance;
                    cursorObjectPlayer2 = currentActiveCursorPlayer2;
                }
            }

            for(int i = 0; i < CursorObjectsList.Length; i++){
                if(CursorObjectsList[i].name == startingCursorP3){
                    Destroy(currentActiveCursorPlayer3);
                    var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                    cursorInstance.transform.SetParent(cursorRect3);
                    cursorInstance.SetActive(true);
                    cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                    currentActiveCursorPlayer3 = cursorInstance;
                    cursorObjectPlayer3 = currentActiveCursorPlayer3;
                }
            }

            for(int i = 0; i < CursorObjectsList.Length; i++){
                if(CursorObjectsList[i].name == startingCursorP4){
                    Destroy(currentActiveCursorPlayer4);
                    var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                    cursorInstance.transform.SetParent(cursorRect4);
                    cursorInstance.SetActive(true);
                    cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                    currentActiveCursorPlayer4 = cursorInstance;
                    cursorObjectPlayer4 = currentActiveCursorPlayer4;
                }
            }
        }

        public void ChangePlayerCount(int count){
            if(count == 0){
                playerCount = 1;
            }else if(count == 1){
                playerCount = 2;
            }else if(count == 2){
                playerCount = 3;
            }else if(count == 3){
                playerCount = 4;
            }
        }

        public void ChangeCursor(string cursorNum){
            for(int i = 0; i < CursorObjectsList.Length; i++){
                if(CursorObjectsList[i].name == cursorNum){
                    Destroy(currentActiveCursorPlayer1);
                    var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                    cursorInstance.transform.SetParent(cursorRect);
                    cursorInstance.SetActive(true);
                    cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                    cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                    currentActiveCursorPlayer1 = cursorInstance;
                    cursorObjectPlayer1 = currentActiveCursorPlayer1;
                    if(overrideTint) cursorInstance.GetComponent<CursorTint>().SetColor(tint);
                }
            }

            fade = cursorObjectPlayer1.GetComponent<Animator>(); // assign animator to current active cursor
            Cursor.visible = false;
        }

        public void ChangeCursorMulti(string cursorNum, int playerNum){
            if(playerNum == 1){
                for(int i = 0; i < CursorObjectsList.Length; i++){
                    if(CursorObjectsList[i].name == cursorNum){
                        Destroy(currentActiveCursorPlayer2);
                        var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                        cursorInstance.transform.SetParent(cursorRect2);
                        cursorInstance.SetActive(true);
                        cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                        cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                        currentActiveCursorPlayer2 = cursorInstance;
                        cursorObjectPlayer2 = currentActiveCursorPlayer2;
                        if(overrideTint) cursorInstance.GetComponent<CursorTint>().SetColor(tint);
                    }
                }
                fade2 = cursorObjectPlayer2.GetComponent<Animator>();
            }else if(playerNum == 2){
                for(int i = 0; i < CursorObjectsList.Length; i++){
                    if(CursorObjectsList[i].name == cursorNum){
                        Destroy(currentActiveCursorPlayer3);
                        var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                        cursorInstance.transform.SetParent(cursorRect3);
                        cursorInstance.SetActive(true);
                        cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                        cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                        currentActiveCursorPlayer3 = cursorInstance;
                        cursorObjectPlayer3 = currentActiveCursorPlayer3;
                        if(overrideTint) cursorInstance.GetComponent<CursorTint>().SetColor(tint);
                    }
                }
                fade3 = cursorObjectPlayer3.GetComponent<Animator>();
            }else if(playerNum == 3){
                for(int i = 0; i < CursorObjectsList.Length; i++){
                    if(CursorObjectsList[i].name == cursorNum){
                        Destroy(currentActiveCursorPlayer4);
                        var cursorInstance = Instantiate(CursorObjectsList[i]) as GameObject;
                        cursorInstance.transform.SetParent(cursorRect4);
                        cursorInstance.SetActive(true);
                        cursorInstance.transform.localScale = new Vector3(generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale,generalSettingsInstance.cursorScale);
                        cursorInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,generalSettingsInstance.cursorZOffset);
                        currentActiveCursorPlayer4 = cursorInstance;
                        cursorObjectPlayer4 = currentActiveCursorPlayer4;
                        if(overrideTint) cursorInstance.GetComponent<CursorTint>().SetColor(tint);
                    }
                }
                fade4 = cursorObjectPlayer4.GetComponent<Animator>();
            }

            Cursor.visible = false;
        }

        public void EnableMultiCursor(int cursor){ // when enabling player, activate specific cursor
            if(cursor == 1){
                cursorRect2.gameObject.SetActive(true);
            }else if(cursor == 2){
                cursorRect3.gameObject.SetActive(true);
            }else if(cursor == 3){
                cursorRect4.gameObject.SetActive(true);
            }
        }

        public void UpdateOffsets(float newLeftOffset, float newRightOffset, float newBottomOffset, float newTopOffset){ // for when the area of the screen that the cursor can move is changed at runtime
            xMin = (boundaries.rect.width / 2 * -1) + newLeftOffset;
            xMax = (boundaries.rect.width / 2) + newRightOffset;
            yMin = (boundaries.rect.height / 2 * -1) + newBottomOffset;
            yMax = (boundaries.rect.height / 2) + newTopOffset;
        }

        public void SwitchToMouse(){
            Cursor.visible = true;
            cursorVisible = true;
            hasSwitchedToVirtualMouse = true;
            hasSwitchedToController = false;
            currentlyUsingDesktop = true;
            GetComponent<EventSystem>().enabled = false;
            mouseInputModule.SetActive(true);
            currentActiveCursorPlayer1.SetActive(false);
            if(currentActiveCursorPlayer2 != null) currentActiveCursorPlayer2.SetActive(false);
            if(currentActiveCursorPlayer3 != null) currentActiveCursorPlayer3.SetActive(false);
            if(currentActiveCursorPlayer4 != null) currentActiveCursorPlayer4.SetActive(false);
            
        }

        public void SwitchToController(){
            Cursor.visible = false;
            cursorVisible = true;
            hasSwitchedToVirtualMouse = false;
            hasSwitchedToController = true;
            currentlyUsingDesktop = false;
            mouseInputModule.SetActive(false);
            GetComponent<EventSystem>().enabled = true;
            currentActiveCursorPlayer1.SetActive(true);
            cursorObjectPlayer1 = currentActiveCursorPlayer1;
            fade = cursorObjectPlayer1.GetComponent<Animator>();

            if(players == Players.Two){
                currentActiveCursorPlayer2.SetActive(true);
                cursorObjectPlayer2 = currentActiveCursorPlayer2;
                fade2 = cursorObjectPlayer2.GetComponent<Animator>();
            }else if(players == Players.Three){
                currentActiveCursorPlayer2.SetActive(true);
                cursorObjectPlayer2 = currentActiveCursorPlayer2;
                fade2 = cursorObjectPlayer2.GetComponent<Animator>();
                currentActiveCursorPlayer3.SetActive(true);
                cursorObjectPlayer3 = currentActiveCursorPlayer3;
                fade3 = cursorObjectPlayer3.GetComponent<Animator>();
            }else if(players == Players.Four){
                currentActiveCursorPlayer2.SetActive(true);
                cursorObjectPlayer2 = currentActiveCursorPlayer2;
                fade2 = cursorObjectPlayer2.GetComponent<Animator>();
                currentActiveCursorPlayer3.SetActive(true);
                cursorObjectPlayer3 = currentActiveCursorPlayer3;
                fade3 = cursorObjectPlayer3.GetComponent<Animator>();
                currentActiveCursorPlayer4.SetActive(true);
                cursorObjectPlayer4 = currentActiveCursorPlayer4;
                fade4 = cursorObjectPlayer4.GetComponent<Animator>();
            }
        }

        
        // Disables Console Cursor Control and enables the Mouse Cursor
        public void SwitchingToConsoleInput(){
            GetComponent<EventSystem>().enabled = true;
            GetComponent<CanvasGroup>().alpha = 1;
            if(mouseEventSystem != null){
                mouseEventSystem.enabled = false;
            }
            currentlyUsingDesktop = false;
            usingDesktopCursorStaticValue = false;
        }

        // Disables Mouse Control and enables Cursor Control
        public void SwitchingToMouseInput(){
            GetComponent<EventSystem>().enabled = false;
            GetComponent<CanvasGroup>().alpha = 0;
            if(mouseEventSystem != null){
                mouseEventSystem.enabled = true;
            }
            currentlyUsingDesktop = true;
            usingDesktopCursorStaticValue = true;
        }

        private void SelectedPlayerMovement(){ // Moves active player with the mouse & gets cursor boundaries
            Vector3 mousePosition;
            if(selfCanvas.renderMode == RenderMode.ScreenSpaceOverlay){
                mousePosition = inputProvider.GetAbsolutePosition();
            }else{
                mousePosition = cameraMain.ScreenToWorldPoint(inputProvider.GetAbsolutePosition());
            }

            if (!usingDesktopCursor){
                if (currentPlayerActive == 0) CursorBoundaries(cursorRect);
                else if (currentPlayerActive == 1) CursorBoundaries(cursorRect2);
                else if (currentPlayerActive == 2) CursorBoundaries(cursorRect3);
                else if (currentPlayerActive == 3) CursorBoundaries(cursorRect4);
            }else if (usingDesktopCursor){
                if (currentPlayerActive == 0) cursorRect.position = mousePosition;
                else if (currentPlayerActive == 1) cursorRect2.position = mousePosition;
                else if (currentPlayerActive == 2) cursorRect3.position = mousePosition;
                else if (currentPlayerActive == 3) cursorRect4.position = mousePosition;
            }
        }

        private void SetMovementToCurrentPlayerInput(){
            Vector2 axisMovement = Vector2.zero;
            if (currentPlayerActive == 0){ // only detect movement for active cursor, ignore the others
                axisMovement = inputProvider.GetRelativeMovement(GamepadPlayerNum.One);
            }else if (currentPlayerActive == 1 && player2Exists){
                axisMovement = inputProvider.GetRelativeMovement(GamepadPlayerNum.Two);
            }else if (currentPlayerActive == 2 && player3Exists){
                axisMovement = inputProvider.GetRelativeMovement(GamepadPlayerNum.Three);
            }else if (currentPlayerActive == 3 && player4Exists){
                axisMovement = inputProvider.GetRelativeMovement(GamepadPlayerNum.Four);
            }
            xMovement = Time.unscaledDeltaTime * (generalSettingsInstance.horizontalSpeed * 100) * axisMovement.x;
            yMovement = Time.unscaledDeltaTime * (generalSettingsInstance.verticalSpeed * 100) * axisMovement.y;
        }

        public void ChangeActivePlayer(int activePlayerIndex){
            cursorRect.gameObject.SetActive(false);
            cursorRect2.gameObject.SetActive(false);
            cursorRect3.gameObject.SetActive(false);
            cursorRect4.gameObject.SetActive(false);
            currentActiveCursorPlayer1.SetActive(false);
            currentActiveCursorPlayer2.SetActive(false);
            currentActiveCursorPlayer3.SetActive(false);
            currentActiveCursorPlayer4.SetActive(false);

            currentPlayerActive = activePlayerIndex;

            if(activePlayerIndex == 0){
                currentPlayerActive = 0;
                cursorRect.gameObject.SetActive(true);
                cursorObjectPlayer1.SetActive(true);
                currentActiveCursor = currentActiveCursorPlayer1;
                currentXAxis = horizontalAxis;
                currentYAxis = verticalAxis;
            }else if(activePlayerIndex == 1){
                currentPlayerActive = 1;
                cursorRect2.gameObject.SetActive(true);
                cursorObjectPlayer2.SetActive(true);
                currentActiveCursor = currentActiveCursorPlayer2;
                currentXAxis = horizontalAxis2;
                currentYAxis = verticalAxis2;
            }else if(activePlayerIndex == 2){
                currentPlayerActive = 2;
                cursorRect3.gameObject.SetActive(true);
                cursorObjectPlayer3.SetActive(true);
                currentActiveCursor = currentActiveCursorPlayer3;
                currentXAxis = horizontalAxis3;
                currentYAxis = verticalAxis3;
            }else if(activePlayerIndex == 3){
                currentPlayerActive = 3;
                cursorRect4.gameObject.SetActive(true);
                cursorObjectPlayer4.SetActive(true);
                currentActiveCursor = currentActiveCursorPlayer4;
                currentXAxis = horizontalAxis4;
                currentYAxis = verticalAxis4;
            }
        }

        private void CheckDeviceInput(){
            if (usingDesktopCursor && !hasSwitchedToVirtualMouse){
                usingDesktopCursorStaticValue = true;
                SwitchToMouse();
            }else if (!usingDesktopCursor && !hasSwitchedToController){
                usingDesktopCursorStaticValue = false;
                if (!hasSwitchedToController){
                    SwitchToController();
                }
            }
        }

        void CursorBoundaries(RectTransform rect){
            if(rect.anchoredPosition.x >= xMin){ 
                rect.anchoredPosition += new Vector2(xMovement, 0);
            }if(rect.anchoredPosition.x <= xMin && rect.anchoredPosition.x <= xMax){ 
                rect.anchoredPosition -= new Vector2(xMovement, 0);
            }if(rect.anchoredPosition.x >= xMax && rect.anchoredPosition.x >= xMin){
                rect.anchoredPosition += new Vector2(-xMovement, 0);
            }
            if(rect.anchoredPosition.y >= yMin){
                rect.anchoredPosition += new Vector2(0, yMovement);
            }if(rect.anchoredPosition.y <= yMin && rect.anchoredPosition.y <= yMax){
                rect.anchoredPosition -= new Vector2(0, yMovement); 
            }if(rect.anchoredPosition.y >= yMax && rect.anchoredPosition.y >= yMin){
                rect.anchoredPosition += new Vector2(0, -yMovement);
            }
        }

        public override void Process (){
            Vector3 screenPos = cameraMain.WorldToScreenPoint(cursorObjectPlayer1.transform.position);

            if(currentPlayerActive == 0){ screenPos = cameraMain.WorldToScreenPoint(cursorObjectPlayer1.transform.position);
            }else if(currentPlayerActive == 1){ screenPos = cameraMain.WorldToScreenPoint(cursorObjectPlayer2.transform.position);
            }else if(currentPlayerActive == 2){ screenPos = cameraMain.WorldToScreenPoint(cursorObjectPlayer3.transform.position);
            }else if(currentPlayerActive == 3){ screenPos = cameraMain.WorldToScreenPoint(cursorObjectPlayer4.transform.position);
            }

            screenVec.x = screenPos.x;
            screenVec.y = screenPos.y;

            pointer.position = screenVec;
            eventSystem.RaycastAll(pointer, this.m_RaycastResultCache);
            RaycastResult raycastResult = FindFirstRaycast(this.m_RaycastResultCache);
            pointer.pointerCurrentRaycast = raycastResult;

            if(raycastResult.distance < generalSettingsInstance.maxDetectionDistance){

                this.ProcessMove(pointer);
                this.ProcessDrag(pointer);

                if(inputProvider.GetSubmitWasPressed()){
                    CancelInvoke("HoldingDown");
                    if(allowHoldRepeat) InvokeRepeating("HoldingDown", 0f, pressHoldRepeatTime);
                }

                if(inputProvider.GetSubmitWasReleased()){
                    CancelInvoke("HoldingDown");
                }

                if(canRepeatSubmit){
                    canRepeatSubmit = false;
                    pointer.pressPosition = screenVec;
                    pointer.clickTime = Time.unscaledTime;
                    pointer.pointerPressRaycast = raycastResult;
                    pointer.eligibleForClick = true;

                    float timeBetweenClicks = pointer.clickTime - lastClickTime;

                    lastClickTime = Time.unscaledTime;
                    if(this.m_RaycastResultCache.Count > 0){  
                        pointer.selectedObject = raycastResult.gameObject;
                        pointer.pointerPress = ExecuteEvents.ExecuteHierarchy ( raycastResult.gameObject, pointer, ExecuteEvents.submitHandler );
                        pointer.rawPointerPress = raycastResult.gameObject;
                    }else{
                        pointer.rawPointerPress = null;
                    }
                }else{
                    pointer.eligibleForClick = false;
                    pointer.pointerPress = null;
                    pointer.pointerDrag = null;
                    pointer.rawPointerPress = null;
                }
            }
        }

        void HoldingDown(){
            canRepeatSubmit = true;
        }

        private bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold){
            if (!useDragThreshold) return true;
            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        protected override void ProcessDrag(PointerEventData pointerEvent){
            if (pointerEvent.pointerDrag == null) return;
    
            if (!pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, 
                pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold)){
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }
    
            // Drag notification
            if (pointerEvent.dragging){
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag){
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler); 
                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        void OnGUI(){
            if(canMoveCursor){
                switch( m_State ){
                    case eInputState.MouseKeyboard:
                        if(inputProvider.GetActiveInputType() == InputType.Gamepad){
                            m_State = eInputState.Controller;
                            usingDesktopCursor = false;
                            if(debugLogging){Debug.Log("Switching to HANDHELD Input");}
                        }
                        break;
                    case eInputState.Controller:
                        if (inputProvider.GetActiveInputType() == InputType.MouseAndKeyboard){
                            m_State = eInputState.MouseKeyboard;
                            usingDesktopCursor = true;
                            if(debugLogging){Debug.Log("Switching to MOUSE & KEYBOARD Input");}
                        }
                        break;
                }
            }
        }

        public eInputState GetInputState(){
            return m_State;
        }

        public void EnableUserControl(){
            canMoveCursor = true;
        }

        public void PauseGame(){
            canMoveCursor = true;
            cursorVisible = true;

            if(hasSwitchedToVirtualMouse){
                SwitchToMouse();
            }else{
                SwitchToController();
            }
        }

        public void ResumeGame(){
            canMoveCursor = false;
            cursorVisible = false;

            Cursor.visible = false;
            tooltipController.tooltipRect.GetComponent<Animator>().SetBool("Show",false);
            if(cursorRect2.gameObject.activeSelf == true)tooltipController.tooltipRect2.GetComponent<Animator>().SetBool("Show",false);
            if(cursorRect3.gameObject.activeSelf == true)tooltipController.tooltipRect3.GetComponent<Animator>().SetBool("Show",false);
            if(cursorRect4.gameObject.activeSelf == true)tooltipController.tooltipRect4.GetComponent<Animator>().SetBool("Show",false);
            currentActiveCursorPlayer1.SetActive(false);
            if(currentActiveCursorPlayer2 != null) currentActiveCursorPlayer2.SetActive(false);
            if(currentActiveCursorPlayer3 != null) currentActiveCursorPlayer3.SetActive(false);
            if(currentActiveCursorPlayer4 != null) currentActiveCursorPlayer4.SetActive(false);
        }

        // Called by trigger when a button is hovered over
        public void HoverSpeed(){
            generalSettingsInstance.horizontalSpeed = generalSettingsInstance.horizontalSpeed * generalSettingsInstance.hoverMultiplier;
            generalSettingsInstance.verticalSpeed = generalSettingsInstance.verticalSpeed * generalSettingsInstance.hoverMultiplier;
        }

        // Called by trigger when the cursor exits the button hover
        public void NormalSpeed(){
            generalSettingsInstance.horizontalSpeed = generalSettingsInstance.tempHspeed;
            generalSettingsInstance.verticalSpeed = generalSettingsInstance.tempVspeed;
        }

        // Called when the cursor hovers over a button (transitions to highlight effect)
        public void FadeIn(){
            tooltipController.countTimer = true;
            if(fade.GetComponent<Animator>()){
                while(tooltipController.timer < tooltipController.popUpDelay && tooltipController.countTimer){
                    tooltipController.timer = tooltipController.timer + Time.deltaTime;
                }

                if(tooltipController.timer >= tooltipController.popUpDelay){
                    fade.SetBool("Fade",true);
                    tooltipController.countTimer = false;
                    tooltipController.timer = 0;
                }
            }
        }

        // Called when the cursor exits a button hover (transitions to default effect)
        public void FadeOut(){
            if(fade.GetComponent<Animator>()){
                fade.SetBool("Fade",false);
                tooltipController.countTimer = false;
                tooltipController.timer = 0;
            }
        }

        void Update(){
            SetMovementToCurrentPlayerInput();

            if(Input.GetKeyDown("escape")){
                PauseGame();
            }

            if (mouseInputModule) CheckDeviceInput();
            else if (!mouseInputModule) Debug.LogWarning("There is no Mouse Input game object in the scene! Please add one.");
            
            if (canMoveCursor){
                SelectedPlayerMovement();
                tooltipController.ToolTipPositions();
                if (currentPlayerActive == 0) tooltipController.ToolTipBoundaries(cursorRect);
                else if (currentPlayerActive == 1) tooltipController.ToolTipBoundaries(cursorRect2);
                else if (currentPlayerActive == 2) tooltipController.ToolTipBoundaries(cursorRect3); 
                else if (currentPlayerActive == 3) tooltipController.ToolTipBoundaries(cursorRect4);
            }

            if (tooltipController.countTimer) {
                tooltipController.ToolTipPopUpDelay();// delay time for showing tooltip
            }

            if(visibilityState != cursorVisible){
                visibilityState = cursorVisible;
                CursorStateChange();
            }
		}

        public void CursorStateChange(){
            // if you want something to happen whenever the cursor state changes, you can add that here!
        }
    }
}