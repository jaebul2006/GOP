using UnityEngine;
using System.Collections;

public class PackageSelectorAttribute : PropertyAttribute
{
}

public class PackageVariantSelectorAttribute : PropertyAttribute
{
}

/// <summary>
/// RemotePackage.
/// 
/// Simply put it in the hierarchy to download a package and instantiate
/// it's prefab in its place!
/// 
/// <example>
/// See the example scene "InstantiatePackageAutomatically" in order to
/// see it in action!
/// </example>
/// </summary>
public class RemotePackage : MonoBehaviour
{
    /// <summary>
    /// The URI of the package to be downloaded.
    /// Unity 4: This is the relative path from the "Assets/RemotePackageManager/AssetBundles/" folder.
    /// Unity 5: This is the AssetBundle name.
    /// </summary>
    [PackageSelector]
    public string package = "";

#if UNITY_5
    /// <summary>
    /// The AssetBundle Variant of the package to be downloaded.
    /// You can find more info at http://docs.unity3d.com/Manual/BuildingAssetBundles5x.html
    /// </summary>
    [PackageVariantSelector]
    public string variant = "";
#endif

    private bool loaded = false;

    public void Load()
    {
        if( !string.IsNullOrEmpty( package ) && !loaded )
        {
            loaded = true;

#if UNITY_5
            RemotePackageManager.Load( package, variant ).GetAndInstantiate( transform.parent, OnLoad );
#else
            RemotePackageManager.Load( package ).GetAndInstantiate( transform.parent, OnLoad );
#endif
        }
    }

    private void Start()
    {
        Load();
    }

    private void OnEnabled()
    {
        Load();
    }

    private void OnLoad( GameObject go )
    {
        go.name = name;
        go.transform.localPosition = transform.localPosition;
        go.transform.localRotation = transform.localRotation;

        Destroy( gameObject );
    }
}
