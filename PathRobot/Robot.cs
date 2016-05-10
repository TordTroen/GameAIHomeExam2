using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode;
using HomeExam;
using HomeExam.States;
using HomeExam.Helpers;
using System.Drawing;

namespace PG4500_2016_Exam2
{
    public class Trotor14MechaGodzilla : AdvancedRobot
    {
		public const double Mass = 1;
		public const double MaxSpeed = 8;
	    private const double TargetNodeDistance = 40;

		public Vector2D Position { get; private set; }
		public Vector2D VelocityVector
		{
			get
			{
				if (Velocity.IsZero()) // If we aren't moving return an empty vector so we don't have to do the cos/sin
				{
					return new Vector2D(0, 0);
				}
				return new Vector2D(Velocity * Math.Cos(HeadingRadians), Velocity * Math.Sin(HeadingRadians));
			}
		}
		public MapNode TargetNode { get; private set; }
		private MapNode CurrentNode { get; set; }
		private MapNode GoalNode { get; set; }

		public Drawing Drawing { get; private set; }
		public EnemyData enemyData { get; private set;}

		private FiniteStateMachine radarFSM;
		private FiniteStateMachine driverFSM;
		private FiniteStateMachine commanderFSM;
		private CollisionMap collisionMap;
		private AStarSearch aStarSearch;
		private Stack<MapNode> nodePath;
		private bool hasReachedStartPosition = false;

		public override void Run()
		{
			InitializeBot();
			
			radarFSM.EnqueueState(StateManager.StateRadarSweep);
			driverFSM.EnqueueState(StateManager.StateChaseTarget);
			commanderFSM.EnqueueState(StateManager.StateIdle);

			// This was used for testing the algorithm
			////startNode = collisionMap.GetNode(0, 10, false);
			////goalNode = collisionMap.GetNode(14, 2, false);
			////goalNode = collisionMap.GetNode(new Vector2D(25, 25), false);

			////UpdateBot();
			////nodePath = aStarSearch.Search(CurrentNode, goalNode);
			//TargetNode = collisionMap.GetNode(0, 0, false);
			//goalNode = collisionMap.GetNode(15, 4, false);

			////SetGoalNode(b);
			//nodePath = aStarSearch.Search(TargetNode, goalNode);
			//Out.WriteLine("======================");
			//Out.WriteLine("Path:");
			//foreach (var node in nodePath)
			//{
			//	Out.WriteLine("=> " + node);
			//}
			//Out.WriteLine("======================");

			UpdateBot();

			// Initially start going to [25, 25]
			var startingNode = collisionMap.GetNode(new Vector2D(25, 25), false);
			SetGoalNode(startingNode);

			while (true)
			{
				UpdateBot();

				// Check if we have reached the initial starting position
				if (!hasReachedStartPosition && Position.Distance(startingNode.PhysicalPosition) < TargetNodeDistance)
				{
					hasReachedStartPosition = true;
				}

				// Check if we have reached the targetnode
				bool reachedTargetNode = CurrentNode == TargetNode;
				if (reachedTargetNode == false && TargetNode != null)
				{
					reachedTargetNode = Position.Distance(TargetNode.PhysicalPosition) < TargetNodeDistance;
				}

				if (nodePath != null && nodePath.Count > 0)
				{
					if (GoalNode != null && (reachedTargetNode || TargetNode == null))
					{
						if (nodePath != null && nodePath.Count > 0)
						{
							TargetNode = nodePath.Pop();
							Out.WriteLine("New targetnode is: " + TargetNode);
						}
						
					}
				}
				if (CurrentNode == GoalNode)
				{
					TargetNode = null;
					nodePath = null;
					GoalNode = null;
					Out.WriteLine("No new target");
				}
				if ((GoalNode == null || nodePath == null) && enemyData.Velocity.IsZero())
				{
					if (enemyData.CurrentNode != null)
					{
						SetGoalNode(enemyData.CurrentNode);
					}
				}

				radarFSM.Update();
				driverFSM.Update();
				commanderFSM.Update();
				Scan();
				Execute();
			}
		}

		private void InitializeBot()
		{
			radarFSM = new FiniteStateMachine("Radar", this);
			driverFSM = new FiniteStateMachine("Driver", this);
			commanderFSM = new FiniteStateMachine("Commander", this);
			Position = new Vector2D();
			Drawing = new Drawing(this);
			collisionMap = new CollisionMap(this);
			enemyData = new EnemyData(this, collisionMap);
			aStarSearch = new AStarSearch(this, collisionMap);

			SetColors(Color.LightGray, Color.DimGray, Color.Gray, Color.Yellow, Color.Red);
			IsAdjustRadarForGunTurn = false;
			IsAdjustGunForRobotTurn = false;
			IsAdjustRadarForRobotTurn = false;
		}

