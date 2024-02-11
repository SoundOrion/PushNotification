using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PushNotification.Server.Models;
using WebPush;

namespace PushNotification.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WebPushController : Controller
{
    private readonly IConfiguration _configuration;

    public WebPushController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("notifications/send")]
    public async Task<IActionResult> SendNotificationAsync(string title, string message, string email = "example@example.com")
    {
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                title,
                message,
                //url = "https://codelabs.developers.google.com/codelabs/push-notifications#0",
            });

            //var keys = VapidHelper.GenerateVapidKeys();
            //ViewBag.PublicKey = keys.PublicKey;
            //ViewBag.PrivateKey = keys.PrivateKey;

            var vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
            var vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

            //本当はDBから取得する
            string fileName = "../subscription/subscription.json";
            var json = System.IO.File.ReadAllText(fileName);
            var subscription = JsonSerializer.Deserialize<NotificationSubscription>(json);

            var endpoint = subscription.Url;
            var p256dh = subscription.P256dh;
            var auth = subscription.Auth;

            var pushSubscription = new PushSubscription(endpoint, p256dh, auth);
            var vapidDetails = new VapidDetails("mailto:" + email, vapidPublicKey, vapidPrivateKey);

            var webPushClient = new WebPushClient();
            webPushClient.SendNotification(pushSubscription, payload, vapidDetails);

            return Ok();
        }
        catch (WebPushException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 本当はDBに保存する
    /// </summary>
    /// <param name="subscription"></param>
    [HttpPost]
    [Route("notifications/subscribe")]
    public async Task RegisterSubscription(NotificationSubscription subscription)
    {
        if (subscription == null) return;

        string fileName = "../subscription/subscription.json";
        await using FileStream createStream = System.IO.File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, subscription);
    }

    [HttpPost]
    [Route("notifications/unsubscribe")]
    public void Delete()
    {
        //DBから該当のsubscription情報を削除する
    }
}