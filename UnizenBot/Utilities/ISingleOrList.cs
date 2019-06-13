using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Utilities
{
    /// <summary>
    /// Represents an enumerable of either a single item or a list of items.
    /// </summary>
    /// <typeparam name="T">The type of the item(s).</typeparam>
    public interface ISingleOrList<T>
    {
        /// <summary>
        /// Adds an item to a <see cref="ListObject{T}"/> or sets the value of a <see cref="SingleObject{T}"/>.
        /// <para>If this object is a <see cref="SingleObject{T}"/> and it already contains a value, throws an <see cref="InvalidOperationException"/>.</para>
        /// </summary>
        /// <param name="item">The item to add or set.</param>
        void Add(T item);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ISingleOrList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for a <see cref="ListObject{T}"/>
        /// or a <see cref="SingleObject{T}.Enumerator"/> for a <see cref="SingleObject{T}"/>.</returns>
        IEnumerator<T> GetEnumerator();
    }

    /// <summary>
    /// Represents an enumerable of a single item.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public class SingleObject<T> : ISingleOrList<T>
    {
        /// <summary>
        /// The currently stored value.
        /// </summary>
        public T Value;

        /// <summary>
        /// Whether <see cref="Value"/> has been set.
        /// </summary>
        public bool HasValue;

        /// <summary>
        /// Constructs a <see cref="SingleObject{T}"/> with the value set to the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public SingleObject()
        {
            Value = default(T);
            HasValue = false;
        }

        /// <summary>
        /// Sets the value of this <see cref="SingleObject{T}"/>. If it already contains a value, throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (HasValue)
            {
                throw new InvalidOperationException("SingleObject already has a set value");
            }
            Value = item;
            HasValue = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SingleObject{T}"/>.
        /// </summary>
        /// <returns>A <see cref="SingleObject{T}.Enumerator"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(Value);
        }

        /// <summary>
        /// Supports a simple iteration over a single element.
        /// </summary>
        public class Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// Gets the single element.
            /// </summary>
            public T Current { get; }

            object IEnumerator.Current => Current;

            /// <summary>
            /// Whether the single element has been iterated over.
            /// </summary>
            public bool HasBeenRead = false;

            /// <summary>
            /// Constructs a new enumerator for a single element.
            /// </summary>
            /// <param name="value">The single element.</param>
            public Enumerator(T value)
            {
                Current = value;
            }

            /// <summary>
            /// Ends the iteration over the single element.
            /// </summary>
            /// <returns>true if the enumerator successfully finishes; false if the enumerator has already finished.</returns>
            public bool MoveNext()
            {
                if (!HasBeenRead)
                {
                    HasBeenRead = true;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the single element, thus allowing it to be iterated over again with this enumerator.
            /// </summary>
            public void Reset()
            {
                HasBeenRead = false;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }

    /// <summary>
    /// Represents an enumerable of a list of items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    public class ListObject<T> : List<T>, ISingleOrList<T>
    {
        /// <summary>
        /// Constructs an empty <see cref="ListObject{T}"/>.
        /// </summary>
        public ListObject()
        {
        }

        IEnumerator<T> ISingleOrList<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
