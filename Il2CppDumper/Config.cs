using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Il2CppDumper
{
	// Explicit game config for automation scenarios
	class GameConfig
	{
		public string EngineVersion = "";
		public ulong CodeRegistrationOffset = 0;
		public Il2CppCodeRegistration CodeRegistration = null;
		public ulong MetadataRegistrationOffset = 0;
		public Il2CppMetadataRegistration MetadataRegistration = null;

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(EngineVersion) && 
				(CodeRegistrationOffset != 0 || CodeRegistration != null) && 
				(MetadataRegistrationOffset != 0 || MetadataRegistration != null);
		}
	}

    class Config
    {
        public bool DumpMethod = true;
        public bool DumpField = true;
        public bool DumpProperty = false;
        public bool DumpAttribute = false;
        public bool DumpFieldOffset = true;
        public bool DumpMethodOffset = true;
        public bool DumpTypeDefIndex = true;
        public bool DummyDll = true;
        public bool MakeFunction = false;
        public bool ForceIl2CppVersion = false;
        public int ForceVersion = 16;
		public Dictionary<string, GameConfig> Games = new Dictionary<string, GameConfig>();
	}
}
