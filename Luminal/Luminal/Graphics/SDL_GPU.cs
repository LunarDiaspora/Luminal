using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SDL2;

namespace Luminal.Graphics
{
    public unsafe struct GPU_Rect
    {
        public float x;
        public float y;
        public float w;
        public float h;
    }

    public enum GPU_BlendPresetEnum
    {
        GPU_BLEND_NORMAL = 0,
        GPU_BLEND_PREMULTIPLIED_ALPHA = 1,
        GPU_BLEND_MULTIPLY = 2,
        GPU_BLEND_ADD = 3,
        GPU_BLEND_SUBTRACT = 4,
        GPU_BLEND_MOD_ALPHA = 5,
        GPU_BLEND_SET_ALPHA = 6,
        GPU_BLEND_SET = 7,
        GPU_BLEND_NORMAL_KEEP_ALPHA = 8,
        GPU_BLEND_NORMAL_ADD_ALPHA = 9,
        GPU_BLEND_NORMAL_FACTOR_ALPHA = 10
    }

    public static class GPU_InitFlagEnum
    {
        public static readonly uint GPU_INIT_ENABLE_VSYNC = 0x1;
        public static readonly uint GPU_INIT_DISABLE_VSYNC = 0x2;
        public static readonly uint GPU_INIT_DISABLE_DOUBLE_BUFFER = 0x4;
        public static readonly uint GPU_INIT_DISABLE_AUTO_VIRTUAL_RESOLUTION = 0x8;
        public static readonly uint GPU_INIT_REQUEST_COMPATIBILITY_PROFILE = 0x10;
        public static readonly uint GPU_INIT_USE_ROW_BY_ROW_TEXTURE_UPLOAD_FALLBACK = 0x20;
        public static readonly uint GPU_INIT_USE_COPY_TEXTURE_UPLOAD_FALLBACK = 0x40;
    }

    public unsafe class SDL_GPU
    {
        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GPU_Init(uint width, uint height, uint flags);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GPU_CopyImageFromSurface(IntPtr surface);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_FreeImage(IntPtr image);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_SetBlendMode(IntPtr image, GPU_BlendPresetEnum mode);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GPU_SetFullscreen(bool fullscreen, bool desktop_res);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_SetLineThickness(float thickness);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern GPU_Rect GPU_MakeRect(float x, float y, float w, float h);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_ClearColor(IntPtr target, SDL.SDL_Color color);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Flip(IntPtr target);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Quit();

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Blit(IntPtr image, ref GPU_Rect rect, IntPtr screen, float x, float y);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_BlitRotate(IntPtr image, ref GPU_Rect rect, IntPtr screen, float x, float y, float degrees);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_BlitScale(IntPtr image, ref GPU_Rect rect, IntPtr screen, float x, float y, float sx, float sy);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_BlitTransform(IntPtr image, ref GPU_Rect rect, IntPtr screen, float x, float y, float rotation, float sx, float sy);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_BlitTransformX(IntPtr image, ref GPU_Rect rect, IntPtr screen, float x, float y, float pivot_x, float pivot_y, float rotation, float sx, float sy);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Rectangle(IntPtr screen, float l, float t, float r, float b, SDL.SDL_Color c);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_RectangleFilled(IntPtr screen, float l, float t, float r, float b, SDL.SDL_Color c);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Line(IntPtr screen, float x1, float y1, float x2, float y2, SDL.SDL_Color c);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_Pixel(IntPtr screen, float x, float y, SDL.SDL_Color c);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_SetBlending(IntPtr image, bool enable);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_FlushBlitBuffer();

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_SetAnchor(IntPtr image, float x, float y);

        [DllImport("SDL2_gpu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GPU_SetPreInitFlags(uint flags);

    }
}
