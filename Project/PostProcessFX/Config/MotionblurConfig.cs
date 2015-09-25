using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessFX.Config
{
    /**
     * The motion blur configuration part.
     */
	public class MotionblurConfig
	{
		public int mode = 0;
		public float velocityScale = 0.375f;
		public float minVelocity = 0.1f;
		public float maxVelocity = 8.0f;
		public float jitter = 0.05f;

		public static MotionblurConfig getDefaultConfig()
		{
			return new MotionblurConfig();
		}
	}
}
