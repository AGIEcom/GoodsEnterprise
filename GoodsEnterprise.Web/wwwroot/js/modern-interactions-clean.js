// Modern UI Interactions for GoodsEnterprise
(function () {
    'use strict';

    // Form submission with loading states
    function initFormSubmissions() {
        const forms = document.querySelectorAll('form');

        forms.forEach(form => {
            form.addEventListener('submit', function (e) {
                const submitBtn = form.querySelector('button[type="submit"]');
                if (submitBtn && !submitBtn.classList.contains('btn-loading')) {
                    // Add loading state
                    submitBtn.classList.add('btn-loading');
                    submitBtn.disabled = true;

                    // Store original text
                    const originalText = submitBtn.innerHTML;

                    // Reset after 10 seconds as fallback
                    setTimeout(() => {
                        submitBtn.classList.remove('btn-loading');
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }, 10000);
                }
            });
        });
    }

    // Enhanced delete confirmations
    function initDeleteConfirmations() {
        const deleteButtons = document.querySelectorAll('.btn-admin-delete, .btn-category-delete, .btn-brand-delete');

        deleteButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();

                const entityName = this.getAttribute('asp-route-firstName') ||
                    this.getAttribute('asp-route-categoryName') ||
                    this.getAttribute('asp-route-brandName') ||
                    'this item';

                showModernConfirm(
                    'Delete Confirmation',
                    `Are you sure you want to delete "${entityName}"? This action cannot be undone.`,
                    'Delete',
                    'Cancel',
                    () => {
                        // Proceed with deletion
                        const form = this.closest('form');
                        if (form) {
                            form.submit();
                        } else {
                            window.location.href = this.href;
                        }
                    }
                );
            });
        });
    }

    // Modern confirmation dialog
    function showModernConfirm(title, message, confirmText, cancelText, onConfirm) {
        // Implementation of the confirmation dialog
        // ... (keep existing implementation)
    }

    // Form validation enhancements
    function initFormValidation() {
        // Implementation of form validation
        // ... (keep existing implementation)
    }

    // DataTable utilities - resize functionality removed
    function initDataTables() {
        // DataTable resize functionality has been removed
        // Tables will use native responsive behavior instead
        
        return {
            // No resize method needed
        };
    }

    // User dropdown functionality
    function initUserDropdown() {
        const userMenuBtn = document.querySelector('.modern-user-menu-btn');
        const userDropdown = document.querySelector('.modern-user-dropdown');

        if (userMenuBtn && userDropdown) {
            userMenuBtn.addEventListener('click', function (e) {
                e.stopPropagation();
                const isExpanded = this.getAttribute('aria-expanded') === 'true';
                this.setAttribute('aria-expanded', !isExpanded);
                userDropdown.style.display = isExpanded ? 'none' : 'block';
            });

            // Close dropdown when clicking outside
            document.addEventListener('click', function () {
                userMenuBtn.setAttribute('aria-expanded', 'false');
                userDropdown.style.display = 'none';
            });
        }
    }

    // Initialize all interactions when DOM is ready
    function init() {
        initFormSubmissions();
        initDeleteConfirmations();
        initFormValidation();
        initDataTables();
        initUserDropdown();
        
        console.log('Modern interactions initialized');
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
