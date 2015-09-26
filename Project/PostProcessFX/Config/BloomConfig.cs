using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessFX.Config
{
    /**
     * The configuration for the Bloom component. 
     */
	public class BloomConfig
	{
		// Bloom configuration
		public bool bloomEnabled = true;
		public float intensity = 0.5f;
		public float threshhold = 0.5f;
		public int blurIterations = 0;
		//public float blurWidth = 0.0f;
		public float blurSpread = 0.1f;

		// Lensflare configuration.
		public bool lensflareEnabled = false;
		public float lensflareIntensity = 0.3f;
		public float lensflareThreshhold = 0.10f;
		public float lensflareStretchWidth = 0.6f;
		public float lensflareSaturation = 0.7f;
		public float lensflareRotation = 0.0f;
		public int lensflareBlurIterations = 8;
        public bool lensflareGhosting = false;

		public bool lensflareSun = false;

		public static BloomConfig getDefaultPreset()
		{
			return new BloomConfig();
		}

		public static BloomConfig getLowPreset()
		{
            BloomConfig preset = getDefaultPreset();
			return preset;
		}

		public static BloomConfig getMediumPreset()
		{
            BloomConfig preset = getLowPreset();           

            preset.intensity = 1.0f;
            preset.threshhold = 0.443181843f;
			preset.blurIterations = 8;
            preset.blurSpread = 1.0f;
			return preset;
		}

		public static BloomConfig getHighPreset()
		{
            BloomConfig preset = getMediumPreset();

            preset.intensity = 2.0f;
            preset.threshhold = 0.5f;
            preset.blurIterations = 2;
            preset.blurSpread = 0.7f;
            preset.lensflareEnabled = true;
			return preset;
		}
	}
}
