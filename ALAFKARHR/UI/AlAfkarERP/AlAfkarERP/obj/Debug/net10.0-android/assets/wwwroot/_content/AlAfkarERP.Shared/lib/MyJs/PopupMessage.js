
//************************************* *//
//            Custome Modal             //
//************************************* *//
window.showModal = (id) => {
    const modalEl = document.querySelector(id);
    if (!modalEl) {
        console.error("Modal element not found:", id);
        return;
    }
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
};

window.hideModal = (id) => {
    const modalEl = document.querySelector(id);
    if (!modalEl) {
        console.error("Modal element not found:", id);
        return;
    }
    const modal = bootstrap.Modal.getInstance(modalEl);
    if (modal) modal.hide();
};

//************************************* *//
//            Custome Modal             //
//************************************* *//