	    private void SetGoalNode(MapNode goal)
	    {
		    if (goal == null)
		    {
			    throw new ArgumentNullException(nameof(goal), "The specified goal node can't be null!");
		    }

			GoalNode = goal;// enemyData.CurrentNode;

			// If the goalnode is a obstacle, use one of the neighbours that probably aren't an obstacle 
			//if (collisionMap.obstacles.Contains(GoalNode))
			//{
			//	var possibleNodes = GoalNode.Neighbours.Except(collisionMap.obstacles).ToList();
			//	if (possibleNodes.Count > 0)
			//	{
			//		GoalNode = possibleNodes[0];
			//	}
			//}
			TargetNode = null;
			nodePath = aStarSearch.Search(CurrentNode, GoalNode);
		}

		/// <summary>
		/// Does everything that should be done every turn.
		/// </summary>
		private void UpdateBot()
		{
			Position.Set(X, Y);

			CurrentNode = collisionMap.GetNode(Position, true);
		}

		public void Print(string msg)
		{
			Out.WriteLine(msg);
		}

		public override void OnScannedRobot(ScannedRobotEvent evnt)
		{
			enemyData.SetData(evnt);
			radarFSM.EnqueueState(StateManager.StateRadarLock);
		}

		public override void OnMouseMoved(MouseEvent e)
		{
			//MapNode node = collisionMap.GetNode(new Vector2D(e.X, e.Y), true);
			//if (node != null)
			//{
			//	SetGoalNode(node);
			//}
		}

		public override void OnMouseClicked(MouseEvent e)
		{
			MapNode node = collisionMap.GetNode(new Vector2D(e.X, e.Y), true);
			if (node != null)
			{
				// Left click to set startnode
				// Right click to set goalnode
				if (e.Button == 1 && GoalNode != null)
				{
					//CurrentNode = node;
					TargetNode = null;
					nodePath = aStarSearch.Search(node, GoalNode);
				}
				else if (e.Button == 3)
				{
					//goalNode = node;
					SetGoalNode(node);
				}
			}
		}

		public override void OnPaint(IGraphics graphics)
		{
			if (CurrentNode != null)
			{
				Drawing.DrawBox(Color.Blue, CurrentNode.PhysicalPosition, 120, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			}
			if (GoalNode != null)
			{
				Drawing.DrawBox(Color.Green, GoalNode.PhysicalPosition, 120, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			}

			if (TargetNode != null)
			{
				Drawing.DrawBox(Color.Pink, TargetNode.PhysicalPosition, 127, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			}

			if (nodePath != null)
			{
				foreach (var node in nodePath)
				{
					//if (node == startNode || node == goalNode) continue;

					Drawing.DrawBox(Color.White, node.PhysicalPosition, 50, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
				}
			}

			// Paint the enemy
			//if (enemyData.CurrentNode != null)
			//{
			//	Drawing.DrawBox(Color.Red, enemyData.CurrentNode.PhysicalPosition, 50, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);

			//	// Paint the neighbours
			//	foreach (var node in enemyData.CurrentNode.Neighbours)
			//	{
			//		Drawing.DrawBox(Color.Orange, node.PhysicalPosition, 40, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			//	}
			//}

			Drawing.DrawString(Color.Black, "Driver: " + driverFSM.CurrentStateID, new Vector2D(0, -20));
			Drawing.DrawString(Color.Black, "Commander: " + commanderFSM.CurrentStateID, new Vector2D(0, -40));
			Drawing.DrawString(Color.Black, "Radar: " + radarFSM.CurrentStateID, new Vector2D(0, -60));
			Drawing.DrawString(Color.Black, "CurrentNode: " + ((CurrentNode == null) ? "null" : CurrentNode.ToString()), new Vector2D(0, -80));
			Drawing.DrawString(Color.Black, "TargetNode: " + ((TargetNode == null) ? "null" : TargetNode.ToString()), new Vector2D(0, -100));
			Drawing.DrawString(Color.Black, "GoalNode: " + ((GoalNode == null) ? "null" : GoalNode.ToString()), new Vector2D(0, -120));
			Drawing.DrawString(Color.Black, "NodePath: " + ((nodePath == null) ? "null" : nodePath.Count.ToString()), new Vector2D(0, -140));
		}
	}
}
