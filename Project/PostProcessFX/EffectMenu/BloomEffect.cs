using PostProcessFX.EffectMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PostProcessFX
{
	/**
	 * Enables and configures the bloom and lensflare effect using the
	 * already added Bloom class of Main Camera.
	 */
	class BloomEffect : IEffectMenu
	{
		private Bloom bloomComponent = null;

		public BloomEffect(EffectConfig config)
		{			
			bloomComponent = Camera.main.GetComponent<Bloom>();
			if (bloomComponent == null)
			{
				Debug.LogError("BloomEffect: The main camera has no component named Bloom.");
			}

			applyConfig(config);
		}
		
		public void drawGUI(EffectConfig config, float x, float y)
		{
			float currentX = x;
			float currentY = y;

			config.bloomIntensity = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 2.0f, "bloomIntensity", config.bloomIntensity);
			currentY += 25;

			config.bloomThreshhold = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 6.0f, "bloomThreshhold", config.bloomThreshhold);
			currentY += 25;

			config.sepBlurSpread = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 10.0f, "bloomBlur", config.sepBlurSpread);
			currentY += 25;

			config.bloomBlurIterations = DrawGUI.drawIntSliderWithLabel(x, currentY, 1, 5, "bloomBlurIterations", config.bloomBlurIterations);
			currentY += 25;

			config.lensflareIntensity = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 2.0f, "lensflareIntensity", config.lensflareIntensity);
			currentY += 25;

			config.lensflareThreshhold = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 10.0f, "lensflareThreshhold", config.lensflareThreshhold);
			currentY += 25;

			config.lensFlareSaturation = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 1.0f, "lensflareSaturation", config.lensFlareSaturation);
			currentY += 25;

			config.flareRotation = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 3.14f, "lensflareRotation", config.flareRotation);
			currentY += 25;

			config.hollyStretchWidth = DrawGUI.drawSliderWithLabel(x, currentY, 0.0f, 5.0f, "lensflareWidth", config.hollyStretchWidth);
			currentY += 25;
			
			config.hollywoodFlareBlurIterations = DrawGUI.drawIntSliderWithLabel(x, currentY, 1, 5, "lensflareBlurIterations", config.hollywoodFlareBlurIterations);
			currentY += 25;

			applyConfig(config);	
		}

		private void applyConfig(EffectConfig config)
		{
			if (config.bloomIntensity < 0.02f)
			{
				Disable();
			}
			else
			{
				Enable();

				bloomComponent.bloomIntensity = config.bloomIntensity;
				bloomComponent.bloomBlurIterations = config.bloomBlurIterations;
				bloomComponent.bloomThreshhold = config.bloomThreshhold;
				bloomComponent.blurWidth = config.blurWidth;
				bloomComponent.flareRotation = config.flareRotation;
				bloomComponent.hollyStretchWidth = config.hollyStretchWidth;
				bloomComponent.hollywoodFlareBlurIterations = config.hollywoodFlareBlurIterations;
				bloomComponent.lensflareIntensity = config.lensflareIntensity;
				bloomComponent.lensFlareSaturation = config.lensFlareSaturation;
				bloomComponent.lensflareThreshhold = config.lensflareThreshhold;
				bloomComponent.sepBlurSpread = config.sepBlurSpread;
			}
		}
		
		private void Enable()
		{
			bloomComponent.enabled = true;
		}

		private void Disable()
		{
			bloomComponent.enabled = false;
		}
	}
}
