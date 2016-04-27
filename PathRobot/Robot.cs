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

		public Drawing Drawing { get; private set; }
		public EnemyData enemyData { get; private set;}

		private FiniteStateMachine radarFSM;
		private FiniteStateMachine driverFSM;
		private FiniteStateMachine commanderFSM;
		private CollisionMap collisionMap;

		public override void Run()
		{
			InitializeBot();
			radarFSM.EnqueueState(StateManager.StateRadarSweep);
			driverFSM.EnqueueState(StateManager.StateIdle);
			commanderFSM.EnqueueState(StateManager.StateIdle);
			long t = Time;

			while (true)
			{
				UpdateBot();

				// Debugging
				Drawing.DrawString(Color.Black, "Driver          : " + driverFSM.CurrentStateID, new Vector2D(0, -20));
				Drawing.DrawString(Color.Black, "Commander : " + commanderFSM.CurrentStateID, new Vector2D(0, -50));
				Drawing.DrawString(Color.Black, "Radar           : " + radarFSM.CurrentStateID, new Vector2D(0, -80));
				Drawing.DrawBox(Color.Red, enemyData.Position, 127);
				//foreach (var node in collisionMap.map)
				//{
				//	//Drawing.DrawBox(Color.Green, node.GetPhysicalPosition(), 127, (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
				//	string i = node.ID + "";
				//	Drawing.DrawString(Color.Red, i, node.GetPhysicalPosition());
				//}
				foreach (var node in collisionMap.map)
				{
					Out.WriteLine("fuckshit");
				}
				//if (Time > t + 100)
				//{
				//	int a = 0;
				//}
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
			//collisionMap.PaintMap(graphics);
			//foreach (var node in collisionMap.map)
			//{
			//	//graphics.DrawBox(node.IsObstacle ? Color.Red : Color.Blue, node.GetPhysicalPosition(), 127, (float)NodeSize, (float)NodeSize);
			//	Vector2D pos = node.GetPhysicalPosition();
			//	Color col = node.IsObstacle ? Color.Red : Color.Green;
			//	graphics.FillRectangle(new SolidBrush(col), (int)(pos.X - (CollisionMap.NodeSize / 2)), (int)(pos.Y - (CollisionMap.NodeSize / 2)), (float)CollisionMap.NodeSize, (float)CollisionMap.NodeSize);
			//}
		}
	}
}
