using System;
using System.Collections.Generic;
public class Timer{
	private class Task{
		public int id;
		public Timer timer;
		public Action action;
		public number interval;
		public bool repeated;
		public number time;
	}
	
	private static int idIncrementer=0;
	private static Stack<Task> taskPool=new Stack<Task>();
	private static Dictionary<int,Task> idTaskDict=new Dictionary<int,Task>();
	
	private static int CreateTask(Timer timer,Action action,number interval,bool repeated){
		if(action==null){
			return 0;
		}
		var task=taskPool.Count>0?taskPool.Pop():new Task();
		task.id=++idIncrementer;
		task.timer=timer;
		task.action=action;
		task.interval=interval;
		task.repeated=repeated;
		task.time=timer.time+interval;
		timer.tasks.Push(task.time,task);
		idTaskDict[task.id]=task;
		return task.id;
	}
	
	private static void UpdateTasks(Heap<Task> tasks,number time){
		while(tasks.Count>0){
			var task=tasks.Peek();
			if(task.time>time){
				break;
			}
			tasks.Pop();
			var action=task.action;
			if(!task.repeated){
				task.action=null;
			}
			if(task.action==null){
				if(task.timer!=null){
					idTaskDict.Remove(task.id);
					task.timer=null;
				}
				taskPool.Push(task);
			}
			else if(task.repeated){
				task.time+=task.interval;
				tasks.Push(task.time,task);
			}
			if(action!=null){
				action();
			}
		}
	}
	
	public static bool Remove(int taskId){
		if(idTaskDict.TryGetValue(taskId,out var task)){
			idTaskDict.Remove(taskId);
			task.action=null;
			task.timer=null;
			return true;
		}
		return false;
	}
	
	private Heap<Task> tasks=new Heap<Task>();
	public int taskCount{get=>tasks.Count;}
	public number time{get;private set;}
	public number deltaTime{get;private set;}
	
	public int Delay(number interval,Action action){
		return CreateTask(this,action,interval,false);
	}
	
	public int Repeat(number interval,Action action){
		return CreateTask(this,action,interval,true);
	}
	
	public void Update(number deltaTime){
		this.deltaTime=deltaTime;
		time+=deltaTime;
		UpdateTasks(tasks,time);
	}
	
	public void Clear(){
		tasks.Clear(task=>{
			Remove(task.id);
		});
	}
	
#if UNITY_2017_1_OR_NEWER
	private class TimeUpdater{
		
		private Timer timer=new Timer();
		private Func<float> getter;
		private bool started;
		private number startTime;
		
		public TimeUpdater(Func<float> getter){
			this.getter=getter;
		}
		
		public int Delay(float interval,Action action){
			Start();
			return timer.Delay((number)interval,action);
		}
		
		public int Repeat(float interval,Action action){
			Start();
			return timer.Repeat((number)interval,action);
		}
		
		private void Start(){
			if(!started){
				started=true;
				startTime=(number)getter();
				Unity.Messager.onUpdate+=Update;
			}
		}
		
		private void Update(){
			var time=(number)getter();
			timer.Update(time-startTime);
			if(timer.taskCount<=0){
				Unity.Messager.onUpdate-=Update;
				started=false;
			}
			else{
				startTime=time;
			}
		}
	}
	
	private static TimeUpdater timeUpdater=new TimeUpdater(()=>{return UnityEngine.Time.time;});
	private static TimeUpdater unscaledUpdater=new TimeUpdater(()=>{return UnityEngine.Time.unscaledTime;});
	private static TimeUpdater realtimeUpdater=new TimeUpdater(()=>{return UnityEngine.Time.realtimeSinceStartup;});
	
	public static int Delay(float interval,Action action){
		return timeUpdater.Delay(interval,action);
	}
	
	public static int Repeat(float interval,Action action){
		return timeUpdater.Repeat(interval,action);
	}
	
	public static int UnscaledDelay(float interval,Action action){
		return unscaledUpdater.Delay(interval,action);
	}
	
	public static int UnscaledRepeat(float interval,Action action){
		return unscaledUpdater.Repeat(interval,action);
	}
	
	public static int RealtimeDelay(float interval,Action action){
		return realtimeUpdater.Delay(interval,action);
	}
	
	public static int RealtimeRepeat(float interval,Action action){
		return realtimeUpdater.Repeat(interval,action);
	}
#endif
}