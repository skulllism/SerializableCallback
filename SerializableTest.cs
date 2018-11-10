using System;
using System.Diagnostics;
using UnityEngine;
using AdvancedUnityPlugin;

public class SerializableTest : MonoBehaviour {
	const int ITERATIONS = 100000;
	public float f = 0.5f;
	public string s;
	public System.Func<float, bool> RegularDelegate;
	public System.Func<float, bool> DynamicDelegate;
	public ConditionArgString condition;
	public SerializableEvent ev;

	void Start() {
		RegularDelegate = TestMethod;
		DynamicDelegate = (System.Func<float, bool>) System.Delegate.CreateDelegate(typeof(System.Func<float, bool>), this, "TestMethod");
	}

	void Update() {
		var method = Stopwatch.StartNew();
		bool methodb = false;
		for (int i = 0; i < ITERATIONS; ++i) {
			methodb = TestMethod(f);
		}
		method.Stop();

		var regularDelegate = Stopwatch.StartNew();
		bool regularDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i) {
			regularDelegateb = RegularDelegate(f);
		}
		regularDelegate.Stop();

		var dynamicDelegate = Stopwatch.StartNew();
		bool dynamicDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i) {
			dynamicDelegateb = DynamicDelegate(f);
		}
		dynamicDelegate.Stop();

		var serializedDelegate = Stopwatch.StartNew();
		bool serializedDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i) {
		}
		serializedDelegate.Stop();

		var serializedEvent = Stopwatch.StartNew();
		for (int i = 0; i < ITERATIONS; ++i) {
			ev.Invoke();
		}
		serializedEvent.Stop();

		UnityEngine.Debug.Log("Method: " + methodb + method.Elapsed);
		UnityEngine.Debug.Log("RegularDelegate: " + regularDelegateb + regularDelegate.Elapsed);
		UnityEngine.Debug.Log("DynamicDelegate: " + dynamicDelegateb + dynamicDelegate.Elapsed);
		UnityEngine.Debug.Log("SerializedCallback: " + serializedDelegateb + serializedDelegate.Elapsed);
		UnityEngine.Debug.Log("SerializedEvent: " + serializedEvent.Elapsed);
	}

	public bool TestMethod(float f) {
		return f > 0.5f;
	}

	public bool TestMethod(string a) {
		return string.IsNullOrEmpty(a);
	}

	public bool TestMethod2(float f, string a) {
		return f > 0.5f && string.IsNullOrEmpty(a);
	}

	public void TestMethod2(string a) {
		s = a;
	}
}