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
                        window.location.href = this.href;
                    }
                );
            });
        });
    }

    // Modern confirmation dialog
    function showModernConfirm(title, message, confirmText, cancelText, onConfirm) {
        // Create modal backdrop
        const backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        backdrop.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 1050;
            display: flex;
            align-items: center;
            justify-content: center;
        `;

        // Create modal content
        const modal = document.createElement('div');
        modal.className = 'modern-card';
        modal.style.cssText = `
            max-width: 400px;
            margin: 0;
            animation: modalSlideIn 0.3s ease;
        `;

        modal.innerHTML = `
            <div class="modern-card-header">
                <h3 class="modern-card-title" style="margin: 0; color: #e53e3e;">
                    <svg width="24" height="24" fill="currentColor" viewBox="0 0 20 20" style="margin-right: 0.5rem;">
                        <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd"></path>
                    </svg>
                    ${title}
                </h3>
            </div>
            <p style="margin: 1rem 0; color: #4a5568;">${message}</p>
            <div style="display: flex; gap: 0.75rem; justify-content: flex-end; margin-top: 1.5rem;">
                <button class="modern-btn modern-btn-secondary cancel-btn">${cancelText}</button>
                <button class="modern-btn modern-btn-danger confirm-btn">${confirmText}</button>
            </div>
        `;

        // Add animation styles
        const style = document.createElement('style');
        style.textContent = `
            @keyframes modalSlideIn {
                from { transform: scale(0.9); opacity: 0; }
                to { transform: scale(1); opacity: 1; }
            }
        `;
        document.head.appendChild(style);

        backdrop.appendChild(modal);
        document.body.appendChild(backdrop);

        // Event listeners
        const cancelBtn = modal.querySelector('.cancel-btn');
        const confirmBtn = modal.querySelector('.confirm-btn');

        function closeModal() {
            document.body.removeChild(backdrop);
            document.head.removeChild(style);
        }

        cancelBtn.addEventListener('click', closeModal);
        backdrop.addEventListener('click', (e) => {
            if (e.target === backdrop) closeModal();
        });

        confirmBtn.addEventListener('click', () => {
            closeModal();
            onConfirm();
        });

        // Focus management
        confirmBtn.focus();
    }

    // Form validation enhancements
    function initFormValidation() {
        const inputs = document.querySelectorAll('.modern-form-input, .modern-select');

        inputs.forEach(input => {
            // Real-time validation feedback
            input.addEventListener('blur', function () {
                validateField(this);
            });

            input.addEventListener('input', function () {
                // Clear error state on input
                if (this.classList.contains('error')) {
                    this.classList.remove('error');
                    const errorMsg = this.parentNode.querySelector('.error-message');
                    if (errorMsg) errorMsg.remove();
                }
            });
        });
    }

    function validateField(field) {
        const value = field.value.trim();
        const isRequired = field.hasAttribute('required');
        const fieldType = field.type;

        // Remove existing error
        field.classList.remove('error');
        const existingError = field.parentNode.querySelector('.error-message');
        if (existingError) existingError.remove();

        let errorMessage = '';

        // Required field validation
        if (isRequired && !value) {
            errorMessage = 'This field is required.';
        }
        // Email validation
        else if (fieldType === 'email' && value && !isValidEmail(value)) {
            errorMessage = 'Please enter a valid email address.';
        }
        // Password validation
        else if (fieldType === 'password' && value && value.length < 6) {
            errorMessage = 'Password must be at least 6 characters long.';
        }

        if (errorMessage) {
            field.classList.add('error');
            const errorDiv = document.createElement('div');
            errorDiv.className = 'error-message';
            errorDiv.style.cssText = 'color: #e53e3e; font-size: 0.8rem; margin-top: 0.25rem;';
            errorDiv.textContent = errorMessage;
            field.parentNode.appendChild(errorDiv);
        }
    }

    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Add error styles to CSS
    function addValidationStyles() {
        const style = document.createElement('style');
        style.textContent = `
            .modern-form-input.error,
            .modern-select.error {
                border-color: #e53e3e !important;
                box-shadow: 0 0 0 3px rgba(229, 62, 62, 0.1) !important;
            }
        `;
        document.head.appendChild(style);
    }

    // Auto-hide success alerts
    function initAutoHideAlerts() {
        const successAlerts = document.querySelectorAll('.modern-alert-success');

        successAlerts.forEach(alert => {
            if (alert.style.display !== 'none') {
                setTimeout(() => {
                    alert.style.opacity = '0';
                    alert.style.transform = 'translateY(-10px)';
                    setTimeout(() => {
                        alert.style.display = 'none';
                    }, 300);
                }, 5000);
            }
        });
    }

    // Smooth transitions for dynamic content
    function initSmoothTransitions() {
        const style = document.createElement('style');
        style.textContent = `
            .modern-alert {
                transition: all 0.3s ease;
            }
            
            .modern-card {
                transition: all 0.3s ease;
            }
            
            .fade-in {
                animation: fadeIn 0.3s ease;
            }
            
            @keyframes fadeIn {
                from { opacity: 0; transform: translateY(10px); }
                to { opacity: 1; transform: translateY(0); }
            }
        `;
        document.head.appendChild(style);
    }

    // Keyboard navigation enhancements
    function initKeyboardNavigation() {
        document.addEventListener('keydown', function (e) {
            // Escape key to close modals
            if (e.key === 'Escape') {
                const backdrop = document.querySelector('.modal-backdrop');
                if (backdrop) {
                    backdrop.click();
                }
            }

            // Enter key on buttons
            if (e.key === 'Enter' && e.target.classList.contains('modern-btn')) {
                e.target.click();
            }
        });
    }

    // Sidebar and Navigation functionality
    function initSidebarNavigation() {
        const sidebar = document.getElementById('sidebar');
        const sidebarToggle = document.getElementById('sidebarCollapse');
        const menuLinks = document.querySelectorAll('.menu-link');
        const pageTitle = document.getElementById('currentPageTitle');

        // Sidebar toggle functionality
        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function () {
                sidebar.classList.toggle('sidebar-collapsed');
                this.classList.toggle('active');

                // Also toggle content areas
                const content = document.getElementById('content');
                const contentHeader = document.getElementById('contentHeader');
                if (content) {
                    content.classList.toggle('sidebar-collapsed');
                }
                if (contentHeader) {
                    contentHeader.classList.toggle('sidebar-collapsed');
                }
            });
        }

        // Set active menu item based on current page with delay to ensure menu is loaded
        setTimeout(function() {
            setActiveMenuItem();
        }, 800);


        function setActiveMenuItem() {
            const currentPath = window.location.pathname;
            const url = currentPath.split('/').pop() || currentPath;

            // Remove active class from all menu items
            menuLinks.forEach(link => link.classList.remove('active'));

            // Handle special cases for submenu items (Product submenu)
            if (url === 'all-promotion-cost' || url === 'all-product' || url === 'all-base-cost') {
                // Find and activate parent Product menu
                const productMenu = document.querySelector('.menu-link[data-toggle="collapse"]');
                if (productMenu) {
                    productMenu.classList.add('active');
                    productMenu.setAttribute('aria-expanded', 'true');

                    // Find submenu - check for both .submenu and .collapse classes
                    const submenu = productMenu.parentElement.querySelector('.submenu, .collapse');
                    if (submenu) {
                        submenu.classList.add('show');
                        submenu.style.display = 'block';
                    }

                    // Also activate the specific submenu item
                    const submenuLink = document.querySelector(`.submenu-link[href*="${url}"]`);
                    if (submenuLink) {
                        submenuLink.classList.add('active');
                    }
                }
                return; // Exit early for submenu items
            }

            // Handle regular menu items - first try direct URL match
            let matchFound = false;
            menuLinks.forEach(link => {
                const href = link.getAttribute('href');
                if (href && href.includes(url) && !link.hasAttribute('data-toggle')) {
                    link.classList.add('active');
                    matchFound = true;
                }
            });

            // If no direct match found, try pattern matching
            if (!matchFound) {
                // Special handling for UploadDownload page (default after login)
                if (url.toLowerCase() === 'uploaddownload' || url.toLowerCase() === '') {
                    const importMenu = Array.from(menuLinks).find(link => {
                        const menuText = link.querySelector('.menu-text');
                        return menuText && menuText.textContent.trim() === 'Import';
                    });
                    if (importMenu) {
                        importMenu.classList.add('active');
                        matchFound = true;
                    }
                }

                if (!matchFound) {
                    var pagePatterns = {
                        'Brand': ['brand', 'all-brand'],
                        'Category': ['category', 'all-category'],
                        'SubCategory': ['subcategory', 'all-subcategory'],
                        'Product': ['product', 'all-product'],
                        'Tax': ['tax', 'all-tax'],
                        'Supplier': ['supplier', 'all-supplier'],
                        'Role': ['role', 'all-role'],
                        'Admin': ['admin', 'all-admin'],
                        'Customer': ['customer', 'all-customer'],
                        'Import': ['uploaddownload', 'upload', 'import'],
                        'Export': ['download', 'export']
                    };

                    for (const [menuName, patterns] of Object.entries(pagePatterns)) {
                        for (const pattern of patterns) {
                            if (url.toLowerCase().includes(pattern)) {
                                // Find menu link by text content
                                const menuLink = Array.from(menuLinks).find(link => {
                                    const menuText = link.querySelector('.menu-text');
                                    return menuText && menuText.textContent.trim() === menuName;
                                });
                                if (menuLink) {
                                    menuLink.classList.add('active');
                                    matchFound = true;
                                    break;
                                }
                            }
                        }
                        if (matchFound) break;
                    }
                }
            }

            // Update page title based on active menu
            const activeMenu = document.querySelector('.menu-link.active .menu-text');
            if (activeMenu && pageTitle) {
                pageTitle.textContent = activeMenu.textContent;
            }
            const submenuLink = document.querySelector(`.submenu-link[href*="${url}"]`);
            if (submenuLink && activeMenu.textContent=="") { 
                pageTitle.textContent = submenuLink.textContent;
            }
        }

        // Handle menu link clicks - separate handlers for regular links and submenu toggles
        menuLinks.forEach(link => {
            if (link.hasAttribute('data-toggle') && link.getAttribute('data-toggle') === 'collapse') {
                // Handle submenu toggle links
                link.addEventListener('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    const isExpanded = this.getAttribute('aria-expanded') === 'true';
                    this.setAttribute('aria-expanded', !isExpanded);
                    
                    const submenu = this.parentElement.querySelector('.submenu, .collapse');
                    if (submenu) {
                        if (isExpanded) {
                            submenu.classList.remove('show');
                            submenu.style.display = 'none';
                        } else {
                            submenu.classList.add('show');
                            submenu.style.display = 'block';
                        }
                    }
                    
                    // Update active state for parent menu
                    menuLinks.forEach(l => {
                        if (!l.hasAttribute('data-toggle')) {
                            l.classList.remove('active');
                        }
                    });
                    this.classList.add('active');
                    
                    // Update page title
                    if (pageTitle) {
                        const menuText = this.querySelector('.menu-text').textContent;
                        pageTitle.textContent = menuText;
                    }
                });
            } else {
                // Handle regular navigation links
                link.addEventListener('click', function (e) {
                    // Remove active class from all links
                    menuLinks.forEach(l => l.classList.remove('active'));
                    // Add active class to clicked link
                    this.classList.add('active');

                    // Update page title
                    if (pageTitle) {
                        const menuText = this.querySelector('.menu-text').textContent;
                        pageTitle.textContent = menuText;
                    }
                });
            }
        });

        // Close sidebar on mobile when clicking outside
        document.addEventListener('click', function (e) {
            if (window.innerWidth <= 768) {
                if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                    sidebar.classList.add('sidebar-collapsed');
                    sidebarToggle.classList.remove('active');
                    const content = document.getElementById('content');
                    const contentHeader = document.getElementById('contentHeader');
                    if (content) {
                        content.classList.add('sidebar-collapsed');
                    }
                    if (contentHeader) {
                        contentHeader.classList.add('sidebar-collapsed');
                    }
                }
            }
        });

        // Handle window resize
        window.addEventListener('resize', function () {
            if (window.innerWidth > 768) {
                sidebar.classList.remove('sidebar-collapsed');
                sidebarToggle.classList.remove('active');
                const content = document.getElementById('content');
                const contentHeader = document.getElementById('contentHeader');
                if (content) {
                    content.classList.remove('sidebar-collapsed');
                }
                if (contentHeader) {
                    contentHeader.classList.remove('sidebar-collapsed');
                }
            }
        });
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
        addValidationStyles();
        initAutoHideAlerts();
        initSmoothTransitions();
        initKeyboardNavigation();
        initSidebarNavigation();
        initUserDropdown();

        // Add fade-in animation to cards
        document.querySelectorAll('.modern-card').forEach(card => {
            card.classList.add('fade-in');
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
