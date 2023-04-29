let currentChatPhone = null;

let people = { };

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

function addNewMessageNotification(phone) {
    const chatPerson = document.querySelector(`[data-phone="${phone}"]`);

    const newMessageImage = document.createElement("img");
    newMessageImage.setAttribute("src", "./new-message.svg");
    newMessageImage.setAttribute("alt", "New Message");
    newMessageImage.classList.add("new-message-icon"); 
    chatPerson.appendChild(newMessageImage);
}

function attachClickListenerToChatPerson(chatPerson) {
    chatPerson.addEventListener("click", function () {
        const phone = chatPerson.getAttribute("data-phone");
        loadChatContent(phone);
        toggleEmptyChat(false);

        // Remove the new message icon if it exists
        const newMessageIcon = chatPerson.querySelector('img.new-message-icon');
        if (newMessageIcon) {
            newMessageIcon.parentNode.removeChild(newMessageIcon);
        }
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

    // Scroll down to the last message
    messages.scrollTop = messages.scrollHeight;

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
    fetchUserChats();

    const userId = "7140ca1a-af0f-4ce5-91d0-e3cf7e262da0"; //TODO: Get user ID from session cookie or something
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
        const phone = chatMessage.customer.contact;
        let person = people[phone];

        if (!person) {
            person = {
                name: chatMessage.customer.name,
                photo: chatMessage.customer.profilePicUrl
                ? chatMessage.customer.profilePicUrl
                : './blank-profile-picture-400x400.jpg',
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
        } else {
            addNewMessageNotification(phone);
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
            content: {
                type: "text",
                "text": input_text
            },
            customerContact: currentChatPhone
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


async function fetchUserChats() {
    try {
      const response = await fetch('http://localhost:50446/messages/7140ca1a-af0f-4ce5-91d0-e3cf7e262da0', {
        method: 'GET',
        headers: {
          'accept': 'application/json'
        },
      });
  
      if (!response.ok) {
        throw new Error(`Error fetching chat data! Status: ${response.status}`);
      }
  
      const data = await response.json();
      processData(data);
    } catch (error) {
      console.error('Error fetching chat data:', error);
    }
}

function processData(data) {
    data.forEach(chat => {
        const phone = chat.customer.contact;
        const profilePicUrl = isValidImageUrl(chat.customer.profilePicUrl) ? chat.customer.profilePicUrl : './blank-profile-picture-400x400.jpg';
        const person = {
        name: chat.customer.name,
        photo: profilePicUrl,
        messages: chat.messageHistory.map(message => ({
            text: message.content.text,
            type: message.type,
            timestamp: new Date(message.date),
        })),
        };
        people[phone] = person;
        addPersonToChatList(phone);
    });
}

function isValidImageUrl(url) {
    return url && /\.(?:jpg|jpeg|gif|png|webp)$/i.test(url);
}