using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Il2CppDumper
{
	public class GameConfigConverter : JavaScriptConverter
	{
		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(GameConfig) }; }
		}

		private static object TryStringHexToNum(object value, Type numType)
		{
			if(value.GetType() == typeof(string))
			{
				string numStr = value as string;
				int radix = 10;
				if (numStr.StartsWith("0x"))
				{
					radix = 16;
				}
				if (numType == typeof(ulong))
					return Convert.ToUInt64(numStr, radix);
				else
					return Convert.ToInt64(numStr, radix);
			}
			return Convert.ToUInt64(value);
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if (type == typeof(GameConfig))
			{
				var gameConfig = new GameConfig();
				foreach (var pair in dictionary)
				{
					if (pair.Key == "CodeRegistration")
					{
						var codeRegSection = pair.Value as IDictionary<string, object>;
						gameConfig.CodeRegistration = (Il2CppCodeRegistration)Deserialize(codeRegSection, typeof(Il2CppCodeRegistration), serializer);
					}
					else if (pair.Key == "MetadataRegistration")
					{
						var codeRegSection = pair.Value as IDictionary<string, object>;
						gameConfig.MetadataRegistration = (Il2CppMetadataRegistration)Deserialize(codeRegSection, typeof(Il2CppMetadataRegistration), serializer);
					}
					else if (pair.Key == "CodeRegistrationOffset" || pair.Key == "MetadataRegistrationOffset")
					{
						type.GetField(pair.Key).SetValue(gameConfig, TryStringHexToNum(pair.Value, typeof(ulong)));
					}
					else
					{
						var fieldInfo = type.GetField(pair.Key);
						if (fieldInfo != null)
						{
							fieldInfo.SetValue(gameConfig, pair.Value);
						}
					}
				}
				return gameConfig;
			}
			else
			{
				object typeToFill = null;
				if(type == typeof(Il2CppMetadataRegistration))
				{
					typeToFill = new Il2CppMetadataRegistration();
				}
				else if (type == typeof(Il2CppCodeRegistration))
				{
					typeToFill = new Il2CppCodeRegistration();
				}
				if (typeToFill != null)
				{
					foreach (var pair in dictionary)
					{
						var field = type.GetField(pair.Key);
						field.SetValue(typeToFill, TryStringHexToNum(pair.Value, field.FieldType));
					}
					return typeToFill;
				}
			}

			throw new NotImplementedException();
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

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
