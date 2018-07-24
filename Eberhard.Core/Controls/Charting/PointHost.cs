using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static Eberhard.Core.Controls.Helpers.ColorHelper;


namespace Eberhard.Core.Controls.Charting
{
    /// <summary>
    /// Provides drawing points for the <see cref="PointCloud"/>.
    /// </summary>
    internal class PointHost : GraphHost
    {
        private const string FORMAT_PLUS = "+0.000";
        private const string FORMAT_MINUS = "-0.000";

        private const double LABELS_MARGIN = 5d;
        private const double BOUNDS_MARGIN = 20d;
        private const double ERROR_MARGIN = 4d;

        private static readonly IDictionary<GraphSize, Rect> _boundsCache = new Dictionary<GraphSize, Rect>();

        //TOOD: respect global styles?
        private static readonly Typeface arial = new Typeface("Calibri");
        private static readonly Func<string, FormattedText> builder = s =>
        {

            FormattedText _formattedString = new FormattedText(s, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, arial, 11, Brushes.White);
            _formattedString.TextAlignment = (TextAlignment)2;
            
            return _formattedString;
        };


        private PointCloud _parent;

        public void SetParent(PointCloud parent)
        {
            _parent = parent;
        }

        protected Visual DrawPoints()
        {
            if (_parent.DataSource == null)
                return null;
            

            var errorBounds = _boundsCache[_parent.GraphSize];

            double originX = ActualWidth / 2.0;
            double originY = ActualHeight / 2.0;
            double transformX = errorBounds.Width / _parent.ErrorToleranceX / 2.0;
            double transformY = errorBounds.Height / _parent.ErrorToleranceY / 2.0;

            double distanceX = (ActualWidth - errorBounds.Width) / 2.0;
            double distanceY = (ActualHeight - errorBounds.Height) / 2.0;

            var visual = new DrawingVisual();
            var context = visual.RenderOpen();
            _parent.Status = GraphStatus.Normal;
            if (_parent.DataSource.Cast<Point>().Any())
            {
                var points = _parent.DataSource.Cast<Point>()
                            .Select(point => new
                            {
                                Point = point,
                                Status = GetPointStatus(point)
                            });
                foreach (var point in points)
                {

                    var transformedPoint = TransformPoint(point.Point, errorBounds, transformX, transformY, originX, originY, distanceX, distanceY);
                    context.DrawEllipse(GetBrushForStatus(point.Status), null, transformedPoint, 3, 3);
                }

                _parent.Status = points.Any() ?
                    points.Max(i => i.Status) :
                    GraphStatus.Normal;

                GraphStatus GetPointStatus(Point point) =>
                    (double.IsNaN(point.X) || double.IsNaN(point.Y)) ?
                        GraphStatus.NotAvilable :
                        ((Math.Abs(point.X) > _parent.ErrorToleranceX) || Math.Abs(point.Y) > _parent.ErrorToleranceY) ?
                            GraphStatus.Error :
                            ((Math.Abs(point.X) > _parent.WarningToleranceX) || Math.Abs(point.Y) > _parent.WarningToleranceY) ?
                            GraphStatus.Warning : GraphStatus.Normal;

            }

            context.Close();
            return visual;
        }

        /// <summary>
        /// Transforms a single point in the cloud to match the coordinate system.
        /// </summary>
        /// <param name="p">The point to transform.</param>
        /// <param name="origin">The origin of the coordinate system.</param>
        /// <param name="distance">The distance between error zone and coordinate system.</param>
        /// <param name="margin">The margin to apply to values out of the error zone.</param>
        private Point TransformPoint(Point p, Rect bounds, double transformX, double transformY, double originX, double originY, double distanceX, double distanceY)
        {
            // transform from point to cartesian coordinates
            p.X = p.X * transformX + originX;
            p.Y = p.Y * -transformY + originY;

            // cap values at error zone and apply the margin at each side
            if (p.X > bounds.Width + distanceX)
                p.X = bounds.Width + distanceX + ERROR_MARGIN;
            if (p.X < 0)
                p.X = distanceX - ERROR_MARGIN;
            if (p.Y > bounds.Height + distanceY)
                p.Y = bounds.Height + distanceY + ERROR_MARGIN;
            if (p.Y < 0)
                p.Y = distanceY - ERROR_MARGIN;

            return p;
        }

