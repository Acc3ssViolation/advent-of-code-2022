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
        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var listingDirectory = false;
            IDirectoryNode root = new DirectoryNode("/");
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
                                cd = cd.Children[target] as IDirectoryNode ?? throw new Exception("Fuck");
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
                            if (!cd.Children.ContainsKey(name))
                                cd.AddChild(new DirectoryNode(name));
                        }
                        else
                        {
                            var name = parts[1];
                            if (!cd.Children.ContainsKey(name))
                            {
                                var size = long.Parse(parts[0]);
                                cd.AddChild(new FileNode(name, size));
                            }
                        }
                    }
                }
            }

            // Print the tree
            Visit(root, (node, depth) => {
                var indent = new string(' ', depth * 2);
                if (node.IsDirectory)
                {
                    Logger.DebugLine($"{indent}- {node.Name} (dir)");
                }
                else
                {
                    Logger.DebugLine($"{indent}- {node.Name} (file, size={node.Size})");
                }
            });

            // Do the assignment
            var sum = 0L;
            Visit(root, (node, _) => {
                var size = node.Size;
                if (node.IsDirectory && size <= 100000)
                    sum += size;
            });

            return Task.FromResult(sum.ToString());
        }

        private static void Visit(IFileSystemNode node, Action<IFileSystemNode, int> action, int depth = 0)
        {
            action(node, depth);
            if (node.IsDirectory)
            {
                foreach (var child in node.Children)
                {
                    Visit(child.Value, action, depth + 1);
                }
            }
        }
    }

    internal interface IFileSystemNode
    {
        string Name { get; }
        long Size { get; }
        bool IsDirectory { get; }
        IReadOnlyDictionary<string, IFileSystemNode> Children { get; }
        IDirectoryNode Parent { get; set; }
    }

    internal interface IDirectoryNode : IFileSystemNode
    {
        bool IFileSystemNode.IsDirectory => true;

        long IFileSystemNode.Size => Children.Aggregate(0L, (s, c) => s + c.Value.Size);

        void AddChild(IFileSystemNode child);
    }

    internal class DirectoryNode : IDirectoryNode
    {
        private readonly Dictionary<string, IFileSystemNode> _children;

        public string Name { get; }

        public IReadOnlyDictionary<string, IFileSystemNode> Children => _children;

        public IDirectoryNode Parent { get; set; }

        public DirectoryNode(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _children= new Dictionary<string, IFileSystemNode>();
            Parent = this;
        }

        public void AddChild(IFileSystemNode child)
        {
            _children.Add(child.Name, child);
            child.Parent = this;
        }
    }

    internal class FileNode : IFileSystemNode
    {
        public string Name { get; }

        public long Size { get; }

        public bool IsDirectory => false;

        public IReadOnlyDictionary<string, IFileSystemNode> Children => throw new NotSupportedException();

        public IDirectoryNode Parent { get; set; }

        public FileNode(string name, long size)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
        }
    }
}
