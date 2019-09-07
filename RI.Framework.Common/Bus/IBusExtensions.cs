using System;

using RI.Framework.ComponentModel;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IBus" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IBusExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates a new receiver registration which can be configured.
        /// </summary>
        /// <param name="bus"> The bus. </param>
        /// <returns>
        ///     The new receiver registration.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Although the name says &quot;Receive&quot;, it does not receive anything by itself.
        ///         In fact, it only creates a new instance of <see cref="ReceiverRegistration" />.
        ///         This method exists purely to allow a fluent syntax.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> is null. </exception>
        public static ReceiverRegistration Receive (this IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            return new ReceiverRegistration(bus);
        }

        /// <summary>
        ///     Creates a new send operation which can be configured.
        /// </summary>
        /// <param name="bus"> The bus. </param>
        /// <returns>
        ///     The new send operation.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Although the name says &quot;Send&quot;, it does not send anything by itself.
        ///         In fact, it only creates a new instance of <see cref="SendOperation" />.
        ///         This method exists purely to allow a fluent syntax.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> is null. </exception>
        public static SendOperation Send (this IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            return new SendOperation(bus);
        }

        /// <summary>
        ///     Starts the bus and opens all connections to remote busses.
        /// </summary>
        /// <param name="bus"> The bus. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="ServiceLocator" /> is used to resolve types needed by the bus.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The bus is already started. </exception>
        public static void Start (this IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            bus.Start(ServiceLocator.Resolver);
        }

        /// <summary>
        ///     Starts the bus and opens all connections to remote busses.
        /// </summary>
        /// <param name="bus"> The bus. </param>
        /// <param name="serviceProvider"> The service provider which is used for resolving types needed by the bus. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="bus" /> or <paramref name="serviceProvider" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The bus is already started. </exception>
        public static void Start (this IBus bus, IServiceProvider serviceProvider)
        {
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            bus.Start(new DependencyResolverWrapper(serviceProvider));
        }

        #endregion
    }
}
