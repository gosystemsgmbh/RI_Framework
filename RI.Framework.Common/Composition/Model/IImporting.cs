namespace RI.Framework.Composition.Model
{
    /// <summary>
    ///     Defines an interface which allows types and objects which use model-based importing (using <see cref="ImportAttribute" />) to be informed when their imports are being resolved or updated.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public interface IImporting
    {
        /// <summary>
        ///     Called after the model-based imports of an object were resolved.
        /// </summary>
        /// <param name="composition"> The composition type which was done on the object. </param>
        /// <param name="updated"> Specifies whether any of the model-based imports of the object were resolved or updated. </param>
        void ImportsResolved (CompositionFlags composition, bool updated);

        /// <summary>
        ///     Called before the model-based imports of an object are resolved.
        /// </summary>
        /// <param name="composition"> The composition type which is going to be done on the object. </param>
        void ImportsResolving (CompositionFlags composition);
    }
}
