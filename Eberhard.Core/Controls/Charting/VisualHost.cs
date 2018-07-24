using System;
using System.Windows;
using System.Windows.Media;

namespace Eberhard.Core.Controls.Charting
{
    /// <summary>
    /// Base class for controls that use the <see cref="DrawingVisual"/>.
    /// </summary>
    internal abstract class VisualHost : FrameworkElement
    {
        #region Fields

        protected readonly VisualCollection _visuals;

        #endregion

        #region Constructor

        public VisualHost()
        {
            _visuals = new VisualCollection(this);
        }

        #endregion

        #region Properties

        protected override int VisualChildrenCount => _visuals.Count;

        #endregion

        #region Methods

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
                throw new ArgumentOutOfRangeException();

            return _visuals[index];
        }

        #endregion
    }
}
