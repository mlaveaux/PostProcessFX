using PostProcessFX.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;
using static UnityStandardAssets.CinematicEffects.AmbientOcclusion;

namespace PostProcessFX.EffectMenu
{
    class AmbientOcclusionEffect : IEffectMenu
    {
        private AmbientOcclusion m_component;
        private AmbientOcclusionConfig m_activeConfig;

        private static String configFilename = "PostProcessFX_ambientocclusion_config.xml";

        public AmbientOcclusionEffect()
        {
            load();
        }
        
        public void disable()
        {
            if (m_component != null)
            {
                MonoBehaviour.DestroyImmediate(m_component);
                m_component = null;
            }
        }

        public void onGUI(float x, float y)
        {
            if (GUI.Button(new Rect(x, y, 75, 20), "Default"))
            {
                m_activeConfig = new AmbientOcclusionConfig();
            }

            if (GUI.Button(new Rect(x + 75, y, 75, 20), "Load"))
            {
                load();
            }
            y += 25;

            m_activeConfig.enabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.enabled, "enable ambient occlusion");
            y += 25;

            m_activeConfig.intensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 4.0f, "Intensity", m_activeConfig.intensity);
            y += 25;

            m_activeConfig.radius = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "Radius", m_activeConfig.radius);
            y += 25;

            m_activeConfig.sampleCount = (SampleCount)PPFXUtility.drawIntSliderWithLabel(x, y, 0, (int)SampleCount.Custom - 1, "Sample Count: ", m_activeConfig.getSampleCountLabel(), (int)m_activeConfig.sampleCount);
            y += 25;

            m_activeConfig.source = (OcclusionSource)PPFXUtility.drawIntSliderWithLabel(x, y, 0, 1, "Occlusion Source: ", m_activeConfig.getOcclusionSourceLabel(), (int)m_activeConfig.source);
            y += 25;

            m_activeConfig.downsampling = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.downsampling, "Enable downsampling");
            y += 25;

            // Apply the changed configuration.
            applyConfig();
        }

        public void save()
        {
            ConfigUtility.Serialize<AmbientOcclusionConfig>(configFilename, m_activeConfig);
        }

        public void load()
        {
            m_activeConfig = ConfigUtility.Deserialize<AmbientOcclusionConfig>(configFilename);
            if (m_activeConfig == null)
            {
                m_activeConfig = new AmbientOcclusionConfig();
            }

            // Apply the loaded config.
            applyConfig();
        }
        
        private void applyConfig()
        {
            if (m_activeConfig.enabled)
            {
                // Find any existing ambient occlusion.
                if (m_component == null)
                {
                    m_component = ModDescription.camera.GetComponent<AmbientOcclusion>();
                }

                // Add it if it was not available.
                if (m_component == null)
                {
                    m_component = ModDescription.camera.AddComponent<AmbientOcclusion>();
                    if (m_component == null)
                    {
                        m_activeConfig.enabled = false;
                        throw new Exception("Couldn't add AmbientOcclusion to Camera.");
                    }
                }

                m_component.settings.intensity = m_activeConfig.intensity;
                m_component.settings.radius = m_activeConfig.radius;
                m_component.settings.sampleCount = m_activeConfig.sampleCount;
                m_component.settings.occlusionSource = m_activeConfig.source;
                m_component.settings.downsampling = m_activeConfig.downsampling;
                m_component.enabled = true;
            }
            else
            {
                disable();
            }
        }
    }
}
