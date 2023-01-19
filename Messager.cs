using System;
using UnityEngine;
namespace Unity{
	public class Messager:MonoBehaviour{
		
		static Messager(){
			var gameObject=new GameObject("Messager");
			GameObject.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<Messager>();
		}
		
		public static event Action onUpdate;
		public static event Action onLateUpdate;
		public static event Action onFixedUpdate;
		public static event Action<bool> onApplicationFocus;
		public static event Action<bool> onApplicationPause;
		public static event Action onApplicationQuit;
		public static event Action onGui;
		
		private void Update(){
			onUpdate?.Invoke();
		}
		
		private void LateUpdate(){
			onLateUpdate?.Invoke();
		}
		
		private void FixedUpdate(){
			onFixedUpdate?.Invoke();
		}
		
		private void OnApplicationFocus(bool hasFocus){
			onApplicationFocus?.Invoke(hasFocus);
		}
		
		private void OnApplicationPause(bool pauseStatus){
			onApplicationPause?.Invoke(pauseStatus);
		}
		
		private void OnApplicationQuit(){
			onApplicationQuit?.Invoke();
		}

		private void OnGUI(){
			onGui?.Invoke();
		}
	}
}