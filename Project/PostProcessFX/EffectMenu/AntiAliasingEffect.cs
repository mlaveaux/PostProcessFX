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
     * Enables and configures various anti aliasing post processing effects
     * by adding the required shaders and applying the config.
     */
    class AntiAliasingEffect : IEffectMenu
    {
        private Antialiasing m_component = null;
        private AntiAliasingConfig m_activeConfig = null;
        private AntiAliasingConfig m_savedConfig = null;

        // Shaders loaded from the asset bundle
        Shader dlaaShader;
        Shader nfaaMaterial;
        Shader fxaa2Material;
        Shader fxaa3ConsoleMaterial;
        Shader fxaaPreset2Material;
        Shader fxaaPreset3Material;
        Shader ssaaMaterial;

        private static String configFilename = "PostProcessFX_aa_config.xml";
        
        public AntiAliasingEffect(AssetBundle bundle)
        {
            m_component = Camera.main.GetComponent<Antialiasing>();

            dlaaShader = bundle.LoadAsset<Shader>("Shaders/DLAA.shader");
            nfaaMaterial = bundle.LoadAsset<Shader>("Shaders/NFAA.shader");
            fxaa2Material = bundle.LoadAsset<Shader>("Shaders/FXAA2.shader");
            fxaa3ConsoleMaterial = bundle.LoadAsset<Shader>("Shaders/FXAA3Console.shader");
            fxaaPreset2Material = bundle.LoadAsset<Shader>("Shaders/FXAAPreset2.shader");
            fxaaPreset3Material = bundle.LoadAsset<Shader>("Shaders/FXAAPreset3.shader");
            ssaaMaterial = bundle.LoadAsset<Shader>("Shaders/SSAA.shader");

            m_activeConfig = ConfigUtility.Deserialize<AntiAliasingConfig>(configFilename);
            if (m_activeConfig == null)
            {
                m_activeConfig = AntiAliasingConfig.getDefaultPreset();
            }

            applyConfig();
        }

        ~AntiAliasingEffect()
        {
            disable();
        }

        public void save()
        {
            ConfigUtility.Serialize<AntiAliasingConfig>(configFilename, m_activeConfig);
            m_savedConfig = m_activeConfig;
        }

        public void onGUI(float x, float y)
        {
            float currentX = x;
            float currentY = y;

            if (GUI.Button(new Rect(x, currentY, 75, 20), "Default"))
            {
                m_activeConfig = AntiAliasingConfig.getDefaultPreset();
            }

            if (GUI.Button(new Rect(x + 75, currentY, 75, 20), "Load"))
            {
                m_activeConfig = ConfigUtility.Deserialize<AntiAliasingConfig>(configFilename);
                if (m_activeConfig == null)
                {
                    m_activeConfig = AntiAliasingConfig.getDefaultPreset();
                }
            }
            currentY += 25;

            GUI.Label(new Rect(currentX, currentY, 200, 20), "Anti Aliasing Mode: ");
            currentY += 25;

            m_activeConfig.mode = PPFXUtility.drawIntSliderWithLabel(currentX, currentY, 0, 7,
                getAntiAliasingType(m_activeConfig.mode), m_activeConfig.mode);
            currentY += 25;

            if (m_activeConfig.mode != 0)
            {
                switch ((AAMode)(m_activeConfig.mode - 1))
                {
                    case AAMode.FXAA3Console:
                        m_activeConfig.FXAA3minThreshhold = PPFXUtility.drawSliderWithLabel(currentX, currentY, 0.0f, 1.0f, "min edge threshhold", m_activeConfig.FXAA3minThreshhold);
                        currentY += 25;

                        m_activeConfig.FXAA3maxThreshhold = PPFXUtility.drawSliderWithLabel(currentX, currentY, 0.0f, 1.0f, "max edge threshhold", m_activeConfig.FXAA3maxThreshhold);
                        currentY += 25;

                        m_activeConfig.FXAA3sharpness = PPFXUtility.drawSliderWithLabel(currentX, currentY, 0.0f, 4.0f, "sharpness", m_activeConfig.FXAA3sharpness);
                        currentY += 25;
                        break;

                    case AAMode.NFAA:
                        m_activeConfig.NFAAoffset = PPFXUtility.drawSliderWithLabel(currentX, currentY, 0.0f, 1.0f, "edge offset", m_activeConfig.NFAAoffset);
                        currentY += 25;

                        m_activeConfig.NFAAblurRadius = PPFXUtility.drawSliderWithLabel(currentX, currentY, 0.0f, 32.0f, "blur radius", m_activeConfig.NFAAblurRadius);
                        currentY += 25;
                        break;

                    case AAMode.DLAA:
                        m_activeConfig.DLAAsharp = GUI.Toggle(new Rect(currentX, currentY, 100, 20), m_activeConfig.DLAAsharp, "sharp");
                        currentY += 25;
                        break;
                }
            }

            applyConfig();
        }

        private void applyConfig()
        {
            if (m_activeConfig.mode == 0)
            {
                disable();
            }
            else
            {
                enable();

                m_component.mode = (AAMode)m_activeConfig.mode - 1;
                m_component.blurRadius = m_activeConfig.NFAAblurRadius;
                m_component.dlaaSharp = m_activeConfig.DLAAsharp;

                m_component.edgeSharpness = m_activeConfig.FXAA3sharpness;
                m_component.edgeThreshold = m_activeConfig.FXAA3maxThreshhold;
                m_component.edgeThresholdMin = m_activeConfig.FXAA3minThreshhold;
            }
        }

        private void enable()
        {
            if (m_component == null)
            {
                m_component = Camera.main.gameObject.AddComponent<Antialiasing>();
                if (m_component == null)
                {
                    PPFXUtility.log("AntiAliasingEffect: Could not add AntialiasingAsPostEffect to Camera.");
                }
                else
                {
                    m_component.enabled = false;


                    m_component.nfaaShader = nfaaMaterial;
                    m_component.dlaaShader = dlaaShader;
                    m_component.shaderFXAAII = fxaa2Material;
                    m_component.shaderFXAAIII = fxaa3ConsoleMaterial;
                    m_component.shaderFXAAPreset2 = fxaaPreset2Material;
                    m_component.shaderFXAAPreset3 = fxaaPreset3Material;
                    m_component.ssaaShader = ssaaMaterial;
                }
            }

            m_component.enabled = true;
        }

        private void disable()
        {
            if (m_component != null)
            {
                MonoBehaviour.DestroyImmediate(m_component);
                m_component = null;
            }
        }

        private String getAntiAliasingType(int mode)
        {
            switch (mode)
            {
                case 1:
                    return "FXAA2";
                case 2:
                    return "FXAA3Console";
                case 3:
                    return "FXAAPresetA";
                case 4:
                    return "FXAAPresetB";
                case 5:
                    return "NFAA";
                case 6:
                    return "SSAA";
                case 7:
                    return "DLAA";
                case 8:
                    return "SMAA";
                default:
                    return "Disabled";
            }
        }
    }
}
