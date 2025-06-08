document.addEventListener("DOMContentLoaded", function () {
    let notificationCount = 0;

    function showNotification(message) {
        saveNotification(message);

        const toast = document.createElement('div');
        toast.className = 'toast show bg-info text-white position-fixed bottom-0 end-0 m-3';
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');
        toast.style.zIndex = 1056;

        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>`;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 500);
        }, 5000);
    }

    function saveNotification(message) {
        const notifications = JSON.parse(localStorage.getItem('notifications') || '[]');
        notifications.push({
            message: message,
            timestamp: new Date().toISOString()
        });
        localStorage.setItem('notifications', JSON.stringify(notifications));
    }

    function loadNotificationsOnPageLoad() {
        let notificationCount = 0;
        const notifications = JSON.parse(localStorage.getItem('notifications') || '[]');
        notificationCount = notifications.length;

        notifications.forEach(n => {
            addNotificationToDropdown(n.message);
        });
        const countSpan = document.getElementById('notificationCount');
        if (countSpan) {
            countSpan.textContent = notificationCount;
            countSpan.style.display = notificationCount > 0 ? 'inline' : 'none';
        }
    }

    function addNotificationToDropdown(messageText) {
        const notificationList = document.getElementById('notificationList');
        if (!notificationList) return;

        const liItems = notificationList.querySelectorAll('li');
        const dividerBeforeViewAll = liItems.length >= 2 ? liItems[liItems.length - 2] : null;

        const newItem = document.createElement('li');
        newItem.innerHTML = `<a class="dropdown-item small" href="#">${messageText}</a>`;

        if (dividerBeforeViewAll) {
            notificationList.insertBefore(newItem, dividerBeforeViewAll);
        } else {
            notificationList.appendChild(newItem);
        }
    }

    function updateNotificationCount() {
        notificationCount++;
        const countSpan = document.getElementById('notificationCount');
        if (countSpan) {
            countSpan.textContent = notificationCount;
            countSpan.style.display = 'inline';
        }
    }

    const notificationButton = document.getElementById('notificationDropdown');
    const countSpan = document.getElementById('notificationCount');

    if (notificationButton && countSpan) {
        notificationButton.addEventListener('click', function () {
            notificationCount = 0;
            countSpan.textContent = '0';
            countSpan.style.display = 'none';

            // Mark all current notifications as seen
            const now = new Date().toISOString();
            localStorage.setItem('notificationsLastSeen', now);
        });
    }

    loadNotificationsOnPageLoad();

    const hubUrl = window.signalRHubUrl || "/notificationHub";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .configureLogging(signalR.LogLevel.Debug)
        .withAutomaticReconnect([0, 2000, 10000, 30000])
        .build();

    connection.on("ReceiveNotification", function (eventName, message) {
        console.log("SignalR Received:", eventName, message);

        let displayMessage = "";
        switch (eventName) {
            case "PatientRegisteredEvent":
                displayMessage = `🩺 New patient registered: ${message.name}`;
                break;
            case "PatientUpdatedEvent":
                displayMessage = `🔄 Patient updated: ${message.name}`;
                break;
            case "PatientDeletedEvent":
                displayMessage = `❌ Patient deleted: ${message.name}`;
                break;
            default:
                displayMessage = `📢 ${eventName}: ${JSON.stringify(message)}`;
                break;
        }

        showNotification(displayMessage);
        addNotificationToDropdown(displayMessage);
        updateNotificationCount();
    });

    connection.start()
        .then(() => console.log("✅ SignalR connected"))
        .catch(err => console.error("❌ SignalR error:", err));
});
