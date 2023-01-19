using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Unity{
	public class Tween:MonoBehaviour{
		public enum Field{
			Alpha,
			FillAmount,
			SizeDeltaX,
			SizeDeltaY,
			AnchoredPositionX,
			AnchoredPositionY,
			AnchoredPositionZ,
			PositionX,
			PositionY,
			PositionZ,
			RotationX,
			RotationY,
			RotationZ,
			ScaleX,
			ScaleY,
			ScaleZ,
			Scale,
			Delay,
		}
		
		public enum Ease{
			Linear,
			SineIn,
			CircIn,
			ExpoIn,
			BackIn,
			SineOut,
			CircOut,
			ExpoOut,
			BackOut,
			BounceOut,
			ElasticOut,
		}
		
		private class Task{
			public Field field;
			public float time;
			public float duration;
			public float from;
			public float to;
			public Ease ease;
			public bool enabled;
			public Action onFinish;
		}
		
		private static Dictionary<Ease,Func<float,float>> easeFuncDict=new Dictionary<Ease,Func<float,float>>(){
			[Ease.SineIn]=delegate(float t){
				return 1f-Mathf.Cos(t*1.57f);
			},
			[Ease.CircIn]=delegate(float t){
				return 1f-Mathf.Sqrt(1f-t*t);
			},
			[Ease.ExpoIn]=delegate(float t){
				return Mathf.Pow(2f,10f*(t-1f));
			},
			[Ease.BackIn]=delegate(float t){
				return t*t*(2.7f*t-1.7f);
			},
			[Ease.SineOut]=delegate(float t){
				return Mathf.Sin(t*1.57f);
			},
			[Ease.CircOut]=delegate(float t){
				return Mathf.Sqrt((2f-t)*t);
			},
			[Ease.ExpoOut]=delegate(float t){
				return 1f-Mathf.Pow(2f,-10f*t);
			},
			[Ease.BackOut]=delegate(float t){
				return (--t*t*(2.7f*t+1.7f)+1f);
			},
			[Ease.BounceOut]=delegate(float t){
				if(t<0.3636f){
					return 7.5625f*t*t;
				}
				else if(t<0.7272f){
					return 7.5625f*(t-=0.5454f)*t+0.75f;
				}
				else if(t<0.9090f){
					return 7.5625f*(t-=0.8181f)*t+0.9375f;
				}
				else{
					return 7.5625f*(t-=0.9545f)*t+0.984375f;
				}
			},
			[Ease.ElasticOut]=delegate(float t){
				return Mathf.Sin(-20.42f*(t+1))*Mathf.Pow(2f,-10f*t)+1f;
			}
		};
		
		private static Dictionary<Field,Action<Tween,float>> fieldMap=new Dictionary<Field,Action<Tween,float>>(){
			[Field.Alpha]=delegate(Tween tween,float value){
				var spriteRenderer=tween.GetComponent<SpriteRenderer>();
				if(spriteRenderer!=null){
					var color=spriteRenderer.color;
					color.a=value;
					spriteRenderer.color=color;
				}
				else{
					var tmpText=tween.GetComponent<TMPro.TMP_Text>();
					if(tmpText!=null){
						tmpText.alpha=value;
					}
					else{
						var canvasGroup=tween.GetComponent<CanvasGroup>();
						if(canvasGroup==null){
							canvasGroup=tween.gameObject.AddComponent<CanvasGroup>();
						}
						canvasGroup.alpha=value;
					}
				}
			},
			[Field.FillAmount]=delegate(Tween tween,float value){
				var image=tween.GetComponent<Image>();
				if(image!=null){
					image.fillAmount=value;
				}
			},
			[Field.SizeDeltaX]=delegate(Tween tween,float value){
				var rectTransform=tween.GetComponent<RectTransform>();
				if(rectTransform!=null){
					var sizeDelta=rectTransform.sizeDelta;
					sizeDelta.x=value;
					rectTransform.sizeDelta=sizeDelta;
				}
			},
			[Field.SizeDeltaY]=delegate(Tween tween,float value){
				var rectTransform=tween.GetComponent<RectTransform>();
				if(rectTransform!=null){
					var sizeDelta=rectTransform.sizeDelta;
					sizeDelta.y=value;
					rectTransform.sizeDelta=sizeDelta;
				}
			},
			[Field.AnchoredPositionX]=delegate(Tween tween,float value){
				var rectTransform=tween.GetComponent<RectTransform>();
				if(rectTransform!=null){
					var anchoredPosition=rectTransform.anchoredPosition;
					anchoredPosition.x=value;
					rectTransform.anchoredPosition=anchoredPosition;
				}
			},
			[Field.AnchoredPositionY]=delegate(Tween tween,float value){
				var rectTransform=tween.GetComponent<RectTransform>();
				if(rectTransform!=null){
					var anchoredPosition=rectTransform.anchoredPosition;
					anchoredPosition.y=value;
					rectTransform.anchoredPosition=anchoredPosition;
				}
			},
			[Field.AnchoredPositionZ]=delegate(Tween tween,float value){
				var rectTransform=tween.GetComponent<RectTransform>();
				if(rectTransform!=null){
					var anchoredPosition3D=rectTransform.anchoredPosition3D;
					anchoredPosition3D.z=value;
					rectTransform.anchoredPosition3D=anchoredPosition3D;
				}
			},
			[Field.PositionX]=delegate(Tween tween,float value){
				var localPosition=tween.transform.localPosition;
				localPosition.x=value;
				tween.transform.localPosition=localPosition;
			},
			[Field.PositionY]=delegate(Tween tween,float value){
				var localPosition=tween.transform.localPosition;
				localPosition.y=value;
				tween.transform.localPosition=localPosition;
			},
			[Field.PositionZ]=delegate(Tween tween,float value){
				var localPosition=tween.transform.localPosition;
				localPosition.z=value;
				tween.transform.localPosition=localPosition;
			},
			[Field.RotationX]=delegate(Tween tween,float value){
				var eulerAngles=tween.transform.localEulerAngles;
				eulerAngles.x=value;
				tween.transform.localEulerAngles=eulerAngles;
			},
			[Field.RotationY]=delegate(Tween tween,float value){
				var eulerAngles=tween.transform.localEulerAngles;
				eulerAngles.y=value;
				tween.transform.localEulerAngles=eulerAngles;
			},
			[Field.RotationZ]=delegate(Tween tween,float value){
				var eulerAngles=tween.transform.localEulerAngles;
				eulerAngles.z=value;
				tween.transform.localEulerAngles=eulerAngles;
			},
			[Field.ScaleX]=delegate(Tween tween,float value){
				var localScale=tween.transform.localScale;
				localScale.x=value;
				tween.transform.localScale=localScale;
			},
			[Field.ScaleY]=delegate(Tween tween,float value){
				var localScale=tween.transform.localScale;
				localScale.y=value;
				tween.transform.localScale=localScale;
			},
			[Field.ScaleZ]=delegate(Tween tween,float value){
				var localScale=tween.transform.localScale;
				localScale.z=value;
				tween.transform.localScale=localScale;
			},
			[Field.Scale]=delegate(Tween tween,float value){
				var localScale=tween.transform.localScale;
				localScale.x=value;
				localScale.y=value;
				localScale.z=value;
				tween.transform.localScale=localScale;
			}
		};
		
		public static float GetSpeed(Component target){
			var tween=target.GetComponent<Tween>();
			return tween==null?1:tween.speed;
		}
		
		public static void SetSpeed(Component target,float speed){
			var tween=target.GetComponent<Tween>();
			if(tween==null){
				tween=target.gameObject.AddComponent<Tween>();
			}
			tween.speed=speed;
		}
		
		public static void Start(float duration,Component target,Field field,float from,float to,Ease ease=Ease.Linear,Action onFinish=null){
			if(target==null){
				return;
			}
			var tween=target.GetComponent<Tween>();
			if(tween==null){
				tween=target.gameObject.AddComponent<Tween>();
			}
			var taskIndex=(int)field;
			var task=tween.tasks[taskIndex];
			if(task==null){
				task=new Task();
				task.field=field;
				tween.tasks[taskIndex]=task;
			}
			if(duration<=0){
				task.enabled=false;
				task.onFinish=null;
				if(field!=Field.Delay){
					fieldMap[field](tween,to);
				}
				if(onFinish!=null){
					onFinish();
				}
			}
			else{
				task.time=0f;
				task.duration=duration;
				task.from=from;
				task.to=to;
				task.ease=ease;
				task.enabled=true;
				task.onFinish=onFinish;
				if(field!=Field.Delay){
					fieldMap[field](tween,from);
				}
				if(!tween.enabled){
					tween.enabled=true;
				}
			}
		}
		
		public static void Shake(float duration,Component target,Field field,float amplitude,float frequency,Action onFinish=null){
			Start(duration,target,field,amplitude,frequency,(Ease)(-1),onFinish);
		}
		
		public static void Delay(float duration,Component target,Action onFinish=null){
			Start(duration,target,Field.Delay,0,1,Ease.Linear,onFinish);
		}
		
		public static void StopTo(Component target,Field field,float to){
			Start(0f,target,field,to,to);
		}
		
		public static void Clear(Component target){
			if(target==null){
				return;
			}
			var tween=target.GetComponent<Tween>();
			if(tween==null){
				return;
			}
			foreach(var task in tween.tasks){
				if(task!=null&&task.enabled){
					task.enabled=false;
					task.onFinish=null;
				}
			}
		}
		
		public static float timerScale=1f;
		private const int taskCount=(int)Field.Delay+1;
		private Task[] tasks=new Task[taskCount];
		private float speed=1f;
		
		private void Update(){
			foreach(var task in tasks){
				if(task!=null&&task.enabled){
					task.time+=Time.unscaledDeltaTime*timerScale*speed;
					var t=task.time/task.duration;
					if(t>0.9999999f){
						if(task.field!=Field.Delay){
							if((int)task.ease==-1){
								fieldMap[task.field](this,0);
							}
							else{
								fieldMap[task.field](this,task.to);
							}
						}
						task.enabled=false;
						var onFinish=task.onFinish;
						if(onFinish!=null){
							task.onFinish=null;
							onFinish();
						}
					}
					else if(t<0.0000001f){
						if(task.field!=Field.Delay){
							if((int)task.ease==-1){
								fieldMap[task.field](this,0);
							}
							else{
								fieldMap[task.field](this,task.from);
							}
						}
					}
					else if(task.field!=Field.Delay){
						if((int)task.ease==-1){
							fieldMap[task.field](this,task.from*(1-t)*Mathf.Sin(6.283f*task.to*task.time));
						}
						else{
							if(task.ease!=Ease.Linear){
								t=easeFuncDict[task.ease](t);
							}
							fieldMap[task.field](this,(task.to-task.from)*t+task.from);
						}
					}
				}
			}
			var disabled=true;
			foreach(var task in tasks){
				if(task!=null&&task.enabled){
					if(disabled){
						disabled=false;
					}
				}
			}
			if(disabled){
				enabled=false;
			}
		}
	}
}
