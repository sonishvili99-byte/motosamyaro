// VRoom — site.js

document.addEventListener('DOMContentLoaded', function () {
    initializeCategoryFilters();
    initializeCardAnimations();
    initializeModalListeners();
    initializeButtonListeners();
    initializeNavLinks();
    initializeNewsletterForm();
    initializeSearchBar();
    initializeScrollReveal();
    initializeScrollTopButton();
    initializeHeroParticles();
    initializeStatCounters();
    initializeAutoToast();
});

/* ─────────────────────────────────────────────
   Category filter AJAX
───────────────────────────────────────────── */
function initializeCategoryFilters() {
    document.addEventListener('click', function (e) {
        const button = e.target.closest('.btn-outline-primary.btn');
        const isFilterButton = button && button.closest('.btn-group') !== null;
        if (!isFilterButton || !button.href) return;

        e.preventDefault();
        e.stopPropagation();

        const url = button.href;
        fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(r => { if (!r.ok) throw new Error('Network error'); return r.text(); })
            .then(html => {
                const parser = new DOMParser();
                const newDoc = parser.parseFromString(html, 'text/html');
                const mainContent = newDoc.querySelector('main');
                if (mainContent) {
                    document.querySelector('main').innerHTML = mainContent.innerHTML;
                    document.querySelectorAll('.btn-group .btn-outline-primary').forEach(b => b.classList.remove('active'));
                    button.classList.add('active');
                    window.history.pushState({ path: url }, '', url);
                    // Force-reveal all cards immediately — they're already in viewport after inject
                    document.querySelectorAll('.reveal, .reveal-left, .reveal-right').forEach(el => {
                        el.classList.add('visible');
                    });
                    initializeModalListeners();
                    initializeButtonListeners();
                }
            })
            .catch(() => { window.location.href = url; });
    });
}

/* ─────────────────────────────────────────────
   Card entrance animations (listing page)
───────────────────────────────────────────── */
function initializeCardAnimations() {
    // Only animate non-reveal cards (modals, sidebar cards etc.)
    // Listing cards use .reveal and are handled by IntersectionObserver
    const cards = document.querySelectorAll('.card:not(.reveal)');
    cards.forEach((card, i) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'all 0.5s ease-out';
            card.style.opacity   = '1';
            card.style.transform = 'translateY(0)';
        }, i * 80);
    });
}

/* ─────────────────────────────────────────────
   Scroll-reveal  (.reveal / .reveal-left / .reveal-right)
───────────────────────────────────────────── */
function initializeScrollReveal() {
    const els = document.querySelectorAll('.reveal, .reveal-left, .reveal-right');
    if (!els.length) return;

    const io = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                io.unobserve(entry.target);
            }
        });
    }, { threshold: 0.15 });

    els.forEach(el => io.observe(el));
}

/* ─────────────────────────────────────────────
   Scroll-to-top button
───────────────────────────────────────────── */
function initializeScrollTopButton() {
    const btn = document.getElementById('scrollTopBtn');
    if (!btn) return;

    window.addEventListener('scroll', () => {
        if (window.scrollY > 300) btn.classList.add('visible');
        else                       btn.classList.remove('visible');
    }, { passive: true });

    btn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));
}

/* ─────────────────────────────────────────────
   Hero floating particles
───────────────────────────────────────────── */
function initializeHeroParticles() {
    const container = document.getElementById('heroParticles');
    if (!container) return;

    const count = 22;
    for (let i = 0; i < count; i++) {
        const p = document.createElement('div');
        p.className = 'particle';
        const size = Math.random() * 12 + 4;
        p.style.cssText = `
            width: ${size}px;
            height: ${size}px;
            left: ${Math.random() * 100}%;
            bottom: ${Math.random() * 20}%;
            animation-duration: ${Math.random() * 8 + 6}s;
            animation-delay: ${Math.random() * 6}s;
            opacity: ${Math.random() * 0.4 + 0.1};
        `;
        container.appendChild(p);
    }
}

/* ─────────────────────────────────────────────
   Animated stat counters (About page)
───────────────────────────────────────────── */
function initializeStatCounters() {
    const counters = document.querySelectorAll('.stat-number[data-target]');
    if (!counters.length) return;

    const io = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            const el     = entry.target;
            const target = parseInt(el.dataset.target, 10);
            const suffix = el.dataset.suffix || '';
            const duration = 1400;
            const step  = 16;
            const steps = duration / step;
            let current = 0;

            const timer = setInterval(() => {
                current += target / steps;
                if (current >= target) {
                    el.textContent = target + suffix;
                    clearInterval(timer);
                } else {
                    el.textContent = Math.floor(current) + suffix;
                }
            }, step);
            io.unobserve(el);
        });
    }, { threshold: 0.5 });

    counters.forEach(el => io.observe(el));
}

