// www.SlimUI.com
// Copyright (c) 2019 - 2019 SlimUI. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// JOIN THE DISCORD FOR SUPPORT OR CONVERSATION: https://discord.gg/7cK4KBf

using UnityEditor;
using UnityEngine;

namespace SlimUI.CursorControllerPro{
    public class CCPControlPanel : EditorWindow{
        #region Instance

        private static CCPControlPanel s_instance;
        protected GUISkin guiSkin;

        // Set up window
        bool inSetupWindow = false;
        bool showWarning = false; // just to display warning on generate errors
        
        string cursorRendererName = "CursorCamera";

        //public string[] renderModeOptions = new string[] {"Screen Space - Overlay", "Screen Space - Camera", "World Space"};
        public string[] renderModeOptions = new string[] {"Screen Space - Camera", "World Space"};
        public int renderindex = 0;

        public static CCPControlPanel Instance{get{
                if (s_instance != null) return s_instance;
                s_instance = Window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<CCPControlPanel>(true, "CCP Control Panel");
                return s_instance;
            }
        }

        private static CCPControlPanel Window{get{
                CCPControlPanel[] windows = Resources.FindObjectsOfTypeAll<CCPControlPanel>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        #endregion

        private static GUIStyle background, redButton, setupButton, moreButton;

        [MenuItem("Window/SlimUI/Cursor Controller Pro/Control Panel", false, 0)]
        public static void Open(){
            GetWindow<CCPControlPanel>(true, "CCP Control Panel");
            Instance.InitWindow();
        }

        private void OnEnable(){
            InitWindow();
            EditorUtility.ClearProgressBar();
        }

        private void InitWindow(){
            titleContent = new GUIContent("CCP Control Panel");
            minSize = new Vector2(336, 429);
            maxSize = minSize;

            InitStyles();
        }

        void InitStyles(){
            guiSkin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/Resources/SlimUI/ControlPanelSkin.guiskin", typeof(GUISkin));

            redButton = guiSkin.GetStyle("RedButton");
            moreButton = guiSkin.GetStyle("MoreButton");
            setupButton = guiSkin.GetStyle("SetupButton");
            background = guiSkin.GetStyle("WindowBackground");
	
            if(guiSkin == null)
                guiSkin = new GUISkin();
        }

        private void OnInspectorUpdate(){Repaint();}

        private void OnGUI(){
            GUI.Label(new Rect(-2, 0, background.fixedWidth, background.fixedHeight), background.normal.background);
            GUILayout.BeginArea(new Rect(0, 172, position.width, position.height - 172));{
                DrawContent();
            }
            GUILayout.EndArea();
        }

        private void DrawContent(){
            if(!inSetupWindow){
                GUILayout.BeginVertical();{
                    DrawSetup();
                    DrawDocumentation();
                    DrawSupport();
                    DrawWatchVideos();
                    DrawMoreSlimUI();
                }
                GUILayout.EndVertical();
            }else{
                GUILayout.BeginVertical();{
                    if (GUILayout.Button("MENU", moreButton)){ 
                        inSetupWindow = false;
                        showWarning = false;
                    }
                    GUILayout.Label("Camera Render Mode", guiSkin.label);
                    renderindex = EditorGUILayout.Popup(renderindex, renderModeOptions, guiSkin.GetStyle("Dropdown"));
                    GUILayout.Space(5);
                    GUILayout.Label("Name of existing Camera object to use as Renderer.        (1) Leaving the name empty will generate a camera.         (2) The name cannot contain spaces.", guiSkin.label);
                    GUILayout.Space(2);
                    cursorRendererName = EditorGUILayout.TextField(cursorRendererName, guiSkin.GetStyle("Textfield"));
                    GUILayout.Space(13);
                    if (GUILayout.Button("GENERATE CONTROLLER", redButton)){
                        Object controlPrefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/SlimUI/Prefabs/CursorControl.prefab", typeof(GameObject));
                        if (controlPrefab != null && FindObjectsOfType<CursorController>().Length < 1){
                            GameObject spawnedPrefab = PrefabUtility.InstantiatePrefab(controlPrefab) as GameObject;
                            spawnedPrefab.GetComponent<CursorController>();

                            if(renderindex==0){
                                spawnedPrefab.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                                if(GameObject.Find(cursorRendererName) == null){
                                    GameObject spawnedCamera = new GameObject("Cursor Camera");
                                    spawnedCamera.AddComponent<Camera>();
                                    //spawnedCamera.tag = cursorRendererName;
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain = spawnedCamera.GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<Canvas>().worldCamera = spawnedCamera.GetComponent<Camera>();
                                    spawnedCamera.GetComponent<Camera>().orthographic = true;
                                    DestroyImmediate(spawnedPrefab.GetComponent<AudioListener>());
                                }else{
                                    spawnedPrefab.GetComponent<Canvas>().worldCamera = GameObject.Find(cursorRendererName).GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain = GameObject.Find(cursorRendererName).GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain.GetComponent<Camera>().GetComponent<Camera>().orthographic = true;
                                    DestroyImmediate(spawnedPrefab.GetComponent<AudioListener>());
                                    //spawnedPrefab.GetComponent<CursorController>().cameraMain.GetComponent<Camera>().name = "Cursor Camera";
                                }
                            }else if(renderindex==1){
                                spawnedPrefab.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                                if(GameObject.Find(cursorRendererName) == null){
                                    GameObject spawnedCamera = new GameObject("Cursor Camera");
                                    spawnedCamera.AddComponent<Camera>();
                                    //spawnedCamera.tag = cursorRendererName;
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain = spawnedCamera.GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<Canvas>().worldCamera = spawnedCamera.GetComponent<Camera>();
                                    spawnedCamera.GetComponent<Camera>().orthographic = true;
                                    DestroyImmediate(spawnedCamera.GetComponent<AudioListener>());
                                }else{
                                    spawnedPrefab.GetComponent<Canvas>().worldCamera = GameObject.Find(cursorRendererName).GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain = GameObject.Find(cursorRendererName).GetComponent<Camera>();
                                    spawnedPrefab.GetComponent<CursorController>().cameraMain.GetComponent<Camera>().orthographic = true;
                                    DestroyImmediate(spawnedPrefab.GetComponent<AudioListener>());
                                    //spawnedPrefab.GetComponent<CursorController>().cameraMain.name = "Cursor Camera";
                                }
                            }

                            showWarning = false;
                            Debug.Log("Scene Successfully Configured!");
                            }else if (controlPrefab == null){
                                showWarning = true;
                                Debug.LogError("Could not find CursorControl.prefab. Make sure it's in the directory: Assets/SlimUI/ConsoleCursors/Prefabs/CursorControl.prefab");
                            }else if(FindObjectsOfType<CursorController>().Length >= 1){
                                showWarning = true;
                                Debug.LogWarning("There is already a CursorControl prefab in your scene. You can only have 1.");
                            }

                    }
                    GUILayout.Space(20);
                    if(showWarning){
                        GUIStyle warning = new GUIStyle(GUI.skin.label);
                        warning.fontSize = 12;
                        warning.wordWrap = true;
                        warning.normal.textColor = Color.red;
                        GUILayout.Label("'                     !! Check Console.", guiSkin.GetStyle("Warning"));
                    }
                }
                GUILayout.EndVertical();
            }
        }

        private void DrawSetup(){
            GUILayout.BeginVertical();{
                if (GUILayout.Button("SET UP", setupButton)) inSetupWindow = true;}
            GUILayout.EndVertical();
        }

        private void DrawDocumentation(){
            GUILayout.BeginVertical();{
                GUILayout.Space(5);
                if (GUILayout.Button("Documentation", redButton)) Application.OpenURL("http://cursorcontrollerpro.slimui.com/documentation/");}
            GUILayout.EndVertical();
        }

        private void DrawSupport(){
            GUILayout.BeginVertical();{
                GUILayout.Space(5);
                if (GUILayout.Button("Get Support", redButton)) Application.OpenURL("http://cursorcontrollerpro.slimui.com/support/");}
            GUILayout.EndVertical();
        }

        private void DrawWatchVideos(){
            GUILayout.BeginVertical();{
                GUILayout.Space(5);
                if (GUILayout.Button("Watch Videos", redButton)) Application.OpenURL("https://www.youtube.com/channel/UCvUS-7AVIBYOgvQZ7ltxfVg/");}
            GUILayout.EndVertical();
        }

        private void DrawMoreSlimUI(){
            GUILayout.BeginVertical(GUILayout.Height(45));{
                GUILayout.Space(5);
                if (GUILayout.Button("MORE CONTENT", moreButton)) Application.OpenURL("https://assetstore.unity.com/publishers/35968");}
            GUILayout.EndVertical();
        }
    }
}