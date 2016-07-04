using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RemotePackageSettings : ScriptableObject
{
#if UNITY_5

    public class Runtime
    {
        static Runtime()
        {
            IsDownloadingManifest = false;
        }

        public static bool HasManifest { get { return manifest != null; } }
        public static bool IsDownloadingManifest { get; set; }

        public static void ParseManifest( WWW www )
        {
            manifestAssetBundle = www.assetBundle;
            if( manifestAssetBundle != null )
            {
                manifest = manifestAssetBundle.LoadAllAssets<AssetBundleManifest>().FirstOrDefault();
            }

            IsDownloadingManifest = false;
        }

        public static Runtime Get( string uri )
        {
            Runtime settings = null;
            if( HasManifest && !registry.TryGetValue( uri, out settings ) )
            {
                settings = new Runtime();
                settings.hash = manifest.GetAssetBundleHash( uri );
                settings.dependencyUris = manifest.GetDirectDependencies( uri );
                registry.Add( uri, settings );
            }

            return settings;
        }

        public bool HasDependency { get { return dependencyUris != null && dependencyUris.Length > 0; } }

        public Hash128 hash;
        public string[] dependencyUris = null;

        private static AssetBundle manifestAssetBundle = null;
        private static AssetBundleManifest manifest = null;

        private static Dictionary<string, Runtime> registry = new Dictionary<string, Runtime>();
    }

#else

    public class Runtime
    {
        public static bool HasManifest { get { return manifest != null; } }

        public static void ParseManifest( WWW www )
        {
            manifest = new Dictionary<string, Runtime>();

            string[] lines = www.text.Split( '\n' );

            foreach( string line in lines )
            {
                try
                {
                    string[] keyPair = line.Split( ':' );

                    if( keyPair.Length >= 2 )
                    {
                        manifest.Add( keyPair[0], Parse( keyPair[1] ) );
                    }
                }
                catch( System.Exception e )
                {
                    Debug.LogException( e );
                }
            }
        }

        public static Runtime Get( string uri )
        {
            Runtime settings = null;

            if( manifest != null && manifest.TryGetValue( uri, out settings ) )
            {
                return settings;
            }

            return null;
        }

        private static Runtime Parse( string text )
        {
            Runtime settings = new Runtime();

            string[] parts = text.Split( ',' );

            if( parts.Length > 0 )
            {
                int.TryParse( parts[0], out settings.version );
            }

            if( parts.Length > 1 )
            {
                settings.parentPackageUri = parts[1];
            }

            return settings;
        }

        public bool HasDependency { get { return !string.IsNullOrEmpty( parentPackageUri ); } }

        public int version = 0;
        public string parentPackageUri = "";

        private static Dictionary<string, Runtime> manifest = null;
    }

#endif

    public int version = 0;

    public List<Object> assets = new List<Object>();
    public RemotePackageSettings parent;

    public bool isExpanded = true;
}
