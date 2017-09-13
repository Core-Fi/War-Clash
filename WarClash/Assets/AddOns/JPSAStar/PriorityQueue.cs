using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue< KeyType, PriorityType > where PriorityType : System.IComparable
{
	struct Element< SubClassKeyType, SubClassPriorityType > where SubClassPriorityType : System.IComparable
	{
		public SubClassKeyType key;
		public SubClassPriorityType priority;

		public Element( SubClassKeyType key, SubClassPriorityType priority )
		{
			this.key = key;
			this.priority = priority;
		}
	}

	List< Element<KeyType, PriorityType> > queue = new List< Element<KeyType, PriorityType> >();

	public void push( KeyType arg_key, PriorityType arg_priority )
	{
		Element<KeyType, PriorityType> new_elem = new Element<KeyType, PriorityType>( arg_key, arg_priority );
/*
		int index = 0;
		foreach ( var element in queue )
		{
			// if my new element's priority is less than than the element in this location
			if ( new_elem.priority.CompareTo( element.priority ) < 0 )
			{
				break;
			}

			++index;
		}

		// Insert at the found index
		queue.Insert( index, new_elem );
*/
		var p = queue.Count;
            queue.Add(new_elem); // E[p] = O

            do
            {
                if (p == 0)
                {
                    break;
                }
                
                var p2 = (p - 1) / 2;

                if (OnCompare(p, p2) < 0)
                {
                    SwitchElements(p, p2);
                    p = p2;
                }
                else
                {
                    break;
                }

            } while (true);

            //return p;
	}
 	private int OnCompare(int i, int j)
    {
        return queue[i].priority.CompareTo(queue[j].priority);
    }
	public KeyType pop()
	{
		/*
		if ( isEmpty() )
		{
			throw new UnityException("Attempted to pop off an empty queue");
		}

		Element<KeyType, PriorityType> top = queue[ 0 ];

		queue.RemoveAt( 0 );

		return top.key;
		*/
		var result = queue[0];
        var p = 0;

        queue[0] = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);

        do
        {
            var pn = p;
            var p1 = 2 * p + 1;
            var p2 = 2 * p + 2;

            if (queue.Count > p1 && OnCompare(p, p1) > 0)
            {
                p = p1;
            }
            if (queue.Count > p2 && OnCompare(p, p2) > 0)
            {
                p = p2;
            }

            if (p == pn)
            {
                break;
            }

            SwitchElements(p, pn);

        } while (true);

        return result.key;
	}
	 private void SwitchElements(int i, int j)
    {
        var h = queue[i];
        queue[i] = queue[j];
        queue[j] = h;
    }

	public bool isEmpty()
	{
		return queue.Count == 0;
	}
}
