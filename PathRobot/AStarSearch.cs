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
			foreach (var node in collisionMap.map)
			{
				node.Parent = null;
				node.H = double.MaxValue;
				node.G = double.MaxValue;
				node.IsClosed = false;
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
					if (neighbour.Parent != null || neighbour.IsClosed)
					{
						continue;
					}

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
				current.IsClosed = true;
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
	}
}
