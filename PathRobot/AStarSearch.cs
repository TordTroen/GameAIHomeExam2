using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam
{
	public class AStarSearch
	{
		private readonly CollisionMap collisionMap;
		private List<MapNode> closedSet = new List<MapNode>();
		private List<MapNode> frontier = new List<MapNode>();
		private Dictionary<MapNode, double> currentCost = new Dictionary<MapNode, double>();
		private Dictionary<MapNode, double> priority = new Dictionary<MapNode, double>();
		private Dictionary<MapNode, MapNode> cameFrom = new Dictionary<MapNode, MapNode>();

		public AStarSearch(CollisionMap map)
		{
			collisionMap = map;

			currentCost = new Dictionary<MapNode, double>();
			priority = new Dictionary<MapNode, double>();
			foreach (var node in collisionMap.map)
			{
				if (currentCost.ContainsKey(node) || priority.ContainsKey(node))
				{
					continue;
				}
				currentCost.Add(node, double.NegativeInfinity);
				priority.Add(node, double.NegativeInfinity);
			}
		}

		public Stack<MapNode> Search(MapNode start, MapNode goal)
		{
			// TODO Use more appropriate datastructures
			frontier = new List<MapNode> { start };
			ResetCollections();
			currentCost[start] = 0;
			start.Score = H(start, goal);
			priority[start] = start.Score;

			while (frontier.Count > 0)
			{
				var current = frontier[0];

				// Stop and reconstruct the path if current is the goal
				if (current == goal)
				{
					// Return path from where we came to goal
					return ReconstructPath(start, current);
				}

				frontier.Remove(current);

				// Check all of the neighbours of the current node
				foreach (var nextNode in current.Neighbours)
				{
					// Check if node is already processed
					if (cameFrom.ContainsKey(nextNode))
					{
						continue;
					}

					//var newScore = currentCost[current] + current.PhysicalPosition.Distance(nextNode.PhysicalPosition);
					var tenativeGscore = currentCost[current] + G(current, goal); 
					// Something wrong here I think, currentCost isn't right
					// Probably store h and g score on the nodes


					if (!frontier.Contains(nextNode))
					{
						frontier.Add(nextNode);
					}
					else if (tenativeGscore >= currentCost[current])
					{
						continue;
					}

					cameFrom[nextNode] = current;
					currentCost[nextNode] = tenativeGscore;
					priority[nextNode] = currentCost[nextNode] + H(nextNode, goal);
				}
				SortFrontier();
			}
			return null;
		}
		
		private Stack<MapNode> ReconstructPath(MapNode start, MapNode current)
		{
			var path = new Stack<MapNode>();
			path.Push(current);
			//while (cameFrom.Keys.Contains(current))
			while (current != start)
			{
				current = cameFrom[current];
				//cameFrom.Remove(current);
				path.Push(current);
			}
			//return new Queue<MapNode>(path.Reverse());
			return path;
		}

		private double G(MapNode from, MapNode to)
		{
			double results = 0;
			results = from.PhysicalPosition.Distance(to.PhysicalPosition);
			return results;
		}

		private double H(MapNode from, MapNode goal)
		{
			// TODO Improve me?
			double results = 0;
			//return Math.Abs(from.X - goal.X) + Math.Abs(from.Y - goal.Y);

			// fidn number of squares
			// multiply by nidesize

			//dx = abs(node.x - goal.x)
			//dy = abs(node.y - goal.y)
			//double dx = Math.Abs(from.X - goal.X);
			//double dy = Math.Abs(from.Y - goal.Y);
			//double D = 1;
			//return D * (dx + dy);
			//return D*(dx + dy) + (D - 2*D)*Math.Min(dx, dy);
			//return D*Math.Sqrt(dx*dx + dy*dy);

			//7 * (deltaX + deltaY)
			//int ld = Math.Min(CollisionMap.Width, CollisionMap.Height);
			//int dx = goal.X - from.X;
			//int dy = goal.Y - from.Y;
			//// ld / 2 * dx + dy
			//results = (ld / 2) * (dx + dy);


			results = from.PhysicalPosition.Distance(goal.PhysicalPosition);
			return results;
		}

		private void SortFrontier()
		{
			//openSet.OrderBy(x => x.Score);
			frontier.Sort((x, y) => -x.Score.CompareTo(y.Score));
		}

		private void ResetCollections()
		{
			cameFrom = new Dictionary<MapNode, MapNode>();
			foreach (var node in collisionMap.map)
			{
				currentCost[node] = double.NegativeInfinity;
				priority[node] = double.NegativeInfinity;
			}
		}
	}
}
