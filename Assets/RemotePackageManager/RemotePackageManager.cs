using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// RemotePackageManager.
///
/// Main class for managing your remote packages (AssetBundles).
///
/// <example>
/// <code>
/// RemotePackageManager.Load( "Your/Package/Name" ).GetAndInstantiate();
/// </code>
/// </example>
/// </summary>
///
/// <seealso cref="Load"/>
/// <seealso cref="RemotePackageSettingsDownloaderRequest"/>
public class RemotePackageManager : MonoBehaviour
{
    /// <summary>
    /// Gets a value indicating whether RemotePackageManager has instance.
    /// </summary>
    /// <value>
    /// <c>true</c> if RemotePackageManager has instance; otherwise, <c>false</c>.
    /// </value>
    public static bool HasInstance { get { return GetInstance( false ) != null; } }

    /// <summary>
    /// Base url to compose the final package url in runtime.
    /// </summary>
    /// <value>
    /// RemotePackageManager's base URL string.
    /// </value>
    public static string BaseUrl
    {
        get
        {
            RemotePackageManager manager = GetInstance( true );

            if( manager != null )
            {
                return manager.SelectedBaseUri;
            }

            return "";
        }
    }

    /// <summary>
    /// Check if a package is already loaded.
    ///
    /// Note that is not needed to call this method before "Load()" as it already checks for
    /// cached packages for you!
    /// </summary>
    ///
    /// <param name='packageUri'>
    /// Unity 4: Package's URI (path) starting from the folder "Assets/RemotePackageManger/AssetBundles/".
    /// Unity 5: Package's URI (AssetBundle name).
    /// </param>
    /// <param name='packageVariant'>
    /// Unity 5 only: Package's Variant (AssetBundle Variant).
    /// </param>
    /// <returns>
    /// True if the package is loaded, false otherwise.
    /// </returns>
    ///
    /// <seealso cref="Load"/>
#if UNITY_5
    public static bool IsLoaded( string packageUri, string packageVariant = null )
#else
    public static bool IsLoaded( string packageUri )
#endif
    {
        RemotePackageManager manager = GetInstance( true );

        if( manager != null )
        {
#if UNITY_5
            return manager.InstanceIsLoaded( packageUri, packageVariant );
#else
            return manager.InstanceIsLoaded( packageUri );
#endif
        }

        return false;
    }

    /// <summary>
    /// RemotePackageManager's main method. It loads a package from the web or from inside the editor.
    ///
    /// <example>
    /// <code>
    /// RemotePackageManager.Load( "Your/Package/Name" ).GetAndInstantiate();
    /// </code>
    /// </example>
    /// </summary>
    ///
    /// <param name='packageUri'>
    /// Unity 4: Package's URI (path) starting from the folder "Assets/RemotePackageManger/AssetBundles/".
    /// Unity 5: Package's URI (AssetBundle name).
    /// </param>
    /// <param name='packageVariant'>
    /// Unity 5 only: Package's Variant (AssetBundle Variant).
    /// </param>
    /// <returns>
    /// A RemotePackageRequest is returned to gain more flexibility on the just downloaded package.
    /// </returns>
    ///
    /// <seealso cref="RemotePackageSettingsDownloaderRequest"/>
#if UNITY_5
    public static RemotePackageRequest Load( string packageUri, string packageVariant = null )
#else
    public static RemotePackageRequest Load( string packageUri )
#endif
    {
        RemotePackageManager manager = GetInstance( true );

        if( manager != null )
        {
#if UNITY_5
            return manager.InstanceLoad( packageUri, packageVariant );
#else
            return manager.InstanceLoad( packageUri );
#endif
        }

        return null;
    }

