/*
 * Agent.cs
 * RVO2 Library C#
 *
 * Copyright 2008 University of North Carolina at Chapel Hill
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Please send all bug reports to <geom@cs.unc.edu>.
 *
 * The authors may be contacted via:
 *
 * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
 * Dept. of Computer Science
 * 201 S. Columbia St.
 * Frederick P. Brooks, Jr. Computer Science Bldg.
 * Chapel Hill, N.C. 27599-3175
 * United States of America
 *
 * <http://gamma.cs.unc.edu/RVO2/>
 */
using System;
using System.Collections.Generic;
using Lockstep;
using Pathfinding.RVO;

namespace RVO
{
    /**
     * <summary>Defines an agent in the simulation.</summary>
     */
    public class RVOAgent
    {
        internal RVOAgent next;
        public Vector3d position;
        internal IList<KeyValuePair<long, RVOAgent>> agentNeighbors_ = new List<KeyValuePair<long, RVOAgent>>();
        internal IList<Line> orcaLines_ = new List<Line>();
        internal Vector2d position_;
        internal Vector2d prefVelocity_;
        internal Vector2d velocity_;
        internal int id_ = 0;
        internal int maxNeighbors_ = 0;
        internal long maxSpeed_ = 0;
        internal long neighborDist_ = 0;
        internal long radius = 0;
        internal long timeHorizon_ = 0;
        internal long timeHorizonObst_ = 0;

        private Vector2d newVelocity_;

        /**
         * <summary>Computes the new velocity of this agent.</summary>
         */
        internal void computeNewVelocity()
        {
            orcaLines_.Clear();

            /* Create obstacle ORCA lines. */

            int numObstLines = orcaLines_.Count;

            long invTimeHorizon = 1 / timeHorizon_;
            
            /* Create agent ORCA lines. */
            for (int i = 0; i < agentNeighbors_.Count; ++i)
            {
                RVOAgent other = agentNeighbors_[i].Value;

                Vector2d relativePosition = other.position_ - position_;
                Vector2d relativeVelocity = velocity_ - other.velocity_;
                long distSq = RVOMath.absSq(relativePosition);
                long combinedRadius = radius + other.radius;
                long combinedRadiusSq = combinedRadius.Mul(combinedRadius);

                Line line;
                Vector2d u;

                if (distSq > combinedRadiusSq)
                {
                    /* No collision. */
                    Vector2d w = relativeVelocity - relativePosition.Mul(invTimeHorizon);

                    /* Vector from cutoff center to relative velocity. */
                    long wLengthSq = RVOMath.absSq(w);
                    long dotProduct1 = w.Dot(relativePosition);

                    if (dotProduct1 < 0 && dotProduct1.Mul(dotProduct1) > combinedRadiusSq.Mul(wLengthSq))
                    {
                        /* Project on cut-off circle. */
                        long wLength = wLengthSq.Mul(wLengthSq);
                        Vector2d unitW = w.Div(wLength);

                        line.direction = new Vector2d(unitW.y, -unitW.x);
                        u = unitW.Mul((combinedRadius.Mul(invTimeHorizon) - wLength));
                    }
                    else
                    {
                        /* Project on legs. */
                        long leg = (distSq - combinedRadiusSq).Mul((distSq - combinedRadiusSq));

                        if (RVOMath.det(relativePosition, w) > 0.0f)
                        {
                            /* Project on left leg. */
                            line.direction = new Vector2d(relativePosition.x .Mul(leg) - relativePosition.y.Mul(combinedRadius), relativePosition.x.Mul(combinedRadius )+ relativePosition.y.Mul(leg)).Div(distSq);
                        }
                        else
                        {
                            /* Project on right leg. */
                            line.direction = -new Vector2d(relativePosition.x.Mul(leg) + relativePosition.y.Mul(combinedRadius), -relativePosition.x.Mul(combinedRadius )+ relativePosition.y.Mul(leg)).Div(distSq);
                        }

                        long dotProduct2 = relativeVelocity.Dot(line.direction);
                        u =   line.direction.Mul(dotProduct2) - relativeVelocity;
                    }
                }
                else
                {
                    /* Collision. Project on cut-off circle of time timeStep. */
                    long invTimeStep = FixedMath.One / 15;//¸ÄÎªÖ¡ÆµÂÊ

                    /* Vector from cutoff center to relative velocity. */
                    Vector2d w = relativeVelocity -  relativePosition.Mul(invTimeStep);
                    
                    long wLength = FixedMath.Sqrt(w.Dot(w));
                    Vector2d unitW = w / wLength;

                    line.direction = new Vector2d(unitW.y, -unitW.x);
                    u = unitW.Mul((combinedRadius .Mul( invTimeStep) - wLength));
                }

                line.point = velocity_ + u.Mul(FixedMath.Half);
                orcaLines_.Add(line);
            }

            int lineFail = linearProgram2(orcaLines_, maxSpeed_, prefVelocity_, false, ref newVelocity_);

            if (lineFail < orcaLines_.Count)
            {
                linearProgram3(orcaLines_, numObstLines, lineFail, maxSpeed_, ref newVelocity_);
            }
        }

