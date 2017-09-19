using UnityEngine;

namespace Brainiac
{
	public enum LogLevel
	{
		Info, Warning, Error
	}

	[AddNodeMenu("Action/Debug")]
	public class DebugLog : Action
	{
		private LogLevel m_level;
		private string m_message;
		
		public LogLevel Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}
		
		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				m_message = value;
			}
		}

		public override string Title
		{
			get
			{
				return "Debug";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
#if UNITY_EDITOR
			switch (m_level)
			{
				case LogLevel.Info:
					DLog.Log(m_message);
					break;
				case LogLevel.Warning:
				    DLog.LogWarning(m_message);
					break;
				case LogLevel.Error:
				    DLog.LogError(m_message);
					break;
			}
#endif
            return BehaviourNodeStatus.Success;
		}
	}
}
