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

		var BUTTON_BLUR = new Button( ()=> Blur( _texture ) );
		{
			BUTTON_BLUR.SetEnabled( _texture!=null );
			BUTTON_BLUR.text = "Blur";
		}

		var BUTTON_GRAYSCALE = new Button( ()=> Grayscale( _texture ) );
		{
			BUTTON_GRAYSCALE.SetEnabled( _texture!=null );
			BUTTON_GRAYSCALE.text = "Grayscale";
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
						BUTTON_BLUR.SetEnabled( true );
						BUTTON_GRAYSCALE.SetEnabled( true );
					}
					else
					{
						_texture = null;
						PREVIEW.image = null;
						PREVIEW.SetEnabled( false );
						BUTTON_INVERT.SetEnabled( false );
						BUTTON_EDGES.SetEnabled( false );
						BUTTON_BLUR.SetEnabled( false );
						BUTTON_GRAYSCALE.SetEnabled( false );
					}
				}
			);
		}

		// add elemenets to root:
		rootVisualElement.Add( FIELD );
		rootVisualElement.Add( PREVIEW );
		rootVisualElement.Add( BUTTON_INVERT );
		rootVisualElement.Add( BUTTON_EDGES );
		rootVisualElement.Add( BUTTON_BLUR );
		rootVisualElement.Add( BUTTON_GRAYSCALE );
    }

	static void InvertColors ( Texture2D tex )
	{
		#if DEBUG
		var timeStart = System.DateTime.Now;
		#endif
		
		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new InvertRGB24Job { data = tex.GetRawTextureData<RGB24>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new InvertRGBA32Job { data = tex.GetRawTextureData<RGBA32>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.R8 || tex.format==TextureFormat.Alpha8 )
		{
			var rawdata = tex.GetRawTextureData<byte>();
			new InvertByteJob { data = tex.GetRawTextureData<byte>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.R16 )
		{
			var rawdata = tex.GetRawTextureData<ushort>();
			new InvertR16Job { data = tex.GetRawTextureData<ushort>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.RGB565 )
		{
			var rawdata = tex.GetRawTextureData<RGB565>();
			new InvertRGB565Job { data = tex.GetRawTextureData<RGB565>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");

		#if DEBUG
		var timeJobEnd = System.DateTime.Now;
		#endif

		tex.Apply();
		
		#if DEBUG
		var now = System.DateTime.Now;
		var timeJob = timeJobEnd - timeStart;
		var timeApply = now - timeJobEnd;
		Debug.Log($"{nameof(InvertColors)} took: {timeJob.TotalMilliseconds:0.00}ms + {timeApply.TotalMilliseconds:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void EdgeDetect ( Texture2D tex )
	{
		#if DEBUG
		var timeStart = System.DateTime.Now;
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new EdgeDetectRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width )
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new EdgeDetectRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width )
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");
		
		#if DEBUG
		var timeJobEnd = System.DateTime.Now;
		#endif

		tex.Apply();
		
		#if DEBUG
		var now = System.DateTime.Now;
		var timeJob = timeJobEnd - timeStart;
		var timeApply = now - timeJobEnd;
		Debug.Log($"{nameof(EdgeDetect)} took: {timeJob.TotalMilliseconds:0.00}ms + {timeApply.TotalMilliseconds:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void Blur ( Texture2D tex )
	{
		#if DEBUG
		var timeStart = System.DateTime.Now;
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new BlurRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width )
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new BlurRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width )
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");
		
		#if DEBUG
		var timeJobEnd = System.DateTime.Now;
		#endif

		tex.Apply();
		
		#if DEBUG
		var now = System.DateTime.Now;
		var timeJob = timeJobEnd - timeStart;
		var timeApply = now - timeJobEnd;
		Debug.Log($"{nameof(Blur)} took: {timeJob.TotalMilliseconds:0.00}ms + {timeApply.TotalMilliseconds:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void Grayscale ( Texture2D tex )
	{
		#if DEBUG
		var timeStart = System.DateTime.Now;
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new GrayscaleRGB24Job { data = tex.GetRawTextureData<RGB24>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new GrayscaleRGBA32Job { data = tex.GetRawTextureData<RGBA32>() }
				.Schedule( rawdata.Length , tex.width ).Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");
		
		#if DEBUG
		var timeJobEnd = System.DateTime.Now;
		#endif

		tex.Apply();
		
		#if DEBUG
		var now = System.DateTime.Now;
		var timeJob = timeJobEnd - timeStart;
		var timeApply = now - timeJobEnd;
		Debug.Log($"{nameof(Grayscale)} took: {timeJob.TotalMilliseconds:0.00}ms + {timeApply.TotalMilliseconds:0.00}ms (job execution + tex.Apply)");
		#endif
	}

    [UnityEditor.MenuItem("Test/Raw Texture Data/Processing Example")]
    static void CreateWindow () => UnityEditor.EditorWindow.GetWindow<RawTextureDataProcessingExamplesWindow>( nameof(RawTextureDataProcessingExamplesWindow) ).Show();

}
