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

            
            root.Children.Add(infoNodes[0]);




            foreach (var node in infoNodes.Skip(1))
            {
                var nodeToAdd = node;
                PropertyMapInfoNode whereToAdd = root.Children.SingleOrDefault(x => x.Path == node.Path);

                if (whereToAdd == null)
                {
                    root.Children.Add(nodeToAdd);
                    continue;
                }
                
                
                while (nodeToAdd.Path == whereToAdd.Path)
                {
                    if (nodeToAdd.Children.Count == 0)
                        break;

                    nodeToAdd = nodeToAdd.Children[0];

                    var tempLocation = whereToAdd.Children.SingleOrDefault(x => x.Path == nodeToAdd.Path);

                    if (tempLocation == null)
                    {
                        whereToAdd.Children.Add(nodeToAdd);
                        break;
                    }

                    whereToAdd = tempLocation;

                    if (nodeToAdd.Children.Count == 0)
                        break;

                    nodeToAdd = nodeToAdd.Children[0];

                }
            }



            return root;
        }
    }
}