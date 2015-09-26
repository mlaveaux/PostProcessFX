using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessFX.Config
{
    /**
     * These settings are important for anti aliasing. 
     */
    public class AntiAliasingConfig
    {
        public int mode = 0;

        public float FXAA3minThreshhold = 0.1f;
        public float FXAA3maxThreshhold = 0.4f;
        public float FXAA3sharpness = 8;
        public float NFAAoffset = 0.2f;
        public float NFAAblurRadius = 18.0f;
        public bool DLAAsharp = true;

        public static AntiAliasingConfig getDefaultPreset()
        {
            return new AntiAliasingConfig();
        }
    }
}
