﻿using HomeExam.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode.Util;

namespace HomeExam.States
{
	public class DriverStateTurnToTarget : State
	{
		public override string OnUpdate()
		{
			double angle = 0;
			if (robot.TargetNode != null)
			{
				angle = Vector2D.RotationAngleFromVectors(robot.Position, robot.TargetNode.Position, robot.Heading);
				robot.SetTurnRight(angle);
			}

			if (angle.IsZero())
			{
				return StateManager.StateChaseTarget;
			}
			return null;
		}
	}
}
