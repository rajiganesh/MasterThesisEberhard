using System.Collections.Generic;
using System.Windows.Media;

namespace Eberhard.Core.Controls.Charting
{
    /// <summary>
    /// Base class for all graph controls that have to display a lot of colored data.
    /// </summary>
    internal abstract class GraphHost : VisualHost
    {
        #region Fields



        protected readonly IDictionary<GraphSize, Visual> _toleranceCache 
            = new Dictionary<GraphSize, Visual>();

        protected readonly IDictionary<GraphSize, Visual> _graphCache
            = new Dictionary<GraphSize, Visual>();

        #endregion

        #region Methods

        /// <summary>
        /// Clears the cache of visuals that represent data.
        /// </summary>
        /// <param name="size"></param>
        public virtual void RedrawSource(GraphSize size)
        {
            if(_visuals.Count > 1)
                _visuals.RemoveAt(1);
            _graphCache.Clear();
            CheckGraphCache(size);
        }

        /// <summary>
        /// Clears the cache and draws everything again.
        /// </summary>
        /// <param name="size"></param>
        public virtual void Redraw(GraphSize size)
        {
            _graphCache.Clear();
            _toleranceCache.Clear();

            Draw(size);
        }

        /// <summary>
        /// Draws from a cache or draws new visuals if neccessary.
        /// </summary>
        /// <param name="size"></param>
        public virtual void Draw(GraphSize size)
        {
            _visuals.Clear();

            CheckToleranceCache(size);
            CheckGraphCache(size);
        }


        private void CheckToleranceCache(GraphSize size)
        {
            if (_toleranceCache.ContainsKey(size))
            {
                _visuals.Add(_toleranceCache[size]);
            }
            else
            {
                var visual = DrawToleranceVisual();
                if(visual != null)
                {
                    _visuals.Add(visual);
                    _toleranceCache.Add(size, visual);
                }
            }
        }

        private void CheckGraphCache(GraphSize size)
        {
            if (_graphCache.ContainsKey(size))
            {
                _visuals.Add(_graphCache[size]);
            }
            else
            {
                var visual = DrawGraphVisual();
                if(visual != null)
                {
                    _visuals.Add(visual);
                    _graphCache.Add(size, visual);
                }
            }
        }

        protected abstract Visual DrawGraphVisual();

        protected abstract Visual DrawToleranceVisual();

        protected virtual Brush GetBrushForStatus(GraphStatus status)
        {
            switch (status)
            {
                case GraphStatus.Error:
                    return Brushes.Red;
                case GraphStatus.Normal:
                    return Brushes.White;
                case GraphStatus.Warning:
                    return Brushes.Yellow;
                default:
                    return null;
            }
        }

        #endregion
    }
}
