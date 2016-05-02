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
			start.Score = HeuristicEstimate(start, goal);
			priority[start] = start.Score;
			MapNode current = null;
			double newScore = 0;

			while (frontier.Count > 0)
			{
				current = frontier[0]; // TODO Get the node from openset with the lowest fscore value

				if (current == goal)
				{
					// TODO Return path from where we came to goal
					return ReconstructPath(start, current);
				}

				frontier.Remove(current);

				foreach (var nextNode in current.Neighbours)
				{
					if (cameFrom.ContainsKey(nextNode))
					{
						continue;
					}

					newScore = currentCost[current] + current.PhysicalPosition.Distance(nextNode.PhysicalPosition);

					if (!frontier.Contains(nextNode))
					{
						frontier.Add(nextNode);
					}
					else if (newScore >= currentCost[nextNode])
					{
						continue;
					}

					cameFrom[nextNode] = current;
					currentCost[nextNode] = newScore;
					priority[nextNode] = currentCost[nextNode] + HeuristicEstimate(nextNode, goal);
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

		private double HeuristicEstimate(MapNode from, MapNode goal)
		{
			// TODO Improve me?
			double results = 0;
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
