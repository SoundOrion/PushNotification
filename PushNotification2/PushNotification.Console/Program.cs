using WebPush;

var vapidKeys = VapidHelper.GenerateVapidKeys();
var vapidPublicKey = vapidKeys.PublicKey;
var vapidPrivateKey = vapidKeys.PrivateKey;

Console.WriteLine(vapidPublicKey);
Console.WriteLine(vapidPrivateKey);