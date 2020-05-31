using System;
using Android.Graphics;

namespace Xamarin.Forms.PancakeView.Droid
{
    public static class DrawingExtensions
    {
        public static Path CreateRoundedRectPath(RectF rect, float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var path = new Path();
            var radii = new[] { topLeft, topLeft,
                                topRight, topRight,
                                bottomRight, bottomRight,
                                bottomLeft, bottomLeft };

            path.AddRoundRect(rect, radii, Path.Direction.Ccw);
            path.Close();

            return path;
        }
        public static Path CreateRoundedRectPath(float width, float height, float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var path = new Path();
            var radii = new[] { topLeft, topLeft,
                                topRight, topRight,
                                bottomRight, bottomRight,
                                bottomLeft, bottomLeft };

            path.AddRoundRect(new RectF(0, 0, width, height), radii, Path.Direction.Ccw);
            path.Close();

            return path;
        }

        public static Path CreatePolygonPath(float width, float height, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;

            var path = new Path();
            var theta = 2 * Math.PI / sides;

            // depends on the rotation
            var widthRadius = (-cornerRadius + Math.Min(width, height)) / 2;
            var center = new Point(width / 2, height / 2);

            var radius = widthRadius + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new Point(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo((float)(corner.X + cornerRadius * Math.Cos(angle + theta)), (float)(corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new Point(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new Point(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new Point(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.LineTo((float)start.X, (float)start.Y);
                path.QuadTo((float)tip.X, (float)tip.Y, (float)end.X, (float)end.Y);
            }

            path.Close();

            return path;
        }

        public static Path CreatePolygonPath(RectF rect, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;

            var path = new Path();
            var theta = 2 * Math.PI / sides;

            // depends on the rotation
            var width = (-cornerRadius + Math.Min(rect.Width(), rect.Height())) / 2;
            var center = new Point(rect.Width() / 2, rect.Height() / 2);

            var radius = width + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new Point(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo((float)(corner.X + cornerRadius * Math.Cos(angle + theta)), (float)(corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new Point(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new Point(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new Point(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.LineTo((float)start.X, (float)start.Y);
                path.QuadTo((float)tip.X, (float)tip.Y, (float)end.X, (float)end.Y);
            }

            path.Close();

            return path;
        }
    }
}
