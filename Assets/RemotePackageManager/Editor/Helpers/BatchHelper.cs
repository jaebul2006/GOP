public static class BatchHelper
{
    public static void RebuildAll()
    {
#if UNITY_5
        RemotePackageManagerWindow.Window.ForEachSelectedBuildTarget( buildTarget => {
            PackageSettingsHelper.BuildAssetBundles( buildTarget, true );
        } );
#else
        RemotePackageManagerWindow.Window.ForEachSelectedBuildTarget( buildTarget => {
            PackageSettingsHelper.BuildAssetBundles( PackageSettingsHelper.AllPackageSettings, buildTarget );
        } );
#endif
    }
}
