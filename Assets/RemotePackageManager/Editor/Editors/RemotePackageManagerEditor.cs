using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[CustomEditor( typeof( RemotePackageManager ) )]
public class RemotePackageManagerEditor : Editor
{
#if UNITY_5
    public static void OnUpdatePackageVariants()
    {        
        variantSmartPopup.SetOptions( AssetBundleHelper.AllAssetBundleVariants );
    }
#endif

    public override void OnInspectorGUI()
    {
#if UNITY_5
        if( !variantSmartPopup.HasOptions )
        {
            OnUpdatePackageVariants();
        }
#endif

        EditorGUI.BeginChangeCheck();

        GUI.tooltip = "Force a WWW request inside the editor for debugging purposes.";
        Manager.forceWWWRequests = EditorGUILayout.Toggle( EditorHelper.Label( "Force WWW (Editor)" ), Manager.forceWWWRequests );

#if UNITY_5
        variantSmartPopup.Show( "Default Variant", Manager.defaultVariant, OnChangeDefaultVariant );
#endif

        baseUrlList.DoLayoutList();

        if( EditorGUI.EndChangeCheck() )
        {
            EditorUtility.SetDirty( Manager );
        }

        if( GUILayout.Button( "Open Package Manager" ) )
        {
            RemotePackageManagerWindow.OpenWindow();
        }
    }

    public void OnEnable()
    {
        baseUrlList = new ReorderableList( Manager.baseUris, typeof( string ) );

        baseUrlList.drawHeaderCallback = BaseUrlDrawHeaderCallback;
        baseUrlList.drawElementCallback = BaseUrlDrawElementCallback;

        baseUrlList.onChangedCallback = BaseUrlOnChange;
        baseUrlList.onCanRemoveCallback = l => l.list.Count > 1;
        baseUrlList.onAddCallback = l => { l.list.Add( "" ); EditorUtility.SetDirty( Manager ); };
    }

#if UNITY_5
    private void OnChangeDefaultVariant( string newVariant, bool isNew )
    {
        Manager.defaultVariant = newVariant;
        EditorUtility.SetDirty( Manager );
    }
#endif

    private void BaseUrlDrawHeaderCallback( Rect position )
    {
        GUI.tooltip = "Base uris to compose the final package uri in runtime.";
        EditorGUI.LabelField( position, EditorHelper.Label( "Base Uris" ) );
    }

    private void BaseUrlDrawElementCallback( Rect position, int index, bool isActive, bool isFocused )
    {
        float leftWidth = 16.0f;
        Rect leftRect = new Rect( position.x, position.y, leftWidth, position.height );
        Rect rightRect = new Rect( position.x + leftRect.width, position.y, position.width - leftWidth, position.height );

        EditorGUI.BeginChangeCheck();

        if( EditorGUI.Toggle( leftRect, Manager.selectedBaseUriIndex == index ) )
        {
            Manager.selectedBaseUriIndex = index;
        }

        Manager.baseUris[index] = EditorGUI.TextField( rightRect, Manager.baseUris[index] );

        if( EditorGUI.EndChangeCheck() )
        {
            EditorUtility.SetDirty( Manager );
        }
    }

    private void BaseUrlOnChange( ReorderableList list )
    {
        if( Manager.baseUris.Count > 0 )
        {
            Manager.selectedBaseUriIndex = Mathf.Clamp( Manager.selectedBaseUriIndex, 0, Manager.baseUris.Count - 1 );
        }
        else
        {
            Manager.selectedBaseUriIndex = -1;
        }

        EditorUtility.SetDirty( Manager );
    }

    private RemotePackageManager Manager { get { return target as RemotePackageManager; } }

    private ReorderableList baseUrlList = null;

#if UNITY_5
    private static SmartPopupHelper variantSmartPopup = new SmartPopupHelper( "The default AssetBundle Variant used when downloading packages." );
#endif
}
