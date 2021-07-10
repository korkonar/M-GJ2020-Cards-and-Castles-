using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IPrioritizable {
    /// <summary>
    /// Priority of the item.
    /// </summary>
    int Priority { get; set; }
}

public sealed class PriorityQueue<Node>
        where Node : IPrioritizable {
    public LinkedList<Node> Entries { get; set; } = new LinkedList<Node>();

    public int Count() {
        return Entries.Count;
    }

    public Node Dequeue() {
        if (Entries.Any()) {
            var itemTobeRemoved = Entries.First.Value;
            Entries.RemoveFirst();
            return itemTobeRemoved;
        }

        return default(Node);
    }

    public void Enqueue(Node entry) {
        var value = new LinkedListNode<Node>(entry);
        if (Entries.First == null) {
            Entries.AddFirst(value);
        } else {
            var ptr = Entries.First;
            while (ptr.Next != null && ptr.Value.Priority < entry.Priority) {
                ptr = ptr.Next;
            }

            if (ptr.Value.Priority <= entry.Priority) {
                Entries.AddAfter(ptr, value);
            } else {
                Entries.AddBefore(ptr, value);
            }
        }
    }

    public void ReSort() {
        PriorityQueue<Node> temp = new PriorityQueue<Node>();
        while (Count() > 0) {
            temp.Enqueue(Dequeue());
        }
        Entries = temp.Entries;
    }
}
