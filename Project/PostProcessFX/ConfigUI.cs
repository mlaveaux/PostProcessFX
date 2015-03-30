using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using ColossalFramework.UI;

namespace PostProcessFX
{
	/**
	 * The menu that is currently active.
	 */
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

		private String m_toggleKeyString;

		private BloomEffect m_bloom;
		private MotionblurEffect m_motionblur;
		private AntiAliasingEffect m_antiAliasing;
		private SSAOEffect m_ssaoEffect;
		private ScreenSpaceReflectionEffect m_screenReflection;
		private DepthOfFieldEffect m_dofEffect;
				
		private EffectConfig m_config = null;
		private MonoBehaviour m_parent;
		private MenuType m_activeMenu;

		private bool m_toggle = false;
		private bool m_dragging = false;
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

			m_bloom = new BloomEffect(m_config);
			m_antiAliasing = new AntiAliasingEffect(m_config);
			m_motionblur = new MotionblurEffect(m_config);
			m_ssaoEffect = new SSAOEffect(m_config);
			m_screenReflection = new ScreenSpaceReflectionEffect(m_config);
			m_dofEffect = new DepthOfFieldEffect(m_config);
			
			m_toggleKeyString = Enum.GetName(typeof(KeyCode), m_config.toggleUIKey);
		}

		public void setParent(MonoBehaviour parent)
		{
			m_parent = parent;
		}

		public void OnGUI()
		{
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

			GUI.Box(new Rect(x, y, 320, 340), "");

			if (GUI.Button(new Rect(x, y, 300, 20), "PostProcessFX ControlUI"))
			{
				m_dragging = true;
			}

			if (GUI.Button(new Rect(x + 300, y, 20, 20), "X"))
			{
				m_config.guiActive = false;
			}

			x += 10;
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

			if (GUI.Button(new Rect(x + 210, y, 90, 20), "Motionblur"))
			{
				m_activeMenu = MenuType.Motionblur;
			}

			/*if (GUI.Button(new Rect(x + 285, y, 90, 20), "DepthOfField"))
			{
				m_activeMenu = MenuType.DepthOfField;
			}*/

			/*if (GUI.Button(new Rect(x + 285, y, 75, 20), "SSAO"))
			{
				m_activeMenu = MenuType.SSAO;
			}

			if (GUI.Button(new Rect(x + 360, y, 75, 20), "Reflection"))
			{
				m_activeMenu = MenuType.Reflection;
			}*/


			y += 25;
			switch (m_activeMenu)
			{
				case MenuType.Global:
					GUI.Label(new Rect(x, y, 200, 20), "ToggleUI");
					y += 25;

					m_toggleKeyString = GUI.TextArea(new Rect(x, y, 200, 20), m_toggleKeyString);
					y += 25;

					m_config.ctrlKey = GUI.Toggle(new Rect(x, y, 50, 20), m_config.ctrlKey, "ctrl");
					m_config.shiftKey = GUI.Toggle(new Rect(x + 60, y, 50, 20), m_config.shiftKey, "shift");
					m_config.altKey = GUI.Toggle(new Rect(x + 120, y, 50, 20), m_config.altKey, "alt");

					try
					{
						m_config.toggleUIKey = (int)Enum.Parse(typeof(KeyCode), m_toggleKeyString);
					}
					catch (Exception ex)
					{
						m_config.toggleUIKey = (int)KeyCode.F9;
						m_toggleKeyString = "F9";
					}
					break;

				case MenuType.AntiAliasing:
					m_antiAliasing.drawGUI(m_config, x, y);
					break;

				case MenuType.Bloom:
					m_bloom.drawGUI(m_config, x, y);
					break;

				case MenuType.Motionblur:
					m_motionblur.drawGUI(m_config, x, y);
					break;

				case MenuType.SSAO:
					m_ssaoEffect.drawGUI(m_config, x, y);
					break;

				case MenuType.Reflection:
					m_screenReflection.drawGUI(m_config, x, y);
					break;

				case MenuType.DepthOfField:
					m_dofEffect.drawGUI(m_config, x, y);
					break;
			}
		}

		public void Update()
		{
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

			var resizeRect = new Rect(m_config.menuPositionX, m_config.menuPositionY, 320, 20);
			var mouse = Input.mousePosition;
			mouse.y = Screen.height - mouse.y;
						
			if (m_dragging)
			{
				if (!Input.GetMouseButton(0)) 
				{
					m_dragging = false;
				} 
				else 
				{
					m_config.menuPositionX = mouse.x - m_lastMouseX;
					m_config.menuPositionY = mouse.y - m_lastMouseY;
				}
			}
			else
			{
				if (Input.GetMouseButton(0) && resizeRect.Contains(mouse))
				{
					m_dragging = true;
					m_lastMouseX = mouse.x - m_config.menuPositionX;
					m_lastMouseY = mouse.y - m_config.menuPositionY;
				}
			}
		}

		public void OnDestroy()
		{
			EffectConfig.Serialize(configFilename, m_config);
		}

		public void OnMouseUp() 
		{
			m_dragging = false;
		}
	}
}
