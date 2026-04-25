using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services;

public class SseConnectionManager
{
    private readonly ConcurrentDictionary<Guid, ConcurrentBag<SseClient>> _connections = new();
    private readonly ILogger<SseConnectionManager> _logger;

    public SseConnectionManager(ILogger<SseConnectionManager> logger)
    {
        _logger = logger;
    }

    public void AddClient(Guid postId, SseClient client)
    {
        var bag = _connections.GetOrAdd(postId, _ => new ConcurrentBag<SseClient>());
        bag.Add(client);
        _logger.LogInformation("[SSE] Client {ClientId} connected to post {PostId}. Total: {Count}",
            client.ClientId, postId, bag.Count);
    }

    public async Task BroadcastAsync(Guid postId, string eventName, string jsonData, string? id = null)
    {
        if (!_connections.TryGetValue(postId, out var bag) || bag.IsEmpty)
            return;

        var clients = bag.ToArray();
        var deadClients = new List<SseClient>();

        foreach (var client in clients)
        {
            if (client.CancellationToken.IsCancellationRequested)
            {
                deadClients.Add(client);
                continue;
            }

            try
            {
                await SseWriter.WriteEventAsync(client.Response, eventName, jsonData, id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[SSE] Failed to write to client {ClientId} on post {PostId}: {Error}",
                    client.ClientId, postId, ex.Message);
                deadClients.Add(client);
            }
        }

        if (deadClients.Count > 0)
        {
            CleanupDeadClients(postId, deadClients);
        }
    }

    public void RemoveClient(Guid postId, SseClient client)
    {
        _logger.LogInformation("[SSE] Client {ClientId} disconnected from post {PostId}", client.ClientId, postId);
        CleanupDeadClients(postId, new List<SseClient> { client });
    }

    private void CleanupDeadClients(Guid postId, List<SseClient> deadClients)
    {
        if (!_connections.TryGetValue(postId, out var bag))
            return;

        var deadIds = new HashSet<Guid>(deadClients.Select(c => c.ClientId));
        var alive = bag.Where(c => !deadIds.Contains(c.ClientId)).ToArray();

        if (alive.Length == 0)
        {
            _connections.TryRemove(postId, out _);
            _logger.LogDebug("[SSE] No more clients for post {PostId}, removed bucket.", postId);
        }
        else
        {
            var newBag = new ConcurrentBag<SseClient>(alive);
            _connections[postId] = newBag;
        }
    }

    public int GetClientCount(Guid postId) =>
        _connections.TryGetValue(postId, out var bag) ? bag.Count : 0;
}

public record SseClient(Guid ClientId, HttpResponse Response, CancellationToken CancellationToken);
