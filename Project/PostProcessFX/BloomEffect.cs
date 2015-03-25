using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class BloomEffect
	{
		public Bloom bloomComponent = null;

		private bool lastState = false;

		public BloomEffect()
		{			
			bloomComponent = Camera.main.GetComponent<Bloom>();
			if (bloomComponent == null)
			{
				Debug.LogError("BloomEffect: The main camera has no component named Bloom.");
			}
		}

		public void Enable()
		{
			if (!lastState)
			{
				bloomComponent.enabled = true;
				lastState = true;
			}
		}

		public void Disable()
		{
			if (lastState)
			{
				bloomComponent.enabled = false;
				lastState = false;
			}
		}

		public void Cleanup()
		{

		}
	}
}
