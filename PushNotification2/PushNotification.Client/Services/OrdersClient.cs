using PushNotification.Client.Models;

namespace PushNotification.Client.Services;

public class OrdersClient
{
    private readonly HttpClient _httpClient;

    public OrdersClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SubscribeToNotifications(NotificationSubscription subscription)
    {
        var response = await _httpClient.PostAsJsonAsync("WebPush/notifications/subscribe", subscription);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnSubscribeToNotifications(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("WebPush/notifications/unsubscribe", id);
        response.EnsureSuccessStatusCode();
    }
}
