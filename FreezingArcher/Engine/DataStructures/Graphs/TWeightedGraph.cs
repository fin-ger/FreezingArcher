﻿//
//  TWeightedGraph.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using FreezingArcher.Core;
using System.Collections.Generic;
using FreezingArcher.Output;

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Weighted graph.
    /// </summary>
    public class WeightedGraph<TData, TWeight> : FAObject where TWeight : IComparable
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public const string ModuleName = "WeightedGraph";

        /// <summary>
        /// Initialize the graph.
        /// </summary>
        public void Init ()
        {
            if (InternalEdges == null)
                InternalEdges = new List<Edge<TData, TWeight>>();
            else
                InternalEdges.Clear();

            if (InternalNodes == null)
                InternalNodes = new List<Node<TData, TWeight>>();
            else
                InternalEdges.Clear();
        }

        /// <summary>
        /// The real edges are stored here for internal use.
        /// </summary>
        protected List<Edge<TData, TWeight>> InternalEdges;

        /// <summary>
        /// The real nodes are stored here for internal use.
        /// </summary>
        protected List<Node<TData, TWeight>> InternalNodes;

        /// <summary>
        /// Get a read only collection of all registered edges.
        /// </summary>
        /// <value>The edges.</value>
        public IReadOnlyCollection<Edge<TData, TWeight>> Edges
        {
            get
            {
                return InternalEdges;
            }
        }

        /// <summary>
        /// Get a read only collection of all registered nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public IReadOnlyCollection<Node<TData, TWeight>> Nodes
        {
            get
            {
                return InternalNodes;
            }
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy()
        {
            InternalEdges.ForEach (e => e.Destroy());
            InternalNodes.ForEach (n => n.Destroy());
            InternalEdges.Clear();
            InternalNodes.Clear();
            base.Destroy();
        }

        /// <summary>
        /// Add a node to this graph.
        /// </summary>
        /// <returns><c>true</c>, if node was added, <c>false</c> otherwise.</returns>
        /// <param name="data">The data the node should hold.</param>
        /// <param name="edgeNodes">Collection of edges to be created. The pair consists of a
        /// neighbour node and an edge weight.</param>
        public virtual bool AddNode (TData data, ICollection<Pair<Node<TData, TWeight>, TWeight>> edgeNodes = null)
        {
            // create new node with object recycler
            Node<TData, TWeight> node = ObjectManager.CreateOrRecycle<Node<TData, TWeight>> (3);

            // initialize new node with data
            node.Init (data);

            // do we have edges?
            if (edgeNodes != null && edgeNodes.Count > 0)
            {
                foreach (var edgeNode in edgeNodes)
                {
                    // does destination node exist? If not adding this node to the graph will fail
                    if (edgeNode.A != null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge to nonexistent node {0}, skipping...", edgeNode.A);

                        // failure
                        return false;
                    }

                    // add new edge
                    AddEdge(node, edgeNode.A, edgeNode.B);
                }
            }

            // add new node to internal node list
            InternalNodes.Add (node);

            return true;
        }

        /// <summary>
        /// Remove the given node by its identifier.
        /// </summary>
        /// <returns><c>true</c>, if node was removed, <c>false</c> otherwise.</returns>
        /// <param name="node">Node identifier.</param>
        public virtual bool RemoveNode (Node<TData, TWeight> node)
        {
            // print error if remove failed
            if (InternalNodes.Remove(node))
            {
                Logger.Log.AddLogEntry (LogLevel.Warning, ModuleName, "The given node {0} does not exist!", node);
                return false;
            }

            // remove all edges associated with this node
            foreach (var edge in node.Edges)
            {
                if (edge.FirstNode == node)
                    edge.SecondNode.InternalEdges.Remove(edge);
                else
                    edge.FirstNode.InternalEdges.Remove(edge);

                InternalEdges.Remove (edge);
                edge.Destroy();
            }

            // destroy the node
            node.Destroy();

            return true;
        }

        /// <summary>
        /// Adds an edge from a given source node to a given destination node with a given edge weight.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="firstNode">The first node.</param>
        /// <param name="secondNode">The second node.</param>
        /// <param name="weight">The edge weight.</param>
        public virtual bool AddEdge (Node<TData, TWeight> firstNode, Node<TData, TWeight> secondNode, TWeight weight)
        {
            // fail if one of the nodes is null
            if (firstNode == null || secondNode == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Cannot create edge on null node!");
                return false;
            }

            // create new edge with object recycler
            Edge<TData, TWeight> edge = ObjectManager.CreateOrRecycle<Edge<TData, TWeight>>(4);

            // initialize edge with data
            edge.Init(weight, firstNode, secondNode);

            // add created edge to source and destination nodes
            firstNode.InternalEdges.Add(edge);
            secondNode.InternalEdges.Add(edge);

            // add created node to graph
            InternalEdges.Add(edge);

            // everything ok
            return true;
        }

        /// <summary>
        /// Removes an edge from the graph.
        /// </summary>
        /// <returns><c>true</c>, if edge was removed, <c>false</c> otherwise.</returns>
        /// <param name="edge">The edge.</param>
        public virtual bool RemoveEdge (Edge<TData, TWeight> edge)
        {
            // fail if edge is null
            if (edge == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Failed to remove edge as the given edge is null!");
                return false;
            }

            // if source or destination node are null we do really have a problem
            if (edge.FirstNode == null || edge.SecondNode == null)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                    "Detected an edge with referenced nodes that do not exist!" +
                    "This is a severe bug in the graph implementation.");
                return false;
            }

            // remove edge from source and destination nodes
            edge.FirstNode.InternalEdges.Remove(edge);
            edge.SecondNode.InternalEdges.Remove(edge);

            // remove edge from graph
            InternalEdges.Remove(edge);

            // destroy the edge
            edge.Destroy();
            return true;
        }
    }
}