    /// <summary>
    /// Unload all packages to save memory.
    ///
    /// Note that this discards all cached references to downloaded assets meaning that subsequent
    /// calls to "Load" will actually download the entire package from the internet again.
    ///
    /// Don't call this method when Objects from downloaded packages are still in use!
    /// This will cause undefined behaviour!
    /// </summary>
    ///
    /// <seealso cref="Load"/>
    public static void Unload()
    {
        RemotePackageManager manager = GetInstance( true );

        if( manager != null )
        {
            manager.InstanceUnload();
        }
    }

    private static string GetCurrentPlatformName()
    {
#if UNITY_EDITOR
        return GetPlatformName( UnityEditor.EditorUserBuildSettings.activeBuildTarget );
#else
        string platformName = Application.platform.ToString().ToLower();

        if( platformName.Contains( "webplayer" ) ) return "webplayer";

        bool windows = platformName.Contains( "windows" );
        bool osx = platformName.Contains( "osx" );
        bool linux = platformName.Contains( "linux" );
        if( windows || osx || linux ) return "standalone";

        return platformName.Replace( "player", "" ).Replace( "iphone", "ios" );
#endif
    }

#if UNITY_5
    private string GetVariantPackageUri( string packageUri, string packageVariant )
    {
        packageVariant = string.IsNullOrEmpty( packageVariant ) ? defaultVariant : packageVariant;
        if( !string.IsNullOrEmpty( packageVariant ) )
        {
            return string.Concat( packageUri, ".", packageVariant );
        }

        return packageUri;
    }

