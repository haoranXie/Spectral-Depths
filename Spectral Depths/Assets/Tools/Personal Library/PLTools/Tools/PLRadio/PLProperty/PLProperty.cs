using System;
using System.Reflection;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public class PLProperty
	{
		public enum MemberTypes { Property, Field }
		public Component TargetComponent;
		public ScriptableObject TargetScriptableObject;
		public MemberTypes MemberType;
		public PropertyInfo MemberPropertyInfo;
		public FieldInfo MemberFieldInfo;
		public Type PropertyType;
		public string MemberName;
        
		public PLProperty(Component targetComponent, MemberTypes type, PropertyInfo propertyInfo, FieldInfo fieldInfo, string memberName, ScriptableObject targetScriptable)
		{
			TargetComponent = targetComponent;
			TargetScriptableObject = targetScriptable;
			MemberType = type;
			MemberPropertyInfo = propertyInfo;
			MemberFieldInfo = fieldInfo;
			MemberName = memberName;
		}
        
		public static PLProperty FindProperty(string propertyName, Component targetComponent, GameObject source, ScriptableObject scriptable)
		{
			FieldInfo fieldInfo = null;
			PropertyInfo propInfo = null;
			PLProperty TargetProperty = null;
            
			if (scriptable == null)
			{
				propInfo = targetComponent.GetType().GetProperty(propertyName);
				if (propInfo == null)
				{
					fieldInfo = targetComponent.GetType().GetField(propertyName);
				}
			}
			else
			{
				fieldInfo = scriptable.GetType().GetField(propertyName);
			}
            
			if (propInfo != null)
			{
				TargetProperty = new PLProperty(targetComponent, MemberTypes.Property, propInfo, null, propertyName, scriptable);
			}
			if (fieldInfo != null)
			{
				TargetProperty = new PLProperty(targetComponent, MemberTypes.Field, null, fieldInfo, propertyName, scriptable);
			}
			if (propertyName == "")
			{
				if (source != null)
				{
					Debug.LogError("The PLProperty on " + source.name + " : you need to pick a property from the Property list");
				}
				return null;
			}
			if ((propInfo == null) && (fieldInfo == null))
			{
				if (source != null)
				{
					Debug.LogError("The PLProperty on " + source.name + " couldn't find any property or field named " + propertyName + " on " + targetComponent.name);
				}                
				return null;
			}

			if (scriptable == null)
			{
				if (TargetProperty.MemberType == MemberTypes.Property)
				{
					TargetProperty.MemberPropertyInfo = targetComponent.GetType().GetProperty(TargetProperty.MemberName);
					TargetProperty.PropertyType = TargetProperty.MemberPropertyInfo.PropertyType;
				}
				else if (TargetProperty.MemberType == MemberTypes.Field)
				{
					TargetProperty.MemberFieldInfo = targetComponent.GetType().GetField(TargetProperty.MemberName);
					TargetProperty.PropertyType = TargetProperty.MemberFieldInfo.FieldType;
				}
			}
			else
			{
				TargetProperty.MemberFieldInfo = scriptable.GetType().GetField(TargetProperty.MemberName);
				TargetProperty.PropertyType = TargetProperty.MemberFieldInfo.FieldType;
			}                       

			return TargetProperty;
		}
	}
}