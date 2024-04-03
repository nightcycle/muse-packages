// written by CJ_Oyer (@nightcycle)
// a surprisingly precise coordinate solution using only the distances from an origin actor and 3 axis offset actors.
// it was designed with Y as up, though I suspect internally things use Z
using System;
using MuseDotNet.Framework;
using System.Numerics;

namespace CoordinateService
{
	public class CoordinateService
	{

		private static readonly CoordinateService instance = new CoordinateService();
		private readonly float XDist;
		private readonly float YDist;
		private readonly float ZDist;

		private Vector3 Origin;
		private Vector3 XAxis;
		private Vector3 YAxis;
		private Vector3 ZAxis;

		private Actor XRef;
		private Actor YRef;
		private Actor ZRef;
		private Actor OriginRef;

		// set up as singleton
		public static CoordinateService Instance
		{
			get
			{
				return instance;
			}
		}

		// https://en.wikipedia.org/wiki/True-range_multilateration#Three_Cartesian_dimensions,_three_measured_slant_ranges
		public Vector3 GetPosition(Actor actor){
			float rO = actor.GetDistance(this.OriginRef);
			float rX = actor.GetDistance(this.XRef);
			float rY = actor.GetDistance(this.YRef);
			float rZ = actor.GetDistance(this.ZRef);

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

		private CoordinateService(){
			
			Muse.ForEachActor((Actor actor) => {
				if (actor.HasTag(Tags.XAxis)){
					this.XRef = actor;
				}else if (actor.HasTag(Tags.YAxis)){
					this.YRef = actor;
				}else if (actor.HasTag(Tags.ZAxis)){
					this.ZRef = actor;
				}else if (actor.HasTag(Tags.Origin)){
					this.OriginRef = actor;
				}
			});

			XDist = this.OriginRef.GetDistance(this.XRef);
			YDist = this.OriginRef.GetDistance(this.YRef);
			ZDist = this.OriginRef.GetDistance(this.ZRef);

			Origin = new Vector3(0, 0, 0);
			XAxis = new Vector3(XDist, 0, 0);
			YAxis = new Vector3(0, YDist, 0);
			ZAxis = new Vector3(0, 0, ZDist);
		}
	}
}
