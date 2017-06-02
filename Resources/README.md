This README explains how the asset bundles should be created.

- Download Unity Engine 5.5.31f, which is what Cities Skylines uses in 1.7.

- Open the postprocessfx.unitypackage (or import the standard assets effects).

- Every file that should be added to the bundle must be part of postprocessfx under asset label.

- Add the BuildAssetBundles.cs script under Assets/Engine.

- Run Assets/BuildAssetBundles

- Copy the resulting <Target>/postprocessfx to Project/Resources, this folder will be copied to the mod location. 