using UnityEngine;
using System;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// String property setter
	/// </summary>
	public class PLPropertyLinkString : PLPropertyLink
	{
		public Func<string> GetStringDelegate;
		public Action<string> SetStringDelegate;

		protected string _initialValue;
		protected string _newValue;

		/// <summary>
		/// On initialization we grab our initial value
		/// </summary>
		/// <param name="property"></param>
		public override void Initialization(PLProperty property)
		{
			base.Initialization(property);
			_initialValue = (string)GetPropertyValue(property);
		}

		/// <summary>
		/// Creates cached getter and setters for properties
		/// </summary>
		/// <param name="property"></param>
		public override void CreateGettersAndSetters(PLProperty property)
		{
			base.CreateGettersAndSetters(property);
			if (property.MemberType == PLProperty.MemberTypes.Property)
			{
				object firstArgument = (property.TargetScriptableObject == null) ? (object)property.TargetComponent : (object)property.TargetScriptableObject;

				if (property.MemberPropertyInfo.GetGetMethod() != null)
				{
					GetStringDelegate = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>),
						firstArgument,
						property.MemberPropertyInfo.GetGetMethod());
				}
				if (property.MemberPropertyInfo.GetSetMethod() != null)
				{
					SetStringDelegate = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>),
						firstArgument,
						property.MemberPropertyInfo.GetSetMethod());
				}
				_getterSetterInitialized = true;
			}
		}

		/// <summary>
		/// Gets the raw value of the property, a normalized float value, caching the operation if possible
		/// </summary>
		/// <param name="emitter"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public override object GetValue(PLPropertyEmitter emitter, PLProperty property)
		{
			return GetValueOptimized(property);
		}

		/// <summary>
		/// Sets the raw property value, float normalized, caching the operation if possible
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="property"></param>
		/// <param name="level"></param>
		public override void SetValue(PLPropertyReceiver receiver, PLProperty property, object newValue)
		{
			SetValueOptimized(property, (string)newValue);
		}

		/// <summary>
		/// Sets the level (above threshold : remap one, under threshold : remap zero)
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="property"></param>
		/// <param name="level"></param>
		public override void SetLevel(PLPropertyReceiver receiver, PLProperty property, float level)
		{
			base.SetLevel(receiver, property, level);
			_newValue = (level > receiver.Threshold) ? receiver.StringRemapOne : receiver.StringRemapZero;

			SetValueOptimized(property, _newValue);
		}

		/// <summary>
		/// Gets either the cached value or the raw value
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected virtual string GetValueOptimized(PLProperty property)
		{
			return _getterSetterInitialized ? GetStringDelegate() : (string)GetPropertyValue(property);
		}

		/// <summary>
		/// Sets either the cached value or the raw value
		/// </summary>
		/// <param name="property"></param>
		/// <param name="newValue"></param>
		protected virtual void SetValueOptimized(PLProperty property, string newValue)
		{
			if (_getterSetterInitialized)
			{
				SetStringDelegate(_newValue);
			}
			else
			{
				SetPropertyValue(property, _newValue);
			}
		}
	}
}