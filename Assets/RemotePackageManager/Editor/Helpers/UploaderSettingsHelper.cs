using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

public static class UploaderSettingsHelper
{
    public static void ShowUploadSettings()
    {
        bool clearAfterUpload = RemotePackageManagerWindow.Window.clearAfterUpload;

        EditorGUI.BeginChangeCheck();
        GUI.tooltip = "Should delete the \"RecentlyBuilt\" temporary folder after upload?";

        clearAfterUpload = GUILayout.Toggle( clearAfterUpload, EditorHelper.Label( "Clear After Upload" ), EditorStyles.toolbarButton );
        if( EditorGUI.EndChangeCheck() )
        {
            RemotePackageManagerWindow.Window.clearAfterUpload = clearAfterUpload;
            EditorPrefs.SetBool( "RemotePackageManager_ClearAfterUpload", clearAfterUpload );
        }
    }

    public static void UploadRecentlyBuiltButton( Uploader uploader )
    {
        if( Directory.Exists( BuilderHelper.recentlyBuiltFolderPath ) )
        {
            Uploader.RequireWeb( () => UploadRecentlyBuilt( uploader ) );
        }
        else
        {
            Debug.LogError( "Recently Built directory does not exist. Nothing to upload!" );
        }
    }

    private static void UploadManifest( Uploader uploader, System.Action<bool> callback )
    {
        if( !Directory.Exists( BuilderHelper.recentlyBuiltFolderPath ) ) return;

        foreach( string buildTargetFolder in Directory.GetDirectories( BuilderHelper.recentlyBuiltFolderPath ) )
        {
            string exportPath = Path.Combine( buildTargetFolder, RemotePackageManager.manifestFileName );

            if( File.Exists( exportPath ) )
            {
#if UNITY_5
                string manifestUri = string.Concat( Path.GetFileNameWithoutExtension( buildTargetFolder ), "/", RemotePackageManager.manifestFileName );
                uploader.UploadFile( exportPath, manifestUri, callback );
#else
                string manifestText = File.ReadAllText( exportPath );
                string manifestUri = string.Concat( Path.GetFileNameWithoutExtension( buildTargetFolder ), "/", RemotePackageManager.manifestFileName );
                uploader.UploadText( manifestText, manifestUri, callback );
#endif
            }
        }
    }

    private static void UploadRecentlyBuilt( Uploader uploader )
    {
#if UNITY_5
        UploadManifest( uploader, s => { } );
#else
        bool success = true;
        UploadManifest( uploader, s => success = ( success && s ) );
#endif

#if UNITY_5
        List<string> allAssetBundleVariants = new List<string>( AssetBundleHelper.AllAssetBundleVariants );
        allAssetBundleVariants.Add( "" );

        foreach( string buildTargetFolder in Directory.GetDirectories( BuilderHelper.recentlyBuiltFolderPath ) )
        {
            PackageSelectorHelper.ForEachSelectedPackage( path => {
                foreach( string variant in allAssetBundleVariants )
                {
                    string p = string.Concat( buildTargetFolder, "/", path );
                    if( !string.IsNullOrEmpty( variant ) )
                    {
                        p = string.Concat( p, ".", variant );
                    }

                    string packageFile = RemotePackageManager.GetExportedPackageFilePath( p );
                    if( !File.Exists( packageFile ) ) continue;
#else
        BuilderHelper.ForEachPackageInFolder( BuilderHelper.recentlyBuiltFolderPath, p => {
                    string packageFile = RemotePackageManager.GetExportedPackageFilePath( p );
#endif
                    string[] folders = p.Split( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
                    folders = folders.Skip( 3 ).ToArray();
                    string uri = string.Join( "/", folders );

                    string packageUri = RemotePackageManager.GetExportedPackageFilePath( uri ).Replace( '\\', '/' );

#if UNITY_5
                    uploader.UploadFile( packageFile, packageUri, s => { } );
                }
            } );
        }
#else
            uploader.UploadFile( packageFile, packageUri, s => success = ( success && s ) );
        } );
#endif

#if !UNITY_5
        if( RemotePackageManagerWindow.Window.clearAfterUpload && success )
        {
            AssetDatabase.DeleteAsset( BuilderHelper.recentlyBuiltFolderPath );
        }
#endif
    }
}
