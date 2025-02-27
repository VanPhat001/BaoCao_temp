﻿using BaoCao.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaoCao
{
    public static class Tool
    {
        private static Random rand = new Random();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int Random(int size)
        {
            return rand.Next(size);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Brush ColorRGB(int r, int g, int b, int alpha = 255)
        {
            return new SolidColorBrush(Color.FromArgb((byte)alpha, (byte)r, (byte)g, (byte)b));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodes"></param>
        /// <param name="edges"></param>
        public static void ExportGraphToFile(string path, List<Node> nodes, List<Edge> edges)
        {
            #region danh index cho tung node trong nodes, va cho tung edge trong edges
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Tag = i;
            }
            for (int i = 0; i < edges.Count; i++)
            {
                edges[i].Tag = i;
            }
            #endregion

            #region xay dung chuoi du lieu de xuat ra file
            StringBuilder datalines = new StringBuilder();

            // first line: n dinh|m cung
            datalines.AppendLine($"{nodes.Count}|{edges.Count}");

            // n next lines: x|y|text
            foreach (Node node in nodes)
            {
                var pos = node.GetCenterLocation();
                datalines.AppendLine($"{pos.X}|{pos.Y}|{node.NodeText}");
            }

            // m next lines: uNodeIndex|vNodeIndex
            foreach (var edge in edges)
            {
                datalines.AppendLine($"{edge.UNode.Tag}|{edge.VNode.Tag}");
            }
            #endregion

            File.WriteAllText(path, datalines.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodes"></param>
        /// <param name="edges"></param>
        /// <param name="createNodeFunc"></param>
        /// <param name="createEdgeFunc"></param>
        /// <returns></returns>
        public static bool ReadGraphFromFile(string path, out List<Node> nodes, out List<Edge> edges,
            Func<double, double, Node> createNodeFunc,
            Func<Node, Node, Edge> createEdgeFunc)
        {
            nodes = new List<Node>();
            edges = new List<Edge>();

            var dataLines = File.ReadAllLines(path);
            // n|m
            string[] args = dataLines[0].Split('|');

            int nodeSize, edgeSize;
            if (!Int32.TryParse(args[0], out nodeSize)
                || !Int32.TryParse(args[1], out edgeSize)) return false;

            double x, y;
            for (int i = 1; i <= nodeSize; i++)
            {
                // x|y|text
                args = dataLines[i].Split('|');

                if (!double.TryParse(args[0], out x)
                    || !double.TryParse(args[1], out y)) return false;

                Node node = createNodeFunc(x, y);
                node.NodeText = args[2];
                node.Tag = i - 1;
                nodes.Add(node);
            }

            int uIndex, vIndex;
            for (int i = nodeSize + 1; i <= nodeSize + edgeSize; i++)
            {
                // uNodeIndex|vNodeIndex
                args = dataLines[i].Split('|');
                if (!Int32.TryParse(args[0], out uIndex)
                    || !Int32.TryParse(args[1], out vIndex)) return false;
                Edge edge = createEdgeFunc(nodes[uIndex], nodes[vIndex]);
                edge.Tag = i - nodeSize - 1;
                edges.Add(edge);
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="myMedia"></param>
        /// <returns></returns>
        public static MediaState GetMediaState(this MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }
    }
}
