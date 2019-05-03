using System;
using System.Collections.Generic;




namespace RI.Framework.Collections.Generic
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IWarehouse{T}" /> type and its implementations.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IWarehouseExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Converts any instance implementing <see cref="IWarehouse{T}" /> to an explicit <see cref="IWarehouse{T}" />.
        /// </summary>
        /// <typeparam name="T"> The type of the items in <paramref name="warehouse" />. </typeparam>
        /// <param name="warehouse"> The instance implementing <see cref="IWarehouse{T}" />. </param>
        /// <returns>
        ///     The instance as explicit <see cref="IWarehouse{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         A conversion to an explicit <see cref="IWarehouse{T}" /> can be useful in cases where the utility/extension methods of <see cref="IWarehouse{T}" /> shall be used instead of the ones implemented by the instance itself.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="warehouse" /> is null. </exception>
        public static IWarehouse<T> AsWarehouse<T>(this IWarehouse<T> warehouse)
        {
            if (warehouse == null)
            {
                throw new ArgumentNullException(nameof(warehouse));
            }

            return warehouse;
        }

        /// <summary>
        /// Releases multiple bays.
        /// </summary>
        /// <typeparam name="T"> The type of the items in <paramref name="warehouse" />. </typeparam>
        /// <param name="warehouse">The warehouse.</param>
        /// <param name="bays">The bays to release.</param>
        /// <returns>
        /// The number of released bays.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="bays" /> is enumerated exactly once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="warehouse" /> or <paramref name="bays" /> is null. </exception>
        public static int ReleaseRange <T> (this IWarehouse<T> warehouse, IEnumerable<int> bays)
        {
            if (warehouse == null)
            {
                throw new ArgumentNullException(nameof(warehouse));
            }

            if (bays == null)
            {
                throw new ArgumentNullException(nameof(bays));
            }

            int releasedCount = 0;
            foreach (int bay in bays)
            {
                warehouse.Release(bay);
                releasedCount++;
            }
            return releasedCount;
        }

        /// <summary>
        /// Reserves multiple bays.
        /// </summary>
        /// <typeparam name="T"> The type of the items in <paramref name="warehouse" />. </typeparam>
        /// <param name="warehouse">The warehouse.</param>
        /// <param name="bayCount">The number of bays to reserve.</param>
        /// <returns>
        /// The array with the reserved bays.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="warehouse" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bayCount"/> is less than zero.</exception>
        public static int[] ReserveRange<T>(this IWarehouse<T> warehouse, int bayCount)
        {
            if (warehouse == null)
            {
                throw new ArgumentNullException(nameof(warehouse));
            }

            if (bayCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bayCount));
            }

            int[] bays = new int[bayCount];
            for (int i1 = 0; i1 < bayCount; i1++)
            {
                bays[i1] = warehouse.Reserve();
            }

            return bays;
        }

        #endregion
    }
}
