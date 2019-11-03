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
