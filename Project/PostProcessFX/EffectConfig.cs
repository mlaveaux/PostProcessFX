using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace PostProcessFX
{
	public class EffectConfig
	{
		public bool guiActive = true;

		// Bloom options.
		public float bloomIntensity = 0.0f;
		public int bloomBlurIterations = 3;

		public float bloomThreshhold = 3.5f;
		public float blurWidth = 1.0f;

		public float flareRotation = 0.0f;
		public float hollyStretchWidth = 0.5f;
		public int hollywoodFlareBlurIterations = 1;

		public bool sunLensflare = false;
		public float lensflareIntensity = 0.0f;
		public float lensFlareSaturation = 0.6f;
		public float lensflareThreshhold = 1.4f;

		public float sepBlurSpread = 0.1f;

		// Anti aliasing options.
		public int antiAliasingMode = 0;

		public float FXAA3minThreshhold = 0.1f;
		public float FXAA3maxThreshhold = 0.4f;
		public float FXAA3sharpness = 8;
		public float NFAAoffset = 0.2f;
		public float NFAAblurRadius = 18.0f;
		public bool DLAAsharp = true;

		// Motion blur options.
		public int motionBlurMode = 0;
		public float motionblurVelocityScale = 0.375f;
		public float motionblurMinVelocity = 0.1f;
		public float motionblurMaxVelocity = 8.0f;
		public float motionblurJitter = 0.05f;

		// SSAO options
		public bool ssaoEnabled = false;
		public float ssaoBlurFilterDistance = 1.25f;
		public float ssaoIntensity = 1.5f;
		public float ssaoRadius = 2.0f;
		public int ssaoBlurFilterIterations = 1;
		public int ssaoDownsample = 0;

		// Reflection options
		public bool reflectionEnabled = false;

		public int reflectionIterations = 32;
		public int reflectionBinarySearchIterations = 16;
		public float reflectionScreenEdgeFadeStart = 0.8f;
		public float reflectionEyeFadeStart = 0.0f;
		public float reflectionEyeFadeEnd = 1.0f;
		public int reflectionPixelStride = 32;
		public float reflectionMaxDistance = 20.0f;

		// Depth of field options.
		public bool dofEnabled = false;

		// Global options.
		public int toggleUIKey = (int)KeyCode.F9;
		public float menuPositionX = 10.0f;
		public float menuPositionY = 100.0f;

		// Modifier keys for toggling the ui.
		public bool ctrlKey = false;
		public bool shiftKey = false;
		public bool altKey = false;

		public static void Serialize(String filename, object instance)
		{
			TextWriter writer = null;

			try
			{
				writer = new StreamWriter(filename);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(EffectConfig));
				xmlSerializer.Serialize(writer, instance);
			}
			catch (Exception ex)
			{
				Debug.LogError("EffectConfig: Failed to save config " + ex.Message);
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
				}
			}
		}

		public static EffectConfig Deserialize(String filename)
		{
			TextReader reader = null;
			try
			{
				reader = new StreamReader(filename);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(EffectConfig));
				return (EffectConfig)xmlSerializer.Deserialize(reader);
			}
			catch (Exception ex)
			{
				Debug.LogError("EffectConfig: " + ex.Message);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}

			return null;
		}
	}
}
