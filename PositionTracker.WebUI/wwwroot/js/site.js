// global vars here!
var isIpcActive = false; // manual control
var editInProgress = false;
var selectedTab = "";

if (isIpcActive) {
    const { ipcRenderer } = require("electron");
} else {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/hub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.onclose(function () {
        console.log("Hub disconnected");

        setTimeout(function () {
            connection.start().catch(err => console.error);;
        }, 1000);
    });

    connection.start().catch(err => console.error(err.toString()));
}

function sendMessage(type, message, done) {
    if (isIpcActive) {
        sendMessageIpc(type, message);
    } else {
        sendMessageSignalR(type, message, done);
    }
}

function sendMessageIpc(type, message) {
    ipcRenderer.send(type, message);
}

function sendMessageSignalR(type, message, done) {
    if (typeof (done) === "undefined") {
        done = function (retVal) {
            if (typeof (retVal) !== 'undefined' && retVal !== null && retVal !== "")
                alert(retVal);
        };
    }

    return connection.invoke(type, message)
        .then((ret) => done(ret))
        .catch((err) => alert(`Error on sendMessage: ${type} Err: ${err}`));
}

function subscribe(type, callback) {
    if (isIpcActive) {
        ipcRenderer.on(type, (event, arg) => callback(event));
    } else {
        connection.on(type, (event, arg) => callback(event));
    }
}