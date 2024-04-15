// written by CJ_Oyer (@nightcycle)
// a surprisingly precise coordinate solution using only the distances from an origin actor and 3 axis offset actors.
// it was designed with Y as up, though I suspect internally things use Z
using System;
using MuseDotNet.Framework;
using System.Numerics;
using OptionProvider;
namespace CoordinateService
{
	public class CoordinateService
	{

		private float XDist = 0;
		private float YDist = 0;
		private float ZDist = 0;

		private Vector3 Origin = new Vector3(0, 0, 0);
		private Vector3 XAxis = new Vector3(0, 0, 0);
		private Vector3 YAxis = new Vector3(0, 0, 0);
		private Vector3 ZAxis = new Vector3(0, 0, 0);

		private Option<Actor> XRef = new();
		private Option<Actor> YRef = new();
		private Option<Actor> ZRef = new();
		private Option<Actor> OriginRef = new();

		private bool HasStarted = false;

		// https://en.wikipedia.org/wiki/True-range_multilateration#Three_Cartesian_dimensions,_three_measured_slant_ranges
		public Vector3 GetPosition(Actor actor){
			if (!HasStarted){
				throw new Exception("'CoordinateService.Instance.Start()' hasn't been called");
			}

			Actor originRef = this.OriginRef.Get();
			Actor xRef = this.XRef.Get();
			Actor yRef = this.YRef.Get();
			Actor zRef = this.ZRef.Get();

			float rO = actor.GetDistance(originRef);
			float rX = actor.GetDistance(xRef);
			float rY = actor.GetDistance(yRef);
			float rZ = actor.GetDistance(zRef);

			// Debug.Log(LogLevel.Display, $"rO={rO}, r1={rX}, r2={rY}, rZ={rZ}");

			Vector3 pO = this.Origin;
			Vector3 pX = this.XAxis;
			Vector3 pY = this.YAxis;
			Vector3 pZ = this.ZAxis;

			// Debug.Log(LogLevel.Display, $"pO={pO}, pX={pX}, pY={pY}, pZ={pZ}");

			float u = pX.X;
			float vX = pY.X;
			float vY = pY.Y;
			float r1 = rO;
			float r2 = rX;
			float r3 = rY;
			
			float x = ((float)Math.Pow(r1, 2)-(float)Math.Pow(r2, 2)+(float)Math.Pow(u, 2))/(2*u);
			float y = ((float)Math.Pow(r1,2)-(float)Math.Pow(r3,2)+(float)Math.Pow(vX,2)+(float)Math.Pow(vY,2)-(2*vX*x))/(2*vY);
			float z = (float)Math.Sqrt((float)Math.Pow(r1, 2)-(float)Math.Pow(x,2)-(float)Math.Pow(y,2));

			Vector3 pointXY1 = new(x,y,z);
			Vector3 pointXY2 = new(x,y,-z);

			float aError = Math.Abs(rZ-(float)(pointXY1-pZ).Length());
			float bError = Math.Abs(rZ-(float)(pointXY2-pZ).Length());

			// Debug.Log(LogLevel.Display,  $"error={Math.Min(aError, bError)}");
			
			if (aError < bError){
				return pO + pointXY1;
			}else{
				return pO + pointXY2;
			}
		}

		public void Start(
			Actor origin,
			Actor xRef,
			Actor yRef,
			Actor zRef
		){
			if (!HasStarted){
				OriginRef.Set(origin);
				XRef.Set(xRef);
				YRef.Set(yRef);
				ZRef.Set(zRef);

				XDist = origin.GetDistance(xRef);
				YDist = origin.GetDistance(yRef);
				ZDist = origin.GetDistance(zRef);

				Origin = new Vector3(0, 0, 0);
				XAxis = new Vector3(XDist, 0, 0);
				YAxis = new Vector3(0, YDist, 0);
				ZAxis = new Vector3(0, 0, ZDist);

				HasStarted = true;
			}else{
				throw new Exception("'CoordinateService.Instance.Start()' has already been called");
			}

		}

		private static CoordinateService instance;
		public static CoordinateService Instance
		{
			get
			{
				instance ??= new CoordinateService();
				return instance;
			}
		}
		private CoordinateService(){}
	}
}
