// taken from https://github.com/maddenvvs/algodore

namespace WorkerWrapper.Domain.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Min binary heap implementation.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the heap.</typeparam>
    public class BinaryHeap<T>
    {
        /// <summary>
        /// Constructs empty default min binary heap.
        ///
        /// Time complexity: O(1).
        ///
        /// Space complexity: O(1).
        /// </summary>
        public BinaryHeap()
            : this(Array.Empty<T>(), Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Constructs default min binary heap from the given collection.
        ///
        /// Time complexity: O(N).
        ///
        /// Space complexity: O(N).
        ///
        /// N is size of the collection.
        /// </summary>
        /// <param name="collection">The elements collection to build the heap from.</param>
        public BinaryHeap(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Constructs empty binary heap with custom element comparer.
        ///
        /// Time complexity: O(1).
        ///
        /// Space complexity: O(1).
        /// </summary>
        /// <param name="comparer">
        /// The comparer used to determine the element order in the heap.
        /// </param>
        public BinaryHeap(IComparer<T> comparer)
            : this(Array.Empty<T>(), comparer)
        {
        }

        /// <summary>
        /// Constructs binary heap with custom element comparer from the given collection.
        ///
        /// Time complexity: O(N).
        ///
        /// Space complexity: O(N).
        ///
        /// N is size of the collection.
        /// </summary>
        /// <param name="collection">The elements collection to build the heap from.</param>
        /// <param name="comparer">
        /// The comparer used to determine the element order in the heap.
        /// </param>
        public BinaryHeap(IEnumerable<T> collection, IComparer<T> comparer)
            : this(collection?.ToList(), comparer)
        {
        }

        private BinaryHeap(List<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            this.Items = collection;
            this.Comparer = comparer;

            this.BuildHeap();
        }

        /// <summary>
        /// Returns the amount of elements in the heap.
        ///
        /// Time complexity: O(1).
        ///
        /// Space complexity: O(1).
        /// </summary>
        /// <value>Returns non-negative number of elements.</value>
        public int Count => this.Items.Count;

        /// <summary>
        /// Determines whether the heap is empty.
        ///
        /// Time complexity: O(1).
        ///
        /// Space complexity: O(1).
        /// </summary>
        /// <value>Returns true if the heap is empty, false otherwise.</value>
        public bool IsEmpty => this.Count == 0;

        private IComparer<T> Comparer { get; }

        private List<T> Items { get; }

        /// <summary>
        /// Returns minimal element from the heap without any modification.
        ///
        /// Time complexity: O(1).
        ///
        /// Space complexity: O(1).
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the heap is empty.
        /// </exception>
        /// <returns>Returns minimal element.</returns>
        public T Min()
        {
            this.EnsureHeapIsNotEmpty();

            return this.Items[0];
        }

        /// <summary>
        /// Extracts minimal element from the heap preserving binary heap invariant.
        ///
        /// Time complexity: O(log(N)).
        ///
        /// Space complexity: O(1).
        ///
        /// N is size of the heap.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the heap is empty.
        /// </exception>
        /// <returns>Returns extracted minimal element.</returns>
        public T ExtractMin()
        {
            this.EnsureHeapIsNotEmpty();

            var minValue = this.Min();

            this.Items[0] = this.Items[this.Count - 1];
            this.Items.RemoveAt(this.Count - 1);
            this.SiftDown(0);

            return minValue;
        }

        /// <summary>
        /// Inserts element into the heap preserving binary heap invariant.
        ///
        /// Time complexity: O(log(N)).
        ///
        /// Space complexity: O(1).
        ///
        /// N is size of the heap.
        /// </summary>
        /// <param name="item">New item to add.</param>
        /// <returns>Returns modified heap with added item.</returns>
        public BinaryHeap<T> Add(T item)
        {
            this.Items.Add(item);
            this.SiftUp(this.Count - 1);

            return this;
        }

        /// <summary>
        /// Merges two heaps into first one.
        /// Both heaps will be modified.
        ///
        /// Time complexity: O(N * lon(M + N)).
        ///
        /// Space complexity: O(N).
        ///
        /// M and N are sizes of both heaps.
        /// </summary>
        /// <param name="other">Another heap to merge from.</param>
        /// <returns>Returns modified heap with elements from both heaps.</returns>
        public BinaryHeap<T> Meld(BinaryHeap<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            while (!other.IsEmpty)
            {
                this.Add(other.ExtractMin());
            }

            return this;
        }

        /// <summary>
        /// Merges two heaps and returns new one.
        /// Initial heaps will not be modified.
        ///
        /// Time complexity: O(M + N).
        ///
        /// Space complexity: O(M + N).
        ///
        /// M and N are sizes of both heaps.
        /// </summary>
        /// <param name="other">Another heap to merge from.</param>
        /// <returns>Returns new binary heap containing elements from both heaps.</returns>
        public BinaryHeap<T> Merge(BinaryHeap<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            List<T> elements = new List<T>(this.Count + other.Count);
            foreach (var item in this.Items)
            {
                elements.Add(item);
            }

            foreach (var item in other.Items)
            {
                elements.Add(item);
            }

            return new BinaryHeap<T>(elements, this.Comparer);
        }

        /// <summary>
        /// Removes all elements from the heap.
        /// </summary>
        /// <returns>Modified heap with no elements.</returns>
        public BinaryHeap<T> Clear()
        {
            this.Items.Clear();

            return this;
        }

        private static int ParentOf(int childIndex) =>
            (childIndex - 1) / 2;

        private static int LeftChildOf(int parentIndex) =>
            (2 * parentIndex) + 1;

        private static int RightChildOf(int parentIndex) =>
            (2 * parentIndex) + 2;

        private void EnsureHeapIsNotEmpty()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty.");
            }
        }

        /// <summary>
        /// Efficiently build binary heap.
        ///
        /// Time complexity: O(N).
        ///
        /// Space complexity: O(1).
        ///
        /// N is the size of the internal collection.
        /// </summary>
        private void BuildHeap()
        {
            for (var idx = (this.Items.Count / 2) - 1; idx >= 0; idx--)
            {
                this.SiftDown(idx);
            }
        }

        private void SiftDown(int index)
        {
            int leftChild = LeftChildOf(index),
                rightChild = RightChildOf(index);

            while (leftChild < this.Items.Count)
            {
                var destIndex = leftChild;
                if (rightChild < this.Items.Count && this.ItemIsLess(rightChild, leftChild))
                {
                    destIndex = rightChild;
                }

                if (!this.ItemIsLess(destIndex, index))
                {
                    break;
                }

                this.SwapItems(index, destIndex);

                index = destIndex;

                leftChild = LeftChildOf(index);
                rightChild = RightChildOf(index);
            }
        }

        private void SiftUp(int index)
        {
            var parentIndex = ParentOf(index);

            while (this.ItemIsLess(index, parentIndex))
            {
                this.SwapItems(index, parentIndex);

                index = parentIndex;
                parentIndex = ParentOf(index);
            }
        }

        private bool ItemIsLess(int first, int second) =>
            this.Comparer.Compare(this.Items[first], this.Items[second]) < 0;

        private void SwapItems(int first, int second)
        {
            var temp = this.Items[first];
            this.Items[first] = this.Items[second];
            this.Items[second] = temp;
        }
    }
}