        public long InsertAgentNeighbour(RVOAgent agent, long rangeSq)
        {
            if (this == agent) return rangeSq;

            //if ((agent.layer & collidesWith) == 0) return rangeSq;

            //2D Dist
            long dist = FixedMath.Sqrt(agent.position.x - position.x) + FixedMath.Sqrt(agent.position.z - position.z);

            if (dist < rangeSq)
            {
                if (agentNeighbors_.Count < 10)
                {
                    agentNeighbors_.Add(new KeyValuePair<long, RVOAgent>(dist, agent));
                }

                int i = agentNeighbors_.Count - 1;
                if (dist < agentNeighbors_[i].Key)
                {
                    while (i != 0 && dist < agentNeighbors_[i-1].Key)
                    {
                        agentNeighbors_[i] = agentNeighbors_[i - 1];
                     
                        i--;
                    }
                    agentNeighbors_[i] = new KeyValuePair<long, RVOAgent>(dist, agent);
                }

                if (agentNeighbors_.Count == 10)
                {
                    rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                }
            }
            return rangeSq;
        }
        /**
         * <summary>Updates the two-dimensional position and two-dimensional
         * velocity of this agent.</summary>
         */
        internal void update()
        {
            velocity_ = newVelocity_;
            position_ += velocity_ * (FixedMath.One/15);
        }

        /**
         * <summary>Solves a one-dimensional linear program on a specified line
         * subject to linear constraints defined by lines and a circular
         * constraint.</summary>
         *
         * <returns>True if successful.</returns>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="lineNo">The specified line constraint.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="optVelocity">The optimization velocity.</param>
         * <param name="directionOpt">True if the direction should be optimized.
         * </param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        private bool linearProgram1(IList<Line> lines, int lineNo, long radius, Vector2d optVelocity, bool directionOpt, ref Vector2d result)
        {
            long dotProduct = lines[lineNo].point .Dot( lines[lineNo].direction);
            long discriminant = RVOMath.sqr(dotProduct) + RVOMath.sqr(radius) - RVOMath.absSq(lines[lineNo].point);

            if (discriminant < 0.0f)
            {
                /* Max speed circle fully invalidates line lineNo. */
                return false;
            }

