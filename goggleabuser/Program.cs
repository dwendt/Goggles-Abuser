using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Web;
using System.Collections;

namespace goggleabuser
{
	class Program
	{
		//shit version of base64 from his javascript
		static String numToB64(int num) {
			String b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
			num = num+131072;
			String result = "";
			for (int i=0; i<3; i++) {
				result = b64[ num%64 ] + result;
				num = (int)num/64;
			}

			return result;

		}
		static void Main(string[] args)
		{
			String bitPath;
			int width, height;

			Console.WriteLine("Image path:");
			bitPath = Console.ReadLine();/*
			Console.WriteLine("Enter size in form (width(enter)height)");
			width = Convert.ToInt32(Console.ReadLine());
			height = Convert.ToInt32(Console.ReadLine());
			*/


			Bitmap safeBit = new Bitmap(bitPath);

			width = safeBit.Width;
			height = safeBit.Height;

			IList<PointData> dotArray = new List<PointData>();

			UnsafeBitmap fastBit = new UnsafeBitmap(safeBit);
			fastBit.LockBitmap();
			
			for(int x=0; x<width; x+=5) {
				for(int y=0; y<height; y+=5) {
					PixelData pixel = fastBit.GetPixel(x, y);
//					int[] temparr = new int[] { pixel.red, pixel.blue, pixel.green, x, y };

//					for(int z=0; z<5; z++) {
					dotArray.Add(new PointData((int)pixel.red, (int)pixel.blue, (int)pixel.green, x, y));
//					}
//					dotArray[x,y] = new int[] {pixel.red, pixel.blue, pixel.green, x, y};
					Console.WriteLine("Dot at " + x + " " + y + " is " + pixel.red + " " + pixel.green + " " + pixel.blue);
				}
			}


			//Shape send data bullshit: {page=theurl, add=t, r=red, g=green, b=blue, a=alpha(1), t=20, p=base64points}
			//URL shit: http://goggles.sneakygcr.net/page?jsonp=jQuery#######_#####&page=http://shit.com/&add=t&title=bullshit&r=0&g=0&b=0&a=1&t=20&p=gDfrtSagb&_=somebigIDnumber _ is actually just to circumvent browsers cacheing it
			
			int scale = 1;
			int yoffset = 1800;
			int xoffset = 0;
			int dotdiameter = 7; //sort of good formula for this: 7 * scale + (scale-1)


			for(int i=0; i<dotArray.Count;i++) {
					PointData dot = dotArray[i];
//					if(dot == null) { Console.WriteLine("EOF"); break; }

					String pointSerialized = numToB64(dot.x * scale + xoffset) + numToB64(dot.y * scale + yoffset);
					//note: after the TLD in URLs, the shitty javascript app appends a second /. ex: http://google.com/maps/myHouse would be http://google.com//maps/myHouse according to goggles
					String fullURL = "http://goggles.sneakygcr.net/page?callback=fuckoff&page=" + HttpUtility.UrlEncode("http://www.minecraftforum.net//topic/730306-my-little-pony-fim-all-pony-related-things-go-here/") + "&add=t&title=" + HttpUtility.UrlEncode("dongs dongs dongs") + "&r=" + dot.red + "&g=" + dot.green + "&b=" + dot.blue + "&a=1&t=" + dotdiameter + "&p=" + HttpUtility.UrlEncode(pointSerialized) + "&_=1420891562000"; //+ HttpUtility.UrlEncode(pointSerialized) + "&_=99999999";

					Console.WriteLine(i + "/" + dotArray.Count + ": Doing URL " + fullURL);

					WebRequest req = WebRequest.Create(fullURL);
					req.Method = "GET";

					WebResponse resp = req.GetResponse();
					Console.WriteLine(((HttpWebResponse)resp).StatusDescription);

//					Stream dataStream = resp.GetResponseStream();
//					StreamReader reader = new StreamReader(dataStream);
//					Console.WriteLine(reader.ReadToEnd());

//					reader.Close();
//					dataStream.Close();
					resp.Close();
					System.Threading.Thread.Sleep(80);
//					if(i%200 == 0) {
//						System.Threading.Thread.Sleep(20000);
//					}
			}

			Console.ReadLine();

		}
	}

	public unsafe class UnsafeBitmap
	{
		Bitmap bitmap;

		// three elements used for MakeGreyUnsafe
		int width;
		BitmapData bitmapData = null;
		Byte* pBase = null;

		public UnsafeBitmap(Bitmap bitmap)
		{
			this.bitmap = new Bitmap(bitmap);
		}

		public UnsafeBitmap(int width, int height)
		{
			this.bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
		}

		public void Dispose()
		{
			bitmap.Dispose();
		}

		public Bitmap Bitmap
		{
			get
			{
				return (bitmap);
			}
		}

		private Point PixelSize
		{
			get
			{
				GraphicsUnit unit = GraphicsUnit.Pixel;
				RectangleF bounds = bitmap.GetBounds(ref unit);

				return new Point((int)bounds.Width, (int)bounds.Height);
			}
		}

		public void LockBitmap()
		{
			GraphicsUnit unit = GraphicsUnit.Pixel;
			RectangleF boundsF = bitmap.GetBounds(ref unit);
			Rectangle bounds = new Rectangle((int)boundsF.X,
			(int)boundsF.Y,
			(int)boundsF.Width,
			(int)boundsF.Height);

			// Figure out the number of bytes in a row
			// This is rounded up to be a multiple of 4
			// bytes, since a scan line in an image must always be a multiple of 4 bytes
			// in length. 
			width = (int)boundsF.Width * sizeof(PixelData);
			if (width % 4 != 0)
			{
				width = 4 * (width / 4 + 1);
			}
			bitmapData =
			bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			pBase = (Byte*)bitmapData.Scan0.ToPointer();
		}

		public PixelData GetPixel(int x, int y)
		{
			PixelData returnValue = *PixelAt(x, y);
			return returnValue;
		}

		public void SetPixel(int x, int y, PixelData colour)
		{
			PixelData* pixel = PixelAt(x, y);
			*pixel = colour;
		}

		public void UnlockBitmap()
		{
			bitmap.UnlockBits(bitmapData);
			bitmapData = null;
			pBase = null;
		}
		public PixelData* PixelAt(int x, int y)
		{
			return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
		}
	}

	public struct PixelData
	{
		public byte blue;
		public byte green;
		public byte red;
	}

	public struct PointData
	{

		public int red;
		public int green;
		public int blue;
		public int x;
		public int y;

		public PointData(int red, int blue, int green, int x, int y)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.x = x;
			this.y = y;
		}
	}

}
