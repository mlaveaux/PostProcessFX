using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class DrawGUI
	{
		public static float drawSliderWithLabel(float x, float y, float min, float max, String label, float configValue)
		{
			GUI.Label(new Rect(x, y, 200, 20), label);
			return GUI.HorizontalSlider(new Rect(x + 200, y, 100, 20), configValue, min, max);
		}

		public static float drawSliderWithLabel(float x, float y, float xsize, float ysize, float min, float max, String label, float configValue)
		{
			GUI.Label(new Rect(x, y, xsize, ysize), label);
			return GUI.HorizontalSlider(new Rect(x + xsize + 25, y, 100, 20), configValue, min, max);
		}

		public static int drawIntSliderWithLabel(float x, float y, int min, int max, String label, int configValue)
		{
			GUI.Label(new Rect(x, y, 200, 20), label);
			return (int)GUI.HorizontalSlider(new Rect(x + 200, y, 100, 20), configValue, min, max);
		}

		public static int drawIntSliderWithLabel(float x, float y, float xsize, float ysize, float min, float max, String label, int configValue)
		{
			GUI.Label(new Rect(x, y, xsize, ysize), label);
			return (int)GUI.HorizontalSlider(new Rect(x + xsize + 25, y, 100, 20), configValue, min, max);
		}
	}
}
