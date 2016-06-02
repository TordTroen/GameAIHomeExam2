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
		private readonly Trotor14MechaGodzilla robot;

		public AStarSearch(Trotor14MechaGodzilla robot, CollisionMap map)
		{
			this.robot = robot;
			collisionMap = map;
		}

		public Stack<MapNode> Search(MapNode start, MapNode goal)
		{
			var open = new List<MapNode>(); // TODO Replace with a min-heap or something similar

			foreach (var node in collisionMap.map)
			{
				node.Parent = null;
				node.H = double.MaxValue;
				node.G = double.MaxValue;
				node.IsClosed = false;
			}
			start.G = 0;
			start.H = 0;
			start.F = 0;
			start.Parent = null;
			open.Add(start);

			while (open.Count > 0)
			{
				var current = GetNodeWithLowestF(open);
				open.Remove(current);

				// If the current node is the goal we stop and recreate the path
				if (current == goal)
				{
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

					neighbour.H = Heuristic(neighbour, goal);
					var newCost = (1 + neighbour.Weight) * (current.G + neighbour.H);
					//var heur = Heuristic(current, neighbour);
					//var newCost = (1 + neighbour.Weight) * (current.G + heur);
					robot.Print(start + " to " + neighbour + " cost: " + newCost);
					//neighbour.H = goal.PhysicalPosition.Distance(neighbour.PhysicalPosition);

					neighbour.F = neighbour.G + neighbour.H;
					if (newCost < neighbour.G)
					{
						//robot.Print("Newg is more than old");
						//continue;
						open.Add(neighbour);
						neighbour.G = newCost;
					}
				}
				current.IsClosed = true;
			}

			robot.Print("Couldn't find a path!");
			return null;
		}

		public double Heuristic(MapNode from, MapNode to)
		{
			return from.PhysicalPosition.Distance(to.PhysicalPosition);
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

		private MapNode GetNodeWithLowestF(List<MapNode> list)
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