            long sqrtDiscriminant = RVOMath.sqrt(discriminant);
            long tLeft = -dotProduct - sqrtDiscriminant;
            long tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; ++i)
            {
                long denominator = RVOMath.det(lines[lineNo].direction, lines[i].direction);
                long numerator = RVOMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

                if (RVOMath.fabs(denominator) <= 0)
                {
                    /* Lines lineNo and i are (almost) parallel. */
                    if (numerator < 0)
                    {
                        return false;
                    }

                    continue;
                }

                long t = numerator.Div(denominator);

                if (denominator >= 0.0f)
                {
                    /* Line i bounds line lineNo on the right. */
                    tRight = Math.Min(tRight, t);
                }
                else
                {
                    /* Line i bounds line lineNo on the left. */
                    tLeft = Math.Max(tLeft, t);
                }

                if (tLeft > tRight)
                {
                    return false;
                }
            }

            if (directionOpt)
            {
                /* Optimize direction. */
                if (optVelocity.Dot(lines[lineNo].direction) > 0)
                {
                    /* Take right extreme. */
                    result = lines[lineNo].point +  lines[lineNo].direction.Mul(tRight);
                }
                else
                {
                    /* Take left extreme. */
                    result = lines[lineNo].point + lines[lineNo].direction.Mul(tRight);
                }
            }
            else
            {
                /* Optimize closest point. */
                long t = lines[lineNo].direction * (optVelocity - lines[lineNo].point);

                if (t < tLeft)
                {
                    result = lines[lineNo].point +  lines[lineNo].direction * tLeft;
                }
                else if (t > tRight)
                {
                    result = lines[lineNo].point + lines[lineNo].direction* tRight;
                }
                else
                {
                    result = lines[lineNo].point +  lines[lineNo].direction * t;
                }
            }

            return true;
        }

        /**
         * <summary>Solves a two-dimensional linear program subject to linear
         * constraints defined by lines and a circular constraint.</summary>
         *
         * <returns>The number of the line it fails on, and the number of lines
         * if successful.</returns>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="optVelocity">The optimization velocity.</param>
         * <param name="directionOpt">True if the direction should be optimized.
         * </param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        private int linearProgram2(IList<Line> lines, long radius, Vector2d optVelocity, bool directionOpt, ref Vector2d result)
        {
            if (directionOpt)
            {
                /*
                 * Optimize direction. Note that the optimization velocity is of
                 * unit length in this case.
                 */
                result = optVelocity * radius;
            }
            else if (RVOMath.absSq(optVelocity) > RVOMath.sqr(radius))
            {
                /* Optimize closest point and outside circle. */
                result = RVOMath.normalize(optVelocity) * radius;
            }
            else
            {
                /* Optimize closest point and inside circle. */
                result = optVelocity;
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                if (RVOMath.det(lines[i].direction, lines[i].point - result) > 0.0f)
                {
                    /* Result does not satisfy constraint i. Compute new optimal result. */
                    Vector2d tempResult = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
                    {
                        result = tempResult;

                        return i;
                    }
                }
            }

            return lines.Count;
        }

        /**
         * <summary>Solves a two-dimensional linear program subject to linear
         * constraints defined by lines and a circular constraint.</summary>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="numObstLines">Count of obstacle lines.</param>
         * <param name="beginLine">The line on which the 2-d linear program
         * failed.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        private void linearProgram3(IList<Line> lines, int numObstLines, int beginLine, long radius, ref Vector2d result)
        {
            long distance = 0;

            for (int i = beginLine; i < lines.Count; ++i)
            {
                if (RVOMath.det(lines[i].direction, lines[i].point - result) > distance)
                {
                    /* Result does not satisfy constraint of line i. */
                    IList<Line> projLines = new List<Line>();
                    for (int ii = 0; ii < numObstLines; ++ii)
                    {
                        projLines.Add(lines[ii]);
                    }

                    for (int j = numObstLines; j < i; ++j)
                    {
                        Line line;

                        long determinant = RVOMath.det(lines[i].direction, lines[j].direction);

                        if (RVOMath.fabs(determinant) <= RVOMath.RVO_EPSILON)
                        {
                            /* Line i and line j are parallel. */
                            if (lines[i].direction * lines[j].direction > 0.0f)
                            {
                                /* Line i and line j point in the same direction. */
                                continue;
                            }
                            else
                            {
                                /* Line i and line j point in opposite direction. */
                                line.point =  (lines[i].point + lines[j].point)* FixedMath.Half;
                            }
                        }
                        else
                        {
                            line.point = lines[i].point +   lines[i].direction * (RVOMath.det(lines[j].direction, lines[i].point - lines[j].point) / determinant);
                        }

                        line.direction = RVOMath.normalize(lines[j].direction - lines[i].direction);
                        projLines.Add(line);
                    }

                    Vector2d tempResult = result;
                    if (linearProgram2(projLines, radius, new Vector2d(-lines[i].direction.y, lines[i].direction.x), true, ref result) < projLines.Count)
                    {
                        /*
                         * This should in principle not happen. The result is by
                         * definition already in the feasible region of this
                         * linear program. If it fails, it is due to small
                         * floating point error, and the current result is kept.
                         */
                        result = tempResult;
                    }

                    distance = RVOMath.det(lines[i].direction, lines[i].point - result);
                }
            }
        }
    }
}
