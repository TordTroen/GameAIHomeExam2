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

		public override void Run()
		{
			InitializeBot();
			radarFSM.EnqueueState(StateManager.StateIdle);
			driverFSM.EnqueueState(StateManager.StateIdle);
			commanderFSM.EnqueueState(StateManager.StateIdle);

			while (true)
			{
				UpdateBot();

				// Debugging
				Drawing.DrawString(Color.Black, "Driver    : " + driverFSM.CurrentStateID, new Vector2D(0, -20));
				Drawing.DrawString(Color.Black, "Commander : " + commanderFSM.CurrentStateID, new Vector2D(0, -50));
				Drawing.DrawString(Color.Black, "Radar     : " + radarFSM.CurrentStateID, new Vector2D(0, -80));

				Execute();
			}
		}

		private void InitializeBot()
		{
			radarFSM = new FiniteStateMachine(this);
			driverFSM = new FiniteStateMachine(this);
			commanderFSM = new FiniteStateMachine(this);
			Position = new Vector2D();
			Drawing = new Drawing(this);
			enemyData = new EnemyData(this);

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
	}
}
