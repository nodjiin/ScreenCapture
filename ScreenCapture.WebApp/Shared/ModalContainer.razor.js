var modal;

export function showModal() {
    if (modal === null || typeof modal === "undefined") {
        const modalElement = document.getElementById("modal-container");
        if (modalElement === null || typeof modalElement === "undefined") {
            return;
        }

        modal = new bootstrap.Modal(modalElement);
    }

    modal.show();
}