using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV.Prediction
{
    class Distorter
    {
        /// <summary>
        /// Adds noise to a function's output.
        /// </summary>
        /// <param name="func">The function to distort.</param>
        /// <param name="maxDeviance">The maximum noise deviance.</param>
        /// <param name="seed">The seed for the internal Random.</param>
        /// <returns>A delegate that returns distorted values of func.</returns>
        public static Func<double, double> CreateDistorterRandomOffset(Func<double, double> func, double maxDeviance, int seed = 42)
        {
            Random r = new Random(seed);

            return (x) =>
            {
                return func(x) + (r.NextDouble() * 2 - 1) * maxDeviance;
            };
        }


    }
}
