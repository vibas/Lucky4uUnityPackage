using System;

namespace Lucky4u.Common
{
	public enum StartupType
	{
		NORMAL,
		PREFAB
	}

	public class StartupAttribute : Attribute
	{
		public StartupType Type { get; set; }
		public Type ParentType { get; set; }
		public StartupAttribute(StartupType type = StartupType.NORMAL)
		{
			Type = type;
			ParentType = null;
		}

		public StartupAttribute(StartupType type, Type parentType, string prefabURL = "")
		{
			Type = type;
			ParentType = parentType;
		}
	}
}