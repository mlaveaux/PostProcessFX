using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PostProcessFX.Config;
using PostProcessFX.EffectMenu;

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace PostProcessFX
{
    /**
     * Enables and configures the bloom and lensflare effect using the
     * already added Bloom class of Main Camera.
     */
    class BloomEffect : IEffectMenu
    {
        private Bloom m_component = null;
        private LensflareEffect m_lensflare = null;

        // Shaders loaded from the assetbundle
        Shader lensFlareCreate;
        Shader blendForBloom;
        Shader blurAndFlares;
        Shader brightPass2;

        private BloomConfig m_activeConfig; // The configuration that is being used.

        private static String configFilename = "PostProcessFX_bloom_config.xml";

        /**
         * Create a new bloom effect menu with or without an existing config.
         */
        public BloomEffect(AssetBundle bundle)
        {
            // Get the existing component if it exists
            m_component = Camera.main.GetComponent<Bloom>();
            m_lensflare = new LensflareEffect(bundle);

            // Load shaders from the asset bundle
            lensFlareCreate = PPFXUtility.checkAndLoadAsset<Shader>(bundle, "Assets/Shaders/Bloom/LensFlareCreate.shader");
            blendForBloom = PPFXUtility.checkAndLoadAsset<Shader>(bundle, "Assets/Shaders/Bloom/BlendForBloom.shader");
            blurAndFlares = PPFXUtility.checkAndLoadAsset<Shader>(bundle, "Assets/Shaders/Bloom/BlurAndFlares.shader");
            brightPass2 = PPFXUtility.checkAndLoadAsset<Shader>(bundle, "Assets/Shaders/Bloom/BrightPassFilter2.shader");            

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

            m_activeConfig.intensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "bloomIntensity", m_activeConfig.intensity);
            y += 25;

            m_activeConfig.threshhold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "bloomThreshhold", m_activeConfig.threshhold);
            y += 25;

            m_activeConfig.blurSpread = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 5.0f, "bloomSpread", m_activeConfig.blurSpread);
            y += 25;

            m_activeConfig.blurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 8, "bloomBlurIterations", m_activeConfig.blurIterations);
            y += 25;

            m_activeConfig.lensflareEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareEnabled, "enable lensflare.");
            y += 25;

            m_activeConfig.lensflareIntensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "lensflareIntensity", m_activeConfig.lensflareIntensity);
            y += 25;

            m_activeConfig.lensflareThreshhold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 0.25f, "lensflareThreshhold", m_activeConfig.lensflareThreshhold);
            y += 25;

            m_activeConfig.lensflareSaturation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "lensflareSaturation", m_activeConfig.lensflareSaturation);
            y += 25;

            m_activeConfig.lensflareRotation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 3.14f, "lensflareRotation", m_activeConfig.lensflareRotation);
            y += 25;

            m_activeConfig.lensflareStretchWidth = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "lensflareWidth", m_activeConfig.lensflareStretchWidth);
            y += 25;

            m_activeConfig.lensflareBlurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 8, "lensflareBlurIterations", m_activeConfig.lensflareBlurIterations);
            y += 25;

            m_activeConfig.lensflareGhosting = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareGhosting, "Enable ghosting lensflare.");
            y += 25;

            applyConfig();
        }

        private void applyConfig()
        {
            if (m_activeConfig.bloomEnabled)
            {
                enable();

                m_component.bloomIntensity = m_activeConfig.intensity;
                m_component.bloomBlurIterations = m_activeConfig.blurIterations;
                m_component.bloomThreshold = m_activeConfig.threshhold;
                //m_bloomComponent.blurWidth = m_activeConfig.blurWidth;
                m_component.sepBlurSpread = m_activeConfig.blurSpread;

                m_component.flareRotation = m_activeConfig.lensflareRotation;
                m_component.hollyStretchWidth = m_activeConfig.lensflareStretchWidth;
                m_component.hollywoodFlareBlurIterations = m_activeConfig.lensflareBlurIterations;
                m_component.lensflareIntensity = m_activeConfig.lensflareEnabled ? m_activeConfig.lensflareIntensity : 0.0f;
                m_component.lensFlareSaturation = m_activeConfig.lensflareSaturation;
                m_component.lensflareThreshold = m_activeConfig.lensflareThreshhold;
                m_component.lensflareMode = m_activeConfig.lensflareGhosting ? Bloom.LensFlareStyle.Combined : Bloom.LensFlareStyle.Anamorphic;
            }
            else
            {
                disable();
            }

            if (m_activeConfig.lensflareSun)
            {
                m_lensflare.enable();
            }
            else
            {
                m_lensflare.disable();
            }
        }

        public void enable()
        {
            if (m_component == null)
            {
                m_component = Camera.main.gameObject.AddComponent<Bloom>();
                if (m_component == null)
                {
                    PPFXUtility.log("BloomEffect: Could not add component Bloom to Camera.");
                    disable();
                }
                else
                {
                    m_component.lensFlareShader = lensFlareCreate;
                    m_component.screenBlendShader = blendForBloom;
                    m_component.blurAndFlaresShader = blurAndFlares;
                    m_component.brightPassFilterShader = brightPass2;
                    m_activeConfig.bloomEnabled = true;
                }
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