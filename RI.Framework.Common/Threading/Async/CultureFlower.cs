using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Implements a flower which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c> await </c>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class CultureFlower : CustomFlower
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public CultureFlower (Task task)
            : base(task)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter" /> to flow things around when awaited. </param>
        public CultureFlower (TaskAwaiter taskAwaiter)
            : base(taskAwaiter)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable" /> to flow things around when awaited. </param>
        public CultureFlower (ConfiguredTaskAwaitable configuredTaskAwaitable)
            : base(configuredTaskAwaitable)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public CultureFlower (CustomAwaiter customAwaiter)
            : base(customAwaiter)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        #endregion




        #region Instance Properties/Indexer

        private CultureInfo FormattingCulture { get; set; }
        private CultureInfo UiCulture { get; set; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void Capture ()
        {
            this.FormattingCulture = CultureInfo.CurrentCulture;
            this.UiCulture = CultureInfo.CurrentUICulture;
        }

        /// <inheritdoc />
        protected override void Restore ()
        {
            CultureInfo.CurrentCulture = this.FormattingCulture;
            CultureInfo.CurrentUICulture = this.UiCulture;
        }

        #endregion
    }

    /// <summary>
    ///     Implements a flower which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c> await </c>.
    /// </summary>
    /// <typeparam name="T"> The type of the return value of the <c> await </c> that is flowed around. </typeparam>
    /// <threadsafety static="true" instance="true" />
    public sealed class CultureFlower <T> : CustomFlower<T>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower{T}" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task{T}" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public CultureFlower (Task<T> task)
            : base(task)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower{T}" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter{T}" /> to flow things around when awaited. </param>
        public CultureFlower (TaskAwaiter<T> taskAwaiter)
            : base(taskAwaiter)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower{T}" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable{T}" /> to flow things around when awaited. </param>
        public CultureFlower (ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
            : base(configuredTaskAwaitable)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CultureFlower{T}" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter{T}" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public CultureFlower (CustomAwaiter<T> customAwaiter)
            : base(customAwaiter)
        {
            this.FormattingCulture = null;
            this.UiCulture = null;
        }

        #endregion




        #region Instance Properties/Indexer

        private CultureInfo FormattingCulture { get; set; }
        private CultureInfo UiCulture { get; set; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void Capture ()
        {
            this.FormattingCulture = CultureInfo.CurrentCulture;
            this.UiCulture = CultureInfo.CurrentUICulture;
        }

        /// <inheritdoc />
        protected override void Restore ()
        {
            CultureInfo.CurrentCulture = this.FormattingCulture;
            CultureInfo.CurrentUICulture = this.UiCulture;
        }

        #endregion
    }
}
