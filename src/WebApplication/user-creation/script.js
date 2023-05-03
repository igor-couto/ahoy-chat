document.addEventListener("DOMContentLoaded", function () {
    const createUserForm = document.getElementById("create-user-form");
    const userGroupsContainer = document.getElementById("user-groups");

    async function fetchUserGroups() {
        try {
            const response = await fetch('your-url-to-get-user-groups');
            const userGroups = await response.json();
            userGroups.forEach(group => {
                const checkbox = document.createElement('div');
                checkbox.className = "checkbox";
                checkbox.innerHTML = `
                    <input type="checkbox" id="${group.id}" name="UserGroups" value="${group.id}">
                    <label for="${group.id}">${group.name} (${group.userCount})</label>
                `;
                userGroupsContainer.appendChild(checkbox);
            });
        } catch (error) {
            console.error('Error fetching user groups:', error);
        }
    }

    createUserForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const formData = new FormData(event.target);
        const selectedUserGroups = Array.from(formData.getAll("UserGroups"));

        // Validate photo size
        const photo = formData.get("Photo");
        const maxSize = 10 * 1024 * 1024; // 10MB
        if (photo.size > maxSize) {
            alert('Photo size should not exceed 10MB');
            return;
        }

        try {
            const response = await fetch('http://localhost:8082/users', {
                method: 'POST',
                body: formData 
            });

            if (response.ok) {
                alert('User created successfully');
                createUserForm.reset();
            } else {
                const error = await response.json();
                alert('Error creating user: ' + error.message);
            }
        } catch (error) {
            console.error('Error creating user:', error);
        }
    });

    fetchUserGroups();
});