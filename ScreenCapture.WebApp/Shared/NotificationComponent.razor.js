var toast;

export function showToast() {
    if (toast === null || typeof toast === "undefined") {
        const toastElement = document.querySelector('.toast');
        if (toastElement === null || typeof toastElement === "undefined") {
            return;
        }

        toast = new bootstrap.Toast(toastElement);
    }

    toast.show();
}