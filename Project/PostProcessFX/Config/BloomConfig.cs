using System;
using static UnityStandardAssets.ImageEffects.Bloom;

namespace PostProcessFX.Config
{
    public class BloomConfig
    {
        // Bloom configuration
        public bool bloomEnabled = true;
        public float intensity = 0.5f;
        public float threshhold = 0.8f;
        public int blurIterations = 5;
        //public float blurWidth = 0.0f;
        public float blurSpread = 0.2f;

        // Lensflare configuration.
        public bool lensflareEnabled = false;
        public float lensflareIntensity = 0.2f;
        public float lensflareThreshold = 0.2f;
        public float lensflareStretchWidth = 0.5f;
        public float lensflareSaturation = 0.5f;
        public float lensflareRotation = 0.0f;
        public int lensflareBlurIterations = 1;
        public LensFlareStyle lensFlareStyle = LensFlareStyle.Anamorphic;

        public Boolean lensflareSun = false;
        
        public static BloomConfig getMediumPreset()
        {
            BloomConfig preset = new BloomConfig();

            preset.bloomEnabled = true;
            preset.intensity = 1.0f;
            preset.threshhold = 0.5f;
            preset.lensflareEnabled = true;

            return preset;
        }

        public static BloomConfig getHighPreset()
        {
            BloomConfig preset = new BloomConfig();

            preset.bloomEnabled = true;
            preset.intensity = 1.1f;
            preset.threshhold = 0.3f;

            preset.lensflareEnabled = true;
            preset.lensflareIntensity = 0.1f;
            preset.lensFlareStyle = LensFlareStyle.Combined;

            return preset;
        }

        public string getLensFlareStyleLabel()
        {
            return lensFlareStyle.ToString();
        }
    }
}
