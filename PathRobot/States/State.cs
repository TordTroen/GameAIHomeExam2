using PG4500_2016_Exam2;

namespace HomeExam.States
{
	public class State
	{
		public string Id { get; private set; }
		protected Trotor14MechaGodzilla robot;

		public void Initialize(string id, Trotor14MechaGodzilla robot)
		{
			Id = id;
			this.robot = robot;
			OnStart();
		}

		/// <summary>
		/// Called just once after the state is initialized.
		/// </summary>
		public virtual void OnStart() { }

		public virtual void OnEnter() { }

		public virtual string OnUpdate()
		{
			return null;
		}

		public virtual void OnExit() { }
	}
}
