using System.Reflection;

using ICities;
using UnityEngine;

using ColossalFramework.Plugins;

namespace PostProcessFX
{
    /**
     * This class initializes the mod, especially loads and destroys the asset bundles.  
     */
    public class ModDescription : IUserMod
    {
        public static string VersionString = "1.7.2-f1.2";
        public static string ModName = "PostProcessFX";

        public static AssetBundle loadedBundle;

        public string Description
        {
            get { return "Enable various postprocessing effects."; }
        }

        public string Name
        {
            get { return ModName; }
        }

        public void OnEnabled()
        {
            PPFXUtility.log(ModName + " " + VersionString + " enabled");
            
            // Load the shader bundle
            string modPath = PluginManager.instance.FindPluginInfo(Assembly.GetAssembly(typeof(ModDescription))).modPath;
            string assetsUri = "file:///" + modPath.Replace("\\", "/") + "/Resources/Windows/postprocessfx";

            WWW www = new WWW(assetsUri);
            // Need to wait until the WWW loading has been done, this blocks the game.
            /*while (!www.isDone)
            {
                System.Threading.Thread.Sleep(1000);
                PPFXUtility.log("Waiting for asset bundle to be loaded...");
            }*/
            
            loadedBundle = www.assetBundle;
            if (loadedBundle == null)
            {
                throw new BrokenAssetException("Assetbundle with uri " + assetsUri + " couldn't be loaded.");
            }
            
            // When we are already in the level we can also trigger it here.
            ModLoader.addConfigUI();
        }

        public void OnDisabled()
        {
            ModLoader.removeConfigUI();

            if (loadedBundle)
            {
                loadedBundle.Unload(true);
                AssetBundle.Destroy(loadedBundle);
                PPFXUtility.log("Unloaded existing asset bundle.");
            }

            PPFXUtility.log(ModName + " " + VersionString + " disabled.");
        }
    }

    public class ModLoader : ILoadingExtension
    {
        /**
         * Add the config UI to the camera object. 
         */
        public static void addConfigUI()
        {
            if (Camera.main == null || Camera.main.gameObject == null) { return; }
            GameObject cameraObject = Camera.main.gameObject;

            removeConfigUI();

            // Then try to add the new one again.
            ConfigUI newUI = cameraObject.AddComponent<ConfigUI>();
            if (newUI != null)
            {
                PPFXUtility.log("Added the ConfigUI game object.");
            }

            newUI.assetBundle = ModDescription.loadedBundle;
            newUI.Initialize();
        }
        
        /**
         * Remove the ConfigUI from the camera if it exists.
         */
         public static void removeConfigUI()
        {
            if (Camera.main == null || Camera.main.gameObject == null) { return; }
            GameObject cameraObject = Camera.main.gameObject;

            // Remove any existing config UI that was added by us.
            Component oldUI = cameraObject.GetComponent("ConfigUI");
            if (oldUI != null)
            {
                Component.Destroy(oldUI);
                PPFXUtility.log("Destroyed the old ConfigUI game object.");
            }
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            // Level is loaded so try to add the config ui to the camera.
            addConfigUI();
        }

        public void OnLevelUnloading()
        {
            removeConfigUI();
        }

        public void OnCreated(ILoading loading) { }

        public void OnReleased()
        { }
    }
}
