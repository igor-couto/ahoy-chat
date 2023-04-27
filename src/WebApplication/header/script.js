const userPhoto = document.getElementById("user-photo");
const userMenu = document.getElementById("user-menu");

function toggleMenu() {
  userMenu.style.display = userMenu.style.display === "block" ? "none" : "block";
}

function closeMenu(e) {
  if (!userMenu.contains(e.target) && e.target !== userPhoto) {
    userMenu.style.display = "none";
  }
}

document.addEventListener("click", closeMenu);
