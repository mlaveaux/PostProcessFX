﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using ColossalFramework;
using System.Xml.Serialization;
using System.IO;

namespace PostProcessFX
{
	public class EffectConfig
	{
		public bool guiActive = true;

		// Bloom options.
		public float bloomIntensity = 0.0f;
		public int bloomBlurIterations = 3;

		public float bloomThreshhold = 4.0f;
		public float blurWidth = 1.0f;

		public float flareRotation = 0.0f;
		public float hollyStretchWidth = 0.75f;
		public int hollywoodFlareBlurIterations = 1;

		public float lensflareIntensity = 0.0f;
		public float lensFlareSaturation = 0.5f;
		public float lensflareThreshhold = 5.0f;

		public float sepBlurSpread = 0.3f;

		// Anti aliasing options.
		public int antiAliasingMode = 0;

		// Motion blur options.
		public int motionBlurMode = 0;
		public float motionblurVelocityScale = 0.375f;
		public float motionblurMinVelocity = 0.1f;
		public float motionblurMaxVelocity = 8.0f;

		// SSAO options
		public bool ssaoEnabled = false;
		public float ssaoBlurFilterDistance = 1.25f;
		public float ssaoIntensity = 0.5f;
		public float ssaoRadius = 0.2f;
		public int ssaoBlurFilterIterations = 2;
		public int ssaoDownsample = 0;

		// Global options.
		public int toggleUIKey = (int)KeyCode.F9;

		// Modifier keys for toggling the ui.
		public bool ctrlKey = false;
		public bool shiftKey = false;
		public bool altKey = false;

		public static void Serialize(String filename, object instance)
		{
			TextWriter writer = null;
			try
			{	
				writer = new StreamWriter(filename);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(EffectConfig));
				xmlSerializer.Serialize(writer, instance);
			}
			catch (Exception ex)
			{
				Debug.LogError("EffectConfig: Failed to save config " + ex.Message);
			} 
			finally 
			{
				if (writer != null) {
					writer.Close();
				}
			}
		}

