﻿using Inventory.MobileApp.Utilities.Code128;
using SkiaSharp;

namespace Inventory.MobileApp.Services;

public static class BarcodeService
{
    private const int CQuietWidth = 10;

    private static readonly int[,] CPatterns = new int[107, 8]
    {
        { 2, 1, 2, 2, 2, 2, 0, 0 },
        { 2, 2, 2, 1, 2, 2, 0, 0 },
        { 2, 2, 2, 2, 2, 1, 0, 0 },
        { 1, 2, 1, 2, 2, 3, 0, 0 },
        { 1, 2, 1, 3, 2, 2, 0, 0 },
        { 1, 3, 1, 2, 2, 2, 0, 0 },
        { 1, 2, 2, 2, 1, 3, 0, 0 },
        { 1, 2, 2, 3, 1, 2, 0, 0 },
        { 1, 3, 2, 2, 1, 2, 0, 0 },
        { 2, 2, 1, 2, 1, 3, 0, 0 },
        { 2, 2, 1, 3, 1, 2, 0, 0 },
        { 2, 3, 1, 2, 1, 2, 0, 0 },
        { 1, 1, 2, 2, 3, 2, 0, 0 },
        { 1, 2, 2, 1, 3, 2, 0, 0 },
        { 1, 2, 2, 2, 3, 1, 0, 0 },
        { 1, 1, 3, 2, 2, 2, 0, 0 },
        { 1, 2, 3, 1, 2, 2, 0, 0 },
        { 1, 2, 3, 2, 2, 1, 0, 0 },
        { 2, 2, 3, 2, 1, 1, 0, 0 },
        { 2, 2, 1, 1, 3, 2, 0, 0 },
        { 2, 2, 1, 2, 3, 1, 0, 0 },
        { 2, 1, 3, 2, 1, 2, 0, 0 },
        { 2, 2, 3, 1, 1, 2, 0, 0 },
        { 3, 1, 2, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 2, 2, 2, 0, 0 },
        { 3, 2, 1, 1, 2, 2, 0, 0 },
        { 3, 2, 1, 2, 2, 1, 0, 0 },
        { 3, 1, 2, 2, 1, 2, 0, 0 },
        { 3, 2, 2, 1, 1, 2, 0, 0 },
        { 3, 2, 2, 2, 1, 1, 0, 0 },
        { 2, 1, 2, 1, 2, 3, 0, 0 },
        { 2, 1, 2, 3, 2, 1, 0, 0 },
        { 2, 3, 2, 1, 2, 1, 0, 0 },
        { 1, 1, 1, 3, 2, 3, 0, 0 },
        { 1, 3, 1, 1, 2, 3, 0, 0 },
        { 1, 3, 1, 3, 2, 1, 0, 0 },
        { 1, 1, 2, 3, 1, 3, 0, 0 },
        { 1, 3, 2, 1, 1, 3, 0, 0 },
        { 1, 3, 2, 3, 1, 1, 0, 0 },
        { 2, 1, 1, 3, 1, 3, 0, 0 },
        { 2, 3, 1, 1, 1, 3, 0, 0 },
        { 2, 3, 1, 3, 1, 1, 0, 0 },
        { 1, 1, 2, 1, 3, 3, 0, 0 },
        { 1, 1, 2, 3, 3, 1, 0, 0 },
        { 1, 3, 2, 1, 3, 1, 0, 0 },
        { 1, 1, 3, 1, 2, 3, 0, 0 },
        { 1, 1, 3, 3, 2, 1, 0, 0 },
        { 1, 3, 3, 1, 2, 1, 0, 0 },
        { 3, 1, 3, 1, 2, 1, 0, 0 },
        { 2, 1, 1, 3, 3, 1, 0, 0 },
        { 2, 3, 1, 1, 3, 1, 0, 0 },
        { 2, 1, 3, 1, 1, 3, 0, 0 },
        { 2, 1, 3, 3, 1, 1, 0, 0 },
        { 2, 1, 3, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 1, 2, 3, 0, 0 },
        { 3, 1, 1, 3, 2, 1, 0, 0 },
        { 3, 3, 1, 1, 2, 1, 0, 0 },
        { 3, 1, 2, 1, 1, 3, 0, 0 },
        { 3, 1, 2, 3, 1, 1, 0, 0 },
        { 3, 3, 2, 1, 1, 1, 0, 0 },
        { 3, 1, 4, 1, 1, 1, 0, 0 },
        { 2, 2, 1, 4, 1, 1, 0, 0 },
        { 4, 3, 1, 1, 1, 1, 0, 0 },
        { 1, 1, 1, 2, 2, 4, 0, 0 },
        { 1, 1, 1, 4, 2, 2, 0, 0 },
        { 1, 2, 1, 1, 2, 4, 0, 0 },
        { 1, 2, 1, 4, 2, 1, 0, 0 },
        { 1, 4, 1, 1, 2, 2, 0, 0 },
        { 1, 4, 1, 2, 2, 1, 0, 0 },
        { 1, 1, 2, 2, 1, 4, 0, 0 },
        { 1, 1, 2, 4, 1, 2, 0, 0 },
        { 1, 2, 2, 1, 1, 4, 0, 0 },
        { 1, 2, 2, 4, 1, 1, 0, 0 },
        { 1, 4, 2, 1, 1, 2, 0, 0 },
        { 1, 4, 2, 2, 1, 1, 0, 0 },
        { 2, 4, 1, 2, 1, 1, 0, 0 },
        { 2, 2, 1, 1, 1, 4, 0, 0 },
        { 4, 1, 3, 1, 1, 1, 0, 0 },
        { 2, 4, 1, 1, 1, 2, 0, 0 },
        { 1, 3, 4, 1, 1, 1, 0, 0 },
        { 1, 1, 1, 2, 4, 2, 0, 0 },
        { 1, 2, 1, 1, 4, 2, 0, 0 },
        { 1, 2, 1, 2, 4, 1, 0, 0 },
        { 1, 1, 4, 2, 1, 2, 0, 0 },
        { 1, 2, 4, 1, 1, 2, 0, 0 },
        { 1, 2, 4, 2, 1, 1, 0, 0 },
        { 4, 1, 1, 2, 1, 2, 0, 0 },
        { 4, 2, 1, 1, 1, 2, 0, 0 },
        { 4, 2, 1, 2, 1, 1, 0, 0 },
        { 2, 1, 2, 1, 4, 1, 0, 0 },
        { 2, 1, 4, 1, 2, 1, 0, 0 },
        { 4, 1, 2, 1, 2, 1, 0, 0 },
        { 1, 1, 1, 1, 4, 3, 0, 0 },
        { 1, 1, 1, 3, 4, 1, 0, 0 },
        { 1, 3, 1, 1, 4, 1, 0, 0 },
        { 1, 1, 4, 1, 1, 3, 0, 0 },
        { 1, 1, 4, 3, 1, 1, 0, 0 },
        { 4, 1, 1, 1, 1, 3, 0, 0 },
        { 4, 1, 1, 3, 1, 1, 0, 0 },
        { 1, 1, 3, 1, 4, 1, 0, 0 },
        { 1, 1, 4, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 1, 4, 1, 0, 0 },
        { 4, 1, 1, 1, 3, 1, 0, 0 },
        { 2, 1, 1, 4, 1, 2, 0, 0 },
        { 2, 1, 1, 2, 1, 4, 0, 0 },
        { 2, 1, 1, 2, 3, 2, 0, 0 },
        { 2, 3, 3, 1, 1, 1, 2, 0 }
    };

