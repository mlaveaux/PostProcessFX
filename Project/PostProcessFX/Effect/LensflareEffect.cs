using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ColossalFramework;

namespace PostProcessFX
{
	class LensflareEffect
	{
		private Flare m_sunflare = null;

		public LensflareEffect()
		{
			enable();
		}

		~LensflareEffect()
		{
			disable();
		}

		public void enable()
		{
			if (m_sunflare == null)
			{
				Light[] lights = GameObject.FindObjectsOfType<Light>();

				foreach (Light light in lights)
				{
					if (light.type == LightType.Directional)
					{
						m_sunflare = (Flare)Resources.Load("50mmZoom.flare");
						if (m_sunflare == null)
						{
							PPFXUtility.log("LensflareEffect: Could not load 50mmZoom.flare");
						} 
						else
						{
							m_sunflare = new Flare();
						}

						light.flare = m_sunflare;
					}
				}
			}
		}

		public void disable()
		{
			if (m_sunflare != null)
			{
				MonoBehaviour.DestroyImmediate(m_sunflare);
			}
		}
	}
}
