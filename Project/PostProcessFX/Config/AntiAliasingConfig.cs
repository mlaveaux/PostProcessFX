using static UnityStandardAssets.CinematicEffects.SMAA;

namespace PostProcessFX.Config
{
    public enum AntiAliasingMode
    {
        Disabled = 0,
        FXAA,
        SMAA,
        TAA,
        Maximum
    }

    /**
     * These settings are important for anti aliasing. 
     */
    public class AntiAliasingConfig
    {
        public AntiAliasingMode mode = AntiAliasingMode.Disabled; // None: 0, FXAA: 1, SMAA: 2, TAA: 3

        // FXAA specific settings.
        public int fxaaQuality = 2;

        // SMAA specific settings.
        public EdgeDetectionMethod smaaEdgeMethod = EdgeDetectionMethod.Color;
        public QualityPreset smaaQuality = QualityPreset.Medium;
        public bool smaaTemporal = false;
        public bool smaaPredication = false;

        public static AntiAliasingConfig getDefaultPreset()
        {
            return new AntiAliasingConfig();
        }

        public string getLabelFromMode()
        {
            return mode.ToString();
        }

        public string getLabelFromFxaaQuality()
        {
            switch(fxaaQuality)
            {
                case 0:
                    return "Very Low";
                case 1:
                    return "Low";
                case 2:
                    return "Medium";
                case 3:
                    return "High";
                case 4:
                    return "Ultra";
            }

            return "Undefined";
        }

        public string getLabelFromSmaaQuality()
        {
            return smaaQuality.ToString();
        }

        public string getLabelFromEdgeDetection()
        {
            return smaaEdgeMethod.ToString();
        }
    }
}