    public static void DrawCode128Barcode(
        string inputData, 
        SKCanvas canvas, 
        SKImageInfo info, 
        int barWeight = 5)
    {
        // get the Code128 codes to represent the message
        var content = new Code128Content(inputData);
        var codes = content.Codes;

        var width = info.Width;
        var height = info.Height - 32;

        // set to white so we don't have to fill the spaces with white
        canvas.Clear(SKColors.White);

        // calculate total barcode width
        int totalWidth = 0;
        for (var codeIdx = 0; codeIdx < codes.Length; codeIdx++)
        {
            var code = codes[codeIdx];
            for (var bar = 0; bar < 8; bar += 2)
            {
                var barWidth = CPatterns[code, bar] * barWeight;
                var spcWidth = CPatterns[code, bar + 1] * barWeight;
                totalWidth += barWidth + spcWidth;
            }
        }

        // calculate starting position of the cursor to center barcode
        var cursor = (width - totalWidth) / 2;

        for (var codeIdx = 0; codeIdx < codes.Length; codeIdx++)
        {
            var code = codes[codeIdx];

            // take the bars two at a time: a black and a white
            for (var bar = 0; bar < 8; bar += 2)
            {
                var barWidth = CPatterns[code, bar] * barWeight;
                var spcWidth = CPatterns[code, bar + 1] * barWeight;

                // if width is zero, don't try to draw it
                if (barWidth > 0)
                {
                    SKPaint paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = SKColors.Black
                    };

                    SKRect rect = new SKRect(cursor, 0, cursor + barWidth, height - 32);
                    canvas.DrawRect(rect, paint);
                }

                // advance cursor beyond this pair
                cursor += barWidth + spcWidth;
            }
        }

        canvas.DrawText(
            inputData, 
            new SKPoint(width * 0.5f, height), 
            new SKPaint 
            { 
                Style = SKPaintStyle.Fill, 
                Color = SKColors.Black, 
                TextSize = 32, 
                FakeBoldText = true, 
                TextEncoding = SKTextEncoding.Utf8,
                TextAlign = SKTextAlign.Center
            });
    }
}




   