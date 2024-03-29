﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.SteeringBehavior;
using HomeExam.Helpers;

namespace HomeExam.States
{
	public class DriverStateChaseTarget : State
	{
		private SeekBehaviour seek;

		public override void OnStart()
		{
			seek = new SeekBehaviour(robot);
		}

		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			// Simply use the steer behavior to move towards the robot's targetposition
			if (robot.TargetPosition != null)
			{
				seek.Steer(robot.TargetPosition);
			}

			return ret;
		}
	}
}
