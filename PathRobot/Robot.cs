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

		public const long CollisionAvoidanceDuration = 16; // Turns before we continue the normal movement
		public const double CollisionAvoidanceDistance = CollisionMap.NodeSize * 4; // Distance to the enemy where we stop for a while
		public const double CollisionAvoidanceSpeed = 1; // Speed when we are slowing down when trying to avoid collision
		public double CurrentSpeed { get; private set; }

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
		private bool hasReachedFirstNodeInPath = false;
		private MapNode startingNode;

		// Testing vars
		bool collisionCourse = false;
		double enemyHeading;
		MapNode prevLocked;

		public override void Run()
		{
			InitializeBot();
			
			radarFSM.EnqueueState(StateManager.StateRadarSweep);
			driverFSM.EnqueueState(StateManager.StateTurnToTarget);
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
			startingNode = collisionMap.GetNode(new Vector2D(25, 25), false);
			SetGoalNode(startingNode);

			// Test var
			long resetStartTime = -1;

			while (true)
			{
				UpdateBot();

				FollowPath();

				//////// Trying to avoid the predicted
				if (enemyData.PredictedNode != null)
				{
					if (nodePath.Contains(enemyData.PredictedNode))
					{
						Print("---------------- Path contains predicted ----------------");
						// TODO Include neighbours of that node
						if (prevLocked != null)
						{
							prevLocked.IsLocked = false;
						}
						enemyData.PredictedNode.IsLocked = true;
						prevLocked = enemyData.PredictedNode;

						nodePath = aStarSearch.Search(TargetNode, GoalNode);
					}
				}


				//////// Slowing down/stopping
				//if (enemyData.PredictedNode != null)
				//{
				//	if (enemyData.PredictedNode == TargetNode || (nodePath != null && nodePath.Contains(enemyData.PredictedNode)))
				//	{
				//		Print("----------------------- Predicted node is TargetNode or Path! -----------------------");
				//		// TODO Slow down
				//		CurrentSpeed = CollisionAvoidanceSpeed;
				//		//resetStartTime = Time;
				//	}
				//	else if (resetStartTime == -1)
				//	{
				//		CurrentSpeed = MaxSpeed;
				//	}
				//}
				//if (resetStartTime == -1 && enemyData.Distance < CollisionAvoidanceDistance)
				//{
				//	// TODO Stop
				//	Print("----------------------- To close for comfort! -----------------------");
				//	CurrentSpeed = 0.1;
				//	resetStartTime = Time;
				//}
				//if (resetStartTime != -1)
				//{
				//	Print("Reset:" + resetStartTime + ", Time: " + Time);
				//	if (Time - resetStartTime > CollisionAvoidanceDuration)
				//	{
				//		CurrentSpeed = MaxSpeed;
				//		resetStartTime = -1;
				//	}
				//}

				//////// Repathing and accounting for the enemy position
				//if (enemyData.Distance < CollisionMap.NodeSize * 4)
				//{
				//	Print("Predicted moved");
				//	if (enemyData.PrevNode != null)
				//	{
				//		enemyData.PrevNode.Weight = 0;
				//		foreach (var node in enemyData.PrevNode.Neighbours)
				//		{
				//			node.Weight = 0;
				//			foreach (var node2 in node.Neighbours)
				//			{
				//				node2.Weight = 0;
				//			}
				//		}
				//	}
				//	if (enemyData.CurrentNode != null)
				//	{
				//		const double enemyNodeWeight = 2;
				//		enemyData.CurrentNode.Weight = enemyNodeWeight;
				//		foreach (var node in enemyData.CurrentNode.Neighbours)
				//		{
				//			node.Weight = enemyNodeWeight;
				//			foreach (var node2 in node.Neighbours)
				//			{
				//				node2.Weight = enemyNodeWeight;
				//			}
				//		}
				//	}
				//	if (GoalNode != null && TargetNode != null)
				//	{
				//		//SetGoalNode(GoalNode);
				//		//TargetNode = null;
				//		//nodePath = aStarSearch.Search(TargetNode, GoalNode);
				//		//TargetNode = nodePath.Pop();
				//	}
				//}

				//////// Repathing and accounting for the enemy predicted position (this moved through obstacles
				//if (enemyData.PredictedNode != enemyData.PrevPredictedNode && enemyData.Distance < CollisionMap.NodeSize * 30)
				//{
				//	Print("Predicted moved");
				//	if (enemyData.PrevPredictedNode != null)
				//	{
				//		enemyData.PrevPredictedNode.Weight = 0;
				//		foreach (var node in enemyData.PrevPredictedNode.Neighbours)
				//		{
				//			node.Weight = 0;
				//		}
				//	}
				//	if (enemyData.PredictedNode != null)
				//	{
				//		enemyData.PredictedNode.Weight = 10;
				//		foreach (var node in enemyData.PredictedNode.Neighbours)
				//		{
				//			node.Weight = 10;
				//		}
				//	}
				//	if (GoalNode != null)
				//	{
				//		//SetGoalNode(GoalNode);
				//		nodePath = aStarSearch.Search(CurrentNode, GoalNode);
				//	}
				//}

				//////// Trying to figure out if we are about to collide
				/* If enemy and player might collide
					could check it simply by checking distance & direction (maybe "raycast" with the width of the tank)
						either just try to turn and move around
						or add the enemy's predicted path to the collision map? 
							maybe just give it a bigger weighting so that if it blocks completly the robot can still find a path
				*/
				//double collisionRange = 100;
				//enemyHeading = enemyData.Heading;
				////if (enemyData.Distance < collisionRange)
				//{
				//	enemyHeading -= 180;
				//	if (enemyHeading < 0)
				//	{
				//		enemyHeading += 360;
				//	}
				//	//collisionCourse = (enemyHeading.IsAngleNear(Heading, 35));
				//	collisionCourse = Math.Abs(enemyData.Position.Dot(Position)) > 0.9;
				//	// TODO Figure out if we are close to non-paralell? the opposite of 0 from the dot product when they are paralell
				//	if (collisionCourse == true)
				//	{
				//		//Print("Collision course!");
				//	}

				//	//Print("Collision range");
				//}

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
			CurrentSpeed = MaxSpeed;
		}

		private void FollowPath()
		{
			// Check if we have reached the initial starting position
			if (!hasReachedStartPosition && Position.Distance(startingNode.PhysicalPosition) < TargetNodeDistance)
			{
				hasReachedStartPosition = true;
			}

			// Check if we have reached the targetnode, and if so pop a node from the path, and set that as the target
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

			// Path to where the enemy stopped if we aren't already pathing
			if ((GoalNode == null || nodePath == null) && enemyData.Velocity.IsZero())
			{
				if (enemyData.CurrentNode != null)
				{
					SetGoalNode(enemyData.CurrentNode);
				}
			}
			if (!hasReachedFirstNodeInPath && TargetNode != null)
			{
				driverFSM.EnqueueState(StateManager.StateTurnToTarget);
				hasReachedFirstNodeInPath = true;
			}
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
			hasReachedFirstNodeInPath = false;
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

			if (enemyData != null && enemyData.PredictedNode != null)
			{
				Drawing.DrawBox(Color.Yellow, enemyData.PredictedNode.PhysicalPosition, 127, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
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

			Drawing.DrawString(Color.Black, "Heading: " + Heading, new Vector2D(200, -20));
			Drawing.DrawString(Color.Black, "RealEnemyHeading: " + enemyData.Heading, new Vector2D(200, -40));
			Drawing.DrawString(Color.Black, "CalcEnemyHeading: " + enemyHeading, new Vector2D(200, -60));
			Drawing.DrawString(Color.Black, "CollisionCourse: " + collisionCourse, new Vector2D(200, -80));
			Drawing.DrawString(Color.Black, "Distance: " + enemyData.Distance, new Vector2D(200, -100));
			Drawing.DrawString(Color.Black, "Speed: " + CurrentSpeed, new Vector2D(200, -120));
		}
	}
}
