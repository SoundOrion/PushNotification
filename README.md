# Blazor Server(.NET8)でプッシュ通知を実装する

## とりあえず使うには
PushNotification2で
- `PushNotification.Server`プロジェクトの`appsettings.json`に公開鍵と秘密鍵をセット
- `PushNotification.Client`プロジェクトの`wwwroot/js/pushNotifications.js`の`applicationServerPublicKey`に公開鍵をセット
- `PushNotification.Server`プロジェクトと`PushNotification.Client`プロジェクトをマルチ スタートアップなどで実行すればよい

## 簡単な流れ
- サーバー側で公開鍵と秘密鍵を準備する
- クライアント側で公開鍵を使って通知先のURL等を作成する
- 通知したい人が上の情報を使って通知する
  
ここではサーバー側(WebApi)で通知を行うことを想定している。

## サーバー側で公開鍵と秘密鍵を準備する
### VAPIDを生成
以下のWebサイト等を利用する。

- https://web-push-codelab.glitch.me/
- https://www.attheminute.com/vapid-key-generator

もしくは https://www.nuget.org/packages/WebPush を利用するとよい。
~~~ csharp
using WebPush;

var vapidKeys = VapidHelper.GenerateVapidKeys();
var vapidPublicKey = vapidKeys.PublicKey;
var vapidPrivateKey = vapidKeys.PrivateKey;
~~~

`appsettings.json`に生成した公開鍵と秘密鍵を以下のようにセットする。
~~~ csharp
  "VapidKeys": {
    "PublicKey": "ABCDEFGHIJKLMN123456789",
    "PrivateKey": "ZZZ1234"
  }
~~~

## クライアント側で公開鍵を使って通知先のURL等を作成
`service-worker.js`と`pushNotifications.js`を作成して`App.razor`に以下のように記述
~~~ js
    <script src="js/pushNotifications.js"></script>
    <script>navigator.serviceWorker.register('service-worker.js');</script>
~~~

### service-worker.js
プッシュ通知を受け取ったときとプッシュ通知をクリックしたときの動作を定義している。

### pushNotifications.js
Push通知をするためにはEndpoint、UserPublicKey、UserAuthToken（上記の公開鍵とは別物）が必要で、これを取得する関数を定義している。`const applicationServerPublicKey = ...`のところに公開鍵をセットするのを忘れないように注意。

`Components/Pages/Home.razor`で`pushNotifications.js`内の関数を呼び出して、サーバー側に通知したりしている。

## 通知したい人が上の情報を使って通知する
本来はEndpoint、UserPublicKey、UserAuthTokenはDBなどに格納して利用すべきだと思うが、ここでは`subscription`フォルダに`subscription.json`という名前で保存するようにしており、これを通知に利用するようにしている。
`WebPushController.cs`に通知の関数などを定義しており、`Swagger`で実行可能。

## 参考にしたサイト
- https://lets-csharp.com/push-test/
- https://qiita.com/MNakae_IG/items/d4eafa1be135b4624865
- https://kagasu.hatenablog.com/entry/2023/04/08/164350
- https://puni-o.hatenablog.com/entry/2018/03/09/183617
- https://github.com/dotnet-presentations/blazor-workshop/blob/main/docs/09-progressive-web-app.md#displaying-notifications
- https://codelabs.developers.google.com/codelabs/push-notifications#0
- https://github.com/dotnet-presentations/blazor-workshop/tree/main
- https://github.com/web-push-libs/web-push-csharp?tab=readme-ov-file
- https://github.com/coryjthompson/WebPushDemo
- https://zukucode.com/2020/05/webpush-javascript-php.htmlWEBアプリでプッシュ通知を実装する