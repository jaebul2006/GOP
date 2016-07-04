using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class BuilderHelper
{
    public const string assetBundlesFolderPath = "Assets/RemotePackageManager/AssetBundles";
    public const string recentlyBuiltFolderPath = "Assets/RemotePackageManager/AssetBundles_RecentlyBuilt";

    public static List<string> GetAllPackagePaths()
    {
        List<string> paths = new List<string>();

        BuilderHelper.ForEachPackageInFolder( BuilderHelper.assetBundlesFolderPath, package => {
            package = package.Replace( '\\', '/' );
            package = package.Remove( 0, BuilderHelper.assetBundlesFolderPath.Length + 1 );

            paths.Add( package );
        } );

        return paths;
    }

#if UNITY_5
    public static void ForEachPackageInFolder( string folderPath, System.Action<string> callback )
    {
        if( !Directory.Exists( folderPath ) || callback == null ) return;

        Queue<string> queue = new Queue<string>();

        foreach( string childFolder in Directory.GetDirectories( folderPath ) )
        {
            queue.Enqueue( childFolder );
        }

        while( queue.Count > 0 )
        {
            string folder = queue.Dequeue();
            string[] childFolders = Directory.GetDirectories( folder );

            foreach( string file in Directory.GetFiles( folder ) )
            {
                bool isManifest = Path.GetFileName( file ).Equals( RemotePackageManager.manifestFileName );
                bool hasPackageSuffix = file.EndsWith( RemotePackageManager.packageFileNameSuffix );
                bool hasPackageExtension = Path.GetExtension( file ).Equals( Path.GetExtension( RemotePackageManager.packageFileNameSuffix ) );

                if( !isManifest && hasPackageSuffix && hasPackageExtension )
                {
                    callback( file );
                }
            }

            foreach( string childFolder in childFolders )
            {
                queue.Enqueue( childFolder );
            }
        }
    }

    public static void ForEachPackageInFolderLegacy( string folderPath, System.Action<string> callback )
#else
    public static void ForEachPackageInFolder( string folderPath, System.Action<string> callback )
#endif
    {
        if( !Directory.Exists( folderPath ) ) return;

        Queue<string> queue = new Queue<string>();

        foreach( string childFolder in Directory.GetDirectories( folderPath ) )
        {
            queue.Enqueue( childFolder );
        }

        while( queue.Count > 0 )
        {
            string folder = queue.Dequeue();
            string[] childFolders = Directory.GetDirectories( folder );

            if( childFolders.Length == 0 )
            {
                callback( folder );
            }

            foreach( string childFolder in childFolders )
            {
                queue.Enqueue( childFolder );
            }
        }
    }

    public static void CreateFolderTree( string path )
    {
        if( !string.IsNullOrEmpty( Path.GetExtension( path ) ) )
        {
            path = Path.GetDirectoryName( path );
        }

        if( Directory.Exists( path ) )
        {
            return;
        }

        string[] folders = path.Split( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
        path = folders[0];

        RemotePackageManagerWindow.ProjectChangeLocked = true;

        foreach( string folder in folders.Skip( 1 ) )
        {
            if( !Directory.Exists( Path.Combine( path, folder ) ) )
            {
                AssetDatabase.CreateFolder( path, folder );
            }

            path = Path.Combine( path, folder );
        }

        RemotePackageManagerWindow.ProjectChangeLocked = false;
        PackageSettingsHelper.RefreshAllPackageSettingsCache();

        AssetDatabase.Refresh();
    }

    public static string GetRecentlyBuiltFolderByTarget( BuildTarget buildTarget )
    {
        string platformName = RemotePackageManager.GetPlatformName( buildTarget );
        return string.Concat( BuilderHelper.recentlyBuiltFolderPath, "/", platformName );
    }

    public static void ExportPackageToFolder( string packagePath, BuildTarget buildTarget, string folderPath )
    {
        RemotePackageSettings settings = PackageSettingsHelper.Get( packagePath );

        folderPath = Path.Combine( folderPath, settings.GetPackageUri() );

        CreateFolderTree( folderPath );
        CreateFolderTree( packagePath );

        string copyPackagePath = RemotePackageManager.GetExportedPackageFilePath( folderPath );

        if( File.Exists( copyPackagePath ) )
        {
            AssetDatabase.DeleteAsset( copyPackagePath );
        }

        AssetDatabase.CopyAsset( RemotePackageManager.GetPackageFilePath( packagePath, buildTarget ), copyPackagePath );
    }

#if UNITY_5
    public static void ExportManifest( BuildTarget target )
    {
        string targetPath = BuilderHelper.GetRecentlyBuiltFolderByTarget( target );
        string oldManifestFile = Path.Combine( targetPath, Path.GetFileNameWithoutExtension( targetPath ) );
        string newManifestFile = Path.Combine( targetPath, RemotePackageManager.manifestFileName );

        if( File.Exists( newManifestFile ) )
        {
            AssetDatabase.DeleteAsset( newManifestFile );
        }

        if( File.Exists( oldManifestFile ) )
        {
            AssetDatabase.MoveAsset( oldManifestFile, newManifestFile );
        }

        string assetBundleManifestExtension = ".manifest";
        oldManifestFile = Path.ChangeExtension( oldManifestFile, assetBundleManifestExtension );
        newManifestFile = Path.ChangeExtension( newManifestFile, assetBundleManifestExtension );

        if( File.Exists( newManifestFile ) )
        {
            AssetDatabase.DeleteAsset( newManifestFile );
        }

        if( File.Exists( oldManifestFile ) )
        {
            AssetDatabase.MoveAsset( oldManifestFile, newManifestFile );
        }
    }
#else
    public static void ExportManifest( BuildTarget target )
    {
        string recentlyBuiltPath = BuilderHelper.GetRecentlyBuiltFolderByTarget( target );
        string exportPath = Path.Combine( recentlyBuiltPath, RemotePackageManager.manifestFileName );
        File.WriteAllText( exportPath, PackageSettingsHelper.SerializeManifest() );
    }
#endif
}
