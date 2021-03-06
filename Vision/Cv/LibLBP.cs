﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Cv
{
    public static class LibLBP
    {
        public static void PyrFeaturesSparse(ref UInt32[] vec, UInt32 vecNDim, UInt32[] imgData, UInt16 imgNumRows, UInt16 imgNumCols, UInt32 vecStartInd = 0)
        {
            uint offset, ww, hh, x, y, center, j, idx, x1hh, x11hh, xhh, y1, y11;
            byte pattern;

            idx = 0;
            offset = 0;
            ww = imgNumCols;
            hh = imgNumRows;
            while (true)
            {
                for (x = 1; x < ww - 1; x++)
                {
                    for (y = 1; y < hh - 1; y++)
                    {
                        xhh = x * imgNumRows;
                        x1hh = (x + 1) * imgNumRows;
                        x11hh = (x - 1) * imgNumRows;
                        y1 = y + 1;
                        y11 = y - 1;
                        pattern = 0;
                        center = imgData[y + xhh];
                        if (imgData[y11 + x11hh] < center) pattern |= 0x01;
                        if (imgData[y11 + xhh] < center) pattern |= 0x02;
                        if (imgData[y11 + x1hh] < center) pattern |= 0x04;
                        if (imgData[y + x11hh] < center) pattern |= 0x08;
                        if (imgData[y + x1hh] < center) pattern |= 0x10;
                        if (imgData[y1 + x11hh] < center) pattern |= 0x20;
                        if (imgData[y1 + xhh] < center) pattern |= 0x40;
                        if (imgData[y1 + x1hh] < center) pattern |= 0x80;

                        vec[vecStartInd + idx++] = offset + pattern;
                        offset += 256;
                    }
                }
                if (vecNDim <= idx)
                    return;

                if (ww % 2 == 1) ww--;
                if (hh % 2 == 1) hh--;

                ww = ww / 2;

                for (x = 0; x < ww; x++)
                {
                    for (j = 0; j < hh; j++)
                    {
                        imgData[GetIndex(j, x, imgNumRows)] = imgData[GetIndex(j, 2 * x, imgNumRows)] + imgData[GetIndex(j, 2 * x + 1, imgNumRows)];
                    }
                }

                hh = hh / 2;

                for (y = 0; y < hh; y++)
                {
                    for (j = 0; j < ww; j++)
                    {
                        imgData[GetIndex(y, j, imgNumRows)] = imgData[GetIndex(2 * y, j, imgNumRows)] + imgData[GetIndex(2 * y + 1, j, imgNumRows)];
                    }
                }
            }
        }

        public static UInt32 PyrGetDim(UInt16 imgNumRows, UInt16 imgNumCols, UInt16 NumPyramids)
        {
            UInt32 w, h, N, i;
            for (w = imgNumCols, h = imgNumRows, N = 0, i = 0; i < NumPyramids && Math.Min(w, h) >= 3; i++)
            {
                N += (w - 2) * (h - 2);

                if (w % 2 == 1) w--;
                if (h % 2 == 1) h--;
                w = w / 2;
                h = h / 2;
            }
            return (256 * N);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 GetIndex(UInt32 row, UInt32 col, UInt32 numRows)
        {
            return col * numRows + row;
        }
    }
}
