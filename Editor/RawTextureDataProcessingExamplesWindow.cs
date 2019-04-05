using UnityEngine;
using UnityEditor;

using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;


public class RawTextureDataProcessingExamplesWindow : EditorWindow
{

    Texture2D texture;
    
    void OnGUI ()
    {
        texture = (Texture2D)EditorGUILayout.ObjectField( texture , typeof(Texture2D) , false );
        EditorGUILayout.HelpBox("Only uncompressed file formats will work. No DXT1/5,ETC etc. This is meant to demonstate runtime processing not offline one.",MessageType.Info);
        if( texture!=null )
        {
            if( texture.isReadable )
            {
                if( GUILayout.Button($"Invert Colors") )
                {
                    if( texture.format==TextureFormat.RGB24 )
                    {
                        var rawdata = texture.GetRawTextureData<RGB24>();
                        new InvertRGB24Job { data = texture.GetRawTextureData<RGB24>() }
                            .Run( rawdata.Length );
                    }
                    else if( texture.format==TextureFormat.RGBA32 )
                    {
                        var rawdata = texture.GetRawTextureData<RGBA32>();
                        new InvertRGBA32Job { data = texture.GetRawTextureData<RGBA32>() }
                            .Run( rawdata.Length );
                    }
                    else if( texture.format==TextureFormat.R8 || texture.format==TextureFormat.Alpha8 )
                    {
                        var rawdata = texture.GetRawTextureData<byte>();
                        new InvertByteJob { data = texture.GetRawTextureData<byte>() }
                            .Run( rawdata.Length );
                    }
                    else if( texture.format==TextureFormat.R16 )
                    {
                        var rawdata = texture.GetRawTextureData<ushort>();
                        new InvertR16Job { data = texture.GetRawTextureData<ushort>() }
                            .Run( rawdata.Length );
                    }
                    else if( texture.format==TextureFormat.RGB565 )
                    {
                        var rawdata = texture.GetRawTextureData<RGB565>();
                        new InvertRGB565Job { data = texture.GetRawTextureData<RGB565>() }
                            .Run( rawdata.Length );
                    }
                    else
                    {
                        throw new System.NotImplementedException($"{texture.format} processing not implemented");
                    }
                    texture.Apply();
                }
            }
            else
            {
                EditorGUILayout.HelpBox( "This texture is not readable. Choose different texture or enable \"Read/Write Enabled\" for needs of this demonstration." , MessageType.Error );
            }
        }
    }

    [MenuItem("Window/Raw Texture Data Processing Example")]
    static void CreateWindow () => EditorWindow.GetWindow<RawTextureDataProcessingExamplesWindow>( nameof(RawTextureDataProcessingExamplesWindow) ).Show();

}
