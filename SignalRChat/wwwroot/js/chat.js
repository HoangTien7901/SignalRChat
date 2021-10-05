"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("ReceiveMessage", function (user, message, type, chatid) {
    // var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var userName = document.getElementById("UserName").value;
    if (type == "Message") {
        var today = new Date();
        var months = today.getMonth() + 1;
        var days = today.getDate();
        var hours = today.getHours();
        var minutes = today.getMinutes();
        if (months + 1 < 10) {
            months = "0" + months;
        }
        if (days < 10) {
            days = "0" + days;
        }
        if (hours < 10) {
            hours = "0" + hours;
        }
        if (minutes < 10) {
            minutes = "0" + minutes;
        }
        var time = days + "/" + months + "/" + today.getFullYear().toString().substr(-2) + " " + hours + ":" + minutes;
        var name;
        var directChatClass, spanUserClass, spanTime, Float;
        console.log("userName: " + userName);
        
        if (user !== userName) {
            name = user;/*Chat.el.senderName.value*/
            //avatar = Chat.el.senderAvatar.value;
            directChatClass = "direct-chat-msg";
            spanUserClass = "direct-chat-name float-left ml-5";
            spanTime = "direct-chat-timestamp float-left ml-2";
            Float = "float-left"


        } else {
            name = userName;/*Chat.el.recipientName.value*/
            //avatar = Chat.el.recipientAvatar.value;
            directChatClass = "direct-chat-msg right";
            spanUserClass = "direct-chat-name float-right mr-5";
            spanTime = "direct-chat-timestamp float-right mr-2";
            Float = "float-right";

        }

        var img = document.createElement("img");
        img.src = "/assets/dist/img/avatar04.png";
        img.className = "direct-chat-img";
        var divMsg = document.createElement("div");
        divMsg.classList.add("direct-chat-text");
        divMsg.classList.add(Float);
        divMsg.textContent = message;
        var spTime = document.createElement("span");
        spTime.className = spanTime;
        spTime.textContent = time;
        var spName = document.createElement("span");
        spName.className = spanUserClass;
        spName.textContent = name;
        var divInfo = document.createElement("div");
        divInfo.classList = "direct-chat-infos";
        divInfo.classList = "clearfix";
        divInfo.appendChild(spName);
        divInfo.appendChild(spTime)
        var encodedMsg = document.createElement("div");
        encodedMsg.className = directChatClass;
        encodedMsg.appendChild(divInfo);
        encodedMsg.appendChild(img);
        encodedMsg.appendChild(divMsg);        
        var li = document.createElement('li');
        li.appendChild(encodedMsg);
        
        document.getElementById("messagesList").appendChild(li);
    } else if (type == "AddPrivate") {
        alert(message)
        $("#sidebar").append("<header><a href='/Home/Chat?ChatID=" + chatid + "'>" + user + "</a></header>");
    } else if (type == "AddGroup") {
        alert(message)
        $("#sidebar").append("<header><a href='/Home/Chat?ChatID=" + chatid + "'>" + user + "</a></header>");
    }

});

var chatid = null;
var chat = document.getElementById("ChatID");
if (chat != null) {
    chatid = chat.value;
}

connection.start().then(function () {
    if (chatid != null) {
        connection.invoke("JoinGroup", chatid).catch(function (err) {
            return console.error(err.toString());
        });
    }

}).catch(function (err) {
    return console.error(err.toString());
});

if (chatid != null) {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        $.post("/Home/SendMessage", {

            Text: message,
            ChatID: chatid

        },
            function (data) {
                resetInput();
                console.log(data)
            }

        )
        event.preventDefault();
    });
}
function resetInput() {
    $("#messageInput").val("");
}
