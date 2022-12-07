using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day07_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var listingDirectory = false;
            var root = new DirectoryNode("/");
            var cd = root;

            foreach (var line in input)
            {
                var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts[0][0] == '$')
                {
                    // This is a command
                    switch (parts[1])
                    {
                        case "cd":
                            var target = parts[2];
                            if (target == "/")
                                cd = root;
                            else if (target == "..")
                                cd = cd.Parent;
                            else
                                cd = cd.Children[target];
                            break;
                        case "ls":
                            listingDirectory = true;
                            break;
                    }
                }
                else
                {
                    // This is output from a command
                    if (listingDirectory)
                    {
                        if (parts[0] == "dir")
                        {
                            var name = parts[1];
                            cd.AddDirectory(name);
                        }
                        else
                        {
                            var size = int.Parse(parts[0]);
                            cd.AddFile(size);
                        }
                    }
                }
            }

            // Prepare sizes
            root.AddChildSizes();

            // Print the tree
            //Visit(root, (node, depth) =>
            //{
            //    var indent = new string(' ', depth * 2);
            //    Logger.DebugLine($"{indent}- {node.Name} (dir, size={node.Size})");
            //});

            // Do the assignment
            var sum = 0;
            Visit(root, (node, _) => {
                var size = node.Size;
                if (size <= 100000)
                    sum += size;
            });

            return sum.ToString();
        }

        private static void Visit(DirectoryNode node, Action<DirectoryNode, int> action, int depth = 0)
        {
            action(node, depth);
            foreach (var child in node.Children)
            {
                Visit(child.Value, action, depth + 1);
            }
        }
    }

    internal class DirectoryNode
    {
        private Dictionary<string, DirectoryNode> _children;
        private DirectoryNode? _parent;

        public string Name { get; }
        public int Size { get; private set; }
        public IReadOnlyDictionary<string, DirectoryNode> Children => _children;
        public DirectoryNode Parent => _parent ?? this;

        public DirectoryNode(string name)
        {
            Name = name;
            _children = new Dictionary<string, DirectoryNode>();
        }

        public void AddDirectory(string name)
        {
            var child = new DirectoryNode(name)
            {
                _parent = this
            };
            _children.Add(name, child);
        }

        public void AddFile(int size)
        {
            Size += size;
        }

        public void AddChildSizes()
        {
            foreach (var child in Children)
            {
                var c = child.Value;
                c.AddChildSizes();
                Size += c.Size;
            }
        }
    }
}
