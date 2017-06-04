using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

namespace PostProcessFX.EffectMenu
{
    class ScreenSpaceReflectionEffect : IEffectMenu
    {
        private ScreenSpaceReflection m_component;
        bool enabled = true;

        public void disable()
        {
            if (m_component != null)
            {
                MonoBehaviour.DestroyImmediate(m_component);
                m_component = null;
            }

            enabled = false;
        }

        public void onGUI(float x, float y)
        {
            enabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), enabled, "enable reflections");
            y += 25;

            applyConfig();
        }

        public void save()
        {

        }

        private void applyConfig()
        {
            if (enabled)
            {
                enable();
            } else
            {
                disable();
            }
        }

        private void enable()
        {
            m_component = Camera.main.GetComponent<ScreenSpaceReflection>();
            if (m_component == null)
            {
                m_component = Camera.main.gameObject.AddComponent<ScreenSpaceReflection>();
                if (m_component == null)
                {
                    throw new Exception("Could not add component ScreenSpaceReflection to Camera.");
                }

                enabled = true;
                m_component.enabled = true;
            }
        }
    }
}
