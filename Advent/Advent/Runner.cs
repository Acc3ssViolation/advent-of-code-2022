using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class Runner
    {
        private List<IAssignment> _assignments = new();

        public void Add(IAssignment assignment) => _assignments.Add(assignment);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();

            foreach (var assingment in _assignments)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var inputName = Path.Combine("Data", assingment.InputFile.ToLowerInvariant());
                    
                    Logger.DebugLine($"Loading data from {inputName} for assignment {assingment.Name}");
                    var lines = await File.ReadAllLinesAsync(inputName, cancellationToken).ConfigureAwait(false);

                    Logger.Line($"Running assignment {assingment.Name}");
                    stopwatch.Restart();
                    var result = await assingment.RunAsync(lines, cancellationToken).ConfigureAwait(false);
                    stopwatch.Stop();
                    Logger.Line($"Result of {assingment.Name}: {result}");
                    Logger.Line($"Took {stopwatch.ElapsedMilliseconds} ms");
                }
                catch(Exception ex) 
                {
                    stopwatch.Stop();
                    Logger.ErrorLine($"Exception when running assignment {assingment.Name}: {ex.Message}\n{ex.StackTrace}");
                    Logger.ErrorLine($"Took {stopwatch.ElapsedMilliseconds} ms");
                }

                Logger.Line();
            }
        }
    }
}
