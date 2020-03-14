export function subscribe(origin, source, callback) {
    window.addEventListener('message', e => {
        if (e.origin === origin && e.source === source) {
            callback(e);
        }
    });
};
export function publish(url, to) {
    to.postMessage(url, `${location.protocol}//${location.host}`);
}

window.aguacongasTheIdServerBlazorOidc = {
    renewCallback: () => {
        const currentLocation = window.location;
        subscribe(`${currentLocation.protocol}//${currentLocation.host}`,
            window.parent, e => {
                window.location.href = e.data;
            });
        publish(currentLocation.href, window.parent);
    }
};