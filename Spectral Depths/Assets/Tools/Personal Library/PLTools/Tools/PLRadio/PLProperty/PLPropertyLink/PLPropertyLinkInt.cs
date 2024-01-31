using UnityEngine;
using System;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Int property setter
	/// </summary>
	public class PLPropertyLinkInt : PLPropertyLink
	{
		public Func<int> GetIntDelegate;
		public Action<int> SetIntDelegate;

		protected int _initialValue;
		protected int _newValue;

		/// <summary>
		/// On init we grab our initial value
		/// </summary>
		/// <param name="property"></param>
		public override void Initialization(PLProperty property)
		{
			base.Initialization(property);
			_initialValue = (int)GetPropertyValue(property);
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
					GetIntDelegate = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>),
						firstArgument,
						property.MemberPropertyInfo.GetGetMethod());
				}

				if (property.MemberPropertyInfo.GetSetMethod() != null)
				{
					SetIntDelegate = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>),
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
			SetValueOptimized(property, (int)newValue);
		}

		/// <summary>
		/// Returns this property link's level between 0 and 1
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="property"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public override float GetLevel(PLPropertyEmitter emitter, PLProperty property)
		{
			float returnValue = GetValueOptimized(property);

			returnValue = PLMaths.Clamp(returnValue, emitter.IntRemapMinToZero, emitter.IntRemapMaxToOne, emitter.ClampMin, emitter.ClampMax);
			returnValue = PLMaths.Remap(returnValue, emitter.IntRemapMinToZero, emitter.IntRemapMaxToOne, 0f, 1f);

			emitter.Level = returnValue;
			return returnValue;
		}

		/// <summary>
		/// Sets the specified level
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="property"></param>
		/// <param name="level"></param>
		public override void SetLevel(PLPropertyReceiver receiver, PLProperty property, float level)
		{
			base.SetLevel(receiver, property, level);

			_newValue = (int)PLMaths.Remap(level, 0f, 1f, receiver.IntRemapZero, receiver.IntRemapOne);

			if (receiver.RelativeValue)
			{
				_newValue = _initialValue + _newValue;
			}

			SetValueOptimized(property, _newValue);
		}

		/// <summary>
		/// Gets either the cached value or the raw value
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected virtual int GetValueOptimized(PLProperty property)
		{
			return _getterSetterInitialized ? GetIntDelegate() : (int)GetPropertyValue(property);
		}

		/// <summary>
		/// Sets either the cached value or the raw value
		/// </summary>
		/// <param name="property"></param>
		/// <param name="newValue"></param>
		protected virtual void SetValueOptimized(PLProperty property, int newValue)
		{
			if (_getterSetterInitialized)
			{
				SetIntDelegate(_newValue);
			}
			else
			{
				SetPropertyValue(property, _newValue);
			}
		}
	}
}