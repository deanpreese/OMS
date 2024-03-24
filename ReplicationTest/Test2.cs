using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Replication.TestDecoding;

public class Test2 : BackgroundService
{
    private readonly ILoggerFactory _loggerFactory;
    string conn_string = "Host=localhost;Database=orders;Username=trading;Password=abc"; 
    LogicalReplicationConnection conn;
    string slot_name = "liveorders_slot";
    TestDecodingReplicationSlot slot;

    public Test2(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
         conn = new LogicalReplicationConnection(conn_string);
         
         
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await conn.Open();
        slot = new TestDecodingReplicationSlot(slot_name);
        await foreach (var message in conn.StartReplication(slot, stoppingToken))
        {
            //Console.WriteLine($"Received message type: {message.GetType().Name}");
            Console.WriteLine($"Received message type: {message.Data}");
            // Always call SetReplicationStatus() or assign LastAppliedLsn and LastFlushedLsn individually
            // so that Npgsql can inform the server which WAL files can be removed/recycled.
        }
    }
}