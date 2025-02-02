using Grpc.Core;

namespace AuthService.Tests.Grpc.Interceptors;

using System;
using System.Threading;
using System.Threading.Tasks;

public class TestServerCallContext : ServerCallContext
{
    protected override string MethodCore { get; }
    protected override string HostCore { get; }
    protected override string PeerCore { get; }
    protected override DateTime DeadlineCore { get; }
    protected override Metadata RequestHeadersCore { get; }
    protected override CancellationToken CancellationTokenCore { get; }
    protected override Metadata ResponseTrailersCore { get; }
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore { get; }

    public TestServerCallContext(
        string method = "/TestService/UnaryCall",
        string host = "localhost",
        string peer = "TestPeer",
        DateTime? deadline = null,
        Metadata? requestHeaders = null,
        CancellationToken cancellationToken = default,
        Metadata? responseTrailers = null,
        Status status = default,
        WriteOptions? writeOptions = null,
        AuthContext? authContext = null
    )
    {
        MethodCore = method;
        HostCore = host;
        PeerCore = peer;
        DeadlineCore = deadline ?? DateTime.UtcNow.AddMinutes(1);
        RequestHeadersCore = requestHeaders ?? [];
        CancellationTokenCore = cancellationToken;
        ResponseTrailersCore = responseTrailers ?? [];
        Status = status;
        WriteOptions = writeOptions;
        AuthContextCore = authContext ?? new AuthContext(null, new Dictionary<string, List<AuthProperty>>());
    }

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) => throw new Exception();
    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) { return Task.CompletedTask; }
}
