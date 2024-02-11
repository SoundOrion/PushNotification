
(function () {

    const applicationServerPublicKey = '';

    //プッシュ通知認証の処理
    window.PushNotifications = {
        isSupported: async () => { //ブラウザがプッシュ通知をサポートしているか判定
            if ("Notification" in window)
                return true;
            return false;
        },
        askPermission: async () => {　//ユーザーがプッシュ通知を許可したか判定
            return new Promise((resolve, reject) => {
                Notification.requestPermission((permission) => {
                    resolve(permission);
                });
            });
        },
        requestSubscription: async () => {　//service worker がプッシュ通知を購読するように指定
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (!existingSubscription) {
                const newSubscription = await subscribe(worker);
            }
        },

        // プッシュ通知送信の処理
        create: async () => {
            var text = 'これはプッシュ通知テストです。';
            var notification = new Notification('プッシュ通知テスト', { body: text });
        }
    };

    async function subscribe(worker) {
        try {
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === 'NotAllowedError') {
                return null;
            }
            throw error;
        }
    }

})();
