using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to pick a property on a target object / component / scriptable object
	/// </summary>
	[Serializable]
	public class PLPropertyPicker
	{
		/// the target object to look for a property on
		public UnityEngine.Object TargetObject;
		/// the component to look for a property on | storage only, not displayed in the inspector
		public Component TargetComponent;
		/// the component to look for a property on | storage only, not displayed in the inspector
		public ScriptableObject TargetScriptableObject;
		/// the name of the property to link to
		public string TargetPropertyName;
		/// whether or not this property has been found
		public bool PropertyFound { get; protected set; }
        
		protected PLProperty _targetMMProperty;
		protected bool _initialized = false;
		protected PLPropertyLink _propertySetter;

		/// <summary>
		/// When the property picker gets initialized, it grabs the stored property or field 
		/// and initializes a PLProperty and PLPropertyLink
		/// </summary>
		/// <param name="source"></param>
		public virtual void Initialization(GameObject source)
		{
			if ((TargetComponent == null) && (TargetScriptableObject == null))
			{
				PropertyFound = false;
				return;
			}
            
			_targetMMProperty = PLProperty.FindProperty(TargetPropertyName, TargetComponent, source, TargetScriptableObject);

			if (_targetMMProperty == null)
			{
				PropertyFound = false;
				return;
			}

			if ((_targetMMProperty.TargetComponent == null) && (_targetMMProperty.TargetScriptableObject == null))
			{
				PropertyFound = false;
				return;
			}
			if ((_targetMMProperty.MemberPropertyInfo == null) && (_targetMMProperty.MemberFieldInfo == null))
			{
				PropertyFound = false;
				return;
			}
			PropertyFound = true;
			_initialized = true;

			// if succession because pattern matching isn't supported before C# 7
			if (_targetMMProperty.PropertyType == typeof(string))
			{
				_propertySetter = new PLPropertyLinkString();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(float))
			{
				_propertySetter = new PLPropertyLinkFloat();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(Vector2))
			{
				_propertySetter = new PLPropertyLinkVector2();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(Vector3))
			{
				_propertySetter = new PLPropertyLinkVector3();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(Vector4))
			{
				_propertySetter = new PLPropertyLinkVector4();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(Quaternion))
			{
				_propertySetter = new PLPropertyLinkQuaternion();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(int))
			{
				_propertySetter = new PLPropertyLinkInt();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(bool))
			{
				_propertySetter = new PLPropertyLinkBool();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
			if (_targetMMProperty.PropertyType == typeof(Color))
			{
				_propertySetter = new PLPropertyLinkColor();
				_propertySetter.Initialization(_targetMMProperty);
				return;
			}
		}

		/// <summary>
		/// Returns the raw value of the target property
		/// </summary>
		/// <returns></returns>
		public virtual object GetRawValue()
		{
			return _propertySetter.GetPropertyValue(_targetMMProperty);
		}
	}
}