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
		private List<MapNode> openSet = new List<MapNode>();
		private Dictionary<MapNode, double> gScore = new Dictionary<MapNode, double>();
		private Dictionary<MapNode, double> fScore = new Dictionary<MapNode, double>();
		private Dictionary<MapNode, MapNode> cameFrom = new Dictionary<MapNode, MapNode>();

		public AStarSearch(CollisionMap map)
		{
			collisionMap = map;

			gScore = new Dictionary<MapNode, double>();
			fScore = new Dictionary<MapNode, double>();
			foreach (var node in collisionMap.map)
			{
				if (gScore.ContainsKey(node) || fScore.ContainsKey(node))
				{
					continue;
				}
				gScore.Add(node, double.NegativeInfinity);
				fScore.Add(node, double.NegativeInfinity);
			}
		}

		public Queue<MapNode> Search(MapNode start, MapNode goal)
		{
			closedSet = new List<MapNode>();
			openSet = new List<MapNode> { start };
			ResetCollections();
			gScore[start] = 0;
			start.Score = HeuristicEstimate(start, goal);
			fScore[start] = start.Score;
			MapNode current = null;
			double tenativeScore = 0;

			while (openSet.Count > 0)
			{
				current = openSet[0]; // TODO Get the node from openset with the lowest fscore value

				if (current == goal)
				{
					// TODO Return path from where we came to goal
					return ReconstructPath(current);
				}

				openSet.Remove(current);
				closedSet.Add(current);

				foreach (var neighbour in current.Neighbours)
				{
					if (closedSet.Contains(neighbour))
					{
						continue;
					}
					tenativeScore = gScore[current] + current.GetPhysicalPosition().Distance(neighbour.GetPhysicalPosition());
					if (!openSet.Contains(neighbour))
					{
						openSet.Add(neighbour);
					}
					else if (tenativeScore >= gScore[neighbour])
					{
						continue;
					}

					if (cameFrom.ContainsKey(neighbour))
					{
						cameFrom[neighbour] = current;
					}
					else
					{
						cameFrom.Add(neighbour, current);
					}
					gScore[neighbour] = tenativeScore;
					fScore[neighbour] = gScore[neighbour] + HeuristicEstimate(neighbour, goal);
					// This path is the best until now. Record it!
					//cameFrom[neighbor] := current
					//gScore[neighbor] := tentative_gScore
					//fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)
				}
				SortOpenSet();
			}
			return null;
		}
		
		private Queue<MapNode> ReconstructPath(MapNode current)
		{
			var path = new Queue<MapNode>();
			path.Enqueue(current);
			while (cameFrom.Keys.Contains(current))
			{
				current = cameFrom[current];
				//cameFrom.Remove(current);
				path.Enqueue(current);
			}
			return path;
		}

		private double HeuristicEstimate(MapNode from, MapNode goal)
		{
			// TODO Improve me
			double results = 0;
			results = from.GetPhysicalPosition().Distance(goal.GetPhysicalPosition());
			return results;
		}

		private void SortOpenSet()
		{
			//openSet.OrderBy(x => x.Score);
			openSet.Sort((x, y) => x.Score.CompareTo(y.Score));
		}

		private void ResetCollections()
		{
			foreach (var node in collisionMap.map)
			{
				gScore[node] = double.NegativeInfinity;
				fScore[node] = double.NegativeInfinity;
			}
		}
	}
}
