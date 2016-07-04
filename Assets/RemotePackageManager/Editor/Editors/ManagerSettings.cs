using UnityEngine;
using UnityEditor;
using System.Collections;

public class ManagerSettings : ScriptableObject
{
    public bool checkForDifferentSelectedBuildTarget = true;
    public bool checkForUpgrade = true;

    public void Save()
    {
        EditorUtility.SetDirty( this );
        AssetDatabase.SaveAssets();
    }

    public static ManagerSettings Instance
    {
        get
        {
            if( instance == null )
            {
#if UNITY_5
                instance = AssetDatabase.LoadAssetAtPath<ManagerSettings>( path );
#else
                instance = AssetDatabase.LoadAssetAtPath( path, typeof( ManagerSettings ) ) as ManagerSettings;
#endif
                if( instance == null )
                {
                    instance = CreateInstance<ManagerSettings>();
                    AssetDatabase.CreateAsset( instance, path );
                    AssetDatabase.Refresh();
                }
            }

            return instance;
        }
    }

    private const string path = "Assets/RemotePackageManager/Editor/ManagerSettings.asset";
    private static ManagerSettings instance = null;
}
