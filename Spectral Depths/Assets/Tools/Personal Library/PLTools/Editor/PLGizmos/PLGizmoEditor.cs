using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// a custom editor for the PLGizmo component 
	/// </summary>
	[CustomEditor(typeof(PLGizmo), true)]
	[CanEditMultipleObjects]
	public class PLGizmoEditor : Editor
	{
		/// <summary>
		/// Lets you press G when in scene view to toggle gizmos on or off
		/// </summary>
		[Shortcut("Toggle Gizmos", typeof(SceneView), KeyCode.G, displayName = "ToggleGizmos")]
		public static void ToggleGizmos()
		{
			SceneView.lastActiveSceneView.drawGizmos = !SceneView.lastActiveSceneView.drawGizmos;
		}
		
		/// <summary>
		/// When the target object is selected, we draw our gizmos
		/// </summary>
		/// <param name="mmGizmo"></param>
		/// <param name="gizmoType"></param>
		[DrawGizmo(GizmoType.Selected)]
		private static void DrawGizmoSelected(PLGizmo mmGizmo, GizmoType gizmoType)
		{
			if (!mmGizmo.DisplayGizmo)
			{
				return;
			}
			DrawGizmos(mmGizmo);
		}
		
		/// <summary>
		/// When the target object is not selected, we draw our gizmos if needed
		/// </summary>
		/// <param name="mmGizmo"></param>
		/// <param name="gizmoType"></param>
		[DrawGizmo(GizmoType.NonSelected)]
		private static void DrawGizmoNonSelected(PLGizmo mmGizmo, GizmoType gizmoType)
		{
			if (!mmGizmo.DisplayGizmo)
			{
				return;
			}
			if (mmGizmo.DisplayMode != PLGizmo.DisplayModes.Always)
			{
				return;
			}
			DrawGizmos(mmGizmo);
		}

		/// <summary>
		/// Draws gizmos and text
		/// </summary>
		/// <param name="mmGizmo"></param>
		private static void DrawGizmos(PLGizmo mmGizmo)
		{
			if (!mmGizmo.Initialized)
			{
				Initialization(mmGizmo);
			}
			
			if (TestDistance(mmGizmo, mmGizmo.ViewDistance))
			{
				Gizmos.color = mmGizmo.GizmoColor;
				Gizmos.matrix = mmGizmo.transform.localToWorldMatrix;
			
				switch (mmGizmo.GizmoType)
				{
					case PLGizmo.GizmoTypes.Collider:
						DrawColliderGizmo(mmGizmo);
						break;
					case PLGizmo.GizmoTypes.Position:
						DrawPositionGizmo(mmGizmo);
						break;
				}
			}
			DrawText(mmGizmo);
		}

		/// <summary>
		/// Tests whether or not gizmos should be drawn based on distance to the scene camera
		/// </summary>
		/// <param name="mmGizmo"></param>
		/// <param name="viewDistance"></param>
		/// <returns></returns>
		private static bool TestDistance(PLGizmo mmGizmo, float viewDistance)
		{
			float distanceToCamera = 0f;
			
			if (SceneView.currentDrawingSceneView == null)
			{
				distanceToCamera = Vector3.Distance(mmGizmo.transform.position, Camera.main.transform.position);
				return (distanceToCamera < viewDistance);
			}
			else
			{
				distanceToCamera = Vector3.Distance(mmGizmo.transform.position, SceneView.currentDrawingSceneView.camera.transform.position);
				return (distanceToCamera < viewDistance);	
			}
		}
		
		/// <summary>
		/// On Enable we initialize our gizmo
		/// </summary>
		protected virtual void OnEnable()
		{
			Initialization(target as PLGizmo);
		}

		/// <summary>
		/// On validate we initialize our gizmo
		/// </summary>
		protected void OnValidate()
		{
			Initialization(target as PLGizmo);
		}

		/// <summary>
		/// Initializes the gizmo, caching components, values, and inits the GUIStyle
		/// </summary>
		/// <param name="mmGizmo"></param>
		private static void Initialization(PLGizmo mmGizmo)
		{
			mmGizmo._sphereCollider = mmGizmo.gameObject.GetComponent<SphereCollider>();
			mmGizmo._boxCollider = mmGizmo.gameObject.GetComponent<BoxCollider>();
			mmGizmo._meshCollider = mmGizmo.gameObject.GetComponent<MeshCollider>();
			mmGizmo._circleCollider2D = mmGizmo.gameObject.GetComponent<CircleCollider2D>();
			mmGizmo._boxCollider2D = mmGizmo.gameObject.GetComponent<BoxCollider2D>();

			mmGizmo._sphereColliderNotNull = (mmGizmo._sphereCollider != null);
			mmGizmo._boxColliderNotNull = (mmGizmo._boxCollider != null);
			mmGizmo._meshColliderNotNull = (mmGizmo._meshCollider != null);
			mmGizmo._circleCollider2DNotNull = (mmGizmo._circleCollider2D != null);
			mmGizmo._boxCollider2DNotNull = (mmGizmo._boxCollider2D != null);

			mmGizmo._vector3Zero = Vector3.zero;
			mmGizmo._textureRect = new Rect(0f, 0f, mmGizmo.TextureSize.x, mmGizmo.TextureSize.y);
			mmGizmo._positionTextureNotNull = (mmGizmo.PositionTexture != null);

			mmGizmo._textGUIStyle = new GUIStyle();
			mmGizmo._textGUIStyle.normal.textColor = mmGizmo.TextColor;
			mmGizmo._textGUIStyle.fontSize = mmGizmo.TextSize;
			mmGizmo._textGUIStyle.fontStyle = mmGizmo.TextFontStyle;
			mmGizmo._textGUIStyle.padding = new RectOffset((int)mmGizmo.TextPadding.x, (int)mmGizmo.TextPadding.y, (int)mmGizmo.TextPadding.z, (int)mmGizmo.TextPadding.w);
			mmGizmo._textGUIStyle.normal.background = PLGUI.MakeTex(600, 100, mmGizmo.TextBackgroundColor);

			mmGizmo.Initialized = true;
		}

		/// <summary>
		/// Draws a gizmo for the associated collider
		/// </summary>
		/// <param name="mmGizmo"></param>
		private static void DrawColliderGizmo(PLGizmo mmGizmo)
		{
			if (mmGizmo._sphereColliderNotNull)
			{
				if (mmGizmo.ColliderRenderType == PLGizmo.ColliderRenderTypes.Full)
				{
					Gizmos.DrawSphere(ComputeGizmoPosition(mmGizmo, mmGizmo._sphereCollider.center), mmGizmo._sphereCollider.radius);	
				}
				else
				{
					Gizmos.DrawWireSphere(ComputeGizmoPosition(mmGizmo, mmGizmo._sphereCollider.center), mmGizmo._sphereCollider.radius);	
				}
			}

			if (mmGizmo._boxColliderNotNull)
			{
				if (mmGizmo.ColliderRenderType == PLGizmo.ColliderRenderTypes.Full)
				{
					Gizmos.DrawCube(ComputeGizmoPosition(mmGizmo, mmGizmo._boxCollider.center), mmGizmo._boxCollider.size);
				}
				else
				{
					Gizmos.DrawWireCube(ComputeGizmoPosition(mmGizmo, mmGizmo._boxCollider.center), mmGizmo._boxCollider.size);
				}
			}

			if (mmGizmo._circleCollider2DNotNull)
			{
				if (mmGizmo.ColliderRenderType == PLGizmo.ColliderRenderTypes.Full)
				{
					Gizmos.DrawSphere((Vector3)ComputeGizmoPosition(mmGizmo, mmGizmo._circleCollider2D.offset), mmGizmo._circleCollider2D.radius);
				}
				else
				{
					Gizmos.DrawWireSphere((Vector3)ComputeGizmoPosition(mmGizmo, mmGizmo._circleCollider2D.offset), mmGizmo._circleCollider2D.radius);
				}
			}

			if (mmGizmo._boxCollider2DNotNull)
			{
				Vector3 gizmoSize = new Vector3();
				gizmoSize.x =  mmGizmo._boxCollider2D.size.x ;
				gizmoSize.y =  mmGizmo._boxCollider2D.size.y ;
				gizmoSize.z = 0.1f;
				if (mmGizmo.ColliderRenderType == PLGizmo.ColliderRenderTypes.Full)
				{
					Gizmos.DrawCube(ComputeGizmoPosition(mmGizmo, mmGizmo._boxCollider2D.offset), gizmoSize);
				}
				else
				{
					Gizmos.DrawWireCube(ComputeGizmoPosition(mmGizmo, mmGizmo._boxCollider2D.offset), gizmoSize);
				}
			}

			if (mmGizmo._meshColliderNotNull)
			{
				if (mmGizmo.ColliderRenderType == PLGizmo.ColliderRenderTypes.Full)
				{
					Gizmos.DrawMesh(mmGizmo._meshCollider.sharedMesh);
				}
				else
				{
					Gizmos.DrawWireMesh(mmGizmo._meshCollider.sharedMesh);
				}
			}
		}

		/// <summary>
		/// Draws a position gizmo
		/// </summary>
		/// <param name="mmGizmo"></param>
		private static void DrawPositionGizmo(PLGizmo mmGizmo)
		{
			switch (mmGizmo.PositionMode)
			{
				case PLGizmo.PositionModes.Point:
					PLDebug.DrawGizmoPoint(ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero), mmGizmo.GizmoColor, mmGizmo.PositionSize);
					break;
				case PLGizmo.PositionModes.Cube:
					Gizmos.DrawCube(ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero), Vector3.one * mmGizmo.PositionSize);
					break;
				case PLGizmo.PositionModes.Sphere:
					Gizmos.DrawSphere(ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero), mmGizmo.PositionSize);
					break;
				case PLGizmo.PositionModes.WireCube:
					Gizmos.DrawWireCube(ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero), Vector3.one * mmGizmo.PositionSize);
					break;
				case PLGizmo.PositionModes.WireSphere:
					Gizmos.DrawWireSphere(ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero), mmGizmo.PositionSize);
					break;
				case PLGizmo.PositionModes.Texture:
					if (mmGizmo._positionTextureNotNull)
					{
						Handles.BeginGUI();
						mmGizmo._worldToGUIPosition = HandleUtility.WorldToGUIPoint(ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false));
						mmGizmo._textureRect = new Rect(mmGizmo._worldToGUIPosition.x - mmGizmo.TextureSize.x/2f, mmGizmo._worldToGUIPosition.y - mmGizmo.TextureSize.y/2f, mmGizmo.TextureSize.x, mmGizmo.TextureSize.y);
						GUI.Label(mmGizmo._textureRect, mmGizmo.PositionTexture);
						Handles.EndGUI();
					}
					break;
				case PLGizmo.PositionModes.Arrows:
					Handles.color = Handles.xAxisColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.right, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					Handles.color = Handles.yAxisColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.up, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					Handles.color = Handles.zAxisColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.forward, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					break;
				case PLGizmo.PositionModes.RightArrow:
					Handles.color = mmGizmo.GizmoColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.right, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					break;
				case PLGizmo.PositionModes.UpArrow:
					Handles.color = mmGizmo.GizmoColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.up, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					break;
				case PLGizmo.PositionModes.ForwardArrow:
					Handles.color = mmGizmo.GizmoColor;
					Handles.ArrowHandleCap(0, ComputeGizmoPosition(mmGizmo, mmGizmo.transform.position, false),
						Quaternion.LookRotation(mmGizmo.transform.forward, mmGizmo.transform.up), mmGizmo.PositionSize, EventType.Repaint);
					break;
				case PLGizmo.PositionModes.Lines:
					Vector3 origin = ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero);
					Vector3 destination = origin + Vector3.right * mmGizmo.PositionSize;
					Gizmos.DrawLine(origin, destination);
					destination = origin + Vector3.up * mmGizmo.PositionSize;
					Gizmos.DrawLine(origin, destination);
					destination = origin + Vector3.forward * mmGizmo.PositionSize;
					Gizmos.DrawLine(origin, destination);
					break;
				case PLGizmo.PositionModes.RightLine:
					Vector3 rightOrigin = ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero);
					Vector3 rightDestination = rightOrigin + Vector3.right * mmGizmo.PositionSize;
					Gizmos.DrawLine(rightOrigin, rightDestination);
					break;
				case PLGizmo.PositionModes.UpLine:
					Vector3 upOrigin = ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero);
					Vector3 upDestination = upOrigin + Vector3.up * mmGizmo.PositionSize;
					Gizmos.DrawLine(upOrigin, upDestination);
					break;
				case PLGizmo.PositionModes.ForwardLine:
					Vector3 fwdOrigin = ComputeGizmoPosition(mmGizmo, mmGizmo._vector3Zero);
					Vector3 fwdDestination = fwdOrigin + Vector3.forward * mmGizmo.PositionSize;
					Gizmos.DrawLine(fwdOrigin, fwdDestination);
					break;
			}
		}

		/// <summary>
		/// Draws our gizmo text
		/// </summary>
		/// <param name="mmGizmo"></param>
		private static void DrawText(PLGizmo mmGizmo)
		{
			if (!mmGizmo.DisplayText)
			{
				return;
			}
			
			if (!TestDistance(mmGizmo, mmGizmo.TextMaxDistance))
			{
				return;
			}

			switch (mmGizmo.TextMode)
			{
				case PLGizmo.TextModes.GameObjectName:
					mmGizmo._textToDisplay = mmGizmo.gameObject.name;
					break;
				case PLGizmo.TextModes.CustomText:
					mmGizmo._textToDisplay = mmGizmo.TextToDisplay;
					break;
				case PLGizmo.TextModes.Position:
					mmGizmo._textToDisplay = mmGizmo.transform.position.ToString();
					break;
				case PLGizmo.TextModes.Rotation:
					mmGizmo._textToDisplay = mmGizmo.transform.rotation.ToString();
					break;
				case PLGizmo.TextModes.Scale:
					mmGizmo._textToDisplay = mmGizmo.transform.localScale.ToString();
					break;
				case PLGizmo.TextModes.Property:
					if (mmGizmo.TargetProperty.PropertyFound)
					{
						mmGizmo._textToDisplay = mmGizmo.TargetProperty.GetRawValue().ToString();
					}
					break;
			}

			if (mmGizmo._textToDisplay != "")
			{
				Handles.Label(mmGizmo.transform.position + mmGizmo.TextOffset, mmGizmo._textToDisplay, mmGizmo._textGUIStyle);	
			}
		}

		/// <summary>
		/// Computes the position at which to draw the gizmo
		/// </summary>
		/// <param name="mmGizmo"></param>
		/// <param name="position"></param>
		/// <param name="relativeLock"></param>
		/// <returns></returns>
		private static Vector3 ComputeGizmoPosition(PLGizmo mmGizmo, Vector3 position, bool relativeLock = true)
		{
			mmGizmo._newPosition = position + mmGizmo.GizmoOffset;

			if (mmGizmo.LockX || mmGizmo.LockY || mmGizmo.LockZ)
			{
				Vector3 mmGizmoNewPosition = mmGizmo._newPosition;
				if (mmGizmo.LockX) { mmGizmoNewPosition.x = relativeLock ? - mmGizmo.transform.position.x + mmGizmo.LockedX : mmGizmo.LockedX; }
				if (mmGizmo.LockY) { mmGizmoNewPosition.y = relativeLock ? - mmGizmo.transform.position.y + mmGizmo.LockedY : mmGizmo.LockedY; }
				if (mmGizmo.LockZ) { mmGizmoNewPosition.z = relativeLock ? - mmGizmo.transform.position.z + mmGizmo.LockedZ : mmGizmo.LockedZ; } 
				mmGizmo._newPosition = mmGizmoNewPosition;
			}

			return mmGizmo._newPosition;
		}
		
	}	
}