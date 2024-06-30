window.ModalInterop = {
    show: (modalId) => {
        $(modalId).modal('show');
    },
    hide: (modalId) => {
        $(modalId).modal('hide');
    }
};