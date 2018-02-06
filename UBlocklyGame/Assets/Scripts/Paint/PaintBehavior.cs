using UnityEngine;
using UnityEngine.UI;

namespace UBlocklyGame.Paint
{
	[RequireComponent(typeof(RawImage))]
	public class PaintBehavior : MonoBehaviour
	{
		[Tooltip("清除颜色")] public Color32 ClearColor = Color.clear;
		[Tooltip("画笔颜色")] public Color32 PaintColor = Color.white;
		[Tooltip("画笔半径")] public int PaintRadius = 24;
		
		[Tooltip("画布长")　] [SerializeField] private int m_TextureWidth;
		[Tooltip("画布宽")　] [SerializeField] private int m_TextureHeight;
		
		private byte[] mPixels;        //byte array for texture painting
		private Texture2D mPaintTex;    //texture used to paint into
		private bool mTextureNeedsUpdate;

		void OnDestroy()
		{
			if (mPaintTex != null)
				Texture2D.Destroy(mPaintTex);
		}

		/// <summary>
		/// Initialize painting behavior with texture width and height
		/// </summary>
		public void Init(int texWidth, int texHeight)
		{
			m_TextureWidth = texWidth;
			m_TextureHeight = texHeight;
	
			if (mPaintTex != null && (mPaintTex.width != texWidth || mPaintTex.height != texHeight))
			{
				Texture2D.Destroy(mPaintTex);
				mPaintTex = null;
			}
			if (mPaintTex == null)
			{
				mPaintTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
				mPaintTex.filterMode = FilterMode.Bilinear;
				mPaintTex.wrapMode = TextureWrapMode.Clamp;
			}

			RawImage rawImage = GetComponent<RawImage>();
			rawImage.texture = mPaintTex;
	
			mPixels = new byte[texWidth * texHeight * 4];
			
			Clear();
		}
	
		/// <summary>
		/// clear painting texture
		/// </summary>
		public void Clear()
		{
			int pixel = 0;
			for (int y = 0; y < m_TextureHeight; y++)
			{
				for (int x = 0; x < m_TextureWidth; x++)
				{
					mPixels[pixel] = ClearColor.r;
					mPixels[pixel + 1] = ClearColor.g;
					mPixels[pixel + 2] = ClearColor.b;
					mPixels[pixel + 3] = ClearColor.a;
					pixel += 4;
				}
			}
	
			mPaintTex.LoadRawTextureData(mPixels);
			mPaintTex.Apply(false);
		}
		
		void Update()
		{
			if (mTextureNeedsUpdate)
			{
				mTextureNeedsUpdate = false;
				mPaintTex.LoadRawTextureData (mPixels);
				mPaintTex.Apply (false);
			}
		}
	
		/// <summary>
		/// main painting function, http://stackoverflow.com/a/24453110
		/// </summary>
		public void DrawCircle(int x, int y)
		{
			int pixel = 0;
	
			// draw fast circle:
			int r2 = PaintRadius * PaintRadius;
			int area = r2 << 2;
			int rr = PaintRadius << 1;
			for (int i = 0; i < area; i++)
			{
				int tx = (i % rr) - PaintRadius;
				int ty = (i / rr) - PaintRadius;
				if (tx * tx + ty * ty < r2)
				{
					if (x + tx < 0 || y + ty < 0 || x + tx >= m_TextureWidth || y + ty >= m_TextureHeight) continue; // temporary fix for corner painting
	
					pixel = (m_TextureWidth * (y + ty) + x + tx) * 4;
	
					mPixels[pixel] = PaintColor.r;
					mPixels[pixel + 1] = PaintColor.g;
					mPixels[pixel + 2] = PaintColor.b;
					mPixels[pixel + 3] = PaintColor.a;
				}
			}

			mTextureNeedsUpdate = true;
		}
	
		/// <summary>
		/// draw line between 2 points (if moved too far/fast)
		/// http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		/// </summary>
		public void DrawLine(Vector2 start, Vector2 end)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			int minDistance=(int)(PaintRadius>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop)
			{
				pixelCount++;
				if (pixelCount >= minDistance)
				{
					pixelCount = 0;
					DrawCircle(x0, y0);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
			
			mTextureNeedsUpdate = true;
		}
	}
}