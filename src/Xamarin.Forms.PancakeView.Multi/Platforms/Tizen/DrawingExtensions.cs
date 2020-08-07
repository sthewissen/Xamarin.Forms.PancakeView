using System;
using SkiaSharp;

namespace Xamarin.Forms.PancakeView.Tizen
{
    public static class DrawingExtensions
    {
        static public SKPath CreateRoundedRectPath(int left, int top, int width, int height, CornerRadius cornerRadius)
        {
            var path = new SKPath();
            var skRoundRect = new SKRoundRect(new SKRect(left, top, width, height));
            SKPoint[] radii = new SKPoint[4]
            {
                new SKPoint((float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft),
                new SKPoint((float)cornerRadius.TopRight, (float)cornerRadius.TopRight),
                new SKPoint((float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight),
                new SKPoint((float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft)
            };
            skRoundRect.SetRectRadii(skRoundRect.Rect, radii);
            path.AddRoundRect(skRoundRect);
            path.Close();
            return path;
        }

        public static SKPath CreatePolygonPath(float rectWidth, float rectHeight, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;
            var path = new SKPath();
            var theta = 2 * Math.PI / sides;

            var width = (-cornerRadius + Math.Min(rectWidth, rectHeight)) / 2;
            var center = new Point(rectWidth / 2, rectHeight / 2);

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
