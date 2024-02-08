using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpectralDepths.Tools
{
	public class PLObjectPool : MonoBehaviour
	{
		[PLReadOnly]
		public List<GameObject> PooledGameObjects;
	}
}