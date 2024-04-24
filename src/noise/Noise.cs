using System;
using System.Collections.Generic;
using System.Numerics;

namespace NoiseProvider{

	public class Noise
	{
		public int Seed { get; set; }
		private Dictionary<Vector2, float> VoronoiCache2D { get; set; }
		private Dictionary<Vector3, float> VoronoiCache3D { get; set; }
		private Dictionary<Vector2, Dictionary<float, Vector2>> PointCache2D { get; set; }
		private Dictionary<Vector3, Dictionary<float, Vector3>> PointCache3D { get; set; }
		private Dictionary<float, float> PerlinHash3D { get; set; }
		private Dictionary<float, float> PerlinHash2D { get; set; }

		public Noise(int seed = 0)
		{
			Seed = seed;
			VoronoiCache2D = new Dictionary<Vector2, float>();
			VoronoiCache3D = new Dictionary<Vector3, float>();
			PointCache2D = new Dictionary<Vector2, Dictionary<float, Vector2>>();
			PointCache3D = new Dictionary<Vector3, Dictionary<float, Vector3>>();
			PerlinHash3D = new Dictionary<float, float>();
			PerlinHash2D = new Dictionary<float, float>();

			Random rng = new Random(Seed);

			for (int i = 1; i <= 257; i++)
			{
				PerlinHash3D[i] = rng.Next(1, 256);
			}

			for (int i = 1; i <= 257; i++)
			{
				PerlinHash2D[i] = rng.Next(1, 256);
			}
		}

		public float Random(float x, float y, float z = 0)
		{
			if (z != 0)
			{
				float xSeed = 0.01f + (float)(new Random((int)(x * Seed * 1)).NextDouble());
				float ySeed = 0.01f + (float)(new Random((int)(y * Seed * 2)).NextDouble());
				float zSeed = 0.01f + (float)(new Random((int)(z * Seed * 3)).NextDouble());
				float seed = 100000 * ((xSeed + ySeed + zSeed) % 1);
				return (float)new Random((int)seed).NextDouble();
			}
			else
			{
				float xSeed = 0.01f + (float)(new Random((int)(x * Seed * 1)).NextDouble());
				float ySeed = 0.01f + (float)(new Random((int)(y * Seed * 2)).NextDouble());
				float seed = 100000 * ((xSeed + ySeed) % 1);
				return (float)new Random((int)seed).NextDouble();
			}
		}

		public float Perlin(float x, float y, float z = 0)
		{
			z = z != 0 ? z : 0;

			int xflr = (int)Math.Floor(x);
			int yflr = (int)Math.Floor(y);
			int zflr = (int)Math.Floor(z);

			int xi = xflr % 256;
			int yi = yflr % 256;
			int zi = zflr % 256;

			float xf = x - xflr;
			float yf = y - yflr;
			float zf = z - zflr;

			float u = PerlinFade(xf);
			float v = PerlinFade(yf);
			float w = PerlinFade(zf);

			Dictionary<float, float> p = PerlinHash3D;

			int a = (int)(p[xi + 1] + yi) % 256;
			int aa = (int)(p[a + 1] + zi) % 256;
			int ab = (int)(p[a + 2] + zi) % 256;

			int b = (int)(p[xi + 2] + yi) % 256;
			int ba = (int)(p[b + 1] + zi) % 256;
			int bb = (int)(p[b + 2] + zi) % 256;

			float la = PerlinLerp(u, PerlinGrad(p[aa + 1], xf, yf, zf), PerlinGrad(p[ba + 1], xf - 1, yf, zf));
			float lb = PerlinLerp(u, PerlinGrad(p[ab + 1], xf, yf - 1, zf), PerlinGrad(p[bb + 1], xf - 1, yf - 1, zf));
			float la1 = PerlinLerp(u, PerlinGrad(p[aa + 2], xf, yf, zf - 1), PerlinGrad(p[ba + 2], xf - 1, yf, zf - 1));
			float lb1 = PerlinLerp(u, PerlinGrad(p[ab + 2], xf, yf - 1, zf - 1), PerlinGrad(p[bb + 2], xf - 1, yf - 1, zf - 1));

			return 0.5f + 0.5f * PerlinLerp(w, PerlinLerp(v, la, lb), PerlinLerp(v, la1, lb1));
		}

