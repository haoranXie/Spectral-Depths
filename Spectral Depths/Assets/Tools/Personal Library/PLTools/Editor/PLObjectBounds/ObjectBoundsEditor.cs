using UnityEngine;
using System.Collections;
using UnityEditor;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{	
	[CustomEditor(typeof(PLObjectBounds),true)]
	public class ObjectBoundsEditor : Editor 
	{
		protected PLObjectBounds _objectBounds;

		public override void OnInspectorGUI()
		{
			_objectBounds = (PLObjectBounds)target;

			DrawDefaultInspector();

			if (_objectBounds.GetComponent<Renderer>()==null && _objectBounds.BoundsBasedOn==PLObjectBounds.WaysToDetermineBounds.Renderer)
			{
				EditorGUILayout.HelpBox("You've defined this object as having Renderer defined bounds, but no renderer is attached to the object. Add a Renderer, or switch to collider based bounds. The bounds are the dimensions that will be used when spawning your object and to determine when it should be recycled.",MessageType.Warning);
			}

			if (_objectBounds.GetComponent<Collider>()==null && _objectBounds.BoundsBasedOn==PLObjectBounds.WaysToDetermineBounds.Collider)
			{
				EditorGUILayout.HelpBox("You've defined this object as having Collider defined bounds, but no Collider is attached to the object. Add a Collider, or switch to renderer based bounds. The bounds are the dimensions that will be used when spawning your object and to determine when it should be recycled.",MessageType.Warning);
			}

			if (_objectBounds.GetComponent<Collider2D>()==null && _objectBounds.BoundsBasedOn==PLObjectBounds.WaysToDetermineBounds.Collider2D)
			{
				EditorGUILayout.HelpBox("You've defined this object as having Collider2D defined bounds, but no Collider2D is attached to the object. Add a Collider2D, or switch to renderer based bounds. The bounds are the dimensions that will be used when spawning your object and to determine when it should be recycled.",MessageType.Warning);
			}
			    
		}
	}
}