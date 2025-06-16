gsap.registerPlugin(ScrollTrigger);

document.addEventListener('DOMContentLoaded', () => {
    gsap.utils.toArray('.card').forEach(card => {
        gsap.from(card, {
            y: 50,
            opacity: 0,
            duration: 0.8,
            scrollTrigger: {
                trigger: card,
                start: "top bottom-=100",
                toggleActions: "play none none reverse"
            }
        });
    });

    gsap.utils.toArray('h1, h2, h3').forEach(heading => {
        gsap.from(heading, {
            y: 30,
            opacity: 0,
            duration: 0.6,
            scrollTrigger: {
                trigger: heading,
                start: "top bottom-=50",
                toggleActions: "play none none reverse"
            }
        });
    });
});

const formUtils = {
    validateForm: (form) => {
        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                isValid = false;
                input.classList.add('border-red-500');

                const errorDiv = document.createElement('div');
                errorDiv.className = 'form-error';
                errorDiv.textContent = 'Это поле обязательно для заполнения';

                const existingError = input.parentElement.querySelector('.form-error');
                if (!existingError) {
                    input.parentElement.appendChild(errorDiv);
                }
            } else {
                input.classList.remove('border-red-500');
                const errorDiv = input.parentElement.querySelector('.form-error');
                if (errorDiv) {
                    errorDiv.remove();
                }
            }
        });

        return isValid;
    },

    resetForm: (form) => {
        form.reset();
        form.querySelectorAll('.form-error').forEach(error => error.remove());
        form.querySelectorAll('.border-red-500').forEach(input => input.classList.remove('border-red-500'));
    }
};

const tableUtils = {
    sortTable: (table, column, type = 'string') => {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));
        const header = table.querySelector('th[data-sort="' + column + '"]');
        const isAsc = header.classList.contains('asc');

        table.querySelectorAll('th').forEach(th => {
            th.classList.remove('asc', 'desc');
        });

        header.classList.add(isAsc ? 'desc' : 'asc');

        rows.sort((a, b) => {
            const aValue = a.querySelector('td[data-column="' + column + '"]').textContent;
            const bValue = b.querySelector('td[data-column="' + column + '"]').textContent;

            if (type === 'number') {
                return isAsc ? bValue - aValue : aValue - bValue;
            }

            return isAsc ?
                bValue.localeCompare(aValue, 'ru') :
                aValue.localeCompare(bValue, 'ru');
        });

        rows.forEach(row => tbody.appendChild(row));
    },

    filterTable: (table, searchTerm) => {
        const rows = table.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const match = text.includes(searchTerm.toLowerCase());
            row.style.display = match ? '' : 'none';
        });
    }
};

const notificationUtils = {
    show: (message, type = 'success') => {
        const notification = document.createElement('div');
        notification.className = `fixed top-4 right-4 p-4 rounded-lg shadow-lg transform transition-all duration-300 translate-x-full`;

        switch (type) {
            case 'success':
                notification.classList.add('bg-green-500', 'text-white');
                break;
            case 'error':
                notification.classList.add('bg-red-500', 'text-white');
                break;
            case 'warning':
                notification.classList.add('bg-yellow-500', 'text-white');
                break;
            default:
                notification.classList.add('bg-blue-500', 'text-white');
        }

        notification.textContent = message;
        document.body.appendChild(notification);

        gsap.to(notification, {
            x: 0,
            duration: 0.3,
            ease: 'power2.out'
        });

        setTimeout(() => {
            gsap.to(notification, {
                x: '100%',
                duration: 0.3,
                ease: 'power2.in',
                onComplete: () => notification.remove()
            });
        }, 3000);
    }
};

window.appUtils = {
    form: formUtils,
    table: tableUtils,
    notification: notificationUtils
};
