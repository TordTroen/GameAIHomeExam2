﻿using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam.States
{
	/// <summary>
	/// State that tries to lock the radar to the enemy.
	/// </summary>
	public class RadarStateScanLock : State
	{
		private const double LockValue = 1;

		public override void OnEnter()
		{
			RadarLock();
		}

		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			// Start the Radar sweep state if the enemydata is invalid (hasn't been updated in a while)
			if (!robot.enemyData.ValidData())
			{
				ret = StateManager.StateRadarSweep;
			}
			else
			{
				RadarLock();
			}
			return ret;
		}

		private void RadarLock()
		{
			double turn = robot.HeadingRadians + robot.enemyData.BearingRadians - robot.RadarHeadingRadians;
			robot.SetTurnRadarRightRadians(LockValue * Utils.NormalRelativeAngle(turn));
		}
	}
}
