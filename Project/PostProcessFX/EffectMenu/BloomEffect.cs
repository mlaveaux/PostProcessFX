using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PostProcessFX.Config;
using PostProcessFX.EffectMenu;

using UnityEngine;
using UnityStandardAssets.ImageEffects;
using static UnityStandardAssets.ImageEffects.Bloom;

namespace PostProcessFX
{
    /**
     * Enables and configures the bloom and lensflare effect using the
     * already added Bloom class of Main Camera.
     */
    class BloomEffect : IEffectMenu
    {
        private Bloom m_bloomComponent = null;
        //private LensflareEffect m_lensflare = new LensflareEffect();

        private BloomConfig m_activeConfig; // The configuration that is being used.

        private static String configFilename = "PostProcessFX_bloom_config.xml";

        /**
         * Create a new bloom effect menu with or without an existing config.
         */
        public BloomEffect()
        {
            //m_lensflare = new LensflareEffect();
            load();
            applyConfig();
        }

        public void save()
        {
            ConfigUtility.Serialize<BloomConfig>(configFilename, m_activeConfig);
        }

        public void onGUI(float x, float y)
        {
            if (GUI.Button(new Rect(x, y, 75, 20), "Default"))
            {
                m_activeConfig = new BloomConfig();
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
                load();
            }

            y += 25;
            m_activeConfig.bloomEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.bloomEnabled, "Enable bloom");
            y += 25;

            m_activeConfig.intensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "Intensity", m_activeConfig.intensity);
            y += 25;

            m_activeConfig.threshhold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "Threshold", m_activeConfig.threshhold);
            y += 25;

            m_activeConfig.blurSpread = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "Spread", m_activeConfig.blurSpread);
            y += 25;

            m_activeConfig.blurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 5, "Spread", m_activeConfig.blurIterations);
            y += 25;

            m_activeConfig.lensflareEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareEnabled, "Enable lensflare.");
            y += 25;

            m_activeConfig.lensflareIntensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "Intensity", m_activeConfig.lensflareIntensity);
            y += 25;

            m_activeConfig.lensflareThreshold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "Threshold", m_activeConfig.lensflareThreshold);
            y += 25;

            m_activeConfig.lensflareSaturation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "Saturation", m_activeConfig.lensflareSaturation);
            y += 25;

            m_activeConfig.lensflareRotation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 3.14f, "Rotation", m_activeConfig.lensflareRotation);
            y += 25;

            m_activeConfig.lensflareStretchWidth = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 5.0f, "Width", m_activeConfig.lensflareStretchWidth);
            y += 25;

            m_activeConfig.lensflareBlurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 5, "Blur Iterations", m_activeConfig.lensflareBlurIterations);
            y += 25;

            m_activeConfig.lensFlareStyle = (LensFlareStyle)PPFXUtility.drawIntSliderWithLabel(x, y, 0, 2, "Style", m_activeConfig.getLensFlareStyleLabel(), (int)m_activeConfig.lensFlareStyle);
            y += 25;
            
            //m_activeConfig.lensflareSun = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareSun, "Enable sun lensflare.");
            //y += 25;

            applyConfig();
        }
        
        public void disable()
        {
            if (m_bloomComponent != null)
            {
                MonoBehaviour.DestroyImmediate(m_bloomComponent);
                m_bloomComponent = null;
            }

            m_activeConfig.bloomEnabled = false;
        }

        private void applyConfig()
        {
            if (m_activeConfig.bloomEnabled)
            {
                enable();

                m_bloomComponent.bloomIntensity = m_activeConfig.intensity;
                m_bloomComponent.bloomBlurIterations = m_activeConfig.blurIterations;
                m_bloomComponent.bloomThreshold = m_activeConfig.threshhold;
                m_bloomComponent.sepBlurSpread = m_activeConfig.blurSpread;
                m_bloomComponent.hdr = Bloom.HDRBloomMode.Auto;
                m_bloomComponent.screenBlendMode = Bloom.BloomScreenBlendMode.Screen;

                m_bloomComponent.lensflareIntensity = m_activeConfig.lensflareEnabled ? m_activeConfig.lensflareIntensity : 0.0f;
                m_bloomComponent.flareRotation = m_activeConfig.lensflareRotation;
                m_bloomComponent.hollyStretchWidth = m_activeConfig.lensflareStretchWidth;
                m_bloomComponent.hollywoodFlareBlurIterations = m_activeConfig.lensflareBlurIterations;
                m_bloomComponent.lensFlareSaturation = m_activeConfig.lensflareSaturation;
                m_bloomComponent.lensflareThreshold = m_activeConfig.lensflareThreshold;
                m_bloomComponent.lensflareMode = m_activeConfig.lensFlareStyle;
            }
            else
            {
                disable();
            }

            if (m_activeConfig.lensflareSun)
            {
                //m_lensflare.enable();
            }
            else
            {
                //m_lensflare.disable();
            }
        }

        private void load()
        {
            m_activeConfig = ConfigUtility.Deserialize<BloomConfig>(configFilename);
        }

        private void enable()
        {
            // Find the existing bloom.
            if (m_bloomComponent == null)
            {
                m_bloomComponent = ModDescription.camera.GetComponent<Bloom>();
            }

            if (m_bloomComponent == null)
            {
                m_bloomComponent = ModDescription.camera.AddComponent<Bloom>();
                if (m_bloomComponent == null)
                {
                    throw new Exception("Could not add component Bloom to Camera.");
                }
            }
                
            m_bloomComponent.lensFlareShader = PPFXUtility.checkAndLoadAsset<Shader>(ModDescription.loadedBundle, "LensFlareCreate.shader");
            m_bloomComponent.screenBlendShader = PPFXUtility.checkAndLoadAsset<Shader>(ModDescription.loadedBundle, "BlendForBloom.shader");
            m_bloomComponent.blurAndFlaresShader = PPFXUtility.checkAndLoadAsset<Shader>(ModDescription.loadedBundle, "BlurAndFlares.shader");
            m_bloomComponent.brightPassFilterShader = PPFXUtility.checkAndLoadAsset<Shader>(ModDescription.loadedBundle, "BrightPassFilter2.shader");
            m_bloomComponent.enabled = true;
        }
    }
}