using System;
using Android.Graphics;

namespace Xamarin.Forms.PancakeView.Droid
{
    public static class PolygonUtils
    {
        public static Path GetPolygonCornerPath(double rectWidth, double rectHeight, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;

            var path = new Path();
            var theta = 2 * Math.PI / sides;

            // depends on the rotation
            var width = (-cornerRadius + Math.Min(rectWidth, rectHeight)) / 2;
            var center = new CGPoint(rectWidth / 2, rectHeight / 2);

            var radius = width + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo((float)(corner.X + cornerRadius * Math.Cos(angle + theta)), (float)(corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new CGPoint(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new CGPoint(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.LineTo(start.X, start.Y);
                path.QuadTo(tip.X, tip.Y, end.X, end.Y);
            }
            path.Close();
            return path;
        }

        public class CGPoint
        {
            public float X { get; set; }
            public float Y { get; set; }
            public CGPoint(double X, double Y)
            {
                this.X = (float)X;
                this.Y = (float)Y;
            }
        }
    }
}
