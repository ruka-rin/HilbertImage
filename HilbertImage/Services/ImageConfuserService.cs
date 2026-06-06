using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace HilbertImage.Services;

public static class ImageConfuserService
{
    private static List<PixelPoint> Hilbert2D(int width, int height)
    {
        List<PixelPoint> coordinates = [];
        if (width >= height)
            GenerateCurve(0, 0, width, 0, 0, height, coordinates);
        else
            GenerateCurve(0, 0, 0, height, width, 0, coordinates);
        return coordinates;
    }
    
    private static void GenerateCurve(int x, int y, int ax, int ay, int bx, int by, List<PixelPoint> coordinates)
    {
        int w = Math.Abs(ax + ay);
        int h = Math.Abs(bx + by);

        int dax = Math.Sign(ax), day = Math.Sign(ay);
        int dbx = Math.Sign(bx), dby = Math.Sign(by);

        if (h == 1)
        {
            for (int i = 0; i < w; i++)
            {
                coordinates.Add(new PixelPoint(x, y));
                x += dax;
                y += day;
            }
            return;
        }

        if (w == 1)
        {
            for (int i = 0; i < h; i++)
            {
                coordinates.Add(new PixelPoint(x, y));
                x += dbx;
                y += dby;
            }
            return;
        }

        static int JsDivByTwo(int x) => (int)Math.Floor(x / 2.0);

        int ax2 = JsDivByTwo(ax), ay2 = JsDivByTwo(ay);
        int bx2 = JsDivByTwo(bx), by2 = JsDivByTwo(by);

        int w2 = Math.Abs(ax2 + ay2);
        int h2 = Math.Abs(bx2 + by2);


        if (2 * w > 3 * h)
        {
            if ((w2 % 2) != 0 && (w > 2))
            {
                ax2 += dax;
                ay2 += day;
            }

            GenerateCurve(x, y, ax2, ay2, bx, by, coordinates);
            GenerateCurve(x + ax2, y + ay2, ax - ax2, ay - ay2, bx, by, coordinates);

        }
        else
        {
            if ((h2 % 2) != 0 && (h > 2))
            {
                bx2 += dbx;
                by2 += dby;
            }

            GenerateCurve(x, y, bx2, by2, ax2, ay2, coordinates);
            GenerateCurve(x + bx2, y + by2, ax, ay, bx - bx2, by - by2, coordinates);
            GenerateCurve(x + (ax - dax) + (bx2 - dbx), y + (ay - day) + (by2 - dby),
                -bx2, -by2, -(ax - ax2), -(ay - ay2), coordinates);
        }
    }

    public static Bitmap Confuse(Bitmap bitmap, bool recover = false)
    {
        var size = bitmap.PixelSize;
        var source = new WriteableBitmap(size, bitmap.Dpi, PixelFormat.Rgba8888, AlphaFormat.Unpremul);
        var result = new WriteableBitmap(size, bitmap.Dpi, PixelFormat.Rgba8888, AlphaFormat.Unpremul);
        using var sourceBuffer = source.Lock();
        using var resultBuffer = result.Lock();
        bitmap.CopyPixels(sourceBuffer);
        
        var curve = Hilbert2D(size.Width, size.Height);
        int offset = (int)Math.Round((Math.Sqrt(5) - 1) / 2 * size.Width * size.Height);
        
        for (int i = 0; i < size.Width * size.Height; i++)
        {
            var oldPointPos = curve[i];
            var newPointPos = curve[(i + offset) % (size.Width * size.Height)];
            var oldOffset = (oldPointPos.X + oldPointPos.Y * size.Width) * sizeof(int);
            var newOffset = (newPointPos.X + newPointPos.Y * size.Width) * sizeof(int);
            var newVal = Marshal.ReadInt32(sourceBuffer.Address, newOffset);
            var oldVal = Marshal.ReadInt32(sourceBuffer.Address, oldOffset);
            if (recover)
                Marshal.WriteInt32(resultBuffer.Address, newOffset, oldVal);
            else
                Marshal.WriteInt32(resultBuffer.Address, oldOffset, newVal);
        }
        return result;
    }
}