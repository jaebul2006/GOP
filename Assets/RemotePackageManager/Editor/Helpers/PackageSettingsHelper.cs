using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

public static class PackageSettingsHelper
{
    public static List<RemotePackageSettings> AllPackageSettings { get; private set; }

    public static void RefreshAllPackageSettingsCache()
    {
#if UNITY_5
        if( AllPackageSettings == null )
        {
            AllPackageSettings = new List<RemotePackageSettings>();
        }

        AllPackageSettings.Clear();

        BuilderHelper.ForEachPackageInFolderLegacy( BuilderHelper.assetBundlesFolderPath, folder => {
            string path = RemotePackageManager.GetSettingsFilePath( folder );
            path = string.Concat( path, "-settings.asset" );
            RemotePackageSettings settings = AssetDatabase.LoadAssetAtPath( path, typeof( RemotePackageSettings ) ) as RemotePackageSettings;

            if( settings != null )
            {
                AllPackageSettings.Add( settings );
            }
        } );

        AllPackageSettings = AllPackageSettings.GroupBy( s => s.GetInstanceID() ).Select( g => g.FirstOrDefault() ).ToList();
#else

        if( AllPackageSettings == null )
        {
            AllPackageSettings = new List<RemotePackageSettings>();
        }

        AllPackageSettings.Clear();

        BuilderHelper.ForEachPackageInFolder( BuilderHelper.assetBundlesFolderPath, folder => {
            AllPackageSettings.Add( PackageSettingsHelper.Get( folder ) );
        } );

        AllPackageSettings = AllPackageSettings.GroupBy( s => s.GetInstanceID() ).Select( g => g.FirstOrDefault() ).ToList();
        AllPackageSettings.Sort( PackageSettingsComparer );
#endif
    }

    public static RemotePackageSettings Get( string packagePath )
    {
        string path = RemotePackageManager.GetSettingsFilePath( packagePath );
        RemotePackageSettings settings = AssetDatabase.LoadAssetAtPath( path, typeof( RemotePackageSettings ) ) as RemotePackageSettings;

        if( settings == null )
        {
            settings = ScriptableObject.CreateInstance<RemotePackageSettings>();

            AssetDatabase.CreateAsset( settings, path );
            AssetDatabase.Refresh();
        }

        return settings;
    }

    public static string SerializeManifest()
    {
        StringBuilder builder = new StringBuilder();

        foreach( RemotePackageSettings settings in AllPackageSettings )
        {
            builder.AppendFormat( "{0}:{1}\n", settings.GetPackageUri(), settings.Serialize() );
        }

        return builder.ToString();
    }

    public static string Serialize( this RemotePackageSettings self )
    {
        string parentPackageUri = "";
        if( self.parent != null )
        {
            parentPackageUri = self.parent.GetPackageUri();
        }

        string[] parts = new string[] { self.version.ToString(), parentPackageUri };
        return string.Join( ",", parts );
    }

#if UNITY_5
    public static void BuildAssetBundles( BuildTarget buildTarget, bool forceRebuild )
    {
        if( !Directory.Exists( BuilderHelper.recentlyBuiltFolderPath ) )
        {
            Directory.CreateDirectory( BuilderHelper.recentlyBuiltFolderPath );
        }

        string buildFolder = BuilderHelper.GetRecentlyBuiltFolderByTarget( buildTarget );

        if( !Directory.Exists( buildFolder ) )
        {
            Directory.CreateDirectory( buildFolder );
        }

        BuildAssetBundleOptions options = forceRebuild ? BuildAssetBundleOptions.ForceRebuildAssetBundle : BuildAssetBundleOptions.None;
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles( buildFolder, options, buildTarget );
        AssetDatabase.Refresh();

        if( manifest != null )
        {
            BuilderHelper.ExportManifest( buildTarget );

            int assetBundleCount = manifest.GetAllAssetBundles().Length;
            Debug.Log( string.Format( "<b>{0}</b> AssetBundles are ready for the <b>{1}</b> platform.", assetBundleCount, buildTarget ) );
        }
    }
#else
    public static void BuildAssetBundles( IEnumerable<RemotePackageSettings> allSetings, BuildTarget buildTarget )
    {
        foreach( HierarchyHelper.SettingsNode root in HierarchyHelper.GetSettingsTree( allSetings ) )
        {
            BuildSettingsNode( root, buildTarget );
        }

        BuilderHelper.ExportManifest( buildTarget );

        AssetDatabase.Refresh();
        EditorApplication.RepaintProjectWindow();
    }
#endif

    public static void BuildAssetBundle( this RemotePackageSettings self, BuildTarget buildTarget )
    {
        List<RemotePackageSettings> hierarchy = new List<RemotePackageSettings>();
        hierarchy.Add( self );

        RemotePackageSettings parent = null;

        while( true )
        {
            parent = hierarchy[0].parent;

            if( parent == null )
            {
                break;
            }

            hierarchy.Insert( 0, parent );
        }

#if !UNITY_5
        BuildAssetBundles( hierarchy, buildTarget );
#endif
    }

    public static string GetPackageFolderPath( this RemotePackageSettings self )
    {
        return Path.GetDirectoryName( AssetDatabase.GetAssetPath( self ) );
    }

    public static string GetPackageUri( this RemotePackageSettings self )
    {
        string path = Path.GetDirectoryName( AssetDatabase.GetAssetPath( self ) );
        return path.Remove( 0, BuilderHelper.assetBundlesFolderPath.Length + 1 );
    }