        private Visual DrawTolerances(double errorToleranceX, double errorToleranceY, double warningX, double warningToleranceY)
        {
            double originX = ActualWidth / 2d, originY = ActualHeight / 2d;
            Rect errorBounds;
            Rect warningBounds;

            if(errorToleranceX > errorToleranceY)
            {
                // Scenario to draw:
                // rectangle takes full width and is proportionally reduced in height
                // ------
                // |    |
                // ------
                var height = ActualHeight * (errorToleranceY / errorToleranceX);
                var Y = (ActualHeight - height) / 2d;
                errorBounds = new Rect(new Point(BOUNDS_MARGIN, Y + BOUNDS_MARGIN), new Size(ActualWidth - 2 * BOUNDS_MARGIN, height - 2 * BOUNDS_MARGIN));
                
            }
            else if(errorToleranceX < errorToleranceY)
            {
                // Scenario to draw:
                // rectangle takes full height and is proportionally reduced in width
                // ----
                // |  |
                // |  |
                // ----
                var width = ActualWidth * (errorToleranceX / errorToleranceY);
                var X = (ActualWidth - width) / 2d;
                errorBounds = new Rect(new Point(X + BOUNDS_MARGIN, BOUNDS_MARGIN), new Size(width - 2 * BOUNDS_MARGIN, ActualHeight - 2 * BOUNDS_MARGIN));
            }
            else
            {
                // Scenario to draw:
                // rectangle takes full height and width
                // -----
                // |   |
                // -----
                errorBounds = new Rect(new Point(BOUNDS_MARGIN, BOUNDS_MARGIN), new Point(ActualWidth - BOUNDS_MARGIN, ActualHeight - BOUNDS_MARGIN));
            }

            // calculate rectangle for warning tolerance
            var y = errorBounds.Height * warningToleranceY / errorToleranceY;
            var x = errorBounds.Width * warningX / errorToleranceX;
            var offsetX = (ActualWidth - x) / 2d;
            var offsetY = (ActualHeight - y) / 2d;
            warningBounds = new Rect(new Point(offsetX, offsetY), new Size(x, y));

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                // Draw a cross in the center
                context.DrawLine(Neutral, new Point(0, originY), new Point(ActualWidth, originY));
                context.DrawLine(Neutral, new Point(originX, 0), new Point(originX, ActualHeight));

                // Draw two rectangles, one for the error and one for warning tolerance
                context.DrawRectangle(null, Error, errorBounds);
                context.DrawRectangle(null, Warning, warningBounds);
                
                if(_parent.GraphSize > GraphSize.Small)
                {
                    var labelOffsetX = originX + LABELS_MARGIN;
                    var labelOffsetY = originY + LABELS_MARGIN;

                    //context.PushTransform(new RotateTransform(-90, labelOffsetX, originY));
                    context.DrawText(builder("0"), new Point(labelOffsetX, originY));
                    //context.Pop();

                    //Tolerenzen und Error Bounds für Y Achse
                    //Wenn PointHost.LayoutTransform in PointCloud.xaml nicht benutzt wird, kann man context.PushTransform() und context.Pop weglöschen
                    //context.PushTransform(new RotateTransform(-90, labelOffsetX, errorBounds.Top));
                    context.DrawText(builder(errorToleranceY.ToString(FORMAT_PLUS)), new Point(labelOffsetX, errorBounds.Top));
                    //context.Pop();

                    //context.PushTransform(new RotateTransform(-90, labelOffsetX, warningBounds.Top));
                    context.DrawText(builder(warningToleranceY.ToString(FORMAT_PLUS)), new Point(labelOffsetX, warningBounds.Top-15));
                    //context.Pop();

                    //context.PushTransform(new RotateTransform(-90, labelOffsetX, errorBounds.Bottom));
                    context.DrawText(builder(errorToleranceY.ToString(FORMAT_MINUS)), new Point(labelOffsetX, errorBounds.Bottom));
                    //context.Pop();
                    
                    //context.PushTransform(new RotateTransform(-90, labelOffsetX, warningBounds.Bottom));
                    context.DrawText(builder(warningToleranceY.ToString(FORMAT_MINUS)), new Point(labelOffsetX, warningBounds.Bottom-15));
                    //context.Pop();

                    //Tolerenzen und Error Bounds für X Achse
                    //Wenn PointHost.LayoutTransform in PointCloud.xaml nicht benutzt wird, kann man context.PushTransform() und context.Pop weglöschen
                    //context.PushTransform(new RotateTransform(-90, errorBounds.Left, labelOffsetY));
                    context.DrawText(builder(errorToleranceX.ToString(FORMAT_MINUS)), new Point(errorBounds.Left, labelOffsetY));
                    //context.Pop();

                    //context.PushTransform(new RotateTransform(-90, warningBounds.Left, labelOffsetY));
                    context.DrawText(builder(warningX.ToString(FORMAT_MINUS)), new Point(warningBounds.Left, labelOffsetY));
                    //context.Pop();

                    //context.PushTransform(new RotateTransform(-90, errorBounds.Right, labelOffsetY));
                    context.DrawText(builder(errorToleranceX.ToString(FORMAT_PLUS)), new Point(errorBounds.Right, labelOffsetY));
                    //context.Pop();

                    //context.PushTransform(new RotateTransform(-90, warningBounds.Right, labelOffsetY));
                    context.DrawText(builder(warningX.ToString(FORMAT_PLUS)), new Point(warningBounds.Right, labelOffsetY));
                    //context.Pop();
                }

            }

            SetBounds(errorBounds);

            return visual;
        }

        private void SetBounds(Rect bounds)
        {
            if (_boundsCache.ContainsKey(_parent.GraphSize))
                _boundsCache[_parent.GraphSize] = bounds;
            else
                _boundsCache.Add(_parent.GraphSize, bounds);
        }

        protected override Visual DrawGraphVisual()
        {
            return DrawPoints();
        }

        protected override Visual DrawToleranceVisual()
        {
            return DrawTolerances(_parent.ErrorToleranceX, _parent.ErrorToleranceY, _parent.WarningToleranceX, _parent.WarningToleranceY);
        }
    }
}
