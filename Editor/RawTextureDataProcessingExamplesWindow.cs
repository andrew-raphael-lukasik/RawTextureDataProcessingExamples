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

		var BUTTON_BOX_BLUR = new Button( ()=> BoxBlur( _texture ) );
		{
			BUTTON_BOX_BLUR.SetEnabled( _texture!=null );
			BUTTON_BOX_BLUR.text = "Box Blur";
		}
		var BUTTON_GAUSSIAN_BLUR = new Button( ()=> GaussianBlur( _texture ) );
		{
			BUTTON_GAUSSIAN_BLUR.SetEnabled( _texture!=null );
			BUTTON_GAUSSIAN_BLUR.text = "Gaussian Blur";
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
						bool newEnabled = true;
						PREVIEW.SetEnabled(newEnabled);
						BUTTON_INVERT.SetEnabled(newEnabled);
						BUTTON_EDGES.SetEnabled(newEnabled);
						BUTTON_BOX_BLUR.SetEnabled(newEnabled);
						BUTTON_GAUSSIAN_BLUR.SetEnabled(newEnabled);
						BUTTON_GRAYSCALE.SetEnabled(newEnabled);
					}
					else
					{
						_texture = null;
						PREVIEW.image = null;
						bool newEnabled = false;
						PREVIEW.SetEnabled(newEnabled);
						BUTTON_INVERT.SetEnabled(newEnabled);
						BUTTON_EDGES.SetEnabled(newEnabled);
						BUTTON_BOX_BLUR.SetEnabled(newEnabled);
						BUTTON_GAUSSIAN_BLUR.SetEnabled(newEnabled);
						BUTTON_GRAYSCALE.SetEnabled(newEnabled);
					}
				}
			);
		}

		// add elemenets to root:
		rootVisualElement.Add( FIELD );
		rootVisualElement.Add( PREVIEW );
		rootVisualElement.Add( BUTTON_INVERT );
		rootVisualElement.Add( BUTTON_EDGES );
		rootVisualElement.Add( BUTTON_BOX_BLUR );
		rootVisualElement.Add( BUTTON_GAUSSIAN_BLUR );
		rootVisualElement.Add( BUTTON_GRAYSCALE );
	}

	static void InvertColors ( Texture2D tex )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
		var timeJob = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();
		#endif

		tex.Apply();
		
		#if DEBUG
		var timeApply = stopwatch.Elapsed.TotalMilliseconds;
		Debug.Log($"{nameof(InvertColors)} took: {timeJob:0.00}ms + {timeApply:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void EdgeDetect ( Texture2D tex )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
		var timeJob = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();
		#endif

		tex.Apply();
		
		#if DEBUG
		var timeApply = stopwatch.Elapsed.TotalMilliseconds;
		Debug.Log($"{nameof(EdgeDetect)} took: {timeJob:0.00}ms + {timeApply:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void BoxBlur ( Texture2D tex )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new BoxBlurRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width , tex.height , 10 )
				.Schedule().Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new BoxBlurRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width , tex.height , 10 )
				.Schedule().Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");
		
		#if DEBUG
		var timeJob = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();
		#endif

		tex.Apply();
		
		#if DEBUG
		var timeApply = stopwatch.Elapsed.TotalMilliseconds;
		Debug.Log($"{nameof(BoxBlur)} took: {timeJob:0.00}ms + {timeApply:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void GaussianBlur ( Texture2D tex )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new GaussianBlurRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width , tex.height , 10 )
				.Schedule().Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new GaussianBlurRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width , tex.height , 10 )
				.Schedule().Complete();
		}
		else throw new System.NotImplementedException($"{tex.format} processing not implemented");
		
		#if DEBUG
		var timeJob = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();
		#endif

		tex.Apply();
		
		#if DEBUG
		var timeApply = stopwatch.Elapsed.TotalMilliseconds;
		Debug.Log($"{nameof(GaussianBlur)} took: {timeJob:0.00}ms + {timeApply:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	static void Grayscale ( Texture2D tex )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
		var timeJob = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();
		#endif

		tex.Apply();
		
		#if DEBUG
		var timeApply = stopwatch.Elapsed.TotalMilliseconds;
		Debug.Log($"{nameof(Grayscale)} took: {timeJob:0.00}ms + {timeApply:0.00}ms (job execution + tex.Apply)");
		#endif
	}

	[UnityEditor.MenuItem("Test/Raw Texture Data/Processing Example")]
	static void CreateWindow () => UnityEditor.EditorWindow.GetWindow<RawTextureDataProcessingExamplesWindow>( nameof(RawTextureDataProcessingExamplesWindow) ).Show();

}
