

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (sender, message) {
    const li = document.createElement("li");
    li.className = "list-group-item bg-dark text-light border-secondary";
    li.innerHTML = `<strong>${sender}:</strong> ${message}`;
    const list = document.getElementById("messagesList");
    //if (window.currentUsername==user)
    list.appendChild(li);
    list.scrollTop = list.scrollHeight;
});

let user = null;

connection.start().then(function () {
    user = window.currentUsername;

    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const messageInput = document.getElementById("messageInput");
    const message = messageInput.value;

    if (message !== "" && user) {
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        messageInput.value = "";
    }
    event.preventDefault();
});

document.getElementById("messageInput").addEventListener("keypress", function (e) {
    if (e.key === "Enter") {
        document.getElementById("sendButton").click();
    }
});
