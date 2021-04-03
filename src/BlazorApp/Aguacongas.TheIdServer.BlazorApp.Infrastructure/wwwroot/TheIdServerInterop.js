﻿window.bootstrapInteropt = {
    showToast: (id, dotnetHelper) => {
        const query = `#${id}`;
        $(query).on('hidden.bs.toast', function () {
            dotnetHelper.invokeMethodAsync('ToastClosed', id)
                .then(_ => { /* invoke it */ });
        });
        $(query).toast('show');
    },
    showModal: id => {
        $(`#${id}`).modal('show');
    },
    dismissModal: id => {
        $(`#${id}`).modal('hide');
    },
    showDropDownMenu: id => {
        $(`#${id}`).dropdown('show');
    },
    hideDropDownMenu: id => {
        $(`#${id}`).dropdown('hide');
    }
};
window.browserInteropt = {
    scrollTo: (id, offset) => {
        const element = document.getElementById(id);
        const y = element.getBoundingClientRect().top + window.pageYOffset;
        offset = offset || 0;
        window.scrollTo({ top: y + offset, behavior: 'smooth' });
    },
    preventEnterKeyPress: (id, dotnetHelper) => {
        const element = document.getElementById(id);
        element.onkeypress = e => {
            var key = e.charCode || e.keyCode || 0;
            if (key === 13) {
                e.preventDefault();
                dotnetHelper.invokeMethodAsync('EnterKeyPressed', id)
                    .then(_ => { /* invoke it */ });
            }
        };
    },
    onScrollEnd: (dotnetHelper, margin) => {
        let pending = false;
        window.onscroll = async () => {
            const top = $(window).scrollTop();
            const diff = $(document).height() - $(window).height() - margin;
            if (top >= diff && !pending) {
                pending = true;
                await dotnetHelper.invokeMethodAsync('ScrollBottomReach');
                pending = false;
            }
        };
    }
};
