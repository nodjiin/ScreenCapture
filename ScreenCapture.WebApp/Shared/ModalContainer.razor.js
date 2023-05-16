const modals = new Map();

function GetOrCreateModal(id) {
    let modal;
    if (modals.has(id)) {
        modal = modals.get(id);
    } else {
        const modalElement = document.getElementById(id);
        if (modalElement === null || typeof modalElement === "undefined") {
            return null;
        }

        modal = new bootstrap.Modal(modalElement);
        modals.set(id, modal);
    }

    return modal;
}

export function showModal(id) {
    const modal = GetOrCreateModal(id);
    if (modal !== null && typeof modal !== "undefined") {
        modal.show();
    }
}

export function hideModal(id) {
    const modal = GetOrCreateModal(id);
    if (modal !== null && typeof modal !== "undefined") {
        modal.hide();
    }
}