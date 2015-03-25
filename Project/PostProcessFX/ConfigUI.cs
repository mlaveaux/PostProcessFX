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

		// Bloom options.
		public float bloomIntensity = 0.0f;
		public int bloomBlurIterations = 3;

		public float bloomThreshhold = 3.5f;
		public float blurWidth = 1.0f;

		public float flareRotation = 0.0f;
		public float hollyStretchWidth = 0.5f;
		public int hollywoodFlareBlurIterations = 1;

		public bool sunLensflare = false;
		public float lensflareIntensity = 0.0f;
		public float lensFlareSaturation = 0.6f;
		public float lensflareThreshhold = 1.4f;

		public float sepBlurSpread = 0.1f;

		// Anti aliasing options.
		public int antiAliasingMode = 0;

		public float FXAA3minThreshhold = 0.1f;
		public float FXAA3maxThreshhold = 0.4f;
		public float FXAA3sharpness = 8;
		public float NFAAoffset = 0.2f;
		public float NFAAblurRadius = 18.0f;
		public bool DLAAsharp = true;

		// Motion blur options.
		public int motionBlurMode = 0;
		public float motionblurVelocityScale = 0.375f;
		public float motionblurMinVelocity = 0.1f;
		public float motionblurMaxVelocity = 8.0f;

		// SSAO options
		public bool ssaoEnabled = false;
		public float ssaoBlurFilterDistance = 1.25f;
		public float ssaoIntensity = 1.5f;
		public float ssaoRadius = 2.0f;
		public int ssaoBlurFilterIterations = 1;
		public int ssaoDownsample = 0;

		// Reflection options
		public bool reflectionEnabled = false;

		public int reflectionIterations = 32;
		public int reflectionBinarySearchIterations = 16;
		public float reflectionScreenEdgeFadeStart = 0.8f;
		public float reflectionEyeFadeStart = 0.0f;
		public float reflectionEyeFadeEnd = 1.0f;
		public int reflectionPixelStride = 32;
		public float reflectionMaxDistance = 20.0f;

		// Depth of field options.
		public bool dofEnabled = false;

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
		SSAO,
		Reflection,
		DepthOfField
	}

	class ConfigUI : MonoBehaviour
	{
		public const String configFilename = "PostProcessFXConfig.xml";
		private String m_toggleUIString;

		private BloomEffect m_bloom;
		private MotionblurEffect m_motionblur;
		private AntiAliasingEffect m_antiAliasing;
		private SunLensflareEffect m_sunLensflare;
		private SSAOEffect m_ssaoEffect;
		private ScreenSpaceReflectionEffect m_screenReflection;
		private DepthOfFieldEffect m_dofEffect;

		private bool m_toggle = false;
		
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
			m_sunLensflare = new SunLensflareEffect();
			m_motionblur = new MotionblurEffect();
			m_ssaoEffect = new SSAOEffect();
			m_screenReflection = new ScreenSpaceReflectionEffect();
			m_dofEffect = new DepthOfFieldEffect();

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

			/*if (GUI.Button(new Rect(325, 100, 75, 20), "SSAO"))
			{
				m_activeMenu = MenuType.SSAO;
			}

			if (GUI.Button(new Rect(400, 100, 75, 20), "Reflection"))
			{
				m_activeMenu = MenuType.Reflection;
			}

			if (GUI.Button(new Rect(475, 100, 90, 20), "DepthOfField"))
			{
				m_activeMenu = MenuType.DepthOfField;
			}*/

			GUI.Box(new Rect(5, 120, 580, 340), "");

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

					if (m_config.antiAliasingMode != 0)
					{						switch ((AAMode)(m_config.antiAliasingMode - 1))
						{
							case AAMode.FXAA3Console:
								GUI.Label(new Rect(210, 175, 120, 20), "min edge threshhold");
								m_config.FXAA3minThreshhold = GUI.HorizontalSlider(new Rect(10, 175, 100, 20), m_config.FXAA3minThreshhold, 0.0f, 1.0f);

								GUI.Label(new Rect(210, 200, 120, 20), "max edge threshhold");
								m_config.FXAA3maxThreshhold = GUI.HorizontalSlider(new Rect(10, 200, 100, 20), m_config.FXAA3maxThreshhold, 0.0f, 1.0f);

								GUI.Label(new Rect(210, 225, 120, 20), "sharpness");
								m_config.FXAA3sharpness = GUI.HorizontalSlider(new Rect(10, 225, 100, 20), m_config.FXAA3sharpness, 5.0f, 20.0f);
								break;

							case AAMode.NFAA:
								GUI.Label(new Rect(210, 175, 120, 20), "edge offset");
								m_config.NFAAoffset = GUI.HorizontalSlider(new Rect(10, 175, 100, 20), m_config.NFAAoffset, 0.0f, 1.0f);

								GUI.Label(new Rect(210, 200, 120, 20), "blur radius");
								m_config.NFAAblurRadius = GUI.HorizontalSlider(new Rect(10, 200, 100, 20), m_config.NFAAblurRadius, 0.0f, 7.1f);
								break;

							case AAMode.DLAA:
								m_config.DLAAsharp = GUI.Toggle(new Rect(10, 175, 100, 20), m_config.DLAAsharp, "sharp");
								break;
						}
					}					
					break;

				case MenuType.Bloom: 
					GUI.Label(new Rect(10, 125, 200, 20), "bloomIntensity");
					m_config.bloomIntensity = GUI.HorizontalSlider(new Rect(210, 125, 100, 20), m_config.bloomIntensity, 0.0f, 2.0f);

					GUI.Label(new Rect(10, 150, 200, 20), "bloomThreshhold");
					m_config.bloomThreshhold = GUI.HorizontalSlider(new Rect(210, 150, 100, 20), m_config.bloomThreshhold, 0.0f, 6.0f);

					GUI.Label(new Rect(10, 175, 200, 20), "bloomBlur");
					m_config.sepBlurSpread = GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.sepBlurSpread, 0.0f, 10.0f);
					
					GUI.Label(new Rect(10, 200, 200, 20), "lensflareIntensity");
					m_config.lensflareIntensity = GUI.HorizontalSlider(new Rect(210, 200, 100, 20), m_config.lensflareIntensity, 0.0f, 2.0f);

					GUI.Label(new Rect(10, 225, 200, 20), "lensflareThreshhold");
					m_config.lensflareThreshhold = GUI.HorizontalSlider(new Rect(210, 225, 100, 20), m_config.lensflareThreshhold, 0.0f, 10.0f);

					GUI.Label(new Rect(10, 250, 200, 20), "lensflareSaturation");
					m_config.lensFlareSaturation = GUI.HorizontalSlider(new Rect(210, 250, 100, 20), m_config.lensFlareSaturation, 0.0f, 10.0f);

					GUI.Label(new Rect(10, 275, 200, 20), "lensflareRotation");
					m_config.flareRotation = GUI.HorizontalSlider(new Rect(210, 275, 100, 20), m_config.flareRotation, 0.0f, 3.14f);

					GUI.Label(new Rect(10, 300, 200, 20), "lensflareWidth");
					m_config.hollyStretchWidth = GUI.HorizontalSlider(new Rect(210, 300, 100, 20), m_config.hollyStretchWidth, 0.0f, 5.0f);
					
					GUI.Label(new Rect(10, 325, 200, 20), "lensflareBlurIterations");
					m_config.hollywoodFlareBlurIterations = (int)GUI.HorizontalSlider(new Rect(210, 325, 100, 20), m_config.hollywoodFlareBlurIterations, 0.0f, 5.0f);

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

				case MenuType.Reflection:
					m_config.reflectionEnabled = GUI.Toggle(new Rect(10, 125, 200, 20), m_config.reflectionEnabled, "enabled");
					
					GUI.Label(new Rect(10, 150, 200, 20), "iterations");
					m_config.reflectionIterations = (int)GUI.HorizontalSlider(new Rect(210, 150, 100, 20), m_config.reflectionIterations, 1.0f, 64.1f);

					GUI.Label(new Rect(10, 175, 200, 20), "binarySearchIterations");
					m_config.reflectionBinarySearchIterations = (int)GUI.HorizontalSlider(new Rect(210, 175, 100, 20), m_config.reflectionBinarySearchIterations, 0.0f, 32.1f);
					
					GUI.Label(new Rect(10, 200, 200, 20), "distance");
					m_config.reflectionMaxDistance = GUI.HorizontalSlider(new Rect(210, 200, 100, 20), m_config.reflectionMaxDistance, 0.0f, 1000.0f);
					
					GUI.Label(new Rect(10, 225, 200, 20), "pixelStride");
					m_config.reflectionPixelStride = (int)GUI.HorizontalSlider(new Rect(210, 225, 100, 20), m_config.reflectionPixelStride, 0.0f, 32.1f);

					GUI.Label(new Rect(10, 250, 200, 20), "eyeFadeStart");
					m_config.reflectionEyeFadeStart = GUI.HorizontalSlider(new Rect(210, 250, 100, 20), m_config.reflectionEyeFadeStart, 0.0f, 1.0f);

					GUI.Label(new Rect(10, 275, 200, 20), "eyeFadeEnd");
					m_config.reflectionEyeFadeEnd = GUI.HorizontalSlider(new Rect(210, 275, 100, 20), m_config.reflectionEyeFadeEnd, 0.0f, 1.0f);
					
					GUI.Label(new Rect(10, 300, 200, 20), "edgeFadeStart");
					m_config.reflectionScreenEdgeFadeStart = GUI.HorizontalSlider(new Rect(210, 300, 100, 20), m_config.reflectionScreenEdgeFadeStart, 0.0f, 1.0f);
					break;

				case MenuType.DepthOfField:
					m_config.dofEnabled = GUI.Toggle(new Rect(10, 125, 200, 20), m_config.dofEnabled, "enabled");
					break;
			}
						
			apply();
		}

		public void apply() 
		{
			if (m_config.antiAliasingMode == 0)
			{
				m_antiAliasing.Disable();
			}
			else if (m_config.antiAliasingMode == 8)
			{
				m_antiAliasing.Disable();
			}
			else
			{
				m_antiAliasing.Enable();
				m_antiAliasing.AAComponent.mode = (AAMode)m_config.antiAliasingMode - 1;
				m_antiAliasing.AAComponent.blurRadius = m_config.NFAAblurRadius;
				m_antiAliasing.AAComponent.dlaaSharp = m_config.DLAAsharp;

				m_antiAliasing.AAComponent.edgeSharpness = m_config.FXAA3sharpness;
				m_antiAliasing.AAComponent.edgeThreshold = m_config.FXAA3maxThreshhold;
				m_antiAliasing.AAComponent.edgeThresholdMin = m_config.FXAA3minThreshhold;
			}

			if (m_config.motionBlurMode == 0)
			{
				m_motionblur.Disable();
			}
			else
			{
				m_motionblur.Enable();

				m_motionblur.motionblurComponent.filterType = (CameraMotionBlur.MotionBlurFilter)m_config.motionBlurMode - 1;
				m_motionblur.motionblurComponent.velocityScale = m_config.motionblurVelocityScale;
				m_motionblur.motionblurComponent.minVelocity = m_config.motionblurMinVelocity;
				m_motionblur.motionblurComponent.maxVelocity = m_config.motionblurMaxVelocity;
			}

			if (m_bloom.bloomComponent.bloomIntensity < 0.02f)
			{
				m_bloom.Disable();
			}
			else
			{
				m_bloom.Enable();

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
			}

			if (m_config.ssaoEnabled)
			{
				m_ssaoEffect.Enable();

				m_ssaoEffect.ssaoComponent.blurIterations = m_config.ssaoBlurFilterIterations;
				m_ssaoEffect.ssaoComponent.blurFilterDistance = m_config.ssaoBlurFilterDistance;
				m_ssaoEffect.ssaoComponent.intensity = m_config.ssaoIntensity;
				m_ssaoEffect.ssaoComponent.radius = m_config.ssaoRadius;
				m_ssaoEffect.ssaoComponent.downsample = m_config.ssaoDownsample;
			}
			else
			{
				m_ssaoEffect.Disable();
			}

			if (m_config.reflectionEnabled)
			{
				m_screenReflection.Enable();

				m_screenReflection.reflectionComponent.binarySearchIterations = m_config.reflectionBinarySearchIterations;
				m_screenReflection.reflectionComponent.eyeFadeEnd = m_config.reflectionEyeFadeEnd;
				m_screenReflection.reflectionComponent.eyeFadeStart = m_config.reflectionEyeFadeStart;
				m_screenReflection.reflectionComponent.iterations = m_config.reflectionIterations;
				m_screenReflection.reflectionComponent.screenEdgeFadeStart = m_config.reflectionScreenEdgeFadeStart;
				m_screenReflection.reflectionComponent.pixelStride = m_config.reflectionPixelStride;
				m_screenReflection.reflectionComponent.maxRayDistance = m_config.reflectionMaxDistance;
			}
			else
			{
				m_screenReflection.Disable();
			}

			if (m_config.dofEnabled)
			{
				m_dofEffect.Enable();
			}
			else
			{
				m_dofEffect.Disable();
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

			m_ssaoEffect.Cleanup();
			m_screenReflection.Cleanup();
			m_motionblur.Cleanup();
			m_bloom.Cleanup();
			m_antiAliasing.Cleanup();
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
