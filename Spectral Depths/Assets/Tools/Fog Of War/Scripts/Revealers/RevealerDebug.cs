using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FOW
{
    public class RevealerDebug : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool DrawDebugStats = true;
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RevealerDebug))]
    public class RevealerDebugEditor : Editor
    {
        FogOfWarRevealer Revealer;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            RevealerDebug stat = (RevealerDebug)target;

            //FogOfWarRevealer rev = stat.GetRevealerComponent();

            if (Revealer == null)
            {
                if (!stat.TryGetComponent<FogOfWarRevealer>(out Revealer))
                {
                    EditorGUILayout.LabelField($"Revealer component not found.");
                    return;
                }
            }

            if (!stat.DrawDebugStats)
                return;

            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField($"NUM SEGMENTS: {Revealer.NumberOfPoints}");
            for (int i = 0; i < Revealer.NumberOfPoints; i++)
            {
                EditorGUILayout.LabelField($"------------- Segment {i} -------------");
                EditorGUILayout.LabelField($"Angle: {Revealer.Angles[i]}");
                EditorGUILayout.LabelField($"Radius: {Revealer.Radii[i]}");
                EditorGUILayout.LabelField($"Did Hit?: {Revealer.AreHits[i]}");
            }
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Debug Toggle Static"))
                {
                    Revealer.SetRevealerAsStatic(!Revealer.StaticRevealer);
                }
            }
        }
    }
#endif
}