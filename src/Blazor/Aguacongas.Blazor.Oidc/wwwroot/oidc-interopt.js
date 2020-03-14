import { subscribe, publish } from './oidc-message-bus.js';

window.aguacongasTheIdServerBlazorOidcInteropt = {
    createSilentRenewIFrame: (url, dotnetHelper) => {
        var iframe = document.createElement('iframe');
        var style = iframe.style;
        style.visibility = 'hidden';
        style.position = 'absolute';
        style.display = 'none';
        style.width = 0;
        style.height = 0;

        document.body.appendChild(iframe);
        iframe.src = url;
        subscribe(`${location.protocol}//${location.host}`, iframe.contentWindow, e => {
            dotnetHelper.invokeMethodAsync('SilentRenewAsync', e.data);
        });
        this.iframe = iframe;
    },
    renewToken: url => {
        publish(url, this.iframe.contentWindow);
    }
}