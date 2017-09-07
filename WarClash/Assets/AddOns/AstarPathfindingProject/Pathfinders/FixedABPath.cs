using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Lockstep;

namespace Pathfinding
{
    public class FixedABPath : Path
    {
        public Vector3d StartPoint {
            get { return startPoint; }
        }
        public Vector3d EndPoint
        {
            get { return endPoint; }
        }
        private Vector3d startPoint;
        private Vector3d endPoint;
        private GraphNode startNode;
        private GraphNode endNode;
        protected PathNode partialBestTarget;
        public static FixedABPath Construct(Vector3d start, Vector3d end, OnPathDelegate callback = null)
        {
            var p = PathPool.GetPath<FixedABPath>();
            p.Setup(start, end, callback);
            return p;
        }

        public void CacualteNow()
        {
            this.PrepareBase(AstarPath.threadInfos[0].runData);
            this.Prepare();
            this.Initialize();
            this.CalculateStep(long.MaxValue);
        }
        protected void Setup(Vector3d start, Vector3d end, OnPathDelegate callbackDelegate)
        {
            callback = callbackDelegate;
            this.startPoint = start;
            this.endPoint = end;
        }

        public override void CalculateStep(long targetTick)
        {
            int counter = 0;

            // Continue to search while there hasn't ocurred an error and the end hasn't been found
            while (CompleteState == PathCompleteState.NotCalculated)
            {
                searchedNodes++;

                // Close the current node, if the current node is the target node then the path is finished
                if (currentR.node == endNode)
                {
                    CompleteState = PathCompleteState.Complete;
                    break;
                }

                if (currentR.H < partialBestTarget.H)
                {
                    partialBestTarget = currentR;
                }

                AstarProfiler.StartFastProfile(4);

                // Loop through all walkable neighbours of the node and add them to the open list.
                currentR.node.Open(this, currentR, pathHandler);

                AstarProfiler.EndFastProfile(4);

                // Any nodes left to search?
                if (pathHandler.HeapEmpty())
                {
                    Error();
                    LogError("Searched whole area but could not find target");
                    return;
                }

                // Select the node with the lowest F score and remove it from the open list
                AstarProfiler.StartFastProfile(7);
                currentR = pathHandler.PopNode();
                AstarProfiler.EndFastProfile(7);

                // Check for time every 500 nodes, roughly every 0.5 ms usually
                if (counter > 500)
                {
                    // Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
                    if (System.DateTime.UtcNow.Ticks >= targetTick)
                    {
                        // Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
                        return;
                    }
                    counter = 0;

                    if (searchedNodes > 1000000)
                    {
                        throw new System.Exception("Probable infinite loop. Over 1,000,000 nodes searched");
                    }
                }

                counter++;
            }


            AstarProfiler.StartProfile("Trace");

            if (CompleteState == PathCompleteState.Complete)
            {
                Trace(currentR);
            }

            AstarProfiler.EndProfile();
        }

        public override void Initialize()
        {
            if (startNode != null) pathHandler.GetPathNode(startNode).flag2 = true;
            if (endNode != null) pathHandler.GetPathNode(endNode).flag2 = true;

            // Zero out the properties on the start node
            PathNode startRNode = pathHandler.GetPathNode(startNode);
            startRNode.node = startNode;
            startRNode.pathID = pathHandler.PathID;
            startRNode.parent = null;
            startRNode.cost = 0;
            startRNode.G = GetTraversalCost(startNode);
            startRNode.H = CalculateHScore(startNode);
            partialBestTarget = startRNode;
            // Check if the start node is the target and complete the path if that is the case
            if (CompleteState == PathCompleteState.Complete) return;

            // Open the start node and puts its neighbours in the open list
            startNode.Open(this, startRNode, pathHandler);

            searchedNodes++;


            // Any nodes left to search?
            if (pathHandler.HeapEmpty())
            {
                Error();
                LogError("No open points, the start node didn't open any nodes");
                return;
            }

            // Pop the first node off the open list
            currentR = pathHandler.PopNode();
        }
        public override uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
        {
            if (startNode != null && endNode != null)
            {
                if (a == startNode)
                {
                    return (uint)((Vector3d.ToInt3(startPoint) - (b == endNode ? hTarget : b.position)).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
                if (b == startNode)
                {
                    return (uint)((Vector3d.ToInt3(startPoint) - (a == endNode ? hTarget : a.position)).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
                if (a == endNode)
                {
                    return (uint)((hTarget - b.position).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
                if (b == endNode)
                {
                    return (uint)((hTarget - a.position).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
            }
            else
            {
                // endNode is null, startNode should never be null for an ABPath
                if (a == startNode)
                {
                    return (uint)((Vector3d.ToInt3(startPoint) - b.position).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
                if (b == startNode)
                {
                    return (uint)((Vector3d.ToInt3(startPoint) - a.position).costMagnitude * (currentCost * 1.0 / (a.position - b.position).costMagnitude));
                }
            }

            return currentCost;
        }

        public override void Prepare()
        {
            NNInfo startNNInfo = AstarPath.active.GetNearest(startPoint);
            startPoint = startNNInfo.constFixedClampedPosition;
            startNode = startNNInfo.node;
            NNInfo endNNInfo = AstarPath.active.GetNearest(endPoint);
            endPoint = endNNInfo.constFixedClampedPosition;
            endNode = endNNInfo.node;
            hTarget = Vector3d.ToInt3(endPoint);
        }

        public override void Cleanup()
        {
            if (startNode != null) pathHandler.GetPathNode(startNode).flag2 = false;
            if (endNode != null) pathHandler.GetPathNode(endNode).flag2 = false;
        }
        public override void Reset()
        {
            base.Reset();
            startNode = null;
            endNode = null;
            startPoint = new Vector3d();
            endPoint = new Vector3d();
            partialBestTarget = null;
            hTarget = new Int3();
        }
    }
}

  
