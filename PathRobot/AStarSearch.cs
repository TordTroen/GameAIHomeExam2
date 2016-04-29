using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam
{
	public class AStarSearch
	{
		private readonly CollisionMap map;

		public AStarSearch(CollisionMap map)
		{
			this.map = map;
		}

		public MapNode Search(MapNode start, MapNode goal)
		{
			var closedSet = new List<MapNode>();
			var openSet = new List<MapNode> { start };

			var gScore = new Dictionary<MapNode, double>();
			var fScore = new Dictionary<MapNode, double>();
			foreach (var node in map.map)
			{
				gScore.Add(node, double.NegativeInfinity);
				fScore.Add(node, double.NegativeInfinity);
			}
			gScore[start] = 0;
			fScore[start] = HeuristicEstimate(start, goal);
			MapNode current = null;

			while (openSet.Count > 0)
			{
				current = openSet[0]; // TODO Get the node from openset with the lowest fscore value

				if (current == goal)
				{
					// TODO Return path from where we came to goal
				}

				openSet.Remove(current);
				closedSet.Add(current);

				foreach (var neighbour in current.Neighbours)
				{
					if (closedSet.Contains(neighbour))
					{
						continue;
					}
					// TODO tenative gscore
					if (!openSet.Contains(neighbour))
					{
						openSet.Add(neighbour);
					}
					else // if tenative gscore >= gscore[neighbour]
					{
						continue;
					}

					// This path is the best until now. Record it!
					//cameFrom[neighbor] := current
					//gScore[neighbor] := tentative_gScore
					//fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)
				}
			}	
			return null;
		}

		private double HeuristicEstimate(MapNode from, MapNode goal)
		{
			// TODO Implement me
			return 0;
		}
	}
}
