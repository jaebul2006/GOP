using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// RemotePackageRequest.
/// 
/// Helper object to handle package load requests.
/// 
/// You never have to instantiate it manually as an instance to it
/// is already returned from RemotePackageManager's "Load" method!
/// 
/// <example>
/// See RemotePackageManager example to see this in use since this
/// class depends on it!
/// </example>
/// </summary>
/// 
/// <seealso cref="RemotePackageSettingsDownloaderManager"/>
public class RemotePackageRequest
{
	/// <summary>
	/// Filter the downloaded Objects and get the first of type "T".
	/// Then it calls the callback delegate passing it as the parameter.
	/// </summary>
	/// <param name='callback'>
	/// Callback to be called with the filtered Object after the download.
	/// </param>
	/// <typeparam name='T'>
	/// Filter the downloaded Objects by type (GameObject, Texture, AudioClip, etc.)
	/// </typeparam>
	/// 
	/// <seealso cref="RemotePackageSettingsDownloaderManager"/>
    public RemotePackageRequest Get<T>( System.Action<T> callback ) where T : Object
    {
        requestCallback += delegate( Object mainAsset, IEnumerable<Object> remotePackage )
        {
            if( callback == null ) return;

            if( mainAsset is T )
            {
                callback( mainAsset as T );
            }
            else
            {
                callback( remotePackage.OfType<T>().FirstOrDefault() );
            }
        };

        return this;
    }

    /// <summary>
    /// Filter the downloaded Objects and get the first of type "T" and if name "objectName".
    /// Then it calls the callback delegate passing it as the parameter.
    /// </summary>
    /// <param name='objectName'>
    /// Filter the downloaded Objects by its name.
    /// </param>
    /// <param name='callback'>
    /// Callback to be called with the filtered Object after the download.
    /// </param>
    /// <typeparam name='T'>
    /// Filter the downloaded Objects by type (GameObject, Texture, AudioClip, etc.)
    /// </typeparam>
    ///
    /// <seealso cref="RemotePackageSettingsDownloaderManager"/>
    public RemotePackageRequest Get<T>( string objectName, System.Action<T> callback ) where T : Object
    {
        requestCallback += delegate( Object mainAsset, IEnumerable<Object> remotePackage )
        {
            if( callback == null ) return;

            if( mainAsset is T && mainAsset.name == objectName )
            {
                callback( mainAsset as T );
            }
            else
            {
                callback( remotePackage.OfType<T>().FirstOrDefault( o => o.name == objectName ) );
            }
        };

        return this;
    }

	/// <summary>
	/// Filter the downloaded Objects and get all of type "T".
	/// Then it calls the callback delegate passing them as the parameter.
	/// 
	/// Don't forget to import "System.Collections.Generic"!
	/// <code>
	/// using System.Collections.Generic;
	/// </code>
	/// </summary>
	/// <param name='callback'>
	/// Callback to be called with the filtered Objects after the download.
	/// </param>
	/// <typeparam name='T'>
	/// Filter the downloaded Objects by type (GameObject, Texture, AudioClip, etc.)
	/// </typeparam>
	/// 
	/// <seealso cref="RemotePackageSettingsDownloaderManager"/>
    public RemotePackageRequest GetAll<T>( System.Action<IEnumerable<T>> callback ) where T : Object
    {
        requestCallback += delegate( Object mainAsset, IEnumerable<Object> remotePackage )
        {
            if( callback != null )
            {
                callback( remotePackage.OfType<T>() );
            }
        };

        return this;
    }

