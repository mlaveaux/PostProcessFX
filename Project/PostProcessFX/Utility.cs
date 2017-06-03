using System;

using UnityEngine;
using ColossalFramework.Plugins;

namespace PostProcessFX
{
    class PPFXUtility
    {
        public static float drawSliderWithLabel(float x, float y, float min, float max, String label, float configValue)
        {
            GUI.Label(new Rect(x, y, 200, 20), label);
            
            float newValue = GUI.HorizontalSlider(new Rect(x + 200, y + 5, 100, 20), configValue, min, max);

            if (newValue == configValue)
            {
                // The value has not changed, but might edit the text field.
                string value = GUI.TextField(new Rect(x + 150, y, 30, 20), configValue.ToString());

                // Read the value from the text field
                if (float.TryParse(value, out newValue))
                {
                    // Check if new value lies with the ranges
                    newValue = Math.Max(newValue, min);
                    newValue = Math.Min(newValue, max);
                }
            }
            else
            {
                // Just show the new value
                string value = GUI.TextField(new Rect(x + 150, y, 30, 20), configValue.ToString());
            }



            return newValue;
        }

        public static float drawSliderWithLabel(float x, float y, float xsize, float ysize, float min, float max, String label, float configValue)
        {
            GUI.Label(new Rect(x, y, xsize, ysize), label);
            return GUI.HorizontalSlider(new Rect(x + xsize, y, 100, 20), configValue, min, max);
        }

        public static int drawIntSliderWithLabel(float x, float y, int min, int max, String label, int configValue)
        {
            GUI.Label(new Rect(x, y, 200, 20), label);
            return (int)GUI.HorizontalSlider(new Rect(x + 200, y + 5, 100, 20), configValue, min, max);
        }

        public static int drawIntSliderWithLabel(float x, float y, int min, int max, String label, String valueLabel, int configValue)
        {
            GUI.Label(new Rect(x, y, 100, 20), label);
            GUI.Label(new Rect(x + 150, y, 100, 20), valueLabel);

            return (int)GUI.HorizontalSlider(new Rect(x + 200, y + 5, 100, 20), configValue, min, max);
        }

        public static int drawIntSliderWithLabel(float x, float y, float xsize, float ysize, float min, float max, String label, int configValue)
        {
            GUI.Label(new Rect(x, y, xsize, ysize), label);
            return (int)GUI.HorizontalSlider(new Rect(x + xsize, y, 100, 20), configValue, min, max);
        }

        public static void log(object message)
        {
            DebugOutputPanel.AddMessage( PluginManager.MessageType.Message, ModDescription.ModName + ":" + message.ToString());
            Debug.Log(ModDescription.ModName + ":" + message);
        }

        public static void logException(object message, Exception ex)
        {
            log(message);

            Exception inner = ex.InnerException;

            while (inner != null)
            {
                log("Exception: " + inner);
                inner = inner.InnerException;
            }
        }

        /**
         * Check the existence of an asset in the bundle, throws on failure.  
         */
        public static T checkAndLoadAsset<T>(AssetBundle bundle, string name) where T:UnityEngine.Object
        {
            if (bundle == null)
            {
                throw new ArgumentException("Asset bundle is null");
            }

            T asset = bundle.LoadAsset<T>(name);
            if (asset == null)
            {
                throw new ArgumentException("Cannot find " + name + " in provided asset bundle");
            }

            return asset;
        }
    }
}
