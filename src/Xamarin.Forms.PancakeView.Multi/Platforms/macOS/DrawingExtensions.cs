using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.PancakeView.MacOS
{
    public static class DrawingExtensions
    {
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