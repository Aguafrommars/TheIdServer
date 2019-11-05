window.bootstrapInteropt = {
    showToast: (id, dotnetHelper) => {
        const query = `#${id}`;
        $(query).on('hidden.bs.toast', function () {
            dotnetHelper.invokeMethodAsync('ToastClosed', id)
                .then(_ => { });
        });
        $(query).toast('show');
    },
    dismissModal: id => {
        const query = `#${id}`;
        $(query).modal('hide');
    }
};
window.browserInteropt = {
    scrollTo: (id, offset) => {
        const element = document.getElementById(id);
        const y = element.getBoundingClientRect().top + window.pageYOffset;
        offset = offset || 0;
        window.scrollTo({ top: y + offset, behavior: 'smooth' });
    }
};