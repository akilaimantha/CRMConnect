// CRMConnect - SLIC themed client scripts

function checkDueTasks() {
    if (typeof $ === 'undefined') return;
    $.ajax({
        url: '/api/notifications',
        type: 'GET',
        success: function (data) {
            var count = parseInt(data, 10);
            if (count > 0) {
                showNotificationToast('You have ' + count + ' task(s) due in the next 3 days!');
            }
        },
        error: function () {
            console.log('Notification check skipped');
        }
    });
}

function showNotificationToast(message) {
    var toast = document.getElementById('notificationToast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'notificationToast';
        toast.className = 'notification-toast';
        document.body.appendChild(toast);
    }
    toast.innerHTML = '<strong>Task Reminder</strong><br>' + message;
    toast.style.display = 'block';
    setTimeout(function () {
        toast.style.display = 'none';
    }, 8000);
}

function validateRequired(fieldId, message) {
    var el = document.getElementById(fieldId);
    if (!el || !el.value || el.value.trim() === '') {
        alert(message);
        return false;
    }
    return true;
}

function setActiveNav() {
    var path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.slic-nav-links .nav-link').forEach(function (link) {
        var href = (link.getAttribute('href') || '').toLowerCase();
        if (href && href !== '/' && path.indexOf(href) === 0) {
            link.classList.add('active');
        } else if (href === '/' && path === '/') {
            link.classList.add('active');
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    setActiveNav();
    if (document.body.dataset.portal === 'admin') {
        checkDueTasks();
        setInterval(checkDueTasks, 300000);
    }
});
