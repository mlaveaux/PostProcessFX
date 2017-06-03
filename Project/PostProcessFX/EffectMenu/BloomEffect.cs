using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PostProcessFX.Config;
using PostProcessFX.EffectMenu;

using UnityEngine;
using UnityStandardAssets.CinematicEffects;

namespace PostProcessFX
{
    /**
     * Enables and configures the bloom and lensflare effect using the
     * already added Bloom class of Main Camera.
     */
    class BloomEffect : IEffectMenu
    {
        private Bloom m_component = null;
        
        private BloomConfig m_activeConfig; // The configuration that is being used.

        private static String configFilename = "PostProcessFX_bloom_config.xml";

        /**
         * Create a new bloom effect menu with or without an existing config.
         */
        public BloomEffect(AssetBundle bundle)
        {
            // Get the existing component if it exists
            //m_lensflare = new LensflareEffect(bundle);
        
            // Read the config for bloom
            m_activeConfig = ConfigUtility.Deserialize<BloomConfig>(configFilename);
            if (m_activeConfig == null)
            {
                m_activeConfig = BloomConfig.getDefaultPreset();
            }

            applyConfig();
        }

        public void save()
        {
            ConfigUtility.Serialize<BloomConfig>(configFilename, m_activeConfig);
        }

        public void onGUI(float x, float y)
        {
            if (GUI.Button(new Rect(x, y, 75, 20), "Low"))
            {
                m_activeConfig = BloomConfig.getLowPreset();
            }

            if (GUI.Button(new Rect(x + 75, y, 75, 20), "Medium"))
            {
                m_activeConfig = BloomConfig.getMediumPreset();
            }

            if (GUI.Button(new Rect(x + 150, y, 75, 20), "High"))
            {
                m_activeConfig = BloomConfig.getHighPreset();
            }

            if (GUI.Button(new Rect(x + 225, y, 75, 20), "Load"))
            {
                m_activeConfig = ConfigUtility.Deserialize<BloomConfig>(configFilename);
                if (m_activeConfig == null)
                {
                    m_activeConfig = BloomConfig.getDefaultPreset();
                }
            }

            y += 25;
            m_activeConfig.bloomEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.bloomEnabled, "enable bloom");
            y += 25;

            m_activeConfig.intensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "Intensity", m_activeConfig.intensity);
            y += 25;

            m_activeConfig.radius = PPFXUtility.drawSliderWithLabel(x, y, 1.0f, 7.0f, "Radius", m_activeConfig.radius);
            y += 25;

            m_activeConfig.threshold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "Threshold", m_activeConfig.threshold);
            y += 25;

            m_activeConfig.softKnee = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "softKnee", m_activeConfig.softKnee);
            y += 25;

            m_activeConfig.highQuality = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.highQuality, "enable high quality");
            y += 25;

            m_activeConfig.antiFlicker = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.antiFlicker, "enable anti flicker");

            applyConfig();
        }

        private void applyConfig()
        {
            if (m_activeConfig.bloomEnabled)
            {
                enable();

                m_component.settings.intensity = m_activeConfig.intensity;
                m_component.settings.radius = m_activeConfig.radius;
                m_component.settings.threshold = m_activeConfig.threshold;
                m_component.settings.softKnee = m_activeConfig.softKnee;
                m_component.settings.highQuality = m_activeConfig.highQuality;
                m_component.settings.antiFlicker = m_activeConfig.antiFlicker;
            }
            else
            {
                disable();
            }
        }

        public void enable()
        {
            // Remove all old bloom components
            Component bloom = Camera.main.GetComponent("Bloom");
            if (bloom != null)
            {
                MonoBehaviour.DestroyImmediate(bloom);
            }

            m_component = Camera.main.GetComponent<Bloom>();
            if (m_component == null)
            {
                m_component = Camera.main.gameObject.AddComponent<Bloom>();
                if (m_component == null)
                {
                    throw new Exception("Could not add component Bloom to Camera.");
                }

                m_activeConfig.bloomEnabled = true;
                m_component.enabled = true;
            }

        }

        public void disable()
        {
            if (m_component != null)
            {
                MonoBehaviour.DestroyImmediate(m_component);
                m_component = null;
            }

            m_activeConfig.bloomEnabled = false;
        }
    }
}