using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day07_2 : IAssignment
    {
        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
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
            var totalSpace = 70000000;
            var requiredSpace = 30000000;
            var freeSpace = totalSpace - root.Size;
            var spaceToDelete = requiredSpace - freeSpace;
            var smallest = int.MaxValue;
            Visit(root, (node, _) =>
            {
                var size = node.Size;
                if (size < smallest && size >= spaceToDelete)
                    smallest = size;
            });

            return Task.FromResult(smallest.ToString());
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
}
