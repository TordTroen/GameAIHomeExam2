using System;
using HomeExam.Helpers;
using Robocode;
using Robocode.Util;
using PG4500_2016_Exam2;

namespace HomeExam
{
	public class EnemyData
	{
		public double Bearing { get; set; }
		public double BearingRadians { get; set; }
		public double Heading { get; set; }
		public double Distance { get; set; }
		public double Velocity { get; set; }
		public long UpdateTime { get; set; } // The time we last set this data
		public Vector2D Position { get; set; }
		public Vector2D LastPosition { get; set; }
		public MapNode CurrentNode { get; set; }
		private MapNode PreviousNode { get; set; }
		public bool EnteredNewNode { get { return CurrentNode != PreviousNode; } }

		private readonly Trotor14MechaGodzilla robot;
		private readonly CollisionMap collisionMap;
		public const long ValidDataTime = 10;

		public EnemyData(Trotor14MechaGodzilla robot, CollisionMap map)
		{
			this.robot = robot;
			collisionMap = map;
			Position = new Vector2D();
			LastPosition = new Vector2D();
			Reset();
		}

		public void Reset()
		{
			SetData(null);
		}

		public void SetData(ScannedRobotEvent scanEvnt)
		{
			LastPosition.Set(Position);
			PreviousNode = CurrentNode;

			if (scanEvnt != null)
			{
				Bearing = scanEvnt.Bearing;
				BearingRadians = scanEvnt.BearingRadians;
				Heading = scanEvnt.Heading;
				Distance = scanEvnt.Distance;
				Velocity = scanEvnt.Velocity;
				UpdateTime = scanEvnt.Time;

				if (robot != null)
				{
					double b = robot.HeadingRadians + scanEvnt.BearingRadians;
					Position.Set(
						robot.X + Distance * Math.Sin(b),
						robot.Y + Distance * Math.Cos(b));
				}
				CurrentNode = collisionMap.GetNode(Position, true);
				if (CurrentNode != PreviousNode)
				{
					robot.OnEnemyMovedNode();
				}
			}
			else
			{
				Bearing = 0.0;
				BearingRadians = 0.0;
				Heading = 0.0;
				Distance = 0.0;
				Velocity = 0.0;
				UpdateTime = 0;
				Position.Set(0.0, 0.0);
			}
		}

		public Vector2D GetFuturePosition(double time)
		{
			return Position.ProjectForTime(Utils.ToRadians(Heading), Velocity, time);
		}

		public bool ValidData()
		{
			long deltaTime = robot.Time - UpdateTime;
			return (deltaTime < ValidDataTime);
		}
	}
}
         