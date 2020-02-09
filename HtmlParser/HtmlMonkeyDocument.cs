﻿// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xNet
{
    /// <summary>
    /// Holds the nodes of a parsed document (starting from the
    /// <see cref="RootNodes"></see> collection). Includes methods to parse
    /// a document and search a parsed document.
    /// </summary>
    public class HtmlMonkeyDocument
    {
        /// <summary>
        /// The source document, if known.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The document root nodes, from which all document nodes can be reached.
        /// </summary>
        public HtmlNodeCollection RootNodes { get; set; }

        /// <summary>
        /// Initializes a new <c>HtmlMonkeyDocument</c> instances with no data.
        /// </summary>
        public HtmlMonkeyDocument()
        {
            Path = string.Empty;
            RootNodes = new HtmlNodeCollection(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToHtml() => string.Join(string.Empty, RootNodes.Select(n => n.OuterHtml));

        /// <summary>
        /// Searches the given nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(string selector)
        {
            return Find(RootNodes, selector);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode
        {
            return FindOfType<T>(RootNodes);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode
        {
            return FindOfType<T>(RootNodes, predicate);
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate)
        {
            return Find(RootNodes, predicate);
        }

        #region Static methods

        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlMonkeyDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <returns>Returns an <see cref="HtmlMonkeyDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlMonkeyDocument FromFile(string path)
        {
            return FromHtml(File.ReadAllText(path));
        }

        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlMonkeyDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>Returns an <see cref="HtmlMonkeyDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlMonkeyDocument FromFile(string path, Encoding encoding)
        {
            return FromHtml(File.ReadAllText(path, encoding));
        }

        /// <summary>
        /// Parses an HTML or XML string and returns an <see cref="HtmlMonkeyDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="html">The HTML or XML string to parse.</param>
        /// <returns>Returns an <see cref="HtmlMonkeyDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlMonkeyDocument FromHtml(string html)
        {
            HtmlParser parser = new HtmlParser();
            return parser.Parse(html);
        }

        /// <summary>
        /// Searches the given nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes, string selector)
        {
            SelectorCollection selectors = Selector.ParseSelector(selector);
            return selectors.Find(nodes);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T) && predicate((T)n)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlNode> Find(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            List<HtmlNode> results = new List<HtmlNode>();
            FindRecursive(nodes, predicate, results);
            return results;
        }

        /// <summary>
        /// Recursive portion of <see cref="Find"></see>.
        /// </summary>
        private static void FindRecursive(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate, List<HtmlNode> results)
        {
            foreach (var node in nodes)
            {
                if (predicate(node))
                    results.Add(node);
                if (node is HtmlElementNode elementNode)
                    FindRecursive(elementNode.Children, predicate, results);
            }
        }

        #endregion

    }
}
