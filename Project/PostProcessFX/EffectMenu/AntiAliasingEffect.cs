using System;

using UnityEngine;

using PostProcessFX.Config;
using PostProcessFX.EffectMenu;
using UnityStandardAssets.CinematicEffects;

// Nice namespaces, automatically poluted by some random SMAA export.
using static UnityStandardAssets.CinematicEffects.SMAA;

namespace PostProcessFX
{
    /**
     * Enables and configures various anti aliasing post processing effects
     * by adding the required shaders and applying the config.
     */
    class AntiAliasingEffect : IEffectMenu
    {
        private AntiAliasing         m_antiAliasingComponent = null;
        private CineTemporalAntiAliasing m_temporalComponent = null;
        private AntiAliasingConfig m_activeConfig = null;

        // Shaders loaded from the asset bundle

        private static String configFilename = "PostProcessFX_aa_config.xml";
        
        public AntiAliasingEffect()
        {
            m_activeConfig = ConfigUtility.Deserialize<AntiAliasingConfig>(configFilename);
            if (m_activeConfig == null)
            {
                m_activeConfig = AntiAliasingConfig.getDefaultPreset();
            }

            applyConfig();
        }
        
        public void save()
        {
            ConfigUtility.Serialize<AntiAliasingConfig>(configFilename, m_activeConfig);
        }

        public void disable()
        {
            if (m_antiAliasingComponent != null)
            {
                MonoBehaviour.DestroyImmediate(m_antiAliasingComponent);
                m_antiAliasingComponent = null;
            }
        }

        public void onGUI(float x, float y)
        {
            if (GUI.Button(new Rect(x, y, 75, 20), "Default"))
            {
                m_activeConfig = AntiAliasingConfig.getDefaultPreset();
            }

            if (GUI.Button(new Rect(x + 75, y, 75, 20), "Load"))
            {
                m_activeConfig = ConfigUtility.Deserialize<AntiAliasingConfig>(configFilename);
                if (m_activeConfig == null)
                {
                    m_activeConfig = AntiAliasingConfig.getDefaultPreset();
                }
            }
            y += 25;
            
            m_activeConfig.mode = (AntiAliasingMode)PPFXUtility.drawIntSliderWithLabel(x, y, 0, (int)AntiAliasingMode.Maximum - 2, "Mode: ", m_activeConfig.getLabelFromMode(), (int)m_activeConfig.mode);
            y += 25;
                        
            if (m_activeConfig.mode == AntiAliasingMode.FXAA)
            {
                m_activeConfig.fxaaQuality = PPFXUtility.drawIntSliderWithLabel(x, y, 0, 4, m_activeConfig.getLabelFromFxaaQuality(), m_activeConfig.fxaaQuality);
                y += 25;
            }

            if (m_activeConfig.mode == AntiAliasingMode.SMAA)
            {
                m_activeConfig.smaaQuality = (QualityPreset)PPFXUtility.drawIntSliderWithLabel(x, y, 0, (int)QualityPreset.Custom - 1, m_activeConfig.getLabelFromSmaaQuality(), (int)m_activeConfig.smaaQuality);
                y += 25;

                m_activeConfig.smaaTemporal = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.smaaTemporal, "Enable temporal");
                y += 25;
                
                //m_activeConfig.smaaPredication = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.smaaPredication, "enable predication");
            }
            
            applyConfig();
        }

        private void applyConfig()
        {
            if (m_activeConfig.mode == 0)
            {
                disable();
            }
            else if (m_activeConfig.mode == AntiAliasingMode.SMAA || m_activeConfig.mode == AntiAliasingMode.FXAA)
            {
                // First try to get it from the camera.
                if (m_antiAliasingComponent == null)
                {
                    m_antiAliasingComponent = ModDescription.camera.GetComponent<AntiAliasing>();
                }

                // If that fails then just add it
                if (m_antiAliasingComponent == null)
                {
                    m_antiAliasingComponent = ModDescription.camera.AddComponent<AntiAliasing>();
                    if (m_antiAliasingComponent == null)
                    {
                        m_activeConfig.mode = 0;
                        throw new Exception("AntiAliasingEffect: Couldn't add Antialiasing to Camera.");
                    }
                }
                
                m_antiAliasingComponent.enabled = true;

                switch (m_activeConfig.mode)
                {
                    case AntiAliasingMode.FXAA:
                        // Enabled the FXAA settings.
                        m_antiAliasingComponent.m_FXAA.preset = FXAA.availablePresets[m_activeConfig.fxaaQuality];
                        m_antiAliasingComponent.method = (int)AntiAliasing.Method.Fxaa;
                        break;
                    case AntiAliasingMode.SMAA:
                        // Enable the required SMAA settings
                        m_antiAliasingComponent.m_SMAA.settings.quality = m_activeConfig.smaaQuality;
                        m_antiAliasingComponent.m_SMAA.settings.edgeDetectionMethod = m_activeConfig.smaaEdgeMethod;
                        m_antiAliasingComponent.m_SMAA.temporal.enabled = m_activeConfig.smaaTemporal;
                        m_antiAliasingComponent.m_SMAA.predication.enabled = m_activeConfig.smaaPredication;
                        m_antiAliasingComponent.m_SMAA.predication.strength = 0.5f;

                        m_antiAliasingComponent.method = (int)AntiAliasing.Method.Smaa;
                        break;
                }
            }
            else
            {
                m_temporalComponent = Camera.main.GetComponent<CineTemporalAntiAliasing>();
                if (m_temporalComponent == null)
                {
                    m_temporalComponent = Camera.main.gameObject.AddComponent<CineTemporalAntiAliasing>();
                    if (m_temporalComponent == null)
                    {
                        m_activeConfig.mode = 0;
                        throw new Exception("AntiAliasingEffect: Couldn't add TemporalAntiAliasing to Camera.");
                    }
                }

                m_temporalComponent.enabled = true;

                m_temporalComponent.settings.jitterSettings.sampleCount = 2;
                m_temporalComponent.settings.jitterSettings.spread = 0.05f;

                m_temporalComponent.settings.sharpenFilterSettings.amount = 0.0f;

                //m_temporalComponent.settings.blendSettings.motionAmplification = 70.0f;
                //m_temporalComponent.settings.blendSettings.stationary = 1.0f;
                //m_temporalComponent.settings.blendSettings.moving = 0.0f;
            }
        }
    }
}
