using System;
using System.Windows.Media.Animation;

namespace Eberhard.Core.Controls.Helpers
{
    internal static class AnimationHelper
    {
        private static readonly IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };

        public readonly static DoubleAnimation ToOpaque = CreateAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        public readonly static DoubleAnimation ToTransparent = CreateAnimation(1, 0, TimeSpan.FromMilliseconds(200));

        public static DoubleAnimation CreateAnimation(double from, double to, TimeSpan duration)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                EasingFunction = easingFunction,
                Duration = new System.Windows.Duration(duration)
            };
            animation.Freeze();
            return animation;
        }
    }
}
