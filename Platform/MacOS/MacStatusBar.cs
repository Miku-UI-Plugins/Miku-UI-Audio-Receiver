using System;
using System.Runtime.InteropServices;

namespace Miku_UI_Music_Center.Platform.MacOS
{
    /// <summary>
    /// Status bar implementation for macOS
    /// </summary>
    public static class MacStatusBar
    {
        private static IntPtr statusItem;

        // Import macOS Objective-C runtime library
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
        private static extern IntPtr objc_getClass(string className);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
        private static extern IntPtr sel_registerName(string selectorName);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr objc_msgSend_Double(IntPtr receiver, IntPtr selector, double value);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr objc_msgSend_alloc(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr objc_msgSend_initWithUTF8String(IntPtr receiver, IntPtr selector, string str);

        public static void Init()
        {
            IntPtr nsStatusBar = objc_getClass("NSStatusBar");
            IntPtr systemBarSelector = sel_registerName("systemStatusBar");
            IntPtr sharedBar = objc_msgSend(nsStatusBar, systemBarSelector);

            IntPtr statusItemSelector = sel_registerName("statusItemWithLength:");
            statusItem = objc_msgSend_Double(sharedBar, statusItemSelector, -1); // -1 = variable length

            SetText(StringResources.StringResources.TbLyricText);  // Set the default text of the status bar
        }

        /// <summary>
        /// Set the text of the status bar
        /// </summary>
        /// <param name="text">Lyrics</param>
        public static void SetText(string text)
        {
            if (statusItem != IntPtr.Zero)
            {
                IntPtr titleSelector = sel_registerName("setTitle:");
                IntPtr nsStringClass = objc_getClass("NSString");

                // Create NSString
                IntPtr allocSelector = sel_registerName("alloc");
                IntPtr initSelector = sel_registerName("initWithUTF8String:");

                IntPtr nsString = objc_msgSend_alloc(nsStringClass, allocSelector);
                nsString = objc_msgSend_initWithUTF8String(nsString, initSelector, text);

                // Set the title of the status bar
                objc_msgSend_IntPtr(statusItem, titleSelector, nsString);
            }
        }
    }


}
