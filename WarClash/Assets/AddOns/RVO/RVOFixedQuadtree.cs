using UnityEngine;
using System.Collections;
using FixedRVO;
using Lockstep;
using Pathfinding.RVO.Sampled;

namespace RVO {
	/** Quadtree for quick nearest neighbour search of agents.
	 */
	public class RVOFixedQuadtree {
		const int LeafSize = 15;

		float maxRadius = 0;

		struct Node {
			public int child00;
			public int child01;
			public int child10;
			public int child11;
			public byte count;
			public RVOFixedAgent linkedList;

			public void Add (RVOFixedAgent rvoFixedAgent) {
				rvoFixedAgent.next = linkedList;
				linkedList = rvoFixedAgent;
			}

			public void Distribute (Node[] nodes, Utility.FixedRect r) {
				Vector2d c = r.center;

				while (linkedList != null) {
					RVOFixedAgent nx = linkedList.next;
					if (linkedList.position.x > c.x) {
						if (linkedList.position.z > c.y) {
							nodes[child11].Add(linkedList);
						} else {
							nodes[child10].Add(linkedList);
						}
					} else {
						if (linkedList.position.z > c.y) {
							nodes[child01].Add(linkedList);
						} else {
							nodes[child00].Add(linkedList);
						}
					}
					linkedList = nx;
				}
				count = 0;
			}
		}

		Node[] nodes = new Node[42];
		int filledNodes = 1;

		Utility.FixedRect bounds;

		public void Clear () {
			nodes[0] = new Node();
			filledNodes = 1;
			maxRadius = 0;
		}

		public void SetBounds (Utility.FixedRect r) {
			bounds = r;
		}

		public int GetNodeIndex () {
			if (filledNodes == nodes.Length) {
				var nds = new Node[nodes.Length*2];
				for (int i = 0; i < nodes.Length; i++) nds[i] = nodes[i];
				nodes = nds;
			}
			nodes[filledNodes] = new Node();
			nodes[filledNodes].child00 = filledNodes;
			filledNodes++;
			return filledNodes-1;
		}

		public void Insert (RVOFixedAgent rvoFixedAgent) {
			int i = 0;
			Utility.FixedRect r = bounds;
			Vector2d p = new Vector2d(rvoFixedAgent.position.x, rvoFixedAgent.position.z);

			rvoFixedAgent.next = null;

			maxRadius = System.Math.Max(rvoFixedAgent.radius, maxRadius);

			int depth = 0;

			while (true) {
				depth++;

				if (nodes[i].child00 == i) {
					// Leaf node. Break at depth 10 in case lots of agents ( > LeafSize ) are in the same spot
					if (nodes[i].count < LeafSize || depth > 10) {
						nodes[i].Add(rvoFixedAgent);
						nodes[i].count++;
						break;
					} else {
						// Split
						Node node = nodes[i];
						node.child00 = GetNodeIndex();
						node.child01 = GetNodeIndex();
						node.child10 = GetNodeIndex();
						node.child11 = GetNodeIndex();
						nodes[i] = node;

						nodes[i].Distribute(nodes, r);
					}
				}
				// Note, no else
				if (nodes[i].child00 != i) {
					// Not a leaf node
					Vector2d c = r.center;
					if (p.x > c.x) {
						if (p.y > c.y) {
							i = nodes[i].child11;
							r = Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax);
						} else {
							i = nodes[i].child10;
							r = Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y);
						}
					} else {
						if (p.y > c.y) {
							i = nodes[i].child01;
							r = Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax);
						} else {
							i = nodes[i].child00;
							r = Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y);
						}
					}
				}
			}
		}

		public void Query (Vector2d p, long radius, RVOFixedAgent rvoFixedAgent) {
			QueryRec(0, p, radius, rvoFixedAgent, bounds);
		}

		long QueryRec (int i, Vector2d p, long radius, RVOFixedAgent rvoFixedAgent, Utility.FixedRect r) {
			if (nodes[i].child00 == i) {
				// Leaf node
				RVOFixedAgent a = nodes[i].linkedList;
				while (a != null) {
				    var v = rvoFixedAgent.InsertAgentNeighbour(a, radius.Mul(radius));
				    if (v < radius * radius)
				    {
				        radius = FixedMath.Sqrt(v);
				    }

                    //Debug.DrawLine (a.position, new Vector3(p.x,0,p.y),Color.black);
                    /*float dist = (new Vector2d(a.position.x, a.position.z) - p).sqrMagnitude;
					 * if ( dist < radius*radius && a != agent ) {
					 *
					 * }*/
                    a = a.next;
				}
			} else {
				// Not a leaf node
				Vector2d c = r.center;
				if (p.x-radius < c.x) {
					if (p.y-radius < c.y) {
						radius = QueryRec(nodes[i].child00, p, radius, rvoFixedAgent, Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
					}
					if (p.y+radius > c.y) {
						radius = QueryRec(nodes[i].child01, p, radius, rvoFixedAgent, Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
					}
				}

				if (p.x+radius > c.x) {
					if (p.y-radius < c.y) {
						radius = QueryRec(nodes[i].child10, p, radius, rvoFixedAgent, Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
					}
					if (p.y+radius > c.y) {
						radius = QueryRec(nodes[i].child11, p, radius, rvoFixedAgent, Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
					}
				}
			}

			return radius;
		}

		public void DebugDraw () {
			DebugDrawRec(0, bounds);
		}

		void DebugDrawRec (int i, Utility.FixedRect r) {
			Debug.DrawLine(new Vector3(r.xMin, 0, r.yMin), new Vector3(r.xMax, 0, r.yMin), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0, r.yMin), new Vector3(r.xMax, 0, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0, r.yMax), new Vector3(r.xMin, 0, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMin, 0, r.yMax), new Vector3(r.xMin, 0, r.yMin), Color.white);

			if (nodes[i].child00 != i) {
				// Not a leaf node
				Vector2d c = r.center;
				DebugDrawRec(nodes[i].child11, Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
				DebugDrawRec(nodes[i].child10, Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
				DebugDrawRec(nodes[i].child01, Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
				DebugDrawRec(nodes[i].child00, Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
			}

			RVOFixedAgent a = nodes[i].linkedList;
			while (a != null) {
				Debug.DrawLine(nodes[i].linkedList.position.ToVector3()+Vector3.up, a.position.ToVector3()+Vector3.up, new Color(1, 1, 0, 0.5f));
				a = a.next;
			}
		}
	}
}
