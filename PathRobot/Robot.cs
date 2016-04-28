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
    public class Trotor14_MechaGodzilla : AdvancedRobot
    {
		public const double Mass = 2;
		public const double MaxSpeed = 1;
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
		public MapNode TargetNode { get; private set; }

		public Drawing Drawing { get; private set; }
		public EnemyData enemyData { get; private set;}

		private FiniteStateMachine radarFSM;
		private FiniteStateMachine driverFSM;
		private FiniteStateMachine commanderFSM;
		private CollisionMap collisionMap;

		public override void Run()
		{
			InitializeBot();
			TargetNode = collisionMap.GetNode(new Vector2D(25, 25));
			radarFSM.EnqueueState(StateManager.StateRadarSweep);
			driverFSM.EnqueueState(StateManager.StateChaseTarget);
			commanderFSM.EnqueueState(StateManager.StateIdle);
			long t = Time;

			while (true)
			{
				UpdateBot();

				// Debugging
				Drawing.DrawString(Color.Black, "Driver          : " + driverFSM.CurrentStateID, new Vector2D(0, -20));
				Drawing.DrawString(Color.Black, "Commander : " + commanderFSM.CurrentStateID, new Vector2D(0, -50));
				Drawing.DrawString(Color.Black, "Radar           : " + radarFSM.CurrentStateID, new Vector2D(0, -80));
				//Drawing.DrawBox(Color.Red, enemyData.Position, 127);
				TargetNode = collisionMap.GetNode(enemyData.Position);

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
			enemyData = new EnemyData(this);
			collisionMap = new CollisionMap(this);

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
			radarFSM.Update();
			driverFSM.Update();
			commanderFSM.Update();
			Position.Set(X, Y);
		}

		public override void OnScannedRobot(ScannedRobotEvent evnt)
		{
			enemyData.SetData(evnt);
			radarFSM.EnqueueState(StateManager.StateRadarLock);
		}

		public override void OnPaint(IGraphics graphics)
		{
			//MapNode node = collisionMap.GetNode(new Vector2D(25, 25));
			//node = collisionMap.GetNode(enemyData.Position);
			//node = collisionMap.GetNode(-7, 17);
			if (TargetNode != null)
			{
				graphics.DrawBox(Color.Yellow, TargetNode.GetPhysicalPosition(), 127, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			}
			//collisionMap.PaintMap();
		}
	}
}
