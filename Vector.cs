using System;

public struct Vector:IEquatable<Vector>{
	
	public readonly static number rotation45=new number(45_0000);
	public readonly static number rotation90=new number(90_0000);
	public readonly static number rotation135=new number(135_0000);
	public readonly static number rotation180=new number(180_0000);
	public readonly static number rotation360=new number(360_0000);
	
	private static number ATAN_A=new number(55_7141);
	private static number ATAN_B=new number(-10_9978);
	private static number[] SIN_LUT={
		new number(175),new number(349),new number(523),new number(698),new number(872),
		new number(1045),new number(1219),new number(1392),new number(1564),new number(1736),
		new number(1908),new number(2079),new number(2250),new number(2419),new number(2588),
		new number(2756),new number(2924),new number(3090),new number(3256),new number(3420),
		new number(3584),new number(3746),new number(3907),new number(4067),new number(4226),
		new number(4384),new number(4540),new number(4695),new number(4848),new number(5000),
		new number(5150),new number(5299),new number(5446),new number(5592),new number(5736),
		new number(5878),new number(6018),new number(6157),new number(6293),new number(6428),
		new number(6561),new number(6691),new number(6820),new number(6947),new number(7071),
		new number(7193),new number(7314),new number(7431),new number(7547),new number(7660),
		new number(7771),new number(7880),new number(7986),new number(8090),new number(8192),
		new number(8290),new number(8387),new number(8480),new number(8572),new number(8660),
		new number(8746),new number(8829),new number(8910),new number(8988),new number(9063),
		new number(9135),new number(9205),new number(9272),new number(9336),new number(9397),
		new number(9455),new number(9511),new number(9563),new number(9613),new number(9659),
		new number(9703),new number(9744),new number(9781),new number(9816),new number(9848),
		new number(9877),new number(9903),new number(9925),new number(9945),new number(9962),
		new number(9976),new number(9986),new number(9994),new number(9998),new number(1_0000)
	};
	
	public static number NormalX(number rotation){
		rotation=rotation%rotation360;
		if(rotation<number.zero){
			rotation+=rotation360;
		}
		var negative=false;
		if(rotation>rotation180){
			rotation-=rotation180;
			negative=true;
		}
		if(rotation>rotation90){
			rotation=rotation180-rotation;
		}
		var index=(int)(rotation);
		var fraction=rotation-(number)index;
		rotation=index>0?SIN_LUT[index-1]:number.zero;
		if(fraction!=number.zero){
			rotation+=(SIN_LUT[index]-rotation)*fraction;
		}
		return negative?-rotation:rotation;
	}
	
	public static number NormalZ(number rotation){
		return NormalX(rotation+rotation90);
	}
	
	public static Vector Normal(number rotation){
		return new Vector(NormalX(rotation),NormalZ(rotation));
	}
	
	public static number Distance(Vector towards){
		return number.Sqrt(SquareDistance(towards));
	}
	
	public static number SquareDistance(Vector towards){
		return number.Square(towards.x)+number.Square(towards.z);
	}
	
	public static Vector Move(Vector position,number rotation,number distance){
		return position+Normal(rotation)*distance;
	}
	
	public static number Rotate(number fromRotation,number toRotation,number rotateRotation){
		if(rotateRotation==number.zero){
			return fromRotation;
		}
		var deltaRotation=DeltaRotation(fromRotation,toRotation);
		if(deltaRotation==number.zero){
			return fromRotation;
		}
		if(deltaRotation>number.zero){
			return number.Clamp(fromRotation+rotateRotation,fromRotation,fromRotation+deltaRotation);
		}
		else{
			return number.Clamp(fromRotation+rotateRotation,fromRotation+deltaRotation,fromRotation);
		}
	}
	
	public static number DeltaRotation(number fromRotation,number toRotation){
		var deltaRotation=(toRotation-fromRotation)%rotation360;
		if(deltaRotation<-rotation180){
			deltaRotation+=rotation360;
		}
		else if(deltaRotation>rotation180){
			deltaRotation-=rotation360;
		}
		return deltaRotation;
	}
	
	public static number Rotation(Vector towards){
		var towardsX=towards.x;
		var towardsZ=towards.z;
		var xLessThanZero=towardsX<number.zero;
		var zLessThanZero=towardsZ<number.zero;
		if(towardsX!=number.zero){
			var x=xLessThanZero?-towardsX:towardsX;
			var z=zLessThanZero?-towardsZ:towardsZ;
			if(x==z){
				z=zLessThanZero?rotation135:rotation45;
				return xLessThanZero?-z:z;
			}
			else if(x>z){
				x=xLessThanZero?-rotation90:rotation90;
				if(z==number.zero){
					return x;
				}
				var slope=towardsZ/towardsX;
				return x-(ATAN_A+ATAN_B*slope*slope)*slope;
			}
			else{
				z=zLessThanZero?rotation180:number.zero;
				var slope=towardsX/towardsZ;
				x=(ATAN_A+ATAN_B*slope*slope)*slope;
				return xLessThanZero?(x-z):(x+z);
			}
		}
		else if(zLessThanZero){
			return rotation180;
		}
		return number.zero;
	}
	
	public static number Cross(Vector a,Vector b){
		return a.x*b.z-b.x*a.z;
	}
	
	public static number Dot(Vector a,Vector b){
		return a.x*b.x+a.z*b.z;
	}
	
	public static Vector operator+(Vector a,Vector b){
		return new Vector(a.x+b.x,a.z+b.z);
	}
	
	public static Vector operator-(Vector a,Vector b){
		return new Vector(a.x-b.x,a.z-b.z);
	}
	
	public static Vector operator*(Vector a,Vector b){
		return new Vector(a.x*b.x,a.z*b.z);
	}
	
	public static Vector operator/(Vector a,Vector b){
		return new Vector(a.x/b.x,a.z/b.z);
	}
	
	public static Vector operator*(Vector a,int d){
		return new Vector(a.x*d,a.z*d);
	}
	
	public static Vector operator*(Vector a,number d){
		return new Vector(a.x*d,a.z*d);
	}
	
	public static Vector operator*(int d,Vector a){
		return new Vector(a.x*d,a.z*d);
	}
	
	public static Vector operator*(number d,Vector a){
		return new Vector(a.x*d,a.z*d);
	}
	
	public static Vector operator/(Vector a,int d){
		return new Vector(a.x/d,a.z/d);
	}
	
	public static Vector operator/(Vector a,number d){
		return new Vector(a.x/d,a.z/d);
	}
	
	public static Vector operator-(Vector value){
		return new Vector(-value.x,-value.z);
	}
	
	public static bool operator==(Vector a,Vector b){
		return a.x==b.x&&a.z==b.z;
	}
	
	public static bool operator!=(Vector a,Vector b){
		return a.x!=b.x||a.z!=b.z;
	}
	
	public override bool Equals(object obj){
		if(obj is Vector){
			var value=(Vector)obj;
			return value.x==x&&value.z==z;
		}
		return false;
	}
	
	public override int GetHashCode(){ 
		return x.GetHashCode()^z.GetHashCode()<<2;
	}
	
	public bool Equals(Vector other){
		return other.x==x&&other.z==z;
	}
	
	public override string ToString(){
		return $"({x},{z})";
	}
	
	public number x;
	public number z;
	
	public Vector(number x,number z){
		this.x=x;
		this.z=z;
	}
}