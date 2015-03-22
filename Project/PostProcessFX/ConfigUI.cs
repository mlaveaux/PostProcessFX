using System;
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

		public int antiAliasingMode = 0;

		public int motionBlurMode = 0;
		public float motionblurVelocityScale = 0.375f;
		public float motionblurMinVelocity = 0.1f;
		public float motionblurMaxVelocity = 8.0f;

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

	class ConfigUI : MonoBehaviour
	{
		public const String configFilename = "PostProcessFXConfig.xml";

		private BloomEffect m_bloom;
		private MotionblurEffect m_motionblur;
		private AntiAliasingEffect m_antiAliasing;
		private SunLensflare m_sunLensflare;

		private bool m_toggle = false;
		private bool m_antiAliasingDisabled = false;
		private bool m_motionblurDisabled = false;

		private EffectConfig m_config = null;
		private MonoBehaviour m_parent;

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
			GUI.TextArea(new Rect(10, 75, 300, 20), "PostProcessFX controls (Toggle with F11)");

			GUI.TextArea(new Rect(10, 100, 200, 20), "bloomIntensity");
			m_config.bloomIntensity = GUI.HorizontalSlider(new Rect(210, 100, 100, 20), m_config.bloomIntensity, 0.0f, 2.0f);

			GUI.TextArea(new Rect(10, 125, 200, 20), "bloomThreshhold");
			m_config.bloomThreshhold = GUI.HorizontalSlider(new Rect(210, 125, 100, 20), m_config.bloomThreshhold, 0.0f, 6.0f);

			GUI.TextArea(new Rect(10, 150, 200, 20), "flareRotation");
			m_config.flareRotation = GUI.HorizontalSlider(new Rect(210, 150, 100, 20), m_config.flareRotation, 0.0f, 3.14f);

			GUI.TextArea(new Rect(10, 175, 200, 20), "hollyStretchWidth");
			m_config.hollyStretchWidth = GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.hollyStretchWidth, 0.0f, 5.0f);

			GUI.TextArea(new Rect(10, 200, 200, 20), "hollywoodFlareBlurIterations");
			m_config.hollywoodFlareBlurIterations = (int)GUI.HorizontalSlider(new Rect(210, 200, 100, 20), m_config.hollywoodFlareBlurIterations, 0.0f, 5.0f);

			GUI.TextArea(new Rect(10, 225, 200, 20), "lensflareIntensity");
			m_config.lensflareIntensity = GUI.HorizontalSlider(new Rect(210, 225, 100, 20), m_config.lensflareIntensity, 0.0f, 2.0f);

			GUI.TextArea(new Rect(10, 250, 200, 20), "lensflareThreshhold");
			m_config.lensflareThreshhold = GUI.HorizontalSlider(new Rect(210, 250, 100, 20), m_config.lensflareThreshhold, 0.0f, 10.0f);

			GUI.TextArea(new Rect(10, 275, 200, 20), "sepBlurSpread");
			m_config.sepBlurSpread = GUI.HorizontalSlider(new Rect(210, 275, 100, 20), m_config.sepBlurSpread, 0.0f, 10.0f);

			GUI.TextArea(new Rect(10, 300, 200, 20), "Anti Aliasing Mode");
			GUI.TextArea(new Rect(320, 300, 120, 20), getAntiAliasingType(m_config.antiAliasingMode));
			m_config.antiAliasingMode = (int)GUI.HorizontalSlider(new Rect(210, 300, 100, 20), m_config.antiAliasingMode, 0.0f, 7.1f);

			GUI.TextArea(new Rect(10, 325, 200, 20), "Motionblur Mode");
			GUI.TextArea(new Rect(320, 325, 120, 20), getMotionBlurType(m_config.motionBlurMode));
			m_config.motionBlurMode = (int)GUI.HorizontalSlider(new Rect(210, 325, 100, 20), m_config.motionBlurMode, 0.0f, 5.1f);

			GUI.TextArea(new Rect(10, 350, 200, 20), "Motionblur Scale");
			m_config.motionblurVelocityScale = GUI.HorizontalSlider(new Rect(210, 350, 100, 20), m_config.motionblurVelocityScale, 0.0f, 1.0f);

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
				m_bloom.bloomComponent.bloomBlurIterations = m_config.bloomBlurIterations;
				m_bloom.bloomComponent.bloomIntensity = m_config.bloomIntensity;
				m_bloom.bloomComponent.bloomThreshhold = m_config.bloomThreshhold;
				m_bloom.bloomComponent.blurWidth = m_config.blurWidth;
				m_bloom.bloomComponent.flareRotation = m_config.flareRotation;
				m_bloom.bloomComponent.hollyStretchWidth = m_config.hollyStretchWidth;
				m_bloom.bloomComponent.hollywoodFlareBlurIterations = m_config.hollywoodFlareBlurIterations;
				m_bloom.bloomComponent.lensflareIntensity = m_config.lensflareIntensity;
				m_bloom.bloomComponent.lensFlareSaturation = m_config.lensFlareSaturation;
				m_bloom.bloomComponent.lensflareThreshhold = m_config.lensflareThreshhold;
				m_bloom.bloomComponent.sepBlurSpread = m_config.sepBlurSpread;
			}			
		}

		public void Update()
		{
			if (m_config == null) { return; }

			if (Input.GetKeyDown(KeyCode.F11))
			{
				if (!m_toggle)
				{
					m_config.guiActive = !m_config.guiActive;
				}
				m_toggle = true;
			}

			if (Input.GetKeyUp(KeyCode.F11))
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
				default:
					return "Disabled";
			}
		}
	}
}
