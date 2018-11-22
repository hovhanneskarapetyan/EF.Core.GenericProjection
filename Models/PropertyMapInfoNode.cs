using System;
using System.Collections.Generic;
using System.Linq;

namespace EF.Core.GenericProjection.Models
{
    public class PropertyMapInfoNode
    {
        public string Path { get; set; }
        public string PropertyName { get; set; }
        public string AssignPropertyName { get; set; }
        public bool IsComplexType => Children?.Any() ?? false;
        public Type PropertyType { get; set; }
        public List<PropertyMapInfoNode> Children { get; set; }


        public static PropertyMapInfoNode Merge<T>(List<PropertyMapInfoNode> infoNodes)
        {
            var root = new PropertyMapInfoNode()
            {
                PropertyType = typeof(T),
                Children = new List<PropertyMapInfoNode>(),
            };

            var tempRoot = infoNodes[0];
            root.Children.Add(infoNodes[0]);



            foreach (var node in infoNodes.Skip(1))
            {
                if (tempRoot.Path != node.Path)
                    root.Children.Add(node);
                else
                {
                    var location = tempRoot;
                    var tempNode = node.Children[0];

                    while (tempNode != null)
                    {


                        if (location.Children.All(x => x.Path != tempNode.Path))
                        {
                            tempRoot.Children.AddRange(node.Children);
                            break;
                        }
                        else
                        {
                            var child = location.Children.Single(x => x.Path == tempNode.Path);

                            location = child;
                            tempNode = tempNode.Children.Count == 0 ? null : tempNode.Children[0];
                        }
                    }
                }
            }



            return root;
        }
    }
}