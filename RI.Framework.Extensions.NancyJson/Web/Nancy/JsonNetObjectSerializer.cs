using Nancy;

using Newtonsoft.Json;

using RI.Framework.Composition.Model;




namespace RI.Framework.Web.Nancy
{
    /// <summary>
    ///     Implements an object serializer which uses JSON.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Export]
    [Export(typeof(IObjectSerializer))]
    public sealed class JsonNetObjectSerializer : IObjectSerializer
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="JsonNetObjectSerializer" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Default JSON serialization settings are used.
        ///     </para>
        /// </remarks>
        public JsonNetObjectSerializer ()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="JsonNetObjectSerializer" />.
        /// </summary>
        /// <param name="settings"> The used JSON serialization settings or null to use default settings. </param>
        public JsonNetObjectSerializer (JsonSerializerSettings settings)
        {
            this.Settings = settings;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used JSON serialization settings.
        /// </summary>
        /// <value>
        ///     The used JSON serialization settings or null if default settings are used.
        /// </value>
        public JsonSerializerSettings Settings { get; }

        #endregion




        #region Interface: IObjectSerializer

        /// <inheritdoc />
        public object Deserialize (string sourceString)
        {
            try
            {
                return JsonConvert.DeserializeObject(sourceString, this.Settings);
            }
            catch
            {
                //The source string to deserialize is considered of "uncontrolled" origin (e.g. the clients web browser) and therefore we can either successfully deserialize, or we can't... but we do not want crashes from garbage sent by the browser (or an attacker...)
                return null;
            }
        }

        /// <inheritdoc />
        public string Serialize (object sourceObject)
        {
            return JsonConvert.SerializeObject(sourceObject, this.Settings);
        }

        #endregion
    }
}
