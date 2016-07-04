using UnityEngine;
using System.Collections;

public class InstantiateCube : MonoBehaviour
{
    public Transform parent;

    private void Start()
    {
        string packageName = "mycube";

        // Load "MyCube" and then instantiate it inside "parent" transform!
        RemotePackageManager.Load( packageName ).OnDownloadProgress( OnProgress ).OnError( OnError ).GetAndInstantiate( parent );

        Debug.Log( "Is 'mycube' loaded? " + RemotePackageManager.IsLoaded( packageName ) );

        // Specifically get the "CubePrefab" prefab and instantiate it!
        RemotePackageManager.Load( packageName ).GetAndInstantiate( "CubePrefab", parent, go => {
            go.transform.Translate( Vector3.left * 2.0f );
        } );
    }

    // Get updates on the "MyCube" download progress.
    private void OnProgress( float p )
    {
        Debug.Log( "On Download Progress: " + p );
    }

    private void OnError()
    {
        Debug.Log( "On Download Error :(" );
    }
}
