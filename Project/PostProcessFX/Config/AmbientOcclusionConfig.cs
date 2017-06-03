using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityStandardAssets.CinematicEffects.AmbientOcclusion;

namespace PostProcessFX.Config
{
    public class AmbientOcclusionConfig
    {
        public bool enabled = true; // Is this effect enabled.

        public float intensity = 2.0f;
        public float radius = 0.6f;
        public OcclusionSource source = OcclusionSource.DepthTexture;
        public SampleCount sampleCount = SampleCount.Medium;
        public bool downsampling = false;

        public string getSampleCountLabel()
        {
            return sampleCount.ToString();
        }

        public string getOcclusionSourceLabel()
        {
            switch (source)
            {
                case OcclusionSource.DepthTexture:
                    return "Depth";
                case OcclusionSource.DepthNormalsTexture:
                    return "Normals";
            }

            return source.ToString();
        }
    }
}
