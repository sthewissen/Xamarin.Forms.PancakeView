using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.PancakeView.iOS
{
    public static class ShapeUtils
    {
        public static CGPoint BottomLeft(this CGRect rect) => new CGPoint(x: rect.Left, y: rect.Bottom);
        public static CGPoint BottomRight(this CGRect rect) => new CGPoint(x: rect.Right, y: rect.Bottom);
        public static CGPoint TopLeft(this CGRect rect) => new CGPoint(x: rect.Left, y: rect.Top);
        public static CGPoint TopRight(this CGRect rect) => new CGPoint(x: rect.Right, y: rect.Top);

        public static CGPoint OffsetBy(this CGPoint point, float x, float y)
        {
            point.X += x;
            point.Y += y;
            return point;
        }

        public static CGPoint OffsetBy(this CGPoint point, double x, double y)
        {
            point.X += (float)x;
            point.Y += (float)y;
            return point;
        }

        public static UIBezierPath CreateRoundedRectPath(CGRect rect, CornerRadius cornerRadius)
        {
            var path = new UIBezierPath();

            path.MoveTo(
                new CGPoint(
                    rect.Width - cornerRadius.TopRight,
                    rect.Y));

            path.AddArc(
                center: new CGPoint(
                    rect.Width - cornerRadius.TopRight,
                    cornerRadius.TopRight),
                radius: (nfloat)cornerRadius.TopRight,
                startAngle: (float)(Math.PI * 1.5),
                endAngle: (float)Math.PI * 2,
                clockWise: true);

            path.AddLineTo(new CGPoint(rect.Width, rect.Height - cornerRadius.BottomRight));
            path.AddArc(
                new CGPoint((float)rect.X + rect.Width - cornerRadius.BottomRight,
                (float)rect.Y + rect.Height - cornerRadius.BottomRight),
                (nfloat)cornerRadius.BottomRight,
                0,
                (float)(Math.PI * .5), true);
            path.AddLineTo(new CGPoint(cornerRadius.BottomLeft, rect.Height));
            path.AddArc(new CGPoint((float)rect.X + cornerRadius.BottomLeft, (float)rect.Y + rect.Height - cornerRadius.BottomLeft), (nfloat)cornerRadius.BottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
            path.AddLineTo(new CGPoint(rect.X, cornerRadius.TopLeft));
            path.AddArc(new CGPoint((float)rect.X + cornerRadius.TopLeft, (float)rect.Y + cornerRadius.TopLeft), (nfloat)cornerRadius.TopLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);

            path.ClosePath();

            return path;
        }

        public static CAShapeLayer ToEdgeLayer(
            this CGRect rect,
            UIRectEdge edge,
            CGColor strokeColor,
            double lineWidth,
            CornerRadius cornerRadius = default,
            BorderDrawingStyle borderStyle = BorderDrawingStyle.Inside)
        {
            var edgeLayer = new CAShapeLayer
            {
                StrokeColor = strokeColor,
                FillColor = null,
                LineWidth = (float)lineWidth,
            };

            var path = new UIBezierPath();

            switch (edge)
            {
                case UIRectEdge.Left:
                    edgeLayer.Name = "leftEdgeBorderLayer";
                    path = rect.ToEdgePath(edge, lineWidth, cornerRadius, borderStyle);
                    break;
                case UIRectEdge.Top:
                    edgeLayer.Name = "topEdgeBorderLayer";
                    path = rect.ToEdgePath(edge, lineWidth, cornerRadius, borderStyle);
                    break;
                case UIRectEdge.Right:
                    edgeLayer.Name = "rightEdgeBorderLayer";
                    path = rect.ToEdgePath(edge, lineWidth, cornerRadius, borderStyle);
                    break;
                case UIRectEdge.Bottom:
                    edgeLayer.Name = "bottomEdgeBorderLayer";
                    path = rect.ToEdgePath(edge, lineWidth, cornerRadius, borderStyle);
                    break;
            }

            edgeLayer.Path = path.CGPath;
            return edgeLayer;
        }

            public static UIBezierPath ToEdgePath(
            this CGRect rect,
            UIRectEdge edge,
            double lineWidth,
            CornerRadius cornerRadius = default,
            BorderDrawingStyle borderStyle = BorderDrawingStyle.Inside)
        {
            var halfBorder = (float)(lineWidth / 2);
            var insetRect =
                borderStyle switch
                {
                    BorderDrawingStyle.Inside => rect.Inset(halfBorder, halfBorder),
                    BorderDrawingStyle.Outside => rect.Inset(-halfBorder, -halfBorder),
                    BorderDrawingStyle.Centered => rect,
                    _ => rect
                };

            var path = new UIBezierPath();

            switch (edge)
            {
                case UIRectEdge.Left:
                    path.MoveTo(insetRect
                        .BottomLeft()
                        .OffsetBy(x: 0, y: halfBorder - cornerRadius.BottomLeft));

                    var topLeft = insetRect
                        .TopLeft()
                        .OffsetBy(x: 0, y: -halfBorder + cornerRadius.TopLeft);

                    path.AddLineTo(topLeft);

                    path.AddArc(
                        center: insetRect
                            .TopLeft()
                            .OffsetBy(x: cornerRadius.TopLeft, y: cornerRadius.TopLeft),
                            //.OffsetBy(x: halfBorder, y: halfBorder),
                        radius: (nfloat)cornerRadius.TopLeft,
                        startAngle: (float)Math.PI,
                        endAngle: (float)(Math.PI * 1.5),
                        clockWise: true);
                    break;

                case UIRectEdge.Top:
                    path.MoveTo(insetRect
                        .TopLeft()
                        .OffsetBy(x: -halfBorder + cornerRadius.TopLeft, y: 0));

                    var topRightPoint = insetRect
                        .TopRight()
                        .OffsetBy(x: halfBorder - cornerRadius.TopRight, y: 0);

                    path.AddLineTo(topRightPoint);

                    path.AddArc(
                        center: topRightPoint
                            .OffsetBy(x: -halfBorder, y: cornerRadius.TopRight),
                        radius: (nfloat)cornerRadius.TopRight,
                        startAngle: (float)(Math.PI * 1.5),
                        endAngle: (float)Math.PI * 2,
                        clockWise: true);
                    break;

                case UIRectEdge.Right:
                    path.MoveTo(insetRect
                        .TopRight()
                        .OffsetBy(x: 0, y: -halfBorder + cornerRadius.TopRight));
                    path.AddLineTo(insetRect
                        .BottomRight()
                        .OffsetBy(x: 0, y: halfBorder - cornerRadius.BottomRight));
                    break;

                case UIRectEdge.Bottom:
                    path.MoveTo(insetRect
                        .BottomRight()
                        .OffsetBy(x: halfBorder - cornerRadius.BottomRight, y: 0));
                    path.AddLineTo(insetRect
                        .BottomLeft()
                        .OffsetBy(x: -halfBorder + cornerRadius.BottomLeft, y: 0));
                    break;
            }

            return path;
        }

        public static UIBezierPath CreatePolygonPath(CGRect rect, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;
            var path = new UIBezierPath();
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
    }
}
