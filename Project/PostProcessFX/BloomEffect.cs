using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class BloomEffect
	{
		public Bloom bloomComponent;

		public BloomEffect()
		{
			Camera camera = Camera.main;
			
			bloomComponent = camera.GetComponent<Bloom>();
			if (bloomComponent == null)
			{
				Debug.LogError("BloomEffect: The main camera has no component named Bloom.");
				return;
			}
		}
	}
}