    private bool InstanceIsLoaded( string packageUri, string packageVariant )
    {
        packageUri = GetVariantPackageUri( packageUri, packageVariant );
#else
    private bool InstanceIsLoaded( string packageUri )
    {
#endif
        AssetBundle assetBundle = null;
        return assetBundleRegistry.TryGetValue( packageUri, out assetBundle ) && assetBundle == null;
    }

#if UNITY_5
    private RemotePackageRequest InstanceLoad( string packageUri, string packageVariant )
    {
        packageUri = GetVariantPackageUri( packageUri, packageVariant );
#else
    private RemotePackageRequest InstanceLoad( string packageUri )
    {
#endif
        RemotePackageRequest request = new RemotePackageRequest( packageUri );
        StartCoroutine( InstanceLoadSync( request ) );

        return request;
    }

    private IEnumerator InstanceLoadSync( RemotePackageRequest request )
    {
        yield return null;

#if UNITY_EDITOR
        if( !forceWWWRequests )
        {
#if UNITY_5
            string[] packageAssetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle( request.PackageUri );
            IEnumerable<Object> assets = packageAssetPaths.Select( p => UnityEditor.AssetDatabase.LoadAssetAtPath( p, typeof( Object ) ) );

            if( assets.Any() )
            {
                request.TryGetDownloadProgress( 0.0f );
                request.GetRemotePackage( assets.First(), assets );
                request.TryGetDownloadProgress( 1.0f );
            }
            else
            {
                Error( request.PackageUri, "Local package not found." );
            }
#else
            string packageUri = Path.Combine( "Assets/RemotePackageManager/AssetBundles", request.PackageUri );

            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath( GetSettingsFilePath( packageUri ), typeof( RemotePackageSettings ) );
            RemotePackageSettings settings = obj as RemotePackageSettings;

            if( settings != null )
            {
                request.TryGetDownloadProgress( 0.0f );
                request.GetRemotePackage( settings.assets.First(), settings.assets );
                request.TryGetDownloadProgress( 1.0f );
            }
            else
            {
                Error( packageUri, "Local Package not found!" );
            }
#endif

            yield break;
        }
#endif

        yield return StartCoroutine( DownloadAssetBundle( request.PackageUri, request ) );

        if( errorSet.Contains( request.PackageUri ) )
        {
            request.GetError();
            errorSet.Remove( request.PackageUri );
            yield break;
        }

        AssetBundle assetBundle;
        if( assetBundleRegistry.TryGetValue( request.PackageUri, out assetBundle ) )
        {
#if UNITY_5
            request.GetRemotePackage( assetBundle.mainAsset, assetBundle.LoadAllAssets() );
#else
            request.GetRemotePackage( assetBundle.mainAsset, assetBundle.LoadAll() );
#endif
        }
    }

    private void InstanceUnload()
    {
        foreach( AssetBundle assetBundle in assetBundleRegistry.Values )
        {
            if( assetBundle != null )
            {
                assetBundle.Unload( true );
            }
        }

        assetBundleRegistry.Clear();
    }

    private WWW ManifestWWW()
    {
        string uid = System.Guid.NewGuid().ToString();
        string manifestUrl = string.Format( "{0}/{1}?_t={2}", PlatformBaseUri, manifestFileName, uid );

        return new WWW( manifestUrl );
    }

    private IEnumerator DownloadAssetBundle( string packageUri, RemotePackageRequest request )
    {
        AssetBundle assetBundle;
        if( assetBundleRegistry.TryGetValue( packageUri, out assetBundle ) )
        {
            while( assetBundleRegistry.TryGetValue( packageUri, out assetBundle ) && assetBundle == null && !errorSet.Contains( packageUri ) )
            {
                // Wait for other download!
                yield return null;
            }
            yield break;
        }

        assetBundleRegistry[packageUri] = null;
        assetBundle = new AssetBundle();

        WWW www = null;

        if( !RemotePackageSettings.Runtime.HasManifest )
        {
#if UNITY_5
            if( RemotePackageSettings.Runtime.IsDownloadingManifest )
            {
                while( !RemotePackageSettings.Runtime.HasManifest )
                {
                    // Wait for other download!
                    yield return null;
                }
            }
            else
            {
                RemotePackageSettings.Runtime.IsDownloadingManifest = true;
#endif
                www = ManifestWWW();
                yield return www;
                if( CheckError( packageUri, www ) ) yield break;

                RemotePackageSettings.Runtime.ParseManifest( www );
#if UNITY_5
            }
#endif
        }

        RemotePackageSettings.Runtime packageSettings = RemotePackageSettings.Runtime.Get( packageUri );

        if( packageSettings == null )
        {
            Error( packageUri, "AssetBundle Manifest does not contains this package." );
            yield break;
        }

        if( packageSettings.HasDependency )
        {
#if UNITY_5
            foreach( string dependencyUri in packageSettings.dependencyUris )
            {
                yield return StartCoroutine( DownloadAssetBundle( dependencyUri, null ) );
            }
#else
            yield return StartCoroutine( DownloadAssetBundle( packageSettings.parentPackageUri, null ) );
#endif
        }

        if( Caching.enabled )
        {
            while( !Caching.ready ) yield return null;

#if UNITY_5
            www = WWW.LoadFromCacheOrDownload( GetPackageUrl( packageUri ), packageSettings.hash );
#else
            www = WWW.LoadFromCacheOrDownload( GetPackageUrl( packageUri ), packageSettings.version );
#endif
        }
        else
        {
            www = new WWW( GetPackageUrl( packageUri ) );
        }

        if( request != null && request.HasDownloadProgressCallback() )
        {
            while( !www.isDone )
            {
                request.GetDownloadProgress( www.progress );
                yield return null;
            }

            request.GetDownloadProgress( 1.0f );
        }
        else
        {
            yield return www;
        }

        if( CheckError( packageUri, www ) ) yield break;

        assetBundle = www.assetBundle;
        www.Dispose();

        if( assetBundle == null )
        {
            Error( packageUri, "AssetBundle is null." );
            yield break;
        }

        assetBundleRegistry[packageUri] = assetBundle;
    }

    private bool CheckError( string packageUri, WWW www )
    {
        if( !string.IsNullOrEmpty( www.error ) )
        {
            Error( packageUri, www.error );
            www.Dispose();

            if( !errorSet.Contains( packageUri ) )
            {
                errorSet.Add( packageUri );
            }

            return true;
        }

        return false;
    }

    private string GetPackageUrl( string packageUri )
    {
        string uid = System.Guid.NewGuid().ToString();

#if UNITY_5
        return string.Format( "{0}/{1}{2}?_t={3}", PlatformBaseUri, packageUri, packageFileNameSuffix, uid );
#else
        string packageName = Path.GetFileNameWithoutExtension( packageUri );
        return string.Format( "{0}/{1}/{2}{3}?_t={4}", PlatformBaseUri, packageUri, packageName, packageFileNameSuffix, uid );
#endif
    }

    private void Error( string packageUri, string errorMessage )
    {
        assetBundleRegistry.Remove( packageUri );
        Debug.LogError( string.Format( "\"{0}\" package error: \"{1}\"", packageUri, errorMessage ) );
    }

    private void Start()
    {
        DontDestroyOnLoad( gameObject );
    }

    private void OnDestroy()
    {
        InstanceUnload();
    }

    private string PlatformBaseUri
    {
        get
        {
            string url = SelectedBaseUri;

            if( url.EndsWith( "/" ) )
            {
                return string.Concat( url, GetCurrentPlatformName() );
            }

            return string.Concat( url, "/", GetCurrentPlatformName() );
        }
    }

    private string SelectedBaseUri
    {
        get
        {
            if( selectedBaseUriIndex >= 0 && selectedBaseUriIndex < baseUris.Count )
            {
                return baseUris[selectedBaseUriIndex];
            }

            return null;
        }
    }

    public bool forceWWWRequests = false;
#if UNITY_5
    public string defaultVariant = "";
#endif

    public int selectedBaseUriIndex = 0;
    public List<string> baseUris = new List<string>( new[] { "https://s3.amazonaws.com/_your_bucket_name_/" } );

    private Dictionary<string, AssetBundle> assetBundleRegistry = new Dictionary<string, AssetBundle>();
    private HashSet<string> errorSet = new HashSet<string>();

    private static RemotePackageManager instance = null;
    private static RemotePackageManager GetInstance( bool showError )
    {
        if( instance == null )
        {
            instance = Object.FindObjectOfType( typeof( RemotePackageManager ) ) as RemotePackageManager;

            if( instance == null && showError )
            {
                Debug.LogError( "Could not locate an instance of " + typeof( RemotePackageManager ).Name + "!" );
            }
        }

        return instance;
    }

#if UNITY_5
    public const string packageFileNameSuffix = "";
    private const string settingsFileNameSuffix = "";
    public const string manifestFileName = "manifest";
#else
    private const string packageFileNameSuffix = "-package.unity3d";
    private const string settingsFileNameSuffix = "-settings.asset";
    public const string manifestFileName = "manifest.txt";
#endif

#if UNITY_EDITOR
    public static string GetExportedPackageFilePath( string packagePath )
    {
#if UNITY_5
        return string.Concat( packagePath, packageFileNameSuffix );
#else
        string packageName = Path.GetFileNameWithoutExtension( packagePath );
        return Path.Combine( packagePath, packageName + packageFileNameSuffix );
#endif
    }

    public static string GetPackageFilePath( string packagePath, UnityEditor.BuildTarget buildTarget )
    {
        string packageName = Path.GetFileNameWithoutExtension( packagePath );
        string platformName = GetPlatformName( buildTarget );

        return Path.Combine( packagePath, string.Concat( packageName, "-", platformName, packageFileNameSuffix ) );
    }

    public static string GetSettingsFilePath( string packagePath )
    {
        string packageName = Path.GetFileNameWithoutExtension( packagePath );
        return Path.Combine( packagePath, packageName + settingsFileNameSuffix );
    }

    public static string GetPlatformName( UnityEditor.BuildTarget target )
    {
        string platformName = target.ToString().ToLower();

        if( platformName.Contains( "webplayer" ) ) return "webplayer";
        if( platformName.Contains( "standalone" ) ) return "standalone";

        return platformName.Replace( "player", "" );
    }
#endif
}
