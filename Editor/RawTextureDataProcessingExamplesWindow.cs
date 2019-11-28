using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.Jobs;

public class RawTextureDataProcessingExamplesWindow : UnityEditor.EditorWindow
{

    Texture2D _texture;
    
    void OnEnable ()
    {
		rootVisualElement.Clear();

		var PREVIEW = new Image();
		{
			PREVIEW.image = _texture;
			PREVIEW.scaleMode = ScaleMode.ScaleAndCrop;
		}

		var BUTTON_INVERT = new Button( ()=> InvertColors( _texture ) );
		{
			BUTTON_INVERT.SetEnabled( _texture!=null );
			BUTTON_INVERT.text = "Invert Colors";
		}

		var BUTTON_EDGES = new Button( ()=> EdgeDetect( _texture ) );
		{
			BUTTON_EDGES.SetEnabled( _texture!=null );
			BUTTON_EDGES.text = "Edge Detect";
		}
		
		var FIELD = new ObjectField();
		{
			FIELD.objectType = typeof(Texture2D);
			FIELD.value = _texture;
			FIELD.RegisterValueChangedCallback(
				(e) =>
				{
					var newTexture = e.newValue as Texture2D;
					if( newTexture!=null )
					{
						if( !newTexture.isReadable )
						{
							UnityEditor.EditorUtility.DisplayDialog(
								"Texture is not readable" ,
								$"Texture '{newTexture.name}' is not readable. Choose different texture or enable \"Read/Write Enabled\" for needs of this demonstration." ,
								"OK"
							);
							return;
						}
						_texture = newTexture;
						PREVIEW.image = newTexture;
						PREVIEW.SetEnabled( true );
						BUTTON_INVERT.SetEnabled( true );
						BUTTON_EDGES.SetEnabled( true );
					}
					else
					{
						_texture = null;
						PREVIEW.image = null;
						PREVIEW.SetEnabled( false );
						BUTTON_INVERT.SetEnabled( false );
						BUTTON_EDGES.SetEnabled( false );
					}
				}
			);
		}

		// add elemenets to root:
		rootVisualElement.Add( FIELD );
		rootVisualElement.Add( PREVIEW );
		rootVisualElement.Add( BUTTON_INVERT );
		rootVisualElement.Add( BUTTON_EDGES );
    }

	static void InvertColors ( Texture2D tex )
	{
		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new InvertRGB24Job { data = tex.GetRawTextureData<RGB24>() }
				.Run( rawdata.Length );
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new InvertRGBA32Job { data = tex.GetRawTextureData<RGBA32>() }
				.Run( rawdata.Length );
		}
		else if( tex.format==TextureFormat.R8 || tex.format==TextureFormat.Alpha8 )
		{
			var rawdata = tex.GetRawTextureData<byte>();
			new InvertByteJob { data = tex.GetRawTextureData<byte>() }
				.Run( rawdata.Length );
		}
		else if( tex.format==TextureFormat.R16 )
		{
			var rawdata = tex.GetRawTextureData<ushort>();
			new InvertR16Job { data = tex.GetRawTextureData<ushort>() }
				.Run( rawdata.Length );
		}
		else if( tex.format==TextureFormat.RGB565 )
		{
			var rawdata = tex.GetRawTextureData<RGB565>();
			new InvertRGB565Job { data = tex.GetRawTextureData<RGB565>() }
				.Run( rawdata.Length );
		}
		else
		{
			throw new System.NotImplementedException($"{tex.format} processing not implemented");
		}
		tex.Apply();
	}

	static void EdgeDetect ( Texture2D tex )
	{
		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new EdgeDetectRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width )
				.Run( rawdata.Length );
		}
		else
		{
			throw new System.NotImplementedException($"{tex.format} processing not implemented");
		}
		tex.Apply();
	}

    [UnityEditor.MenuItem("Test/Raw Texture Data/Processing Example")]
    static void CreateWindow () => UnityEditor.EditorWindow.GetWindow<RawTextureDataProcessingExamplesWindow>( nameof(RawTextureDataProcessingExamplesWindow) ).Show();

}
