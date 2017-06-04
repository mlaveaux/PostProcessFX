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
            m_sunflare = PPFXUtility.checkAndLoadAsset<Flare>(ModDescription.loadedBundle, "50mmZoom.flare");
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
                        if (m_sunflare != null)
                        {
                            light.flare = m_sunflare;
                        }
                    }
                }
            }
        }

        public void disable()
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.flare = null;
                }
            }
        }
    }
}
