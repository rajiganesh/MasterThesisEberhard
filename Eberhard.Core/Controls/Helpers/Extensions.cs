using System;
using System.Collections;

namespace Eberhard.Core.Controls.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        /// Gets the amount of items in a sequence.
        /// </summary>
        /// <param name="source">The sequence to count.</param>
        /// <returns></returns>
        public static int Count(this IEnumerable source)
        {
            if (source == null)
                return 0;

            if (source is ICollection c)
                return c.Count;

            int result = 0;
            IEnumerator enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
                result++;

            var d = enumerator as IDisposable;
            d?.Dispose();

            return result;
        }
    }
}
