using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class RemotePackageManagerWindow : EditorWindow
{
    public bool CheckDifferentSelectedBuildTarget()
    {
        if( !ManagerSettings.Instance.checkForDifferentSelectedBuildTarget ) return true;

        BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;

        bool hasDifferent = false;
        ForEachSelectedBuildTarget( target => {
            hasDifferent = hasDifferent || ( activeTarget != target );
        }, false );

        if( hasDifferent )
        {
            string dialog = string.Format( "You are about to build packages to a different platform.\nCurrent selected \"{0}\".", activeTarget );
            return EditorUtility.DisplayDialog( "Different Platform Warning", dialog, "Continue", "Cancel" );
        }

        return true;
    }

    public void ForEachSelectedBuildTarget( System.Action<BuildTarget> action, bool log = true )
    {
        if( action == null ) return;

        if( selectedBuildTargets == 0 )
        {
            if( log )
            {
                Debug.Log( "Please, selecte a Build Target first!" );
            }

            return;
        }

        int mask = selectedBuildTargets;
        int count = System.Enum.GetValues( typeof( BuildTarget ) ).Length;

        for( int i = 0; i < count; i++ )
        {
            if( ( mask & 1 ) == 1 )
            {
                BuildTarget buildTarget = IndexToBuildTarget( i );
                action( buildTarget );
            }

            mask >>= 1;
        }
    }

#if !UNITY_5
    private void OnProjectChange()
    {
        if( !ProjectChangeLocked )
        {
            PackageSettingsHelper.RefreshAllPackageSettingsCache();
        }
    }
#endif

    private void OnGUI()
    {
#if UNITY_5
        ShowPackagesTab();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        ShowUploadTab();
#else
        ShowToolbarMenu();

        EditorGUILayout.Space();

        switch( selectedTab )
        {
        case Tab.Packages: ShowPackagesTab(); break;
        case Tab.Upload: ShowUploadTab(); break;
        default: break;
        }

        CheckDeleteSettings();
#endif
    }

#if !UNITY_5
    private void ShowToolbarMenu()
    {
        EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
        switch( selectedTab )
        {
        case Tab.Packages: ShowPackagesHeader(); break;
        case Tab.Upload: ShowUploadHeader(); break;
        default: break;
        }

        GUILayout.FlexibleSpace();

        foreach( System.Enum tabEnum in System.Enum.GetValues( typeof( Tab ) ) )
        {
            Tab tab = ( Tab ) tabEnum;

            if( GUILayout.Toggle( tab == selectedTab, tab.ToString(), EditorStyles.toolbarButton ) )
            {
                selectedTab = tab;
            }
        }

        EditorGUILayout.EndHorizontal();
    }
#endif

    private void ShowPackagesHeader()
    {
        GUI.tooltip = "Choose in which platform the packages are going to be used.";

        EditorGUI.BeginChangeCheck();
        string[] options = System.Enum.GetNames( typeof( BuildTarget ) );
        selectedBuildTargets = EditorGUILayout.MaskField( selectedBuildTargets, options, EditorStyles.toolbarPopup, GUILayout.Width( 160.0f ) );
        if( EditorGUI.EndChangeCheck() )
        {
            EditorPrefs.SetInt( "RemotePackageManager_SelectedBuildTargets", selectedBuildTargets );
        }
    }

    private void ShowUploadHeader()
    {
        UploaderSettingsHelper.ShowUploadSettings();
    }

    private void ShowPackagesTab()
    {
#if UNITY_5
        EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
        EditorGUILayout.LabelField( "Build", EditorStyles.boldLabel );
        GUILayout.FlexibleSpace();
        ShowPackagesHeader();
        EditorGUILayout.EndHorizontal();

        ShowBuildPackages();
        if( AssetBundleHelper.ShowAssetBundleMenu() )
        {
            PackageSelectorHelper.BuildPackageSelectorHierarchy();
        }
#else
        ShowBuildPackages();

        EditorGUI.BeginDisabledGroup( showOutdatedPackages );
        EditorGUILayout.HelpBox( "Type to search or create packages.", MessageType.None );
        packagesSearch = EditorHelper.SearchField( packagesSearch );
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        scroll = EditorGUILayout.BeginScrollView( scroll );

        if( showOutdatedPackages )
        {
            CheckOutdatedPackages();
            ListPackages( outdatedPackagesSettings );
        }
        else if( string.IsNullOrEmpty( packagesSearch ) )
        {
            HierarchyHelper.DrawHierarchyView();
        }
        else
        {
            ShowSearchCreatePackage();
        }

        EditorGUILayout.EndScrollView();

        Repaint();
#endif
    }

    private void ShowUploadTab()
    {
#if UNITY_5
        EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
        EditorGUILayout.LabelField( "Upload", EditorStyles.boldLabel );
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
#endif

        Uploader.Get<AmazonS3Uploader, RemotePackageManagerWindow>().ShowSettingsInspector( UploaderSettingsHelper.UploadRecentlyBuiltButton );
        Uploader.Get<FtpUploader, RemotePackageManagerWindow>().ShowSettingsInspector( UploaderSettingsHelper.UploadRecentlyBuiltButton );

#if UNITY_5
        PackageSelectorHelper.ShowSelectablePackages();
#endif
    }

#if UNITY_5
    private static Color32 buildPackagesButtonColor = new Color32( 166, 191, 225, 255 );
    private void ShowBuildPackages()
    {
        GUI.tooltip = "Incrementally build all packages.";
        GUI.backgroundColor = buildPackagesButtonColor;
        if( GUILayout.Button( EditorHelper.Label( "Build Packages" ), GUILayout.Height( 32.0f ) ) )
        {
            if( CheckDifferentSelectedBuildTarget() )
            {
                ForEachSelectedBuildTarget( target => PackageSettingsHelper.BuildAssetBundles( target, false ) );
                AssetDatabase.Refresh();
            }
        }
        GUI.backgroundColor = Color.white;

        GUI.tooltip = "Force all packages to be rebuilt.";
        if( GUILayout.Button( EditorHelper.Label( "Force Rebuild" ), GUILayout.Height( 24.0f ) ) )
        {
            string dialog = "Are you sure you want to rebuild all packages?\nIt may take a while";
            if( EditorUtility.DisplayDialog( "Force Rebuild", dialog, "Rebuild", "Cancel" ) )
            {
                if( CheckDifferentSelectedBuildTarget() )
                {
                    AssetDatabase.DeleteAsset( BuilderHelper.recentlyBuiltFolderPath );
                    AssetDatabase.Refresh();

                    ForEachSelectedBuildTarget( target => PackageSettingsHelper.BuildAssetBundles( target, true ) );
                    AssetDatabase.Refresh();
                }
            }
        }
    }
#else
    private void ShowBuildPackages()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup( !Selection.objects.Any( o => o is RemotePackageSettings ) );
        GUI.tooltip = "Build all selected packages.";
        if( GUILayout.Button( EditorHelper.Label( "Build Selected" ), EditorStyles.miniButtonLeft ) )
        {
            IEnumerable<RemotePackageSettings> allSettings = Selection.objects.Where( o => o is RemotePackageSettings ).Cast<RemotePackageSettings>();

            ForEachSelectedBuildTarget( buildTarget => PackageSettingsHelper.BuildAssetBundles( allSettings, buildTarget ) );
        }
        EditorGUI.EndDisabledGroup();

        if( showOutdatedPackages )
        {
            GUI.tooltip = "Build only outdated packages.";
            if( GUILayout.Button( EditorHelper.Label( "Build Outdated" ), EditorStyles.miniButtonMid ) )
            {
                ForEachSelectedBuildTarget( buildTarget => PackageSettingsHelper.BuildAssetBundles( outdatedPackagesSettings, buildTarget ) );
                showOutdatedPackages = false;
            }
        }
        else
        {
            GUI.tooltip = "Rebuild all packages from scratch.";
            if( GUILayout.Button( EditorHelper.Label( "Rebuild All" ), EditorStyles.miniButtonMid ) )
            {
                if( EditorUtility.DisplayDialog( "Rebuild All", "Are you sure you want to rebuild all packages?\nIt may take a while", "Rebuild", "Cancel" ) )
                {
                    PackageSettingsHelper.RefreshAllPackageSettingsCache();

                    ForEachSelectedBuildTarget( buildTarget => PackageSettingsHelper.BuildAssetBundles( PackageSettingsHelper.AllPackageSettings, buildTarget ) );
                }
            }
        }

        EditorGUI.BeginDisabledGroup( !Directory.Exists( BuilderHelper.recentlyBuiltFolderPath ) );
        GUI.tooltip = "Delete the 'AssetBundles_RecentlyBuilt' temp folder.";
        if( GUILayout.Button( EditorHelper.Label( "Clear Recently Built" ), EditorStyles.miniButtonRight ) )
        {
            AssetDatabase.DeleteAsset( BuilderHelper.recentlyBuiltFolderPath );
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.FlexibleSpace();

        bool noOutdatedPackages = outdatedPackagesSettings.Count == 0;
        if( noOutdatedPackages )
        {
            showOutdatedPackages = false;
        }

        EditorGUI.BeginDisabledGroup( noOutdatedPackages );
        GUI.tooltip = "Show only outdated packages.";
        showOutdatedPackages = GUILayout.Toggle( showOutdatedPackages, EditorHelper.Label( "Show Outdated" ), EditorStyles.miniButton );
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }
#endif

    private void ShowSearchCreatePackage()
    {
        string lowerSearch = packagesSearch.ToLower();

        IEnumerable<RemotePackageSettings> filteredSettings = PackageSettingsHelper.AllPackageSettings.Where( settings => {
            return settings.name.ToLower().Contains( lowerSearch );
        } );

        if( filteredSettings.Count() > 0 )
        {
            ListPackages( filteredSettings );
        }
        else if( GUILayout.Button( "Create Package" ) )
        {
            packagesSearch = packagesSearch.Replace( " ", "" );

            string newPackagePath = Path.Combine( BuilderHelper.assetBundlesFolderPath, packagesSearch );
            BuilderHelper.CreateFolderTree( newPackagePath );

            RemotePackageSettings settings = PackageSettingsHelper.Get( newPackagePath );
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject( settings );

            packagesSearch = "";
            PackageSettingsHelper.RefreshAllPackageSettingsCache();
        }
    }

    private void ListPackages( IEnumerable<RemotePackageSettings> settingsCollection )
    {
        foreach( RemotePackageSettings settings in settingsCollection )
        {
            Rect position = EditorHelper.Rect( EditorStyles.label );

            if( Event.current.type == EventType.MouseDown )
            {
                bool mouseOver = position.Contains( Event.current.mousePosition );
                DragDropHelper.SelectSettings( settings, mouseOver );
            }

            DragDropHelper.ShowSelection( position, settings );
            EditorGUI.LabelField( position, settings.name );
        }
    }

    private void CheckOutdatedPackages()
    {
        if( checkedOutdated ) return;

        outdatedPackagesSettings.Clear();

        foreach( RemotePackageSettings settings in PackageSettingsHelper.AllPackageSettings )
        {
            CheckOutdated( settings );
        }

        checkedOutdated = true;
    }

    private void CheckOutdated( RemotePackageSettings settings )
    {
        string settingsPath = AssetDatabase.GetAssetPath( settings );
        System.DateTime settingsTime = File.GetLastWriteTime( settingsPath );

        List<System.DateTime> packagesTime = new List<System.DateTime>();
        ForEachSelectedBuildTarget( buildTarget => {
            string packagePath = RemotePackageManager.GetPackageFilePath( settings.GetPackageFolderPath(), buildTarget );
            packagesTime.Add( File.GetLastWriteTime( packagePath ) );
        } );

        foreach( Object asset in settings.assets )
        {
            string assetPath = AssetDatabase.GetAssetPath( asset );

            List<string> dependencies = AssetDatabase.GetDependencies( new string[] { assetPath } ).ToList();
            dependencies.Add( assetPath );

            bool anyOutdatedDependency = dependencies.Any( d => {
                if( string.IsNullOrEmpty( d ) ) return false;

                System.DateTime lastWriteTime = File.GetLastWriteTime( d );
                bool anyPackageOutDated = packagesTime.Any( time => time.CompareTo( lastWriteTime ) < 0 );

                return anyPackageOutDated || settingsTime.CompareTo( lastWriteTime ) < 0;
            } );

            if( anyOutdatedDependency )
            {
                outdatedPackagesSettings.Add( settings );
                break;
            }
        }
    }

    private void CheckDeleteSettings()
    {
        if( Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Delete )
        {
            foreach( Object obj in Selection.objects )
            {
                if( obj is RemotePackageSettings )
                {
                    RemotePackageSettings settings = obj as RemotePackageSettings;
                    TryDeletePackage( settings.GetPackageFolderPath() );
                }
            }
        }
    }

    private void TryDeletePackage( string packageFolderPath )
    {
        if( selectedTab != Tab.Packages ) return;

        string message = "Are you sure you want to delete all selected packages?\nIt's a destructive operation!";
        if( EditorUtility.DisplayDialog( "Delete Selected", message, "Ok", "Cancel" ) )
        {
            AssetDatabase.MoveAssetToTrash( packageFolderPath );
        }
    }

    private static BuildTarget IndexToBuildTarget( int index )
    {
        if( indexToBuildTarget.Count == 0 )
        {
            System.Array values = System.Enum.GetValues( typeof( BuildTarget ) );

            for( int i = 0; i < values.Length; i++ )
            {
                indexToBuildTarget.Add( i, ( BuildTarget ) values.GetValue( i ) );
            }
        }

        return indexToBuildTarget[index];
    }

    public Tab selectedTab = Tab.Packages;
    public Vector2 scroll = Vector2.zero;

    public int selectedBuildTargets = 0;
    public string packagesSearch = "";

    public bool showOutdatedPackages = false;
    public List<RemotePackageSettings> outdatedPackagesSettings = new List<RemotePackageSettings>();
    private bool checkedOutdated = false;

    public bool clearAfterUpload = false;

    private bool hasInitialized = false;

    public static RemotePackageManagerWindow Window { get { return GetWindow( false ); } }

    public static bool ProjectChangeLocked { get; set; }

    private static Dictionary<int, BuildTarget> indexToBuildTarget = new Dictionary<int, BuildTarget>();

    static private int? defaultBuildTargetMask;
    static private int DefaultBuildTargetMask
    {
        get
        {
            if( !defaultBuildTargetMask.HasValue )
            {
                BuildTarget defaultBuildTarget = BuildTarget.StandaloneWindows;
                defaultBuildTargetMask = 1 << System.Array.IndexOf( System.Enum.GetValues( typeof( BuildTarget ) ), defaultBuildTarget );
            }

            return defaultBuildTargetMask.Value;
        }
    }

    public enum Tab
    {
        Packages,
        Upload
    }

    [MenuItem( "Window/Remote Package Manager" )]
    public static void OpenWindow()
    {
        GetWindow( true );
    }

    private static RemotePackageManagerWindow GetWindow( bool focus )
    {
        RemotePackageManagerWindow window = EditorWindow.GetWindow<RemotePackageManagerWindow>( "Pkg Manager", focus );
        window.Initialize();
        return window;
    }

    private void OnFocus()
    {
        LoadPrefs();
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private void Initialize()
    {
        if( !hasInitialized )
        {
            hasInitialized = true;

            minSize = new Vector2( 400.0f, 400.0f );
            LoadPrefs();
            VersionHelper.CheckVersion();

#if UNITY_5
            PackageSettingsHelper.RefreshAllPackageSettingsCache();
            LegacyUpgradeHelper.CheckUpgrade();
#endif
        }
    }

    private void LoadPrefs()
    {
        selectedBuildTargets = EditorPrefs.GetInt( "RemotePackageManager_SelectedBuildTargets", DefaultBuildTargetMask );
        clearAfterUpload = EditorPrefs.GetBool( "RemotePackageManager_ClearAfterUpload", clearAfterUpload );

#if UNITY_5
        PackageSelectorHelper.BuildPackageSelectorHierarchy();
        AssetBundleHelper.UpdateMenuOptions();
#else
        PackageSettingsHelper.RefreshAllPackageSettingsCache();
#endif

        checkedOutdated = false;
        hasInitialized = true;
    }
}
