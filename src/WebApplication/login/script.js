document.querySelectorAll('.message a').forEach((element) => {
    element.addEventListener('click', (event) => {
      event.preventDefault();
      const forms = document.querySelectorAll('.form form');
      forms.forEach((form) => {
        form.classList.toggle('hidden');
      });
    });
  });
  