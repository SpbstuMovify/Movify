using System.Diagnostics;

namespace ChunkerService.Utils.ProcessRunners;

public interface IProcessRunner
{
    Task RunProcessAsync(
        ProcessStartInfo startInfo,
        CancellationToken cancellationToken
    );
}
