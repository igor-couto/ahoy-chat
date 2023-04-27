let currentChatPhone = null;

let people = {

};

function toggleEmptyChat(showEmptyChat) {
    const form = document.getElementsByClassName("send-section")[0];
  
    if (showEmptyChat) {
      form.style.display = "none";
    } else {
      form.style.display = "";
    }
}

function addPersonToChatList(phone) {
    const person = people[phone];
    const chatList = document.getElementById("chat-list");
    
    const chatPerson = document.createElement("div");
    chatPerson.classList.add("chat-person");
    chatPerson.setAttribute("data-phone", phone);
  
    const img = document.createElement("img");
    img.setAttribute("src", person.photo);
    img.setAttribute("alt", person.name);
    chatPerson.appendChild(img);
  
    const chatPersonInfo = document.createElement("div");
    chatPersonInfo.classList.add("chat-person-info");
  
    const chatPersonName = document.createElement("span");
    chatPersonName.classList.add("chat-person-name");
    chatPersonName.textContent = person.name;
    chatPersonInfo.appendChild(chatPersonName);
  
    const chatPersonPhone = document.createElement("span");
    chatPersonPhone.classList.add("chat-person-phone");
    chatPersonPhone.textContent = phone;
    chatPersonInfo.appendChild(chatPersonPhone);
  
    chatPerson.appendChild(chatPersonInfo);
    chatList.appendChild(chatPerson);

    attachClickListenerToChatPerson(chatPerson);
}

function attachClickListenerToChatPerson(chatPerson) {
    chatPerson.addEventListener("click", function () {
        const phone = chatPerson.getAttribute("data-phone");
        loadChatContent(phone);
        toggleEmptyChat(false);
    });
}

function loadChatContent(phone) {
    const person = people[phone];
    if (!person) {
        currentChatPhone = null;
        return;
    }
    
    currentChatPhone = phone;

    const messages = document.getElementById("messages");
    messages.innerHTML = "";
  
    person.messages.forEach((message) => {
      addMessage(message.text, message.type, message.timestamp);
    });

  // Highlight the selected person in the chat list
    const chatList = document.getElementById("chat-list");
    const chatPeople = chatList.getElementsByClassName("chat-person");
    for (let i = 0; i < chatPeople.length; i++) {
            const chatPerson = chatPeople[i];
        if (chatPerson.getAttribute("data-phone") === phone) {
            chatPerson.classList.add("selected");
        } else {
            chatPerson.classList.remove("selected");
        }
    }
}

function addMessage(text, messageType, timestampValue) {
    const messages = document.getElementById("messages");
    const messageContainer = document.createElement("div");
    messageContainer.classList.add(`${messageType}-container`);

    const messageDiv = document.createElement("div");
    messageDiv.classList.add("message", messageType);
    messageDiv.textContent = text;

    const timestamp = document.createElement("span");
    timestamp.classList.add("timestamp");
    const currentTime = new Date();
    timestamp.textContent = `${currentTime.getHours().toString().padStart(2, "0")}:${currentTime.getMinutes().toString().padStart(2, "0")}`;
    messageDiv.appendChild(timestamp);

    messageContainer.appendChild(messageDiv);
    messages.appendChild(messageContainer);
    messages.scrollTop = messages.scrollHeight;
}   

window.addEventListener("load", function () {
    const userId = "7140ca1a-af0f-4ce5-91d0-e3cf7e262da0";
    const mySocket = new WebSocket("ws://localhost:50446/ws/" + userId);
    
    toggleEmptyChat(true);

    if (people !== null && Object.keys(people).length > 0) {
        for (let key in people) {
            addPersonToChatList(key);
        }
    }    

    mySocket.onmessage = function (event) {

        const chatMessage = JSON.parse(event.data);
        
        if (chatMessage.content.type !== 'text') {
            console.log("only working with text messages currently.");
            return;
        }

        const text = chatMessage.content.text;
        const phone = chatMessage.from.contact;
        let person = people[phone];

        if (!person) {
            person = {
                name: chatMessage.from.name,
                photo: 'https://www.nailseatowncouncil.gov.uk/wp-content/uploads/blank-profile-picture-973460_1280-400x400.jpg',
                messages: [],
            };
            people[phone] = person;
            addPersonToChatList(phone);
        }

        person.messages.push({
            text: text,
            type: 'incoming',
            timestamp: new Date(chatMessage.date),
        });

        if (phone === currentChatPhone) {
            addMessage(text, 'incoming');
        }
    };

    const form = document.getElementsByClassName("send-section");
    const input = document.getElementById("input");
    input.focus();
    
    form[0].addEventListener("submit", function (e) {
      input_text = input.value;

      try {
        addMessage(input_text, "sent");

        const person = people[currentChatPhone];

        const chatMessage = JSON.stringify({
            id: crypto.randomUUID(),
            date: new Date(),
            from: {
                name: "",
                contact: "",
                role: "seller"
            },
            to: {
                name: person.name,
                contact: currentChatPhone,
                role: "customer"
            },
            content: {
                type: "text",
                "text": input_text
            }
        });

        mySocket.send(chatMessage);

        person.messages.push({
            text: input_text,
            type: 'sent',
            timestamp: new Date(chatMessage.date),
        });
      } catch (error) {
        console.error(error);
        // TODO: make the message red or something
      }
      
      input.value = "";
      e.preventDefault();
    });
});