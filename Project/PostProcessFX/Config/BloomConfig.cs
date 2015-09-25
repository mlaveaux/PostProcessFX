using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessFX.Config
{
	class BloomConfig
	{
		// Bloom configuration
		public bool bloomEnabled = true;
		public float intensity = 0.5f;
		public float threshhold = 3.5f;
		public int blurIterations = 0;
		//public float blurWidth = 0.0f;
		public float blurSpread = 0.1f;

		// Lensflare configuration.
		public bool lensflareEnabled = false;
		public float lensflareIntensity = 0.2f;
		public float lensflareThreshhold = 1.4f;
		public float lensflareStretchWidth = 0.5f;
		public float lensflareSaturation = 0.5f;
		public float lensflareRotation = 0.0f;
		public int lensflareBlurIterations = 1;

		public Boolean lensflareSun = false;

		public static BloomConfig getDefaultPreset()
		{
			return new BloomConfig();
		}

		public static BloomConfig getLowPreset()
		{
			BloomConfig preset = new BloomConfig();

			preset.bloomEnabled = true;

			return preset;
		}

		public static BloomConfig getMediumPreset()
		{
			BloomConfig preset = new BloomConfig();

            preset.bloomEnabled = true;
            preset.lensflareEnabled = true;
			preset.intensity = 1.0f;

			return preset;
		}

		public static BloomConfig getHighPreset()
		{
			BloomConfig preset = new BloomConfig();

			preset.bloomEnabled = true;
			preset.intensity = 2.0f;

			return preset;
		}
	}
}
