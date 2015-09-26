Link to the steam workshop 
=======
# PostProcessFX

Enable additional post processing effects.

## Bloom and lensflare

Can be fully controlled with the ingame UI with the following settings:

- bloomIntensity: The intensity of the bloom effect.
- bloomThreshhold: Lower means bloom occurs with lower light intensity.
- bloomBlur: The amount of blur applied to the bloom effect.
- lensflareRotation: In which rotation the lensflare expands.
- lensflareWidth: The width of the lensflare effect.
- lensflareIntensity: The intensity of the lensflare effect.
- lensflareThreshhold: Lower means that less bloom is needed to create lensflare.
- lensFlareSaturation: Higher saturation makes the lensflare more like the bloom color.
- lensflareBlurIterations: How much the lensflare ends are blurred.
	
## Anti Aliasing

Supports FXAA2, FXAA3, NFAA, SSAA, DLAA. 

- FXAA is Fast Approximate Anti Aliasing.
	    (Following settings only available on FXAA3)
	 - min edge threshhold: Change what fxaa considers an edge.
	 - max edge threshhold: Change what fxaa considers an edge.
	 - sharpness: How sharp the image should be.

- NFAA is Normal Filter Anti Aliasing.
	 - edge offset: Offset the actual edge.
	 - blur radius: Higher results in a more blurred image.

- SSAA is Screen Space Anti Aliasing, simple edge blur.

- DLAA is Directionally Localized Anti Aliasing, which is the most expensive.
	 - sharp: Whether DLAA should produce a sharp image.

Not as effective as the DynamicResolution mod by nlight, but cheaper and can be combined if desired. 

## Motion Blur

Supports CameraMotion, LocalBlur, Reconstruction, ReconstructionDX11 and ReconstructionDisk. 

## Additional Information

All effects can be disabled so they don't have any performance impact. An ingame UI is provided to change 
and tweak the options and can be toggled with F9 (default), the toggle key can be changed in UI.

## Recommended and Compactible Mods

- [SSAO](http://steamcommunity.com/sharedfiles/filedetails/?id=410329674) by Ulysius.
- [Sun Shafts](http://steamcommunity.com/sharedfiles/filedetails/?id=410805639) by Ulysius.
- [Dynamic Resolution](http://steamcommunity.com/sharedfiles/filedetails/?id=406629464) by nlight.
