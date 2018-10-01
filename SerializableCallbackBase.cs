using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using AdvancedUnityPlugin;

public abstract class SerializableCallbackBase<TReturn> : SerializableCallbackBase {
	public InvokableCallbackBase<TReturn> func;

	public override void ClearCache() {
		base.ClearCache();
		func = null;
	}

	protected InvokableCallbackBase<TReturn> GetPersistentMethod() {
		Type[] types = new Type[ArgTypes.Length + 1];
		Array.Copy(ArgTypes, types, ArgTypes.Length);
		types[types.Length - 1] = typeof(TReturn);

		Type genericType = null;
		switch (types.Length) {
			case 1:
				genericType = typeof(InvokableCallback<>).MakeGenericType(types);
				break;
			case 2:
				genericType = typeof(InvokableCallback<,>).MakeGenericType(types);
				break;
			case 3:
				genericType = typeof(InvokableCallback<, ,>).MakeGenericType(types);
				break;
			case 4:
				genericType = typeof(InvokableCallback<, , ,>).MakeGenericType(types);
				break;
			case 5:
				genericType = typeof(InvokableCallback<, , , ,>).MakeGenericType(types);
				break;
			default:
				throw new ArgumentException(types.Length + "args");
		}
		return Activator.CreateInstance(genericType, new object[] { target, methodName }) as InvokableCallbackBase<TReturn>;
	}
}

/// <summary> An inspector-friendly serializable function </summary>
[System.Serializable]
public abstract class SerializableCallbackBase : ISerializationCallbackReceiver {

	/// <summary> Target object </summary>
	public Object target { get { return _target; } set { _target = value; ClearCache(); } }
	/// <summary> Target method name </summary>
	public string methodName { get { return _methodName; } set { _methodName = value; ClearCache(); } }
	public object[] Args { get { return args != null ? args : args = _args.Select(x => x.GetValue()).ToArray(); } }
	public object[] args;
	public Type[] ArgTypes { get { return argTypes != null ? argTypes : argTypes = _args.Select(x => Arg.RealType(x.argType)).ToArray(); } }
	public Type[] argTypes;
	public bool dynamic { get { return _dynamic; } set { _dynamic = value; ClearCache(); } }

	[SerializeField] protected Object _target;
	[SerializeField] protected string _methodName;
	[SerializeField] protected Arg[] _args;
	[SerializeField] protected bool _dynamic;
#if UNITY_EDITOR
#pragma warning disable 0414
	[SerializeField] private string _typeName;
#pragma warning restore 0414
#endif

#if UNITY_EDITOR
	[SerializeField] private bool dirty;
#endif

#if UNITY_EDITOR
	protected SerializableCallbackBase() {
		_typeName = base.GetType().AssemblyQualifiedName;
	}
#endif

	public virtual void ClearCache() {
		argTypes = null;
		args = null;
	}

	public void SetMethod(Object target, string methodName, bool dynamic, params Arg[] args) {
		_target = target;
		_methodName = methodName;
		_dynamic = dynamic;
		_args = args;
		ClearCache();
	}

	protected abstract void Cache();

	public void OnBeforeSerialize() {
#if UNITY_EDITOR
		if (dirty) { ClearCache(); dirty = false; }
#endif
	}

	public void OnAfterDeserialize() {
#if UNITY_EDITOR
		_typeName = base.GetType().AssemblyQualifiedName;
#endif
	}
}

[System.Serializable]
public struct Arg {
	public enum ArgType { Unsupported, Bool, Int, Float, String, Object, GameObject, ScriptableObject, KeyValuePairsArrayVariable, StringVariable, FloatVariable}
    public bool boolValue;
	public int intValue;
	public float floatValue;
	public string stringValue;
	public Object objectValue;
	public ArgType argType;
    public GameObject gameObjectValue;
    public StringVariable stringVariableValue;
    public ScriptableObject scriptableObjectValue;
    public KeyValuePairsArrayVariable keyValuePairsArrayVariable;
    public FloatVariable floatVariable;

    public object GetValue() {
		return GetValue(argType);
	}

	public object GetValue(ArgType type) {
		switch (type) {
			case ArgType.Bool:
				return boolValue;
			case ArgType.Int:
				return intValue;
			case ArgType.Float:
				return floatValue;
			case ArgType.String:
				return stringValue;
			case ArgType.Object:
				return objectValue;
            case ArgType.GameObject:
                return gameObjectValue;
            case ArgType.StringVariable:
                return stringVariableValue;
            case ArgType.ScriptableObject:
                return scriptableObjectValue;
            case ArgType.KeyValuePairsArrayVariable:
                return keyValuePairsArrayVariable;
            case ArgType.FloatVariable:
                return floatVariable;
            default:
				return null;
		}
	}

	public static Type RealType(ArgType type) {
		switch (type) {
			case ArgType.Bool:
				return typeof(bool);
			case ArgType.Int:
				return typeof(int);
			case ArgType.Float:
				return typeof(float);
			case ArgType.String:
				return typeof(string);
			case ArgType.Object:
				return typeof(Object);
            case ArgType.GameObject:
                return typeof(GameObject);
            case ArgType.StringVariable:
                return typeof(StringVariable);
            case ArgType.ScriptableObject:
                return typeof(ScriptableObject);
            case ArgType.KeyValuePairsArrayVariable:
                return typeof(KeyValuePairsArrayVariable);
            case ArgType.FloatVariable:
                return typeof(FloatVariable);
            default:
				return null;
		}
	}

	public static ArgType FromRealType(Type type) {
		if (type == typeof(bool)) return ArgType.Bool;
		else if (type == typeof(int)) return ArgType.Int;
		else if (type == typeof(float)) return ArgType.Float;
		else if (type == typeof(String)) return ArgType.String;
		else if (type == typeof(Object)) return ArgType.Object;
        else if (type == typeof(GameObject)) return ArgType.GameObject;
        else if (type == typeof(StringVariable)) return ArgType.StringVariable;
        else if (type == typeof(ScriptableObject)) return ArgType.ScriptableObject;
        else if (type == typeof(KeyValuePairsArrayVariable)) return ArgType.KeyValuePairsArrayVariable;
        else if (type == typeof(FloatVariable)) return ArgType.FloatVariable;
        else return ArgType.Unsupported;
	}

	public static bool IsSupported(Type type) {
		return FromRealType(type) != ArgType.Unsupported;
	}
}