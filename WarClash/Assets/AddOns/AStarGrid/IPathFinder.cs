using System.Collections.Generic;

namespace AStar
{
    public interface IPathFinder
    {
        List<PathFinderNode> FindPath(Point start, Point end);
    }

    public struct Point
    {
        public static readonly Point Empty = new Point();

        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of this <see cref='System.Drawing.Point'/>.
        ///    </para>
        /// </devdoc>
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
    }
}