/* ─────────────────────────────────────────────
   Toast helper  — call showToast('msg', 'success'|'error'|'info')
───────────────────────────────────────────── */
window.showToast = function (message, type = 'info', duration = 3500) {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const icons = { success: '✓', error: '✕', info: 'ℹ' };
    const toast = document.createElement('div');
    toast.className = `site-toast ${type}`;
    toast.innerHTML = `<span>${icons[type] || 'ℹ'}</span><span>${message}</span>`;
    container.appendChild(toast);

    requestAnimationFrame(() => {
        requestAnimationFrame(() => toast.classList.add('show'));
    });

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 400);
    }, duration);
};

/* ─────────────────────────────────────────────
   Auto-convert Bootstrap alerts to toasts on
   pages that already render TempData alerts
───────────────────────────────────────────── */
function initializeAutoToast() {
    document.querySelectorAll('.alert[role="alert"]').forEach(alert => {
        // Only toast-ify simple inline success/danger alerts (not form validation blocks)
        const isSimple = !alert.querySelector('ul') && !alert.querySelector('form');
        if (!isSimple) return;

        const type = alert.classList.contains('alert-success') ? 'success'
                   : alert.classList.contains('alert-danger')  ? 'error'
                   : 'info';
        const msg = alert.innerText.trim();
        if (msg) showToast(msg, type, 4000);
    });
}

/* ─────────────────────────────────────────────
   Modal animations
───────────────────────────────────────────── */
function initializeModalListeners() {
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('shown.bs.modal', function () {
            const body = this.querySelector('.modal-body');
            if (body) body.style.animation = 'fadeIn 0.3s ease-out';
        });
    });
}

/* ─────────────────────────────────────────────
   Ripple effect on buttons
───────────────────────────────────────────── */
function initializeButtonListeners() {
    document.addEventListener('click', function (e) {
        const button = e.target.closest('.btn');
        if (!button) return;
        if (button.closest('.btn-group') || button.hasAttribute('data-bs-toggle')) return;
        if (!button.classList.contains('btn-primary') && !button.classList.contains('btn-secondary')) return;

        const ripple = document.createElement('span');
        const rect   = button.getBoundingClientRect();
        const size   = Math.max(rect.width, rect.height);
        ripple.style.cssText = `
            width:${size}px; height:${size}px;
            left:${e.clientX - rect.left - size / 2}px;
            top:${e.clientY - rect.top - size / 2}px;
        `;
        ripple.classList.add('ripple');
        button.appendChild(ripple);
        setTimeout(() => ripple.remove(), 600);
    }, true);
}

/* ─────────────────────────────────────────────
   Active nav-link highlight
───────────────────────────────────────────── */
function initializeNavLinks() {
    const current = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(link => {
        if (link.getAttribute('href') === current) link.classList.add('active-page');
    });
}

/* ─────────────────────────────────────────────
   Newsletter form
───────────────────────────────────────────── */
function initializeNewsletterForm() {
    const form = document.getElementById('newsletterForm');
    if (!form) return;
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        const email = document.getElementById('newsletterEmail').value;
        if (!email) return;
        const btn  = form.querySelector('button[type="submit"]');
        const orig = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = 'დაელოდეთ...';
        setTimeout(() => {
            showToast('გამოწერა წარმატებულია!', 'success');
            btn.disabled = false;
            btn.innerHTML = orig;
            form.reset();
        }, 1000);
    });
}

/* ─────────────────────────────────────────────
   Search bar focus glow
───────────────────────────────────────────── */
function initializeSearchBar() {
    const input = document.querySelector('.navbar-search-form input[name="query"]');
    if (!input) return;
    input.addEventListener('focus', () => input.parentElement.classList.add('focused'));
    input.addEventListener('blur',  () => input.parentElement.classList.remove('focused'));
}

/* ─────────────────────────────────────────────
   ESC closes modals
───────────────────────────────────────────── */
document.addEventListener('keydown', function (e) {
    if (e.key !== 'Escape') return;
    document.querySelectorAll('.modal.show').forEach(m => {
        bootstrap.Modal.getInstance(m)?.hide();
    });
});