    /// <summary>
    /// Filter the downloaded Objects and get the first of type GameObject (Prefab) and then instantiates it.
    /// Then it calls the callback delegate passing the instantiated GameObject as the parameter.
    /// </summary>
    /// <param name='parent'>
    /// Parent transform to instantiate the downloaded Prefab as its child.
    /// </param>
    /// <param name='callback'>
    /// Callback to be called with the instantiated GameObject after the download and instantiation.
    /// </param>
    ///
    /// <seealso cref="RemotePackageSettingsDownloaderManager"/>
    public RemotePackageRequest GetAndInstantiate( Transform parent = null, System.Action<GameObject> callback = null )
    {
        requestCallback += delegate( Object mainAsset, IEnumerable<Object> remotePackage )
        {
            GameObject prefab = null;

            if( mainAsset is GameObject )
            {
                prefab = mainAsset as GameObject;
            }
            else
            {
                prefab = remotePackage.OfType<GameObject>().FirstOrDefault();
            }

            if( prefab != null )
            {
                GameObject go = Object.Instantiate( prefab ) as GameObject;

                go.transform.parent = parent;
                go.transform.localPosition = Vector3.zero;

                if( callback != null )
                {
                    callback( go );
                }
            }
        };
        
        return this;
    }

	/// <summary>
	/// Filter the downloaded Objects and get the first of type GameObject (Prefab) and name "objectName"
    /// and then instantiates it. Then it calls the callback delegate passing the instantiated GameObject
    /// as the parameter.
	/// </summary>
    /// <param name='objectName'>
    /// Filter the downloaded Objects by its name.
    /// </param>
	/// <param name='parent'>
	/// Parent transform to instantiate the downloaded Prefab as its child.
	/// </param>
	/// <param name='callback'>
	/// Callback to be called with the instantiated GameObject after the download and instantiation.
	/// </param>
	/// 
	/// <seealso cref="RemotePackageSettingsDownloaderManager"/>
    public RemotePackageRequest GetAndInstantiate( string objectName, Transform parent = null, System.Action<GameObject> callback = null )
    {
        requestCallback += delegate( Object mainAsset, IEnumerable<Object> remotePackage )
        {
            GameObject prefab = null;

            if( mainAsset is GameObject && mainAsset.name == objectName )
            {
                prefab = mainAsset as GameObject;
            }
            else
            {
                prefab = remotePackage.OfType<GameObject>().FirstOrDefault( o => o.name == objectName );
            }

            if( prefab != null )
            {
                GameObject go = Object.Instantiate( prefab ) as GameObject;

                go.transform.parent = parent;
                go.transform.localPosition = Vector3.zero;

                if( callback != null )
                {
                    callback( go );
                }
            }
        };
        
        return this;
    }

    /// <summary>
    /// Called if an error occurred while trying to download the package.
    /// </summary>
    /// <param name='callback'>
    /// Callback to be called when there is an error.
    /// </param>
    ///
    /// <seealso cref="Get"/>
    public RemotePackageRequest OnError( System.Action callback )
    {
        if( callback != null )
        {
            requestErrorCallback += callback;
        }

        return this;
    }

    /// <summary>
    /// Get the download progress as a callback. The callback is a float that is in the
    /// range 0..1. 0 is the download just started and 1, the download is completed.
    /// </summary>
    /// <param name='callback'>
    /// Callback to be called with the filtered Object after the download.
    /// </param>
    ///
    /// <seealso cref="Get"/>
    public RemotePackageRequest OnDownloadProgress( System.Action<float> callback )
    {
        if( callback != null )
        {
            downloadProgressCallback += callback;
        }

        return this;
    }

	public RemotePackageRequest( string packageUri )
    {
        PackageUri = packageUri;
    }
	
    public void GetRemotePackage( Object mainAsset, IEnumerable<Object> remotePackage )
    {
        if( requestCallback != null )
        {
            requestCallback( mainAsset, remotePackage );
        }
    }

    public void GetError()
    {
        if( requestErrorCallback != null )
        {
            requestErrorCallback();
        }
    }

    public bool HasDownloadProgressCallback()
    {
        return downloadProgressCallback != null;
    }

    public void GetDownloadProgress( float progress )
    {
        downloadProgressCallback( progress );
    }

    public void TryGetDownloadProgress( float progress )
    {
        if( HasDownloadProgressCallback() )
        {
            downloadProgressCallback( progress );
        }
    }

    public string PackageUri { get; private set; }

    private System.Action<Object, IEnumerable<Object>> requestCallback;
    private System.Action requestErrorCallback;
    private System.Action<float> downloadProgressCallback;
}
