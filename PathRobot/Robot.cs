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
		public const double Mass = .1;
		public const double MaxSpeed = 8;
		public const double PrefferedEnemyDistance = 250;

		public Vector2D Position { get; private set; }
		public Vector2D VelocityVector
		{
			get
			{
				if (Velocity.IsZero())
				{
					return new Vector2D();
				}
				return new Vector2D(Velocity * Math.Cos(HeadingRadians), Velocity * Math.Sin(HeadingRadians));
			}
		}
		public MapNode CurrentNode { get; private set; }
		public MapNode TargetNode { get; private set; }

		public Drawing Drawing { get; private set; }
		public EnemyData enemyData { get; private set;}

		private FiniteStateMachine radarFSM;
		private FiniteStateMachine driverFSM;
		private FiniteStateMachine commanderFSM;
		private CollisionMap collisionMap;
		private AStarSearch aStarSearch;
		//private MapNode startNode;
		private MapNode goalNode;
		private Stack<MapNode> nodePath;

		public override void Run()
		{
			InitializeBot();
			//TargetNode = collisionMap.GetNode(new Vector2D(25, 25), false);
			
			radarFSM.EnqueueState(StateManager.StateRadarSweep);
			driverFSM.EnqueueState(StateManager.StateChaseTarget);
			commanderFSM.EnqueueState(StateManager.StateIdle);

			//startNode = collisionMap.GetNode(0, 10, false);
			//goalNode = collisionMap.GetNode(14, 2, false);
			//goalNode = collisionMap.GetNode(new Vector2D(25, 25), false);

			//UpdateBot();
			//nodePath = aStarSearch.Search(CurrentNode, goalNode);

			while (true)
			{
				UpdateBot();

				if (enemyData.EnteredNewNode)
				{
					goalNode = enemyData.CurrentNode;
					TargetNode = null;
					nodePath = aStarSearch.Search(CurrentNode, goalNode);
				}

				if (goalNode != null && (TargetNode == null || TargetNode == CurrentNode || TargetNode.Neighbours.Contains(CurrentNode)))
				{
					if (nodePath != null && nodePath.Count > 0)
					{
						TargetNode = nodePath.Pop();
						Out.WriteLine("New targetnode is: " + TargetNode);
					}
					else
					{
						TargetNode = null;
						Out.WriteLine("Targetnode is null");
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
			aStarSearch = new AStarSearch(collisionMap);

			SetColors(Color.LightGray, Color.DimGray, Color.Gray, Color.Yellow, Color.Red);
			IsAdjustRadarForGunTurn = false;
			IsAdjustGunForRobotTurn = false;
			IsAdjustRadarForRobotTurn = false;
		}

		/// <summary>
		/// Does everything that should be done every turn.
		/// </summary>
		private void UpdateBot()
		{
			
			Position.Set(X, Y);
			CurrentNode = collisionMap.GetNode(Position, true);
		}

		public override void OnScannedRobot(ScannedRobotEvent evnt)
		{
			enemyData.SetData(evnt);
			radarFSM.EnqueueState(StateManager.StateRadarLock);
		}

		public override void OnMouseMoved(MouseEvent e)
		{
			return;
			MapNode node = collisionMap.GetNode(new Vector2D(e.X, e.Y), true);
			if (node != null)
			{
				goalNode = node;
				TargetNode = null;
				nodePath = aStarSearch.Search(CurrentNode, goalNode);
			}
		}

		public override void OnMouseClicked(MouseEvent e)
		{
			MapNode node = collisionMap.GetNode(new Vector2D(e.X, e.Y), true);
			if (node != null)
			{
				// Left click to set startnode
				// Right click to set goalnode
				if (e.Button == 1)
				{
					goalNode = node;
					TargetNode = null;
					nodePath = aStarSearch.Search(CurrentNode, goalNode);
				}
				else if (e.Button == 3)
				{
					//goalNode = node;
				}
			}
		}

		public override void OnPaint(IGraphics graphics)
		{
			if (CurrentNode != null)
			{
				Drawing.DrawBox(Color.Blue, CurrentNode.PhysicalPosition, 50, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			}
			if (goalNode != null)
			{
				Drawing.DrawBox(Color.Green, goalNode.PhysicalPosition, 50, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
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

			Drawing.DrawString(Color.Black, "Driver          : " + driverFSM.CurrentStateID, new Vector2D(0, -20));
			Drawing.DrawString(Color.Black, "Commander : " + commanderFSM.CurrentStateID, new Vector2D(0, -50));
			Drawing.DrawString(Color.Black, "Radar           : " + radarFSM.CurrentStateID, new Vector2D(0, -80));
		}
	}
}
