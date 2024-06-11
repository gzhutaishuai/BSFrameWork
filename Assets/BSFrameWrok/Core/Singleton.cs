

	public abstract class Singleton<T> where T : class, IModule
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				//if (_instance == null)
					//MotionLog.Error($"{typeof(T)} is not create. Use {nameof(MotionEngine)}.{nameof(MotionEngine.CreateModule)} create.");
				return _instance;
			}
		}

		protected Singleton()
		{
			if (_instance != null)
				throw new System.Exception($"{typeof(T)} instance already created.");
			_instance = this as T;
		}
		protected void DestroySingleton()
		{
			_instance = null;
		}
	}
