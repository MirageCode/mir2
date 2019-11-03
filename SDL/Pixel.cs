using System;

namespace SDL
{
    public enum PixelType
    {
        Unknown,
        Index1,
        Index4,
        Index8,
        Packed8,
        Packed16,
        Packed32,
        ArrayU8,
        ArrayU16,
        ArrayU32,
        ArrayF16,
        ArrayF32,
    }

    public enum PixelOrder
    {
        /* BITMAPORDER */
        Bitmap_None,
        Bitmap_4321,
        Bitmap_1234,
        /* PACKEDORDER */
        Packed_None = 0,
        Packed_XRGB,
        Packed_RGBX,
        Packed_ARGB,
        Packed_RGBA,
        Packed_XBGR,
        Packed_BGRX,
        Packed_ABGR,
        Packed_BGRA,
        /* ARRAYORDER */
        Array_None = 0,
        Array_RGB,
        Array_RGBA,
        Array_ARGB,
        Array_BGR,
        Array_BGRA,
        Array_ABGR,
    }

    public enum PackedLayout
    {
        None,
        _332,
        _4444,
        _1555,
        _5551,
        _565,
        _8888,
        _2101010,
        _1010102,
    }

    public enum PixelFormat : uint
    {
        Unknown,

        Index1LSB = ((1 << 28) | (PixelType.Index1 << 24) |
                     (PixelOrder.Bitmap_4321 << 20) | (1 << 8)),
        Index1MSB = ((1 << 28) | (PixelType.Index1 << 24) |
                     (PixelOrder.Bitmap_1234 << 20) | (1 << 8)),
        Index4LSB = ((1 << 28) | (PixelType.Index4 << 24) |
                     (PixelOrder.Bitmap_4321 << 20) | (4 << 8)),
        Index4MSB = ((1 << 28) | (PixelType.Index4 << 24) |
                     (PixelOrder.Bitmap_1234 << 20) | (4 << 8)),

        Index8 = ((1 << 28) | (PixelType.Index8 << 24) | (8 << 8) | (1 << 0)),

        RGB332 = ((1 << 28) | (PixelType.Packed8 << 24) |
                  (PixelOrder.Packed_XRGB << 20) | (PackedLayout._332 << 16) |
                  (8 << 8) | (1 << 0)),
        RGB444 = ((1 << 28) | (PixelType.Packed16 << 24) |
                  (PixelOrder.Packed_XRGB << 20) | (PackedLayout._4444 << 16) |
                  (12 << 8) | (2 << 0)),
        RGB555 = ((1 << 28) | (PixelType.Packed16 << 24) |
                  (PixelOrder.Packed_XRGB << 20) | (PackedLayout._1555 << 16) |
                  (15 << 8) | (2 << 0)),
        BGR555 = ((1 << 28) | (PixelType.Packed16 << 24) |
                  (PixelOrder.Packed_XBGR << 20) | (PackedLayout._1555 << 16) |
                  (15 << 8) | (2 << 0)),

        ARGB444 = ((1 << 28) | (PixelType.Packed16 << 24) |
                   (PixelOrder.Packed_ARGB << 20) | (PackedLayout._4444 << 16) |
                   (16 << 8) | (2 << 0)),
        RGBA444 = ((1 << 28) | (PixelType.Packed16 << 24) |
                   (PixelOrder.Packed_RGBA << 20) | (PackedLayout._4444 << 16) |
                   (16 << 8) | (2 << 0)),
        ABGR444 = ((1 << 28) | (PixelType.Packed16 << 24) |
                   (PixelOrder.Packed_ABGR << 20) | (PackedLayout._4444 << 16) |
                   (16 << 8) | (2 << 0)),
        BGRA444 = ((1 << 28) | (PixelType.Packed16 << 24) |
                   (PixelOrder.Packed_BGRA << 20) | (PackedLayout._4444 << 16) |
                   (16 << 8) | (2 << 0)),

        ARGB1555 = ((1 << 28) | (PixelType.Packed16 << 24) |
                    (PixelOrder.Packed_ARGB << 20) | (PackedLayout._1555 << 16) |
                    (16 << 8) | (2 << 0)),
        RGBA5551 = ((1 << 28) | (PixelType.Packed16 << 24) |
                    (PixelOrder.Packed_RGBA << 20) | (PackedLayout._5551 << 16) |
                    (16 << 8) | (2 << 0)),
        ABGR1555 = ((1 << 28) | (PixelType.Packed16 << 24) |
                    (PixelOrder.Packed_ABGR << 20) | (PackedLayout._1555 << 16) |
                    (16 << 8) | (2 << 0)),
        BGRA5551 = ((1 << 28) | (PixelType.Packed16 << 24) |
                    (PixelOrder.Packed_BGRA << 20) | (PackedLayout._5551 << 16) |
                    (16 << 8) | (2 << 0)),

        RGB565 = ((1 << 28) | (PixelType.Packed16 << 24) |
                  (PixelOrder.Packed_XRGB << 20) | (PackedLayout._565 << 16) |
                  (16 << 8) | (2 << 0)),
        BGR565 = ((1 << 28) | (PixelType.Packed16 << 24) |
                  (PixelOrder.Packed_XBGR << 20) | (PackedLayout._565 << 16) |
                  (16 << 8) | (2 << 0)),

        RGB24 = ((1 << 28) | (PixelType.ArrayU8 << 24) |
                 (PixelOrder.Array_RGB << 20) | (24 << 8) | (3 << 0)),
        BGR24 = ((1 << 28) | (PixelType.ArrayU8 << 24) |
                 (PixelOrder.Array_BGR << 20) | (24 << 8) | (3 << 0)),

        RGB888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                  (PixelOrder.Packed_XRGB << 20) | (PackedLayout._8888 << 16) |
                  (24 << 8) | (4 << 0)),
        RGBX8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_RGBX << 20) | (PackedLayout._8888 << 16) |
                    (24 << 8) | (4 << 0)),
        BGR888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                  (PixelOrder.Packed_XBGR << 20) | (PackedLayout._8888 << 16) |
                  (24 << 8) | (4 << 0)),
        BGRX8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_BGRX << 20) | (PackedLayout._8888 << 16) |
                    (24 << 8) | (4 << 0)),

        ARGB8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_ARGB << 20) | (PackedLayout._8888 << 16) |
                    (32 << 8) | (4 << 0)),
        RGBA8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_RGBA << 20) | (PackedLayout._8888 << 16) |
                    (32 << 8) | (4 << 0)),
        ABGR8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_ABGR << 20) | (PackedLayout._8888 << 16) |
                    (32 << 8) | (4 << 0)),
        BGRA8888 = ((1 << 28) | (PixelType.Packed32 << 24) |
                    (PixelOrder.Packed_BGRA << 20) | (PackedLayout._8888 << 16) |
                    (32 << 8) | (4 << 0)),

        ARGB2101010 = ((1 << 28) | (PixelType.Packed32 << 24) |
                       (PixelOrder.Packed_ARGB << 20) | (PackedLayout._2101010 << 16) |
                       (32 << 8) | (4 << 0)),

        YV12 = ('Y' | ('V' << 8) | ('1' << 16) | ('2' << 24)),
        IYUV = ('I' | ('Y' << 8) | ('U' << 16) | ('V' << 24)),
        YUY2 = ('Y' | ('U' << 8) | ('Y' << 16) | ('V' << 24)),
        UYVY = ('U' | ('Y' << 8) | ('V' << 16) | ('Y' << 24)),
        YVYU = ('Y' | ('V' << 8) | ('Y' << 16) | ('U' << 24)),
    }
}
