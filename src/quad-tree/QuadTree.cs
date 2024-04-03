// https://github.com/nightcycle/quad-tree/blob/main/src/init.luau

using System;
using System.Collections.Generic;
using MuseDotNet.Framework;
using Option;
namespace QuadTree		
{

    public class Node<V>
    {
        public V Value { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public bool GetIfContainedBy(float minX, float minY, float maxX, float maxY)
        {
            return X >= minX && X < maxX && Y >= minY && Y < maxY;
        }
    }

    public interface IQuadTree {
        public static bool GetIfCircleOverlaps(float x1, float y1, float r1, float x2, float y2, float r2)
        {
            return Math.Pow(r1 + r2, 2) >= Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
        }

        public static bool GetIfCircleContains(float inX, float inY, float inR, float outX, float outY, float outR)
        {
            return Math.Pow(outR - inR, 2) >= Math.Pow(outX - inX, 2) + Math.Pow(outY - inY, 2);
        }

        public static bool GetIfCircleContainsPoint(float pX, float pY, float cX, float cY, float radius)
        {
            return Math.Pow(cX - pX, 2) + Math.Pow(cY - pY, 2) <= Math.Pow(radius, 2);
        }

        public static bool GetIfOverlaps(float aMinX, float aMinY, float aMaxX, float aMaxY, float bMinX, float bMinY, float bMaxX, float bMaxY)
        {
            return aMinX < bMaxX && aMaxX > bMinX && aMinY < bMaxY && aMaxY > bMinY;
        }

        public static bool GetIfContains(float insideMinX, float insideMinY, float insideMaxX, float insideMaxY, float outsideMinX, float outsideMinY, float outsideMaxX, float outsideMaxY)
        {
            return insideMinX >= outsideMinX && insideMaxX <= outsideMaxX && insideMinY >= outsideMinY && insideMaxY <= outsideMaxY;
        }
    }

    public class QuadTree<V>
    {
        private float _MinX;
        private float _MinY;
        private float _MaxX;
        private float _MaxY;
        private float _Width;
        private float _Height;
        private float _Radius;
        private float _CenterX;
        private float _CenterY;
        private int _Capacity;
        private List<Node<V>> _Nodes = new();
        private readonly Option<List<QuadTree<V>>> _Subdivisions = new();

        public QuadTree(float minX, float minY, float maxX, float maxY, int capacity)
        {
            _MinX = minX;
            _MinY = minY;
            _MaxX = maxX;
            _MaxY = maxY;
            _Width = _MaxX - _MinX;
            _Height = _MaxY - _MinY;
            _CenterX = _MinX + _Width / 2;
            _CenterY = _MinY + _Height / 2;
            _Capacity = capacity;
            _Radius = (float)Math.Sqrt(Math.Pow(_Width * 0.5, 2) + Math.Pow(_Height * 0.5, 2));
        }

        private void AddDescendants(List<V> array)
        {
            foreach (var node in _Nodes)
            {
                array.Add(node.Value);
            }

            if (_Subdivisions.GetIfNull() == false)
            {
                List<QuadTree<V>> subs = _Subdivisions.Get();
                foreach (var sub in subs)
                {
                    sub.AddDescendants(array);
                }
            }
        }

        private void Subdivide()
        {
            if (_Subdivisions.GetIfNull() == false)
            {
                throw new Exception("already subdivided");
            }
            _Subdivisions.Set(new List<QuadTree<V>>()
            {
                new QuadTree<V>(_CenterX, _CenterY, _MaxX, _MaxY, _Capacity),
                new QuadTree<V>(_CenterX, _MinY, _MaxX, _CenterY, _Capacity),
                new QuadTree<V>(_MinX, _MinY, _CenterX, _CenterY, _Capacity),
                new QuadTree<V>(_MinX, _CenterY, _CenterX, _MaxY, _Capacity)
            });
        }

        private bool _Insert(Node<V> node)
        {
            if (node.GetIfContainedBy(_MinX, _MinY, _MaxX, _MaxY) == false)
            {
                return false;
            }
            if (_Nodes.Count < _Capacity)
            {
                _Nodes.Add(node);
                return true;
            }
            else
            {
                if (_Subdivisions.GetIfNull() == true)
                {
                    Subdivide();
                }

                List<QuadTree<V>> subs = _Subdivisions.Get();

                // Debug.Log(LogLevel.Display, $"subs={subs.Count}");

                if (node.X < _CenterX)
                {
                    if (node.Y < _CenterY)
                    {
                        return subs[2]._Insert(node);
                    }
                    else
                    {
                        return subs[3]._Insert(node);
                    }
                }
                else
                {
                    if (node.Y < _CenterY)
                    {
                        return subs[1]._Insert(node);
                    }
                    else
                    {
                        return subs[0]._Insert(node);
                    }
                }
            }
        }

