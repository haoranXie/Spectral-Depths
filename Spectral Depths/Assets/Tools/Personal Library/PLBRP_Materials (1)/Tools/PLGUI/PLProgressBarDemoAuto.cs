using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{	
	public class PLProgressBarDemoAuto : MonoBehaviour 
	{
		public enum TestModes { Permanent, OneTime }
		public TestModes TestMode = TestModes.Permanent;

		[PLEnumCondition("TestMode", (int)TestModes.Permanent)]
		public float CurrentValue = 0f;
		[PLEnumCondition("TestMode", (int)TestModes.Permanent)]
		public float MinValue = 0f;
		[PLEnumCondition("TestMode", (int)TestModes.Permanent)]
		public float MaxValue = 100f;
		[PLEnumCondition("TestMode", (int)TestModes.Permanent)]
		public float Speed = 1f;

		[PLEnumCondition("TestMode", (int)TestModes.OneTime)]
		public float OneTimeNewValue;
		[PLEnumCondition("TestMode", (int)TestModes.OneTime)]
		public float OneTimeMinValue;
		[PLEnumCondition("TestMode", (int)TestModes.OneTime)]
		public float OneTimeMaxValue;
		[PLEnumCondition("TestMode", (int)TestModes.OneTime)]
		[PLInspectorButton("OneTime")]
		public bool OneTimeButton;

		protected float _direction = 1f;
		protected PLProgressBar _progressBar;

		protected virtual void Start()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			_progressBar = GetComponent<PLProgressBar> ();
		}

		protected virtual void Update()
		{
			if (TestMode == TestModes.Permanent)
			{
				_progressBar.UpdateBar(CurrentValue, MinValue, MaxValue);
				CurrentValue += Speed * Time.deltaTime * _direction;
				if ((CurrentValue <= MinValue) || (CurrentValue >= MaxValue))
				{
					_direction *= -1;
				}
			}
		}

		protected virtual void OneTime()
		{
			_progressBar.UpdateBar(OneTimeNewValue, OneTimeMinValue, OneTimeMaxValue);
		}
	}
}