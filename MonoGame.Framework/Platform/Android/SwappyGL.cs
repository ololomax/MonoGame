using System;
using System.IO;
using System.Runtime.InteropServices;
using Java.Interop;
using Javax.Microedition.Khronos.Egl;
using MonoGame.Utilities;

namespace MonoGame.OpenGL
{
    internal static class SwappyGL
    {
        internal static IntPtr NativeLibrary = GetNativeLibrary();

        internal static IntPtr GetNativeLibrary()
        {
            var ret = FuncLoader.LoadLibrary("libswappy.so");

            if (ret == IntPtr.Zero)
            {
                var appFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var appDir = Path.GetDirectoryName(appFilesDir);
                var lib = Path.Combine(appDir, "lib", "libswappy.so");

                ret = FuncLoader.LoadLibrary(lib);
            }

            return ret;
        }

        //* @brief Initialize Swappy, getting the required Android parameters from the display subsystem via JNI.
        //* @param env The JNI environment where Swappy is used
        //* @param jactivity The activity where Swappy is used
        //* @return false if Swappy failed to initialize.
        //* @see SwappyGL_destroy
        //static inline bool SwappyGL_init(JNIEnv* env, jobject jactivity);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_swappygl_init_internal(IntPtr env, IntPtr jactivity);
        internal static d_swappygl_init_internal Init = FuncLoader.LoadFunction<d_swappygl_init_internal>(NativeLibrary, "SwappyGL_init_internal");

        //* @brief Check if Swappy was successfully initialized.
        //* @return false if either the `swappy.disable` system property is not `false`
        //* or the required OpenGL extensions are not available for Swappy to work.
        //bool SwappyGL_isEnabled();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_swappygl_isenabled();
        internal static d_swappygl_isenabled IsEnabled = FuncLoader.LoadFunction<d_swappygl_isenabled>(NativeLibrary, "SwappyGL_isEnabled");

        //* @brief Destroy resources and stop all threads that Swappy has created.
        //* @see SwappyGL_init
        //void SwappyGL_destroy();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_swappygl_destroy();
        internal static d_swappygl_destroy Destroy = FuncLoader.LoadFunction<d_swappygl_destroy>(NativeLibrary, "SwappyGL_destroy");

        //* @brief Replace calls to eglSwapBuffers with this. Swappy will wait for the previous frame's
        //* buffer to be processed by the GPU before actually calling eglSwapBuffers.
        //bool SwappyGL_swap(EGLDisplay display, EGLSurface surface);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_swappygl_swap(IntPtr display, IntPtr surface);
        internal static d_swappygl_swap SwapBuffers = FuncLoader.LoadFunction<d_swappygl_swap>(NativeLibrary, "SwappyGL_swap");

        //void SwappyGL_setUseAffinity(bool tf);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_swappygl_setuseaffinity(bool tf);
        internal static d_swappygl_setuseaffinity SetUseAffinity = FuncLoader.LoadFunction<d_swappygl_setuseaffinity>(NativeLibrary, "SwappyGL_setUseAffinity");

        //void SwappyGL_setSwapIntervalNS(uint64_t swap_ns);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_swappygl_setswapintervalns(ulong swap_ns);
        internal static d_swappygl_setswapintervalns SetSwapInterval = FuncLoader.LoadFunction<d_swappygl_setswapintervalns>(NativeLibrary, "SwappyGL_setSwapIntervalNS");

        //void SwappyGL_setAutoSwapInterval(bool enabled);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_swappygl_setautoswapinterval(bool enabled);
        internal static d_swappygl_setautoswapinterval SetAutoSwapInterval = FuncLoader.LoadFunction<d_swappygl_setautoswapinterval>(NativeLibrary, "SwappyGL_setAutoSwapInterval");

        //void SwappyGL_setAutoPipelineMode(bool enabled);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_swappygl_setautopipelinemode(bool enabled);
        internal static d_swappygl_setautopipelinemode SetAutoPipelineMode = FuncLoader.LoadFunction<d_swappygl_setautopipelinemode>(NativeLibrary, "SwappyGL_setAutoPipelineMode");
    }
}
