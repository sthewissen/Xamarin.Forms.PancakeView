using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.PancakeView.MacOS
{
    public static class DrawingExtensions
    {
        public static NSBezierPath CreateRoundedRectPath(this CGRect rect, CornerRadius cornerRadius)
        {
            var path = new NSBezierPath();

            path.MoveTo(new CGPoint(rect.Width - cornerRadius.TopRight, rect.Y));
            path.AddArc(new CGPoint((float)rect.X + rect.Width - cornerRadius.TopRight, (float)rect.Y + cornerRadius.TopRight), (nfloat)cornerRadius.TopRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
            path.AddLineTo(new CGPoint(rect.Width, rect.Height - cornerRadius.BottomRight));
            path.AddArc(new CGPoint((float)rect.X + rect.Width - cornerRadius.BottomRight, (float)rect.Y + rect.Height - cornerRadius.BottomRight), (nfloat)cornerRadius.BottomRight, 0, (float)(Math.PI * .5), true);
            path.AddLineTo(new CGPoint(cornerRadius.BottomLeft, rect.Height));
            path.AddArc(new CGPoint((float)rect.X + cornerRadius.BottomLeft, (float)rect.Y + rect.Height - cornerRadius.BottomLeft), (nfloat)cornerRadius.BottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
            path.AddLineTo(new CGPoint(rect.X, cornerRadius.TopLeft));
            path.AddArc(new CGPoint((float)rect.X + cornerRadius.TopLeft, (float)rect.Y + cornerRadius.TopLeft), (nfloat)cornerRadius.TopLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);

            path.ClosePath();

            return path;
        }

        public static NSBezierPath CreatePolygonPath(this CGRect rect, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;
            var path = new NSBezierPath();
            var theta = 2 * Math.PI / sides;

            var width = (-cornerRadius + Math.Min(rect.Size.Width, rect.Size.Height)) / 2;
            var center = new CGPoint(rect.Width / 2, rect.Height / 2);

            var radius = width + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo(new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new CGPoint(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new CGPoint(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.AddLineTo(start);
                path.AddQuadCurveToPoint(end, tip);
            }

            path.ClosePath();

            return path;
        }

        public static void AddQuadCurveToPoint(this NSBezierPath bezierPath, CGPoint endPoint, CGPoint controlPoint)
        {
            var d1x = controlPoint.X - bezierPath.CurrentPoint.X;
            var d1y = controlPoint.Y - bezierPath.CurrentPoint.Y;
            var d2x = endPoint.X - controlPoint.X;
            var d2y = endPoint.Y - controlPoint.Y;

            var cp1 = new CGPoint(controlPoint.X - d1x / 3.0, controlPoint.Y - d1y / 3.0);
            var cp2 = new CGPoint(controlPoint.X + d2x / 3.0, controlPoint.Y + d2y / 3.0);

            bezierPath.CurveTo(endPoint, cp1, cp2);
        }

        public static void AddLineTo(this NSBezierPath bezierPath, CGPoint point)
        {
            bezierPath.LineTo(point);
        }

        public static void AddArc(this NSBezierPath bezierPath, CGPoint center, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
        {
            startAngle = (nfloat)(startAngle * 180.0f / Math.PI);
            endAngle = (nfloat)(endAngle * 180.0f / Math.PI);
            bezierPath.AppendPathWithArc(center, radius, startAngle, endAngle, !clockwise);
        }

        public static CGPath ToCGPath(this NSBezierPath bezierPath)
        {
            var path = new CGPath();

            for (int i = 0; i < bezierPath.ElementCount; i++)
            {
                var type = bezierPath.ElementAt(i, out var points);

                switch (type)
                {
                    case NSBezierPathElement.MoveTo:
                        path.MoveToPoint(points[0]);
                        break;
                    case NSBezierPathElement.LineTo:
                        path.AddLineToPoint(points[0]);
                        break;
                    case NSBezierPathElement.CurveTo:
                        path.AddCurveToPoint(points[0], points[1], points[2]);
                        break;
                    case NSBezierPathElement.ClosePath:
                        path.CloseSubpath();
                        break;
                    default:
                        break;
                }
            }

            return path;
        }
    }
}