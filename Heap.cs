using System;
public class Heap<T>{
	private class Node{
		public number value;
		public T element;
		public int id;
		public bool LessThan(Node node){
			return value<node.value||(value==node.value&&id<node.id);
		}
	}
	
	private int id;
	private int size;
	private Node[] nodes;
	
	public Heap(int capacity=0){
		if(capacity>0){
			nodes=new Node[capacity];
		}
	}
	
	public int Count{
		get{return size;}
	}
	
	public void Push(number value,T element){
		if(nodes==null){
			nodes=new Node[4];
		}
		else if(nodes.Length==size){
			Array.Resize(ref nodes,size<<1);
		}
		var nodeIndex=size++;
		var node=nodes[nodeIndex];
		if(node==null){
			node=new Node();
			nodes[nodeIndex]=node;
		}
		node.id=++id;
		node.value=value;
		node.element=element;
		while(nodeIndex>0){
			var parentIndex=(nodeIndex-1)>>2;
			var parent=nodes[parentIndex];
			if(node.LessThan(parent)){
				nodes[nodeIndex]=parent;
				nodeIndex=parentIndex;
			}
			else{
				break;
			}
		}
		nodes[nodeIndex]=node;
	}
	
	public T Pop(){
		if(size==0){
			return default;
		}
		size--;
		var node=nodes[size];
		var firstNode=nodes[0];
		var element=firstNode.element;
		firstNode.element=default;
		nodes[size]=firstNode;
		int i,nodeIndex=0;
		while((i=(nodeIndex<<2)+1)<size){
			var minChild=nodes[i];
			var minChildIndex=i;
			var childIndexUpperBound=i+4;
			if(childIndexUpperBound>size){
				childIndexUpperBound=size;
			}
			while(++i<childIndexUpperBound){
				var nextChild=nodes[i];
				if(nextChild.LessThan(minChild)){
					minChild=nextChild;
					minChildIndex=i;
				}
			}
			if(node.LessThan(minChild)){
				break;
			}
			nodes[nodeIndex]=minChild;
			nodeIndex=minChildIndex;
		}
		nodes[nodeIndex]=node;
		return element;
	}
	
	public T Peek(){
		if(size==0){
			return default;
		}
		return nodes[0].element;
	}
	
	public void Clear(Action<T> cleaner=null){
		if(size>0){
			for(var i=0;i<size;i++){
				var node=nodes[i];
				if(cleaner!=null){
					cleaner(node.element);
				}
				node.element=default;
			}
			size=0;
		}
	}
}