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
		public float menuPositionX = 10.0f;
		public float menuPositionY = 100.0f;

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
		private SSAOEffect m_ssaoEffect;
		private ScreenSpaceReflectionEffect m_screenReflection;
		private DepthOfFieldEffect m_dofEffect;

		private bool m_toggle = false;
		
		private EffectConfig m_config = null;
		private MonoBehaviour m_parent;
		private MenuType m_activeMenu;

		private float m_lastMouseX;
		private float m_lastMouseY;

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
			m_motionblur = new MotionblurEffect();
			m_ssaoEffect = new SSAOEffect();
			m_screenReflection = new ScreenSpaceReflectionEffect();
			m_dofEffect = new DepthOfFieldEffect();

			m_toggleUIString = Enum.GetName(typeof(KeyCode), m_config.toggleUIKey);
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
			float x = m_config.menuPositionX;
			float y = m_config.menuPositionY;

			GUI.Box(new Rect(x, y, 580, 340), "");
			x += 10;
			y += 10;

			GUI.Label(new Rect(x, y, 200, 20), "PostProcessFX ControlUI");

			if (GUI.Button(new Rect(x + 200, y, 50, 20), "hide"))
			{
				m_config.guiActive = false;
			}

			y += 25;
			if (GUI.Button(new Rect(x, y, 60, 20), "Global"))
			{
				m_activeMenu = MenuType.Global;
			}

			if (GUI.Button(new Rect(x + 60, y, 60, 20), "Bloom"))
			{
				m_activeMenu = MenuType.Bloom;
			}

			if (GUI.Button(new Rect(x + 120, y, 90, 20), "AntiAliasing"))
			{
				m_activeMenu = MenuType.AntiAliasing;
			}

			if (GUI.Button(new Rect(x + 210, y, 75, 20), "Motionblur"))
			{
				m_activeMenu = MenuType.Motionblur;
			}

			if (GUI.Button(new Rect(x + 285, y, 75, 20), "SSAO"))
			{
				m_activeMenu = MenuType.SSAO;
			}

			if (GUI.Button(new Rect(x + 360, y, 75, 20), "Reflection"))
			{
				m_activeMenu = MenuType.Reflection;
			}

			if (GUI.Button(new Rect(x + 435, y, 90, 20), "DepthOfField"))
			{
				m_activeMenu = MenuType.DepthOfField;
			}

			y += 25;
			switch (m_activeMenu)
			{
				case MenuType.Global:
					GUI.Label(new Rect(x, y, 200, 20), "ToggleUI");
					y += 25;

					m_toggleUIString = GUI.TextArea(new Rect(x, y, 200, 20), m_toggleUIString);
					y += 25;

					m_config.ctrlKey = GUI.Toggle(new Rect(x, y, 50, 20), m_config.ctrlKey, "ctrl");
					m_config.shiftKey = GUI.Toggle(new Rect(x + 60, y, 50, 20), m_config.shiftKey, "shift");
					m_config.altKey = GUI.Toggle(new Rect(x + 120, y, 50, 20), m_config.altKey, "alt");

					try
					{
						m_config.toggleUIKey = (int)Enum.Parse(typeof(KeyCode), m_toggleUIString);
					}
					catch (Exception ex)
					{
						m_config.toggleUIKey = (int)KeyCode.F9;
						m_toggleUIString = "F9";
					}
					break;

				case MenuType.AntiAliasing:
					m_antiAliasing.drawGUI(m_config, x, y);
					m_antiAliasing.applyConfig(m_config);
					break;

				case MenuType.Bloom:
					m_bloom.drawGUI(m_config, x, y);
					m_bloom.applyConfig(m_config);
					break;

				case MenuType.Motionblur:
					m_motionblur.drawGUI(m_config, x, y);
					m_motionblur.applyConfig(m_config);
					break;

				case MenuType.SSAO:
					m_ssaoEffect.drawGUI(m_config, x, y);
					m_ssaoEffect.applyConfig(m_config);
					break;

				case MenuType.Reflection:
					m_screenReflection.drawGUI(m_config, x, y);
					m_screenReflection.applyConfig(m_config);
					break;

				case MenuType.DepthOfField:
					m_dofEffect.drawGUI(m_config, x, y);
					break;
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

		public void OnMouseClick()
		{
			//m_lastMouseX = Input.mousePosition.x;
			//m_lastMouseY = Input.mousePosition.y;
		}

		public void OnMouseDrag()
		{

		}
	}
}