		public static EffectConfig Deserialize(String filename)
		{
			TextReader reader = null;
			try 
			{
				reader = new StreamReader(filename);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(EffectConfig));
				return (EffectConfig)xmlSerializer.Deserialize(reader);
			}
			catch (Exception ex)
			{
				Debug.LogError("EffectConfig: " + ex.Message);
			}			
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}

			return null;
		}
	}

	enum MenuType
	{
		Global,
		Bloom,
		AntiAliasing,
		Motionblur,
		SSAO
	}

	class ConfigUI : MonoBehaviour
	{
		public const String configFilename = "PostProcessFXConfig.xml";
		private String m_toggleUIString;

		private BloomEffect m_bloom;
		private MotionblurEffect m_motionblur;
		private AntiAliasingEffect m_antiAliasing;
		private SunLensflare m_sunLensflare;
		private SSAOEffect m_ssaoEffect;

		private bool m_toggle = false;
		private bool m_antiAliasingDisabled = false;
		private bool m_motionblurDisabled = false;
		private bool m_bloomDisabled = false;
		
		private EffectConfig m_config = null;
		private MonoBehaviour m_parent;
		private MenuType m_activeMenu;

		public ConfigUI()
		{
			m_config = EffectConfig.Deserialize(configFilename);
			if (m_config == null)
			{
				m_config = new EffectConfig();
				EffectConfig.Serialize(configFilename, m_config);
			}

			m_bloom = new BloomEffect();
			m_antiAliasing = new AntiAliasingEffect();
			m_sunLensflare = new SunLensflare();
			m_motionblur = new MotionblurEffect();
			m_ssaoEffect = new SSAOEffect();

			m_toggleUIString = Enum.GetName(typeof(KeyCode), m_config.toggleUIKey);
			
			apply();
		}

		public void setParent(MonoBehaviour parent)
		{
			m_parent = parent;
		}

		public void OnGUI()
		{
			if (m_config == null) { return;	}

			if (m_parent != null)
			{
				if (m_config.guiActive && m_parent.enabled)
				{
					drawGUI();
				}
			}
			else
			{
				if (m_config.guiActive)
				{
					drawGUI();
				}
			}
		}

		public void drawGUI()
		{
			GUI.Label(new Rect(10, 75, 200, 20), "PostProcessFX ControlUI");

			if (GUI.Button(new Rect(210, 75, 50, 20), "hide"))
			{
				m_config.guiActive = false;
			}

			if (GUI.Button(new Rect(10, 100, 75, 20), "Global"))
			{
				m_activeMenu = MenuType.Global;
			}

			if (GUI.Button(new Rect(85, 100, 75, 20), "Bloom"))
			{
				m_activeMenu = MenuType.Bloom;
			}

			if (GUI.Button(new Rect(160, 100, 90, 20), "AntiAliasing"))
			{
				m_activeMenu = MenuType.AntiAliasing;
			}

			if (GUI.Button(new Rect(250, 100, 75, 20), "Motionblur"))
			{
				m_activeMenu = MenuType.Motionblur;
			}

			if (GUI.Button(new Rect(325, 100, 75, 20), "SSAO"))
			{
				m_activeMenu = MenuType.SSAO;
			}

			switch (m_activeMenu)
			{
				case MenuType.Global:
					GUI.Label(new Rect(10, 125, 200, 20), "ToggleUI");
					m_toggleUIString = GUI.TextArea(new Rect(210, 125, 200, 20), m_toggleUIString);

					m_config.ctrlKey = GUI.Toggle(new Rect(10, 150, 50, 20), m_config.ctrlKey, "ctrl");
					m_config.shiftKey = GUI.Toggle(new Rect(70, 150, 50, 20), m_config.shiftKey, "shift");
					m_config.altKey = GUI.Toggle(new Rect(130, 150, 50, 20), m_config.altKey, "alt");
					break;

				case MenuType.AntiAliasing:
					GUI.Label(new Rect(10, 125, 200, 20), "Anti Aliasing Mode: ");
					GUI.Label(new Rect(210, 125, 120, 20), getAntiAliasingType(m_config.antiAliasingMode));
					m_config.antiAliasingMode = (int)GUI.HorizontalSlider(new Rect(10, 150, 100, 20), m_config.antiAliasingMode, 0.0f, 7.1f);
					break;

				case MenuType.Bloom: 
					GUI.Label(new Rect(10, 125, 200, 20), "bloomIntensity");
					m_config.bloomIntensity = GUI.HorizontalSlider(new Rect(210, 125, 100, 20), m_config.bloomIntensity, 0.0f, 2.0f);

					GUI.Label(new Rect(10, 150, 200, 20), "bloomThreshhold");
					m_config.bloomThreshhold = GUI.HorizontalSlider(new Rect(210, 150, 100, 20), m_config.bloomThreshhold, 0.0f, 6.0f);

					GUI.Label(new Rect(10, 175, 200, 20), "flareRotation");
					m_config.flareRotation = GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.flareRotation, 0.0f, 3.14f);

					GUI.Label(new Rect(10, 200, 200, 20), "hollyStretchWidth");
					m_config.hollyStretchWidth = GUI.HorizontalSlider(new Rect(210, 200, 100, 20), m_config.hollyStretchWidth, 0.0f, 5.0f);

					GUI.Label(new Rect(10, 225, 200, 20), "hollywoodFlareBlurIterations");
					m_config.hollywoodFlareBlurIterations = (int)GUI.HorizontalSlider(new Rect(210, 225, 100, 20), m_config.hollywoodFlareBlurIterations, 0.0f, 5.0f);

					GUI.Label(new Rect(10, 250, 200, 20), "lensflareIntensity");
					m_config.lensflareIntensity = GUI.HorizontalSlider(new Rect(210, 250, 100, 20), m_config.lensflareIntensity, 0.0f, 2.0f);

					GUI.Label(new Rect(10, 275, 200, 20), "lensflareThreshhold");
					m_config.lensflareThreshhold = GUI.HorizontalSlider(new Rect(210, 275, 100, 20), m_config.lensflareThreshhold, 0.0f, 10.0f);

					GUI.Label(new Rect(10, 300, 200, 20), "sepBlurSpread");
					m_config.sepBlurSpread = GUI.HorizontalSlider(new Rect(210, 300, 100, 20), m_config.sepBlurSpread, 0.0f, 10.0f);
					break;

				case MenuType.Motionblur:
					GUI.Label(new Rect(10, 125, 200, 20), "Motionblur Mode: ");
					GUI.Label(new Rect(210, 125, 120, 20), getMotionBlurType(m_config.motionBlurMode));
					m_config.motionBlurMode = (int)GUI.HorizontalSlider(new Rect(10, 150, 100, 20), m_config.motionBlurMode, 0.0f, 5.1f);

					GUI.Label(new Rect(10, 175, 200, 20), "Motionblur Scale");
					m_config.motionblurVelocityScale = GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.motionblurVelocityScale, 0.0f, 1.0f);
					break;

				case MenuType.SSAO:
					m_config.ssaoEnabled = GUI.Toggle(new Rect(10, 125, 200, 20), m_config.ssaoEnabled, "enabled");

					GUI.Label(new Rect(10, 150, 200, 20), "ssaoIntensity");
					m_config.ssaoIntensity = GUI.HorizontalSlider(new Rect(210, 150, 100, 20), m_config.ssaoIntensity, 0.0f, 3.0f);

					GUI.Label(new Rect(10, 175, 200, 20), "ssaoRadius");
					m_config.ssaoRadius = GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.ssaoRadius, 0.0f, 5.0f);

					GUI.Label(new Rect(10, 200, 200, 20), "ssaoBlurFilterDistance");
					m_config.ssaoBlurFilterDistance = GUI.HorizontalSlider(new Rect(210, 200, 100, 20), m_config.ssaoBlurFilterDistance, 0.0f, 5.0f);

					GUI.Label(new Rect(10, 225, 200, 20), "ssaoBlurFilterIterations");
					m_config.ssaoBlurFilterIterations = (int)GUI.HorizontalSlider(new Rect(210, 225, 100, 20), m_config.ssaoBlurFilterIterations, 0.0f, 3.1f);

					GUI.Label(new Rect(10, 250, 200, 20), "ssaoDownsample");
					m_config.ssaoDownsample = (int)GUI.HorizontalSlider(new Rect(210, 250, 100, 20), m_config.ssaoDownsample, 0.0f, 1.1f);
					break;
			}
						
			apply();
		}

		public void apply() 
		{
			if (m_antiAliasing.AAComponent != null)
			{
				if (m_config.antiAliasingMode == 0)
				{
					m_antiAliasing.AAComponent.enabled = false;
					m_antiAliasingDisabled = true;
				}
				else if (m_config.antiAliasingMode == 8)
				{
					m_antiAliasing.AAComponent.enabled = false;
					m_antiAliasingDisabled = true;
				}
				else
				{
					if (m_antiAliasingDisabled)
					{
						m_antiAliasing.AAComponent.enabled = true;
						m_antiAliasingDisabled = false;
					}
					m_antiAliasing.AAComponent.mode = (AAMode)m_config.antiAliasingMode - 1;
				}
			}

			if (m_motionblur.motionBlur != null)
			{
				if (m_config.motionBlurMode == 0)
				{
					m_motionblur.motionBlur.enabled = false;
					m_motionblurDisabled = true;
				}
				else
				{
					if (m_motionblurDisabled)
					{
						m_motionblur.motionBlur.enabled = true;
						m_motionblurDisabled = false;
					}

					m_motionblur.motionBlur.filterType = (CameraMotionBlur.MotionBlurFilter)m_config.motionBlurMode - 1;
					m_motionblur.motionBlur.velocityScale = m_config.motionblurVelocityScale;
					m_motionblur.motionBlur.minVelocity = m_config.motionblurMinVelocity;
					m_motionblur.motionBlur.maxVelocity = m_config.motionblurMaxVelocity;
				}
			}

			if (m_bloom.bloomComponent != null)
			{
				m_bloom.bloomComponent.bloomIntensity = m_config.bloomIntensity;
				m_bloom.bloomComponent.bloomBlurIterations = m_config.bloomBlurIterations;
				m_bloom.bloomComponent.bloomThreshhold = m_config.bloomThreshhold;
				m_bloom.bloomComponent.blurWidth = m_config.blurWidth;
				m_bloom.bloomComponent.flareRotation = m_config.flareRotation;
				m_bloom.bloomComponent.hollyStretchWidth = m_config.hollyStretchWidth;
				m_bloom.bloomComponent.hollywoodFlareBlurIterations = m_config.hollywoodFlareBlurIterations;
				m_bloom.bloomComponent.lensflareIntensity = m_config.lensflareIntensity;
				m_bloom.bloomComponent.lensFlareSaturation = m_config.lensFlareSaturation;
				m_bloom.bloomComponent.lensflareThreshhold = m_config.lensflareThreshhold;
				m_bloom.bloomComponent.sepBlurSpread = m_config.sepBlurSpread;

				if (m_bloom.bloomComponent.bloomIntensity < 0.02f)
				{
					m_bloom.bloomComponent.enabled = false;
					m_bloomDisabled = true;
				}
				else
				{
					if (m_bloomDisabled)
					{
						m_bloom.bloomComponent.enabled = true;

					}
				}
			}

			if (m_ssaoEffect.ssaoComponent != null)
			{
				m_ssaoEffect.ssaoComponent.enabled = m_config.ssaoEnabled;
				m_ssaoEffect.ssaoComponent.blurIterations = m_config.ssaoBlurFilterIterations;
				m_ssaoEffect.ssaoComponent.blurFilterDistance = m_config.ssaoBlurFilterDistance;
				m_ssaoEffect.ssaoComponent.intensity = m_config.ssaoIntensity;
				m_ssaoEffect.ssaoComponent.radius = m_config.ssaoRadius;
				m_ssaoEffect.ssaoComponent.downsample = m_config.ssaoDownsample;
			}

			try
			{
				m_config.toggleUIKey = (int)Enum.Parse(typeof(KeyCode), m_toggleUIString);
			} 
			catch(Exception ex) 
			{
				m_config.toggleUIKey = (int)KeyCode.F9;
				m_toggleUIString = "F9";
			}

			//Camera.main.fieldOfView = m_config.fieldOfView;
		}

		public void Update()
		{
			if (m_config == null) { return; }

			bool ctrlMod = false;
			bool altMod = false;
			bool shiftMod = false;

			if (m_config.ctrlKey && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
			{
				ctrlMod = true;
			}

			if (m_config.altKey && (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)))
			{
				altMod = true;
			}

			if (m_config.shiftKey && (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
			{
				shiftMod = true;
			}

			if (Input.GetKeyDown((KeyCode)m_config.toggleUIKey) &&
				m_config.ctrlKey == ctrlMod &&
				m_config.altKey == altMod &&
				m_config.shiftKey == shiftMod) 
			{
				if (!m_toggle)
				{
					m_config.guiActive = !m_config.guiActive;
				}
				m_toggle = true;
			}
			
			if (Input.GetKeyUp((KeyCode)m_config.toggleUIKey))
			{
				m_toggle = false;
			}
		}

		public void OnDestroy()
		{
			EffectConfig.Serialize(configFilename, m_config);
		}

		private String getMotionBlurType(int filter) {
			switch (filter)
			{
				case 1:
					return "CameraMotion";
				case 2:
					return "LocalBlur";
				case 3:
					return "Reconstruction";
				case 4:
					return "ReconstructionDX11";
				case 5:
					return "ReconstructionDisc";
				default:
					return "Disabled";
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
