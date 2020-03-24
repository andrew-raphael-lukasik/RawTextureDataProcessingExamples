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
			PREVIEW.style.flexGrow = 1f;
		}

		var INVERT = new VisualElement();
		SetupStyle( INVERT );
		var BUTTON_INVERT = new Button( ()=> InvertColors( _texture ) );
		{
			BUTTON_INVERT.SetEnabled( _texture!=null );
			BUTTON_INVERT.text = "Invert Colors";
		}
		INVERT.Add( BUTTON_INVERT );

		var EDGES = new VisualElement();
		SetupStyle( EDGES );
		var BUTTON_EDGES = new Button( ()=> EdgeDetect( _texture ) );
		{
			BUTTON_EDGES.SetEnabled( _texture!=null );
			BUTTON_EDGES.text = "Edge Detect";
		}
		EDGES.Add( BUTTON_EDGES );

		var BOX_BLUR = new VisualElement();
		SetupStyle( BOX_BLUR , 46 );
		var SLIDER_BOX_BLUR = new SliderInt( 1 , 100 );
		{
			SLIDER_BOX_BLUR.value = 10;
		}
		var BUTTON_BOX_BLUR = new Button( ()=> BoxBlur( _texture , SLIDER_BOX_BLUR.value ) );
		{
			BUTTON_BOX_BLUR.SetEnabled( _texture!=null );
			BUTTON_BOX_BLUR.text = "Box Blur";
		}
		BOX_BLUR.Add( BUTTON_BOX_BLUR );
		BOX_BLUR.Add( SLIDER_BOX_BLUR );

		var GAUSSIAN_BLUR = new VisualElement();
		SetupStyle( GAUSSIAN_BLUR , 46 );
		var SLIDER_GAUSSIAN_BLUR = new SliderInt( 1 , 100 );
		{
			SLIDER_GAUSSIAN_BLUR.value = 10;
		}
		var BUTTON_GAUSSIAN_BLUR = new Button( ()=> GaussianBlur( _texture , SLIDER_GAUSSIAN_BLUR.value ) );
		{
			BUTTON_GAUSSIAN_BLUR.SetEnabled( _texture!=null );
			BUTTON_GAUSSIAN_BLUR.text = "Gaussian Blur";
		}
		GAUSSIAN_BLUR.Add( BUTTON_GAUSSIAN_BLUR );
		GAUSSIAN_BLUR.Add( SLIDER_GAUSSIAN_BLUR );
		
		var GRAYSCALE = new VisualElement();
		SetupStyle( GRAYSCALE );
		var BUTTON_GRAYSCALE = new Button( ()=> Grayscale( _texture ) );
		{
			BUTTON_GRAYSCALE.SetEnabled( _texture!=null );
			BUTTON_GRAYSCALE.text = "Grayscale";
		}
		GRAYSCALE.Add( BUTTON_GRAYSCALE );
		
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
						
						bool b = true;
						PREVIEW.SetEnabled(b);
						BUTTON_INVERT.SetEnabled(b);
						BUTTON_EDGES.SetEnabled(b);
						BUTTON_BOX_BLUR.SetEnabled(b);
						BUTTON_GAUSSIAN_BLUR.SetEnabled(b);
						BUTTON_GRAYSCALE.SetEnabled(b);
					}
					else
					{
						_texture = null;
						PREVIEW.image = null;

						bool b = false;
						PREVIEW.SetEnabled(b);
						BUTTON_INVERT.SetEnabled(b);
						BUTTON_EDGES.SetEnabled(b);
						BUTTON_BOX_BLUR.SetEnabled(b);
						BUTTON_GAUSSIAN_BLUR.SetEnabled(b);
						BUTTON_GRAYSCALE.SetEnabled(b);
					}
				}
			);
		}

		// add elemenets to root:
		rootVisualElement.Add( FIELD );
		rootVisualElement.Add( PREVIEW );
		rootVisualElement.Add( INVERT );
		rootVisualElement.Add( EDGES );
		rootVisualElement.Add( BOX_BLUR );
		rootVisualElement.Add( GAUSSIAN_BLUR );
		rootVisualElement.Add( GRAYSCALE );
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

	static void BoxBlur ( Texture2D tex , int radius )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new BoxBlurRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width , tex.height , radius )
				.Schedule().Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new BoxBlurRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width , tex.height , radius )
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

	static void GaussianBlur ( Texture2D tex , int radius )
	{
		#if DEBUG
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		#endif

		if( tex.format==TextureFormat.RGB24 )
		{
			var rawdata = tex.GetRawTextureData<RGB24>();
			new GaussianBlurRGB24Job( tex.GetRawTextureData<RGB24>() , tex.width , tex.height , radius )
				.Schedule().Complete();
		}
		else if( tex.format==TextureFormat.RGBA32 )
		{
			var rawdata = tex.GetRawTextureData<RGBA32>();
			new GaussianBlurRGBA32Job( tex.GetRawTextureData<RGBA32>() , tex.width , tex.height , radius )
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
	
	void SetupStyle ( VisualElement ve , int minHeight = 28 )
	{
		var style = ve.style;
		style.borderTopWidth = style.borderLeftWidth = style.borderRightWidth = style.borderBottomWidth = 1;
		style.borderTopColor = style.borderLeftColor = style.borderRightColor = style.borderBottomColor = new Color{ a=0.5f };
		style.paddingTop = style.paddingBottom = 4;
		style.paddingLeft = style.paddingRight = 8;

		style.minHeight = minHeight;
	}
}
