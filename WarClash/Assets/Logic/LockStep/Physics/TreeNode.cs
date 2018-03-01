using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class TreeNode<T>
{
    /// <summary>
    /// Enlarged AABB
    /// </summary>
    internal AABB AABB;

    internal int Child1;
    internal int Child2;

    internal int Height;
    internal int ParentOrNext;
    internal T UserData;

    internal bool IsLeaf()
    {
        return Child1 == DynamicTree<T>.NullNode;
    }
}