using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.UI.Desktop
{
    public static class DesktopResourceHelper
    {
        /// <summary>
        /// Gets an image from the calling assembly.
        /// </summary>
        /// <returns></returns>
        public static Image GetImage(string name)
        {
            return GetImage(Assembly.GetCallingAssembly(), name);
        }

        /// <summary>
        /// Gets an image from the calling assembly.
        /// </summary>
        /// <returns></returns>
        public static Image GetImage(Assembly asm, string name)
        {
            var stream = ResourceHelper.GetResourceStream(asm, name);
            if (stream == null)
                throw new InvalidOperationException("stream is null.");

            // mbr - 06-07-2007 - added using.			
            using (stream)
                return Image.FromStream(stream);
        }

        /// <summary>
        /// Gets an icon from the calling assembly.
        /// </summary>
        /// <returns></returns>
        public static Icon GetIcon(string name)
        {
            return GetIcon(Assembly.GetCallingAssembly(), name);
        }

        /// <summary>
        /// Gets an icon from the calling assembly.
        /// </summary>
        /// <returns></returns>
        private static Icon GetIcon(Assembly asm, string name)
        {
            var stream = ResourceHelper.GetResourceStream(asm, name);
            if (stream == null)
                throw new InvalidOperationException("stream is null.");

            // mbr - 06-07-2007 - added using.			
            using (stream)
                return new Icon(stream);
        }
    }
}
