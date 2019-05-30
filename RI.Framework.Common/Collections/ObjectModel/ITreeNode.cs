using System.Collections.Generic;




namespace RI.Framework.Collections.ObjectModel
{
    /// <summary>
    ///     Defines a generic interface for a single node in a tree.
    /// </summary>
    /// <typeparam name="T"> The type of the tree nodes in the tree. This is usually the type itself which inherits from <see cref="ITreeNode{T}" />, e.g. <c> public class MyNode : ITreeNode&lt;MyNode&gt; { } </c>. </typeparam>
    /// <remarks>
    ///     <para>
    ///         A tree consists of tree nodes but no other outside or contextual construct. Each tree node has zero or one parent node (<see cref="Parent" />) and zero, one, or more child nodes (<see cref="Children" /> ).
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface ITreeNode <T>
        where T : class, ITreeNode<T>
    {
        /// <summary>
        ///     Gets the list of child nodes of this tree node.
        /// </summary>
        /// <value>
        ///     The list of child nodes of this tree node.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="Children" /> must not be null.
        ///     </note>
        /// </remarks>
        IList<T> Children { get; }

        /// <summary>
        ///     Gets the parent node of this tree node.
        /// </summary>
        /// <value>
        ///     The parent node of this tree node or null if this tree node has no parent node.
        /// </value>
        T Parent { get; }
    }
}
