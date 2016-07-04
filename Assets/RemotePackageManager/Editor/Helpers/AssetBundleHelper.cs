using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_5
public class AssetBundlePostprocessorHelper : AssetPostprocessor
{
    public void OnPostprocessAssetbundleNameChanged( string assetPath, string previousAssetBundleName, string newAssetBundleName )
    {
        AssetBundleHelper.UpdateMenuOptions();

        PackageVariantSelectorDrawer.OnUpdatePackageVariants();
        RemotePackageManagerEditor.OnUpdatePackageVariants();
    }
}

public static class AssetBundleHelper
{
    public static string[] AllAssetBundleNames
    {
        get { if( assetBundleNames == null ) UpdateMenuOptions(); return assetBundleNames; }
    }

    public static string[] AllAssetBundleVariants
    {
        get { if( assetBundleVariants == null ) UpdateMenuOptions(); return assetBundleVariants; }
    }

    public static void FilterButton( string assetBundleName, GUIStyle style, params GUILayoutOption[] options )
    {
        if( ProjectBrowserHelper.HasImplementation )
        {
            EditorGUI.BeginDisabledGroup( string.IsNullOrEmpty( assetBundleName ) || assetBundleName.Equals( SmartPopupHelper.NoneOption ) );
            GUI.tooltip = "Filter the project window to only show this Asset Bundle's assets.";
            if( GUILayout.Button( EditorHelper.Label( "Filter" ), style, options ) )
            {
                ProjectBrowserHelper.SetSearch( string.Concat( "b:", assetBundleName ) );
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    public static bool ShowAssetBundleMenu()
    {
        if( !namesSmartPopup.HasOptions )
        {
            UpdateMenuOptions();
        }

        EditorHelper.BeginBoxHeader();

        EditorGUILayout.LabelField( "AssetBundle" );
        GUILayout.FlexibleSpace();

        bool requireRefresh = false;

        GUI.tooltip = "Remove all unused AssetBundle names.";
        if( GUILayout.Button( EditorHelper.Label( "Clear Unused" ), EditorStyles.toolbarButton ) )
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();            
            UpdateMenuOptions();
            requireRefresh = true;
        }

        EditorHelper.EndBoxHeaderBeginContent( Vector2.zero );

		Object[] assets = Selection.GetFiltered( typeof( Object ), SelectionMode.Assets );

		GUI.tooltip = "The currently selected asset in project.";
		if( assets.Length > 1 )
		{
			EditorGUILayout.LabelField( "Selected", "Multiple selected assets." );
		}
		else if( assets.Length > 0 )
		{
			EditorGUILayout.ObjectField( EditorHelper.Label( "Selected" ), assets[0], typeof( Object ), false );
		}
		else
		{
			EditorGUILayout.LabelField( "Selected", "No asset selected." );
		}
        
		if( assets.Length > 0 )
		{
			string assetBundleName = GetObjectsAssetBundle( assets );
            string assetBundleVariant = GetObjectsVariant( assets );

			EditorGUILayout.BeginHorizontal();
            namesSmartPopup.Show( "Package", assetBundleName, OnChangeAssetBundleName );
            FilterButton( assetBundleName, EditorStyles.miniButton, GUILayout.Width( 40.0f ) );
			EditorGUILayout.EndHorizontal();

            variantsSmartPopup.Show( "Variant", assetBundleVariant, OnChangeAssetBundleVariant );
        }
		else
		{
            EditorHelper.Rect( EditorStyles.popup );
            EditorHelper.Rect( EditorStyles.popup );
        }

        EditorHelper.EndBox();

        return requireRefresh;
    }

    private static void OnChangeAssetBundleName( string newName, bool isNew )
    {
        Object[] assets = Selection.GetFiltered( typeof( Object ), SelectionMode.Assets );
        SetObjectsAssetBundle( assets, newName );
    }

    private static void OnChangeAssetBundleVariant( string newName, bool isNew )
    {
        Object[] assets = Selection.GetFiltered( typeof( Object ), SelectionMode.Assets );
        SetObjectsVariant( assets, newName );
    }

    public static void UpdateMenuOptions()
    {
        HashSet<string> names = new HashSet<string>();
        HashSet<string> variants = new HashSet<string>();

        foreach( string assetBundleName in AssetDatabase.GetAllAssetBundleNames() )
        {
            int index = assetBundleName.LastIndexOf( '.' );

            if( index >= 0 )
            {
                names.Add( assetBundleName.Substring( 0, index ) );

                if( assetBundleName.Length > index + 1 )
                {
                    variants.Add( assetBundleName.Substring( index + 1 ) );
                }
            }
            else
            {
                names.Add( assetBundleName );
            }
        }

        assetBundleNames = names.ToArray();
        assetBundleVariants = variants.ToArray();

        System.Array.Sort( assetBundleNames );
        System.Array.Sort( assetBundleVariants );

        namesSmartPopup.SetOptions( assetBundleNames );
        variantsSmartPopup.SetOptions( assetBundleVariants );
    }

	private static string GetObjectsAssetBundle( Object[] assets )
	{
		string assetBundleName = null;
		foreach( Object asset in assets )
		{
			string name = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( asset ) ).assetBundleName;
			name = string.IsNullOrEmpty( name ) ? SmartPopupHelper.NoneOption : name;

			if( assetBundleName == null )
			{
				assetBundleName = name;
			}
			else if( !assetBundleName.Equals( name ) )
			{
				return "";
			}
		}
		
		return assetBundleName;
	}

	private static void SetObjectsAssetBundle( Object[] assets, string assetBundleName )
	{
		foreach( Object asset in assets )
		{
			AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( asset ) ).assetBundleName = assetBundleName;
		}
	}

    private static string GetObjectsVariant( Object[] assets )
    {
        string assetBundleVariant = null;
        foreach( Object asset in assets )
        {
            string variant = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( asset ) ).assetBundleVariant;
            variant = string.IsNullOrEmpty( variant ) ? SmartPopupHelper.NoneOption : variant;

            if( assetBundleVariant == null )
            {
                assetBundleVariant = variant;
            }
            else if( !assetBundleVariant.Equals( variant ) )
            {
                return "";
            }
        }

        return assetBundleVariant;
    }

    private static void SetObjectsVariant( Object[] assets, string assetBundleVariant )
    {
        foreach( Object asset in assets )
        {
            AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( asset ) ).assetBundleVariant = assetBundleVariant;
        }
    }

    private static SmartPopupHelper namesSmartPopup = new SmartPopupHelper(
        "NewPackageNameTextField",
        "Select the AssetBundle in which this asset will be included.",
        "Create a new AssetBundle."
    );

    private static SmartPopupHelper variantsSmartPopup = new SmartPopupHelper(
        "NewPackageVariantTextField",
        "Select the AssetBundle Variant in which this asset will be included.",
        "Create a new AssetBundle Variant."
    );

    private static string[] assetBundleNames = null;
    private static string[] assetBundleVariants = null;
}
#endif