        public void Insert(float x, float y, V value)
        {
            _Insert(new Node<V> { X = x, Y = y, Value = value });
        }

        private void _SearchRegion(float minX, float minY, float maxX, float maxY, List<V> found)
        {
            if (IQuadTree.GetIfOverlaps(_MinX, _MinY, _MaxX, _MaxY, minX, minY, maxX, maxY) == false)
            {
                return;
            }
            foreach (var node in _Nodes)
            {
                if (node.GetIfContainedBy(minX, minY, maxX, maxY))
                {
                    found.Add(node.Value);
                }
            }
            if (_Subdivisions.GetIfNull() == false)
            {
                List<QuadTree<V>> subs = _Subdivisions.Get();
                foreach (var child in subs)
                {
                    child._SearchRegion(minX, minY, maxX, maxY, found);
                }
            }
        }

        public List<V> SearchRegion(float minX, float minY, float maxX, float maxY)
        {
            var found = new List<V>();
            _SearchRegion(minX, minY, maxX, maxY, found);
            return found;
        }

        private void _SearchRadius(float x, float y, float radius, List<V> found)
        {
            if (IQuadTree.GetIfCircleContains(_CenterX, _CenterY, _Radius, x, y, radius))
            {
                AddDescendants(found);
                return;
            }
            if (IQuadTree.GetIfCircleOverlaps(_CenterX, _CenterY, _Radius, x, y, radius) == false)
            {
                return;
            }
            foreach (var node in _Nodes)
            {
                if (IQuadTree.GetIfCircleContainsPoint(node.X, node.Y, x, y, radius))
                {
                    found.Add(node.Value);
                }
            }
            if (_Subdivisions.GetIfNull() == false)
            {
                List<QuadTree<V>> subs = _Subdivisions.Get();
                foreach (var child in subs)
                {
                    child._SearchRadius(x, y, radius, found);
                }
            }
        }

        public List<V> SearchRadius(float x, float y, float radius)
        {
            var found = new List<V>();
            _SearchRadius(x, y, radius, found);
            return found;
        }

        private void _FilterSearchRadius(float x, float y, float radius, float fX, float fY, float fRadius, List<V> found)
        {
            if (IQuadTree.GetIfCircleOverlaps(_CenterX, _CenterY, _Radius, x, y, radius) == false || IQuadTree.GetIfCircleContains(_CenterX, _CenterY, _Radius, fX, fY, fRadius))
            {
                return;
            }
            foreach (var node in _Nodes)
            {
                if (IQuadTree.GetIfCircleContainsPoint(node.X, node.Y, x, y, radius) && IQuadTree.GetIfCircleContainsPoint(node.X, node.Y, fX, fY, fRadius) == false)
                {
                    found.Add(node.Value);
                }
            }
            if (_Subdivisions.GetIfNull() == false)
            {
                List<QuadTree<V>> subs = _Subdivisions.Get();
                foreach (var child in subs)
                {
                    child._FilterSearchRadius(x, y, radius, fX, fY, fRadius, found);
                }
            }
        }

        public List<V> FilterSearchRadius(float x, float y, float radius, float fX, float fY, float fRadius)
        {
            var found = new List<V>();
            _FilterSearchRadius(x, y, radius, fX, fY, fRadius, found);
            return found;
        }

        private void _FilterSearchRegion(float minX, float minY, float maxX, float maxY, float fMinX, float fMinY, float fMaxX, float fMaxY, List<V> found)
        {
            if (IQuadTree.GetIfOverlaps(_MinX, _MinY, _MaxX, _MaxY, minX, minY, maxX, maxY) == false || IQuadTree.GetIfContains(_MinX, _MinY, _MaxX, _MaxY, fMinX, fMinY, fMaxX, fMaxY))
            {
                return;
            }
            foreach (var node in _Nodes)
            {
                if (node.GetIfContainedBy(minX, minY, maxX, maxY) && node.GetIfContainedBy(fMinX, fMinY, fMaxX, fMaxY) == false)
                {
                    found.Add(node.Value);
                }
            }
            if (_Subdivisions.GetIfNull() == false)
            {
                List<QuadTree<V>> subs = _Subdivisions.Get();
                foreach (var child in subs)
                {
                    child._FilterSearchRegion(minX, minY, maxX, maxY, fMinX, fMinY, fMaxX, fMaxY, found);
                }
            }
        }

        public List<V> FilterSearchRegion(float minX, float minY, float maxX, float maxY, float fMinX, float fMinY, float fMaxX, float fMaxY)
        {
            var found = new List<V>();
            _FilterSearchRegion(minX, minY, maxX, maxY, fMinX, fMinY, fMaxX, fMaxY, found);
            return found;
        }


    }



}