		public float Cellular(float x, float y, float z = 0)
		{
			if (z != 0)
			{
				Vector3 vec = new Vector3(x, y, z);
				List<Vector3> points = new List<Vector3>();
				Vector3 origin = new Vector3((float)Math.Floor(x), (float)Math.Floor(y), (float)Math.Floor(z));

				if (PointCache3D.ContainsKey(origin))
				{
					points = new List<Vector3>(PointCache3D[origin].Values);
				}
				else
				{
					for (int pX = -1; pX <= 1; pX++)
					{
						for (int pY = -1; pY <= 1; pY++)
						{
							for (int pZ = -1; pZ <= 1; pZ++)
							{
								points.Add(GetPoint3D(Seed, origin.X + pX, origin.Y + pY, origin.Z + pZ));
							}
						}
					}
					PointCache3D[origin] = new Dictionary<float, Vector3>();
					for (int i = 0; i < points.Count; i++)
					{
						PointCache3D[origin][i] = points[i];
					}
				}

				Vector3 closest = Vector3.Zero;
				float cDist = float.MaxValue;
				Vector3 farthest = Vector3.Zero;
				float fDist = 0;

				foreach (Vector3 point in points)
				{
					float dist = Vector3.Distance(point, vec);
					if (closest == Vector3.Zero || dist < cDist)
					{
						closest = point;
						cDist = dist;
					}
					if (farthest == Vector3.Zero || dist > fDist)
					{
						farthest = point;
						fDist = dist;
					}
				}

				return cDist;
			}
			else
			{
				Vector2 vec = new(x, y);
				List<Vector2> points = new List<Vector2>();
				
				Vector2 origin = new(((float)Math.Floor(x)), (float)Math.Floor(y));

				if (PointCache2D.ContainsKey(origin))
				{
					points = new List<Vector2>(PointCache2D[origin].Values);
				}
				else
				{
					for (int pX = -1; pX <= 1; pX++)
					{
						for (int pY = -1; pY <= 1; pY++)
						{
							points.Add(GetPoint2D(Seed, origin.X + pX, origin.Y + pY));
						}
					}
					PointCache2D[origin] = new Dictionary<float, Vector2>();
					for (int i = 0; i < points.Count; i++)
					{
						PointCache2D[origin][i] = points[i];
					}
				}

				Vector2 closest = Vector2.Zero;
				float cDist = float.MaxValue;

				foreach (Vector2 point in points)
				{
					float dist = Vector2.Distance(point, vec);
					if (closest == Vector2.Zero || dist < cDist)
					{
						closest = point;
						cDist = dist;
					}
				}

				return (cDist - 3 / 8) / (7 / 10);
			}
		}

		public float Voronoi(float x, float y, float z = 0)
		{
			if (z != 0)
			{
				Vector3 vec = new Vector3(x, y, z);
				List<Vector3> points = new List<Vector3>();
				Vector3 origin = new Vector3((float)Math.Floor(x), (float)Math.Floor(y), (float)Math.Floor(z));

				if (PointCache3D.ContainsKey(origin))
				{
					points = new List<Vector3>(PointCache3D[origin].Values);
				}
				else
				{
					for (int pX = -1; pX <= 1; pX++)
					{
						for (int pY = -1; pY <= 1; pY++)
						{
							for (int pZ = -1; pZ <= 1; pZ++)
							{
								Vector3 offset = new Vector3(pX, pY, pZ);
								points.Add(GetPoint3D(Seed, origin.X + offset.X, origin.Y + offset.Y, origin.Z + offset.Z));
							}
						}
					}
					PointCache3D[origin] = new Dictionary<float, Vector3>();
					for (int i = 0; i < points.Count; i++)
					{
						PointCache3D[origin][i] = points[i];
					}
				}

				Vector3 closest = Vector3.Zero;
				float cDist = float.MaxValue;

				foreach (Vector3 point in points)
				{
					float dist = Vector3.Distance(point, vec);
					if (closest == Vector3.Zero || dist < cDist)
					{
						closest = point;
						cDist = dist;
					}
				}

				if (!VoronoiCache3D.ContainsKey(closest))
				{
					VoronoiCache3D[closest] = Random(closest.X, closest.Y, closest.Z);
				}
				return VoronoiCache3D[closest];
			}
			else
			{
				Vector2 vec = new Vector2(x, y);
				List<Vector2> points = new List<Vector2>();
				Vector2 origin = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

				if (PointCache2D.ContainsKey(origin))
				{
					points = new List<Vector2>(PointCache2D[origin].Values);
				}
				else
				{
					for (int pX = -1; pX <= 1; pX++)
					{
						for (int pY = -1; pY <= 1; pY++)
						{
							Vector2 offset = new Vector2(pX, pY);
							points.Add(GetPoint2D(Seed, origin.X + offset.X, origin.Y + offset.Y));
						}
					}
					PointCache2D[origin] = new Dictionary<float, Vector2>();
					for (int i = 0; i < points.Count; i++)
					{
						PointCache2D[origin][i] = points[i];
					}
				}

				Vector2 closest = Vector2.Zero;
				float cDist = float.MaxValue;

				foreach (Vector2 point in points)
				{
					float dist = Vector2.Distance(point, vec);
					if (closest == Vector2.Zero || dist < cDist)
					{
						closest = point;
						cDist = dist;
					}
				}

				if (!VoronoiCache2D.ContainsKey(closest))
				{
					VoronoiCache2D[closest] = Random(closest.X, closest.Y);
				}
				return VoronoiCache2D[closest];
			}
		}

