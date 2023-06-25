using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Queue.Application.Job.Models;

namespace Queue.Application.Job.HttpServices;

public class ReceiverService : IReceiverService
{
    private readonly HttpClient _httpClient;

    public ReceiverService(string baseAddress)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    public async Task<ReceiverResponse> Send(string processId)
    {
        using var response = await _httpClient.PostAsJsonAsync("SaveProcessId", new { processId });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReceiverResponse>();
    }
}