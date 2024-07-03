function showModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "block";
        modal.classList.add("show");

        // Add backdrop
        var backdrop = document.createElement("div");
        backdrop.className = "modal-backdrop fade show";
        document.body.appendChild(backdrop);
    }
}

function hideModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "none";
        modal.classList.remove("show");

        // Remove backdrop
        var backdrop = document.querySelector(".modal-backdrop");
        if (backdrop) {
            backdrop.parentNode.removeChild(backdrop);
        }
    }
}