		private Vector2 NextUnitVector2D(Random rng){
			float x = (float)(rng.NextDouble());
			float y = (float)(rng.NextDouble());
			Vector2 vec = new Vector2(x,y);
			return vec / vec.Length();
		}

		private Vector3 NextUnitVector3D(Random rng){
			float x = (float)(rng.NextDouble());
			float y = (float)(rng.NextDouble());
			float z = (float)(rng.NextDouble());
			Vector3 vec = new Vector3(x,y,z);
			return vec / vec.Length();
		}

		private Vector3 GetPoint3D(float seed, float roundX, float roundY, float roundZ)
		{
			float xSeed = 0.01f + (float)(new Random((int)(roundX * seed + 1)).NextDouble());
			float ySeed = 0.01f + (float)(new Random((int)(roundY * seed + 2)).NextDouble());
			float zSeed = 0.01f + (float)(new Random((int)(roundZ * seed + 3)).NextDouble());
			float omniSeed = 10000000 * (xSeed + ySeed + zSeed);

			return new Vector3(roundX, roundY, roundZ) + Vector3.One * 0.5f + 0.5f * NextUnitVector3D(new Random((int)omniSeed)); //.NextVector3();
		}

		private Vector2 GetPoint2D(float seed, float roundX, float roundY)
		{
			float xSeed = 0.01f + (float)(new Random((int)(roundX * seed + 1)).NextDouble());
			float ySeed = 0.01f + (float)(new Random((int)(roundY * seed + 2)).NextDouble());
			float omniSeed = 10000000 * (xSeed + ySeed);

			Vector2 vec = NextUnitVector2D(new Random((int)omniSeed)); //.NextVector2();

			return new Vector2(roundX, roundY) + Vector2.One * 0.5f + 0.5f * new Vector2(vec.X, vec.Y);
		}

		private float PerlinFade(float t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private float PerlinLerp(float t, float a, float b)
		{
			return a + t * (b - a);
		}

		private float PerlinGrad(float hash, float x, float y, float z)
		{
			float[][] kPerlinGrad = new float[][] {
				new float[] { 1, 1, 0 },
				new float[] { -1, 1, 0 },
				new float[] { 1, -1, 0 },
				new float[] { -1, -1, 0 },
				new float[] { 1, 0, 1 },
				new float[] { -1, 0, 1 },
				new float[] { 1, 0, -1 },
				new float[] { -1, 0, -1 },
				new float[] { 0, 1, 1 },
				new float[] { 0, -1, 1 },
				new float[] { 0, 1, -1 },
				new float[] { 0, -1, -1 },
				new float[] { 1, 1, 0 },
				new float[] { 0, -1, 1 },
				new float[] { -1, 1, 0 },
				new float[] { 0, -1, -1 }
			};

			int index = (int)(hash % 16);
			float[] g = kPerlinGrad[index];

			return g[0] * x + g[1] * y + g[2] * z;
		}
	}
}
