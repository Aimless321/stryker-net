using StreamJsonRpc;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Reporters;

namespace Stryker.CLI.Server;

public class RpcReporterFactory : IReporterFactory
{
    private readonly JsonRpc _rpc;

    public RpcReporterFactory(JsonRpc rpc) => _rpc = rpc;

    public IReporter Create(IStrykerOptions options, IGitInfoProvider branchProvider = null) => new RpcReporter(_rpc);
}
