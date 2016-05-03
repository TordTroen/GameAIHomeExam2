using PG4500_2016_Exam2;
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
		private readonly Trotor14MechaGodzilla robot;

		public AStarSearch(Trotor14MechaGodzilla robot, CollisionMap map)
		{
			this.robot = robot;
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
			//robot.Out.WriteLine("--- using A* v2 ---");
			var open = new List<MapNode>();
			var closed = new List<MapNode>();
			foreach (var node in collisionMap.map)
			{
				node.Parent = null;
				node.H = double.MaxValue;
				node.G = double.MaxValue;
			}
			start.G = 0;
			start.H = 0;
			start.Parent = null;
			open.Add(start);

			while (open.Count > 0)
			{
				var current = GetLowestFNode(open);
				open.Remove(current);
				//robot.Out.WriteLine("Node: " + current);

				// If the current node is the goal we stop and recreate the path
				if (current == goal)
				{
					//robot.Print("Node " + current + " is goal, creating path...");
					return ReconstructPath(goal);
				}

				foreach (var neighbour in current.Neighbours)
				{
					// Skip this neighbour if the parent is already set or the neibour has already been processed 
					if (neighbour.Parent != null || closed.Contains(neighbour)) continue;

					neighbour.Parent = current;
					//robot.Print("-> " + neighbour + " (" + neighbour.Parent + ")");

					var newG = current.G + neighbour.PhysicalPosition.Distance(current.PhysicalPosition);
					neighbour.H = goal.PhysicalPosition.Distance(neighbour.PhysicalPosition);
					neighbour.F = neighbour.G + neighbour.H; // TODO is this best, or is calculating F after setting G best?

					if (newG >= neighbour.G)
					{
						continue;
					}

					//robot.Print("Adding successor to frontier: " + neighbour);
					open.Add(neighbour);
					neighbour.G = newG;
					//neighbour.F = neighbour.G + neighbour.H;
				}
				closed.Add(current);
			}

			robot.Print("Couldn't find a goal!");
			return null;
		}


		private Stack<MapNode> ReconstructPath(MapNode goal)
		{
			var path = new Stack<MapNode>();
			var current = goal;
			while (current != null)
			{
				path.Push(current);
				current = current.Parent;
			}
			return path;
		}

		private MapNode GetLowestFNode(List<MapNode> list)
		{
			int best = 0;

			for (int i = 0; i < list.Count; i ++)
			{
				if (list[i].F < list[best].F)
				{
					best = i;
				}
			}
			return list[best];
		}

		//	public Stack<MapNode> Search2(MapNode start, MapNode goal)
		//	{
		//		robot.Out.WriteLine("--- using A* v1 ---");

		//		// TODO Use more appropriate datastructures
		//		frontier = new List<MapNode> { start };
		//		ResetCollections();
		//		currentCost[start] = 0;
		//		start.Score = H(start, goal);
		//		priority[start] = start.Score;

		//		while (frontier.Count > 0)
		//		{
		//			var current = frontier[0];

		//			// Stop and reconstruct the path if current is the goal
		//			if (current == goal)
		//			{
		//				// Return path from where we came to goal
		//				return ReconstructPath(start, goal);
		//			}

		//			frontier.Remove(current);

		//			// Check all of the neighbours of the current node
		//			foreach (var nextNode in current.Neighbours)
		//			{
		//				// Check if node is already processed
		//				if (cameFrom.ContainsKey(nextNode))
		//				{
		//					continue;
		//				}

		//				var newScore = currentCost[current] + current.PhysicalPosition.Distance(nextNode.PhysicalPosition);
		//				//var tenativeGscore = currentCost[current] + G(current, goal);
		//				// Something wrong here I think, currentCost isn't right
		//				// Probably store h and g score on the nodes


		//				if (!frontier.Contains(nextNode))
		//				{
		//					frontier.Add(nextNode);
		//				}
		//				else if (newScore >= currentCost[current])
		//				{
		//					continue;
		//				}

		//				cameFrom[nextNode] = current;
		//				currentCost[nextNode] = newScore;
		//				priority[nextNode] = currentCost[nextNode] + H(nextNode, goal);
		//			}
		//			SortFrontier();
		//		}
		//		return null;
		//	}

		//	private Stack<MapNode> ReconstructPath(MapNode start, MapNode current)
		//	{
		//		var path = new Stack<MapNode>();
		//		path.Push(current);
		//		//while (cameFrom.Keys.Contains(current))
		//		while (current != start)
		//		//while(cameFrom.ContainsKey(current))
		//		{
		//			current = cameFrom[current];
		//			//cameFrom.Remove(current);
		//			path.Push(current);
		//		}
		//		//return new Queue<MapNode>(path.Reverse());
		//		return path;
		//	}

		//	private double G(MapNode from, MapNode to)
		//	{
		//		double results = 0;
		//		results = from.PhysicalPosition.Distance(to.PhysicalPosition);
		//		return results;
		//	}

		//	private double H(MapNode from, MapNode goal)
		//	{
		//		// TODO Improve me?
		//		double results = 0;
		//		//return Math.Abs(from.X - goal.X) + Math.Abs(from.Y - goal.Y);

		//		// fidn number of squares
		//		// multiply by nidesize

		//		//dx = abs(node.x - goal.x)
		//		//dy = abs(node.y - goal.y)
		//		//double dx = Math.Abs(from.X - goal.X);
		//		//double dy = Math.Abs(from.Y - goal.Y);
		//		//double D = 1;
		//		//return D * (dx + dy);
		//		//return D*(dx + dy) + (D - 2*D)*Math.Min(dx, dy);
		//		//return D*Math.Sqrt(dx*dx + dy*dy);

		//		//7 * (deltaX + deltaY)
		//		//int ld = Math.Min(CollisionMap.Width, CollisionMap.Height);
		//		//int dx = goal.X - from.X;
		//		//int dy = goal.Y - from.Y;
		//		//// ld / 2 * dx + dy
		//		//results = (ld / 2) * (dx + dy);


		//		results = from.PhysicalPosition.Distance(goal.PhysicalPosition);
		//		return results;
		//	}

		//	private void SortFrontier()
		//	{
		//		//openSet.OrderBy(x => x.Score);
		//		frontier.Sort((x, y) => -x.Score.CompareTo(y.Score));
		//	}

		//	private void ResetCollections()
		//	{
		//		cameFrom = new Dictionary<MapNode, MapNode>();
		//		foreach (var node in collisionMap.map)
		//		{
		//			currentCost[node] = double.NegativeInfinity;
		//			priority[node] = double.NegativeInfinity;
		//		}
		//	}
	}
}
