using System;

using ICities;
using UnityEngine;

namespace PostProcessFX
{
    public class ModDescription : IUserMod
    {
        public static string VersionString = "1.7.0.7";
        public static string ModName = "PostProcessFX";

        public string Description
        {
            get { return "Enable bloom, lensflare, motionblur and anti aliasing effects."; }
        }

        public string Name
        {
            get { return ModName; }
        }

        public void OnEnabled()
        {
            PPFXUtility.log(ModName + " " + VersionString + " enabled.");

            // When we are already in the level we can also trigger it here.
            ModLoader.addConfigUI();
        }

        public void OnDisabled()
        {
            PPFXUtility.log(ModName + " " + VersionString + " disabled.");
        }
    }

    public class ModLoader : ILoadingExtension
    {
        public static void addConfigUI()
        {
            GameObject cameraObject = Camera.main.gameObject;
            if (cameraObject != null)
            {
                // Remove any existing config UI that was added by us.
                Component oldUI = cameraObject.GetComponent("ConfigUI");
                if (oldUI != null)
                {
                    PPFXUtility.log("Destroying the old ConfigUI game object.");
                    Component.Destroy(oldUI);
                }

                // Then try to add the new one again.
                ConfigUI newUI = cameraObject.AddComponent<ConfigUI>();
                if (newUI != null)
                {
                    PPFXUtility.log("Added the ConfigUI game object.");
                }                
            }
        }
        
        public void OnLevelLoaded(LoadMode mode)
        {
            // Level is loaded so try to add the config ui to the camera.
            addConfigUI();
        }

        public void OnLevelUnloading()
        { }

        public void OnCreated(ILoading loading) { }

        public void OnReleased()
        { }
    }
}
