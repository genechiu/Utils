using System;
public struct number:IEquatable<number>,IComparable<number>{

	private const long RawSqrtOne=100L;
	private const long RawOne=RawSqrtOne*RawSqrtOne;
	private const long RawHalf=RawOne/2L;
	private const long MaxFraction=RawOne-1L;
	
	public readonly static number zero=new number(0);
	public readonly static number one=new number(RawOne);
	
	public static number Floor(number num){
		var raw=num.raw;
		if(raw>0L){
			num.raw=raw/RawOne*RawOne;
		}
		else if(raw<0L){
			num.raw=(raw-MaxFraction)/RawOne*RawOne;
		}
		return num;
	}
	
	public static int FloorToInt(number num){
		var raw=num.raw;
		if(raw>0L){
			return (int)(raw/RawOne);
		}
		else if(raw<0L){
			return (int)((raw-MaxFraction)/RawOne);
		}
		return 0;
	}
	
	public static number Ceil(number num){
		var raw=num.raw;
		if(raw>0L){
			num.raw=(raw+MaxFraction)/RawOne*RawOne;
		}
		else if(raw<0L){
			num.raw=raw/RawOne*RawOne;
		}
		return num;
	}
	
	public static int CeilToInt(number num){
		var raw=num.raw;
		if(raw>0L){
			return (int)((raw+MaxFraction)/RawOne);
		}
		else if(raw<0L){
			return (int)(raw/RawOne);
		}
		return 0;
	}
	
	public static number Round(number num){
		var raw=num.raw;
		if(raw>0L){
			num.raw=(raw+RawHalf)/RawOne*RawOne;
		}
		else if(raw<0L){
			num.raw=(raw-RawHalf)/RawOne*RawOne;
		}
		return num;
	}
	
	public static int RoundToInt(number num){
		var raw=num.raw;
		if(raw>0L){
			return (int)((raw+RawHalf)/RawOne);
		}
		else if(raw<0L){
			return (int)((raw-RawHalf)/RawOne);
		}
		return 0;
	}
	
	public static number Sqrt(number num){
		var raw=num.raw;
		if(raw>0L){
			var x=raw;
			for(var z=1L;x>z;z=raw/x){
				x=(x+z)/2L;
			}
			num.raw=x*RawSqrtOne;
		}
		return num;
	}
	
	public static number Square(number num){
		var raw=num.raw;
		if(raw!=0L){
			num.raw=raw*raw/RawOne;
		}
		return num;
	}
	
	public static number Abs(number num){
		var raw=num.raw;
		if(raw<0L){
			num.raw=-raw;
		}
		return num;
	}
	
	public static number Min(number a,number b){
		return a.raw<b.raw?a:b;
	}
	
	public static number Max(number a,number b){
		return a.raw>b.raw?a:b;
	}
	
	public static number Clamp(number num,number min,number max){
		var raw=num.raw;
		if(raw<min.raw){
			return min;
		}
		if(raw>max.raw){
			return max;
		}
		return num;
	}
	
	public static number operator-(number num){
		num.raw=-num.raw;
		return num;
	}
	
	public static number operator%(number a, number b){
		return new number(a.raw%b.raw);
	}
	
	public static number operator+(number a,number b){
		return new number(a.raw+b.raw);
	}
	
	public static number operator-(number a,number b){
		return new number(a.raw-b.raw);
	}
	
	public static number operator*(number a,number b){
		return new number(a.raw*b.raw/RawOne);
	}
	
	public static number operator*(number a,int b){
		return new number(a.raw*(long)b);
	}
	
	public static number operator*(int a,number b){
		return new number((long)a*b.raw);
	}
	
	public static number operator*(number a,long b){
		return new number(a.raw*b);
	}
	
	public static number operator*(long a,number b){
		return new number(a*b.raw);
	}
	
	public static number operator/(number a,number b){
		return new number(a.raw*RawOne/b.raw);
	}
	
	public static number operator/(number a,int b){
		return new number(a.raw/(long)b);
	}
	
	public static number operator/(number a,long b){
		return new number(a.raw/b);
	}
	
	public static bool operator==(number a,number b){
		return a.raw==b.raw;
	}
	
	public static bool operator!=(number a,number b){
		return a.raw!=b.raw;
	}
	
	public static bool operator>(number a,number b){
		return a.raw>b.raw;
	}
	
	public static bool operator<(number a,number b){
		return a.raw<b.raw;
	}
	
	public static bool operator>=(number a,number b){
		return a.raw>=b.raw;
	}
	
	public static bool operator<=(number a,number b){
		return a.raw<=b.raw;
	}
	
	public static explicit operator number(int value){
		return new number((long)value*RawOne);
	}
	
	public static explicit operator int(number value){
		return (int)(value.raw/RawOne);
	}
	
	public static explicit operator number(long value){
		return new number(value*RawOne);
	}
	
	public static explicit operator long(number value){
		return value.raw/RawOne;
	}
	
	public static explicit operator number(float value){
		return (number)(double)value;
	}
	
	public static explicit operator float(number value){
		return (float)(double)value;
	}
	
	public static explicit operator number(double value){
		const double Scale=(double)RawOne;
		return new number((long)(value*Scale+(value<0.0?-0.5:0.5)));
	}
	
	public static explicit operator double(number value){
		const double Scale=1.0/RawOne;
		return (double)value.raw*Scale;
	}
	
	public override bool Equals(object obj){
		return (obj is number)&&((number)obj).raw==raw;
	}
	
	public override int GetHashCode(){
		return raw.GetHashCode();
	}
	
	public bool Equals(number other){
		return raw==other.raw;
	}
	
	public int CompareTo(number other){
		return raw.CompareTo(other.raw);
	}
	
	public override string ToString(){
		return ((double)this).ToString();
	}
	
	private long raw;
	
	public number(long raw){
		this.raw=raw;
	}
}