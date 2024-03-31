using System.Collections.Concurrent;
using Grpc.Core;

namespace trb_officer_backend.Services;

public class Helper
{
    private readonly ConcurrentDictionary<string, IServerStreamWriter<Transaction>> _streams;
    private readonly ILogger<Helper> _logger;

    public Helper(ILogger<Helper> logger)
    {
        Console.WriteLine("Helper Init");
        _logger = logger;
        _streams = new ConcurrentDictionary<string, IServerStreamWriter<Transaction>>();
    }

    public async Task HandleMessage(string message)
    {
        foreach (var stream in _streams.Values)
        {
            try
            {
                await stream.WriteAsync(new Transaction { Result = message });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var item = _streams.FirstOrDefault(it => it.Value == stream);
                _streams.TryRemove(item);
            }
        }
    }

    public void AddStream(string key, IServerStreamWriter<Transaction> stream)
    {
        _streams.TryAdd(key, stream);
    }

    public void RemoveStream(string key, IServerStreamWriter<Transaction> stream)
    {
        _streams.TryRemove(new KeyValuePair<string, IServerStreamWriter<Transaction>>(key, stream));
    }
}