﻿using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class Uploader
{
    public static UploaderList Get<U, C>() where U : Uploader
    {
        return Get( typeof( U ), typeof( C ) );
    }

    public static UploaderList Get( System.Type uploaderType, System.Type context )
    {
        if( !uploaderType.IsSubclassOf( typeof( Uploader ) ) )
        {
            string message = string.Format( "Type \"{0}\" is not a subclass of \"{1}\"", uploaderType, typeof( Uploader ) );
            throw new System.Exception( message );
        }

        UploaderList uploaders;
        Dictionary<System.Type, UploaderList> uploaderRegistry;

        if( registry.TryGetValue( uploaderType, out uploaderRegistry ) )
        {
            if( !uploaderRegistry.TryGetValue( context, out uploaders ) )
            {
                uploaders = new UploaderList( uploaderType, context );
                uploaderRegistry.Add( context, uploaders );
            }
        }
        else
        {
            uploaders = new UploaderList( uploaderType, context );
            uploaderRegistry = new Dictionary<System.Type, UploaderList>();

            uploaderRegistry.Add( context, uploaders );
            registry.Add( uploaderType, uploaderRegistry );
        }

        return uploaders;
    }

    public static void RequireWeb( System.Action action )
    {
        BuildTarget savedBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        bool isWebplayer = savedBuildTarget == BuildTarget.WebPlayer;
        isWebplayer = isWebplayer || savedBuildTarget == BuildTarget.WebPlayerStreamed;

#if UNITY_EDITOR_OSX
        if( isWebplayer ) EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneOSXIntel );
#else
        if( isWebplayer ) EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneWindows );
#endif

        if( action != null ) action();
        if( isWebplayer ) EditorUserBuildSettings.SwitchActiveBuildTarget( savedBuildTarget );
    }

    public void UploadText( string text, string itemKey, System.Action<bool> callback )
    {
        Upload( Encoding.ASCII.GetBytes( text ), itemKey, callback );
    }

    public void UploadFile( string filePath, string itemKey, System.Action<bool> callback )
    {
        if( File.Exists( filePath ) )
        {
            Upload( File.ReadAllBytes( filePath ), itemKey, callback );
        }
    }

    public virtual void Upload( byte[] itemBytes, string itemKey, System.Action<bool> callback )
    {
    }

    public virtual bool HasSettingsNoRetrieve()
    {
        return false;
    }

    public virtual string GetUrl( string itemKey )
    {
        return "";
    }

    public virtual void OnShowSettingsInspector( Rect rect )
    {
    }

    public virtual void RetrieveSettings( string json )
    {
    }

    public virtual string SaveSettings()
    {
        return "";
    }

    protected void EndUpload( string itemKey, bool success, string errorMessage, System.Action<bool> callback )
    {
        if( success )
        {
            Debug.Log( string.Format( "\"{0}\" upload status: OK", itemKey ) );
        }
        else
        {
            Debug.LogError( string.Format( "\"{0}\" upload status: {1}", itemKey, errorMessage ) );
        }

        if( callback != null )
        {
            callback( success );
        }
    }

    protected Rect LineRect( Rect rect, int verticalIndex )
    {
        rect.height = EditorHeightAttribute.SingleLineHeight;
        rect.y += EditorHeightAttribute.SingleLineHeight * verticalIndex;
        return rect;
    }

    private static Dictionary<System.Type, Dictionary<System.Type, UploaderList>> registry = new Dictionary<System.Type, Dictionary<System.Type, UploaderList>>();
}

[System.AttributeUsage( System.AttributeTargets.Class )]
public class EditorHeightAttribute : System.Attribute
{
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
    public static float SingleLineHeight { get { return 16.0f; } }
#else
    public static float SingleLineHeight { get { return EditorGUIUtility.singleLineHeight; } }
#endif

    public static float GetHeight( System.Type type )
    {
        EditorHeightAttribute[] attributes = type.GetCustomAttributes( typeof( EditorHeightAttribute ), true ) as EditorHeightAttribute[];
        EditorHeightAttribute attribute = attributes.FirstOrDefault();

        return attribute != null ? attribute.Height : 0.0f;
    }

    public float Height { get { return SingleLineHeight * lineCount + offset; } }

    public EditorHeightAttribute( int lineCount, float offset = 0.0f )
    {
        this.lineCount = lineCount;
        this.offset = offset;
    }

    private int lineCount = 1;
    private float offset = 0.0f;
}
