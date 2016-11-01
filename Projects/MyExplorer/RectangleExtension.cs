using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MyExplorer
{
    static class RectangleExtension
    {
        public static Point TopRight(this Rectangle rect)
        {
            return (new Point(rect.X + rect.Width, rect.Y));
        }

        public static Point BottomLeft(this Rectangle rect)
        {
            return (new Point(rect.X, rect.Y + rect.Height));
        }

        public static Point BottomRight(this Rectangle rect)
        {
            return (new Point(rect.X + rect.Width, rect.Y + rect.Height));
        }
    }
}
