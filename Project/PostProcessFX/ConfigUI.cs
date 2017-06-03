using System;

using UnityEngine;
using PostProcessFX.Config;
using PostProcessFX.EffectMenu;

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
        public const String configFilename = "PostProcessFX_gui_config.xml";

        private String m_toggleKeyString;

        private BloomEffect m_bloom;
        private AntiAliasingEffect m_antiAliasing;
        private AmbientOcclusionEffect m_ambientOcclusion;

        private GUIConfig m_config = null;
        private MenuType m_activeMenu;

        private bool m_toggle = false;
        private bool m_dragging = false;
        private float m_lastMouseX;
        private float m_lastMouseY;

        // This is actually the parameter from ConfigUI, but as that isn't possible we just
        // initialize it and call Initialize.
        public AssetBundle assetBundle;

        public ConfigUI()
        {
            //  Load the GUIConfig or otherwise return to default settings
            m_config = ConfigUtility.Deserialize<GUIConfig>(configFilename);
            if (m_config == null)
            {
                m_config = GUIConfig.getDefaultConfig();
            }
            
            // Obtain the UI toggle key
            m_toggleKeyString = Enum.GetName(typeof(KeyCode), m_config.toggleUIKey);

            // Save the settings at least once
            save();
        }

        public void Initialize()
        {
            // Create effects from the assetbundle, they should read the required assets by themselves.
            m_bloom = new BloomEffect(assetBundle);
            m_antiAliasing = new AntiAliasingEffect(assetBundle);
            m_ambientOcclusion = new AmbientOcclusionEffect();
        }
        
        public void OnGUI()
        {
            if (!m_config.active) { return; }
            
            float x = m_config.menuPositionX;
            float y = m_config.menuPositionY;

            GUI.Box(new Rect(x, y, 320, 400), "");

            if (GUI.Button(new Rect(x, y, 300, 20), ModDescription.ModName + " UI " + ModDescription.VersionString))
            {
                m_dragging = true;
            }

            if (GUI.Button(new Rect(x + 300, y, 20, 20), "X"))
            {
                m_config.active = false;
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

            if (GUI.Button(new Rect(x + 120, y, 30, 20), "AA"))
            {
                m_activeMenu = MenuType.AntiAliasing;
            }
            
            if (GUI.Button(new Rect(x + 150, y, 30, 20), "AO"))
            {
                m_activeMenu = MenuType.SSAO;
            }

            y += 25;

            // Select what sub-menu to show depending on the active menu.
            switch (m_activeMenu)
            {
                case MenuType.Global:
                    GUI.Label(new Rect(x, y, 200, 20), "Toggle Key: ");
                    m_toggleKeyString = GUI.TextArea(new Rect(x + 150, y, 150, 20), m_toggleKeyString);
                    y += 25;

                    m_config.ctrlKey = GUI.Toggle(new Rect(x, y, 50, 20), m_config.ctrlKey, "ctrl");
                    m_config.shiftKey = GUI.Toggle(new Rect(x + 60, y, 50, 20), m_config.shiftKey, "shift");
                    m_config.altKey = GUI.Toggle(new Rect(x + 120, y, 50, 20), m_config.altKey, "alt");

                    try
                    {
                        m_config.toggleUIKey = (int)Enum.Parse(typeof(KeyCode), m_toggleKeyString);
                    }
                    catch (Exception)
                    {
                        // Silently ignore this exception, because it just means that it cannot be converted to a keycode.
                        m_config.toggleUIKey = (int)KeyCode.F9;
                        m_toggleKeyString = "F9";
                    }
                    break;

                case MenuType.AntiAliasing:
                    if (m_antiAliasing != null)
                    {
                        m_antiAliasing.onGUI(x, y);
                    }
                    break;

                case MenuType.Bloom:
                    if (m_bloom != null)
                    {
                        m_bloom.onGUI(x, y);
                    }
                    break;

                case MenuType.Motionblur:
                    /*if (m_motionblur != null)
                    {
                        m_motionblur.onGUI(x, y);
                    }*/
                    break;

                case MenuType.SSAO:
                    if (m_ambientOcclusion != null)
                    {
                        m_ambientOcclusion.onGUI(x, y);
                    }
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
                    m_config.active = !m_config.active;
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
            save();

            // Destroy the other components that might have assets loaded
            if (m_bloom != null) m_bloom.disable();
            if (m_antiAliasing != null) m_antiAliasing.disable();
            if (m_ambientOcclusion != null) m_ambientOcclusion.disable();
        }

        public void OnMouseUp()
        {
            m_dragging = false;
        }

        private void save()
        {
            // For some reason there can be an exception in the constructor, but the object is still created.
            PPFXUtility.log("PostProcessFX: Saving settings.");
            if (m_bloom != null ) m_bloom.save();
            if (m_antiAliasing != null) m_antiAliasing.save();
            if (m_ambientOcclusion != null) m_ambientOcclusion.save();

            ConfigUtility.Serialize<GUIConfig>(configFilename, m_config);
        }
    }
}
