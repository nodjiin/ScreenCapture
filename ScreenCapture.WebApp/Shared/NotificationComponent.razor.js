var toast;

export function showToast() {
    if (toast === null || typeof toast === "undefined") {
        toast = new bootstrap.Toast(document.querySelector('.toast'));
    }

    toast.show();
}