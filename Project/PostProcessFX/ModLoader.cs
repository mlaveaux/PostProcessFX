using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICities;
using UnityEngine;
using ColossalFramework.UI;
using System.IO;
using System.Reflection;

namespace PostProcessFX
{
    public class ModDescription : IUserMod
    {
        public string Description
        {
            get { return "Enable bloom, lensflare, motionblur and anti aliasing effects."; }
        }

        public string Name
        {
            get { return "PostProcessFX"; }
        }

        public void OnEnabled()
        {
            PPFXUtility.log("PostProcessFX v1.6.0.2 unpacking shaders...");

            // Take the embedded unitypackage and save it next to the executable.
            Stream embeddedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PostProcessFX.Resources.shaders.unitypackage");
            var fileStream = File.Create("PostProcessFX.unitypackage");
            embeddedStream.Seek(0, SeekOrigin.Begin);
            embeddedStream.CopyTo(fileStream);
            fileStream.Close();
        }

        public void OnDisabled()
        { }
    }

    public class ModLoader : ILoadingExtension
    {
        private ConfigUI m_configUI;

        public void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                UIView view = UIView.GetAView();
                if (view == null)
                {
                    PPFXUtility.log("PostProcessFX: Can't find the UIView component.");
                }
                else
                {
                    m_configUI = view.gameObject.GetComponent<ConfigUI>();
                    if (m_configUI == null)
                    {
                        m_configUI = view.gameObject.AddComponent<ConfigUI>();
                    }

                    m_configUI.setParent(view);
                }
            }
            catch (Exception ex)
            {
                PPFXUtility.log("PostProcessFX: failed to initialize " + ex.Message);
            }
        }

        public void OnLevelUnloading() { }

        public void OnCreated(ILoading loading) { }

        public void OnReleased()
        {
            m_configUI.OnDestroy();
        }
    }
}