    public static bool CheckParentCicle( this RemotePackageSettings self, RemotePackageSettings parentSettings )
    {
        RemotePackageSettings lastSettings = parentSettings;

        for( ; parentSettings != null; parentSettings = parentSettings.parent )
        {
            if( parentSettings == self )
            {
                Selection.activeObject = lastSettings;

                string message = string.Format( "\"{0}\" already depends on \"{1}\"", lastSettings.GetPackageUri(), self.GetPackageUri() );
                EditorUtility.DisplayDialog( "Dependency Cicle Encountered", message, "Ok" );

                return false;
            }

            lastSettings = parentSettings;
        }

        return true;
    }

#if UNITY_5
    public static long? GetMeanPackageSize( string packageName )
    {
        long meanSize = 0;
        int count = 0;

        List<string> allVariants = new List<string>( AssetBundleHelper.AllAssetBundleVariants );
        allVariants.Add( "" );

        RemotePackageManagerWindow.Window.ForEachSelectedBuildTarget( buildTarget => {
            foreach( string variant in allVariants )
            {
                string packageFilePath = BuilderHelper.recentlyBuiltFolderPath;
                packageFilePath = Path.Combine( packageFilePath, RemotePackageManager.GetPlatformName( buildTarget ) );
                packageFilePath = Path.Combine( packageFilePath, packageName );

                string directory = Path.GetDirectoryName( packageFilePath );
                string filename = Path.GetFileNameWithoutExtension( packageFilePath );

                packageFilePath = Path.Combine( directory, filename );

                if( !string.IsNullOrEmpty( variant ) )
                {
                    packageFilePath = Path.ChangeExtension( packageFilePath, variant );
                }

                packageFilePath = string.Concat( packageFilePath, RemotePackageManager.packageFileNameSuffix );

                long? packageFileSize = FileHelper.GetFileSize( packageFilePath );

                if( packageFileSize.HasValue )
                {
                    count++;
                    meanSize += packageFileSize.Value;
                }
            }
        } );

        return count > 0 ? ( long? ) meanSize / count : null;
    }
#else
    public static long? GetMeanPackageSize( this RemotePackageSettings self )
    {
        long meanSize = 0;
        int count = 0;

        RemotePackageManagerWindow.Window.ForEachSelectedBuildTarget( buildTarget => {
            string packageFilePath = RemotePackageManager.GetPackageFilePath( self.GetPackageFolderPath(), buildTarget );
            long? packageFileSize = FileHelper.GetFileSize( packageFilePath );

            if( packageFileSize.HasValue )
            {
                count++;
                meanSize += packageFileSize.Value;
            }
        } );

        return count > 0 ? ( long? ) meanSize / count : null;
    }
#endif

    private static void BuildSettingsNode( HierarchyHelper.SettingsNode node, BuildTarget buildTarget )
    {
#if !UNITY_5
        BuildPipeline.PushAssetDependencies();

        try
        {
            BuildSingleAssetBundle( node.self, buildTarget );

            foreach( HierarchyHelper.SettingsNode child in node.children )
            {
                BuildSettingsNode( child, buildTarget );
            }
        }
        finally
        {
            BuildPipeline.PopAssetDependencies();
        }
#endif
    }

    private static void BuildSingleAssetBundle( RemotePackageSettings settings, BuildTarget buildTarget )
    {
#if !UNITY_5
        string packagePath = settings.GetPackageFolderPath();

        if( settings.assets.Count() > 0 )
        {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;

            settings.IncrementVersion();

            string packageFilePath = RemotePackageManager.GetPackageFilePath( packagePath, buildTarget );
            BuilderHelper.CreateFolderTree( packageFilePath );

            BuildPipeline.BuildAssetBundle( settings.assets.First(), settings.assets.ToArray(), packageFilePath, options, buildTarget );

            string exportPath = BuilderHelper.GetRecentlyBuiltFolderByTarget( buildTarget );
            BuilderHelper.ExportPackageToFolder( packagePath, buildTarget, exportPath );
        }

        LogBuiltAssets( packagePath, settings.assets, buildTarget );
#endif
    }

    private static void IncrementVersion( this RemotePackageSettings self )
    {
        self.version++;

        EditorUtility.SetDirty( self );
        AssetDatabase.Refresh();
    }

    private static void LogBuiltAssets( string folderPath, IEnumerable<Object> assets, BuildTarget buildTarget )
    {
        folderPath = folderPath.Remove( 0, BuilderHelper.assetBundlesFolderPath.Length + 1 );

        if( assets.Count() > 0 )
        {
            string[] dependencies = AssetDatabase.GetDependencies( assets.Select( a => AssetDatabase.GetAssetPath( a ) ).ToArray() );

            System.Text.StringBuilder log = new System.Text.StringBuilder();
            log.AppendFormat( "<b>{0}</b> AssetBundle built with the following assets for <b>{1}</b> platform:", folderPath, buildTarget );
            log.AppendLine();

            foreach( string dependency in dependencies )
            {
                string fileName = Path.GetFileNameWithoutExtension( dependency );
                string extension = Path.GetExtension( dependency );
                string path = Path.GetDirectoryName( dependency );

                log.AppendFormat( "{0}/<b>{1}</b>{2}\n", path, fileName, extension );
            }

            Debug.Log( log.ToString() );
        }
        else
        {
            Debug.LogError( string.Format( "<b>{0}</b> Tried to build embty AssetBundle!", folderPath ) );
        }
    }

    private static int PackageSettingsComparer( RemotePackageSettings a, RemotePackageSettings b )
    {
        return string.Compare( a.name, b.name );
    }
}
