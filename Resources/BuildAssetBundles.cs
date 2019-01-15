using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
	static void BuildAssetBundle(string assetBundleDirectory, BuildTarget buildTarget) 
	{
		if(!Directory.Exists(assetBundleDirectory))
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}

		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, buildTarget);
	}


	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		BuildAssetBundle ("Assets/AssetBundles/Windows", BuildTarget.StandaloneWindows);
		BuildAssetBundle ("Assets/AssetBundles/Linux", BuildTarget.StandaloneLinuxUniversal);
		BuildAssetBundle ("Assets/AssetBundles/Mac", BuildTarget.StandaloneOSXUniversal);		
	}
}