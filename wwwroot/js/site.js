// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    const categoryButtons = document.querySelectorAll('.btn-outline-primary');
    categoryButtons.forEach(button => {
        button.addEventListener('click', function() {
            categoryButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
        });
    });

    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease-out';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    const modals = document.querySelectorAll('.modal');
    modals.forEach(modal => {
        modal.addEventListener('shown.bs.modal', function() {
            const modalBody = this.querySelector('.modal-body');
            if (modalBody) {
                modalBody.style.animation = 'fadeIn 0.3s ease-out';
            }
        });
    });

    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');

            this.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });
    });

    const navLinks = document.querySelectorAll('.nav-link');
    const currentUrl = window.location.pathname;
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentUrl) {
            link.classList.add('active');
        }
    });

    const newsletterForm = document.getElementById('newsletterForm');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const email = document.getElementById('newsletterEmail').value;
            if (!email) return;

            // Simple client-side feedback (no server integration in demo)
            const btn = newsletterForm.querySelector('button[type="submit"]');
            const orig = btn.innerHTML;
            btn.disabled = true;
            btn.innerHTML = 'გაგზავნა...';

            setTimeout(() => {
                btn.innerHTML = 'გმადლობთ!';
                btn.classList.remove('btn-outline-primary');
                btn.classList.add('btn-success');
                setTimeout(() => {
                    btn.disabled = false;
                    btn.innerHTML = orig;
                    btn.classList.remove('btn-success');
                    btn.classList.add('btn-outline-primary');
                    newsletterForm.reset();
                }, 2000);
            }, 1000);
        });
    }
});

document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        const modals = document.querySelectorAll('.modal.show');
        modals.forEach(modal => {
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }
        });
    }
});
