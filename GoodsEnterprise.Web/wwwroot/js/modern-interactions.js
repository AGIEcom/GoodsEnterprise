// Modern UI Interactions for GoodsEnterprise
(function () {
    'use strict';

    // Form submission with loading states
    function initFormSubmissions() {
        const forms = document.querySelectorAll('form');

        forms.forEach(form => {
            form.addEventListener('submit', function (e) {
                // Handle password field submission for edit mode
                handlePasswordFieldSubmission(form);
                
                // Validate passwords before submission
                if (!validatePasswordsOnSubmit(form)) {
                    e.preventDefault();
                    return false;
                }
                
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

    function handlePasswordFieldSubmission(form) {
        // Handle Customer form
        const customerPassword = form.querySelector('#txtCustomerPassword');
        const customerConfirmPassword = form.querySelector('#txtCustomerConfirmPassword');
        const customerChangeToggle = form.querySelector('#chkChangeCustomerPassword');
        const customerIdField = form.querySelector('#hdnCustomerID');
        
        if (customerPassword && customerChangeToggle && customerIdField) {
            const isEditMode = customerIdField.value && customerIdField.value > 0;
            const isPasswordChangeEnabled = customerChangeToggle.checked;
            
            if (isEditMode && !isPasswordChangeEnabled) {
                // Remove name attribute to prevent null submission
                customerPassword.removeAttribute('name');
                if (customerConfirmPassword) customerConfirmPassword.removeAttribute('name');
            } else {
                // Ensure name attribute is present for submission
                customerPassword.setAttribute('name', 'objCustomer.Password');
            }
        }
        
        // Handle Admin form
        const adminPassword = form.querySelector('#txtAdminPassword');
        const adminConfirmPassword = form.querySelector('#txtAdminConfirmPassword');
        const adminChangeToggle = form.querySelector('#chkChangePassword');
        const adminIdField = form.querySelector('#hdnAdminID');
        
        if (adminPassword && adminChangeToggle && adminIdField) {
            const isEditMode = adminIdField.value && adminIdField.value > 0;
            const isPasswordChangeEnabled = adminChangeToggle.checked;
            
            if (isEditMode && !isPasswordChangeEnabled) {
                // Remove name attribute to prevent null submission
                adminPassword.removeAttribute('name');
                if (adminConfirmPassword) adminConfirmPassword.removeAttribute('name');
            } else {
                // Ensure name attribute is present for submission
                adminPassword.setAttribute('name', 'objAdmin.Password');
            }
        }
    }

    function validatePasswordsOnSubmit(form) {
        let isValid = true;
        
        // Check Customer form
        const customerPassword = form.querySelector('#txtCustomerPassword');
        const customerConfirmPassword = form.querySelector('#txtCustomerConfirmPassword');
        const customerChangeToggle = form.querySelector('#chkChangeCustomerPassword');
        
        if (customerPassword && customerConfirmPassword) {
            // Only validate if password fields are visible and required
            const isPasswordRequired = customerPassword.hasAttribute('required');
            const isEditModeWithPasswordChange = customerChangeToggle && customerChangeToggle.checked;
            
            if (isPasswordRequired || isEditModeWithPasswordChange) {
                if (customerPassword.value !== customerConfirmPassword.value) {
                    customerConfirmPassword.classList.add('error');
                    ModernAlert.error('Passwords do not match. Please check your password and confirm password fields.', 'Validation Error');
                    customerConfirmPassword.focus();
                    isValid = false;
                }
                
                if (customerPassword.value.length > 0 && customerPassword.value.length < 6) {
                    customerPassword.classList.add('error');
                    ModernAlert.error('Password must be at least 6 characters long.', 'Validation Error');
                    customerPassword.focus();
                    isValid = false;
                }
                
                if (isPasswordRequired && customerPassword.value.length === 0) {
                    customerPassword.classList.add('error');
                    ModernAlert.error('Password is required.', 'Validation Error');
                    customerPassword.focus();
                    isValid = false;
                }
            }
        }
        
        // Check Admin form
        const adminPassword = form.querySelector('#txtAdminPassword');
        const adminConfirmPassword = form.querySelector('#txtAdminConfirmPassword');
        const adminChangeToggle = form.querySelector('#chkChangePassword');
        
        if (adminPassword && adminConfirmPassword) {
            // Only validate if password fields are visible and required
            const isPasswordRequired = adminPassword.hasAttribute('required');
            const isEditModeWithPasswordChange = adminChangeToggle && adminChangeToggle.checked;
            
            if (isPasswordRequired || isEditModeWithPasswordChange) {
                if (adminPassword.value !== adminConfirmPassword.value) {
                    adminConfirmPassword.classList.add('error');
                    ModernAlert.error('Passwords do not match. Please check your password and confirm password fields.', 'Validation Error');
                    adminConfirmPassword.focus();
                    isValid = false;
                }
                
                if (adminPassword.value.length > 0 && adminPassword.value.length < 6) {
                    adminPassword.classList.add('error');
                    ModernAlert.error('Password must be at least 6 characters long.', 'Validation Error');
                    adminPassword.focus();
                    isValid = false;
                }
                
                if (isPasswordRequired && adminPassword.value.length === 0) {
                    adminPassword.classList.add('error');
                    ModernAlert.error('Password is required.', 'Validation Error');
                    adminPassword.focus();
                    isValid = false;
                }
            }
        }
        
        return isValid;
    }

    // Enhanced delete confirmations
    function initDeleteConfirmations() {
        const deleteButtons = document.querySelectorAll('.btn-admin-delete, .btn-brand-delete,  .btn-category-delete, .btn-customer-delete, .btn-product-delete, .btn-PromotionCost-delete, .btn-role-delete, .btn-subCategory-delete, .btn-supplier-delete, .btn-tax-delete');

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

    // Make showModernConfirm globally available
    window.showModernConfirm = showModernConfirm;

    // Modern Alert System
    window.ModernAlert = {
        container: null,
        
        init: function() {
            if (!this.container) {
                this.container = document.createElement('div');
                this.container.className = 'modern-alert-container';
                document.body.appendChild(this.container);
            }
        },
        
        show: function(options) {
            this.init();
            
            const {
                type = 'info',
                title = '',
                message = '',
                duration = 4000,
                closable = true
            } = options;
            
            const alertId = 'alert-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
            
            const alertElement = document.createElement('div');
            alertElement.className = `modern-alert-notification ${type}`;
            alertElement.id = alertId;
            
            const icon = this.getIcon(type);
            
            alertElement.innerHTML = `
                <div class="alert-icon">
                    ${icon}
                </div>
                <div class="alert-content">
                    ${title ? `<div class="alert-title">${title}</div>` : ''}
                    <div class="alert-message">${message}</div>
                </div>
                ${closable ? `
                    <button class="alert-close" onclick="ModernAlert.close('${alertId}')">
                        <svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                            <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                        </svg>
                    </button>
                ` : ''}
            `;
            
            this.container.appendChild(alertElement);
            
            // Trigger animation
            setTimeout(() => {
                alertElement.classList.add('show', 'bounce');
            }, 10);
            
            // Auto close
            if (duration > 0) {
                setTimeout(() => {
                    this.close(alertId);
                }, duration);
            }
            
            return alertId;
        },
        
        close: function(alertId) {
            const alertElement = document.getElementById(alertId);
            if (alertElement) {
                alertElement.classList.add('hide');
                alertElement.classList.remove('show');
                
                setTimeout(() => {
                    if (alertElement.parentNode) {
                        alertElement.parentNode.removeChild(alertElement);
                    }
                }, 400);
            }
        },
        
        success: function(message, title = 'Success!', options = {}) {
            return this.show({
                type: 'success',
                title: title,
                message: message,
                ...options
            });
        },
        
        error: function(message, title = 'Error!', options = {}) {
            return this.show({
                type: 'error',
                title: title,
                message: message,
                ...options
            });
        },
        
        warning: function(message, title = 'Warning!', options = {}) {
            return this.show({
                type: 'warning',
                title: title,
                message: message,
                ...options
            });
        },
        
        info: function(message, title = 'Info', options = {}) {
            return this.show({
                type: 'info',
                title: title,
                message: message,
                ...options
            });
        },
        
        getIcon: function(type) {
            const icons = {
                success: `<svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M10.97 4.97a.235.235 0 0 0-.02.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05z"/>
                </svg>`,
                error: `<svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                </svg>`,
                warning: `<svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
                </svg>`,
                info: `<svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
                    <path d="m8.93 6.588-2.29.287-.082.38.45.083c.294.07.352.176.288.469l-.738 3.468c-.194.897.105 1.319.808 1.319.545 0 1.178-.252 1.465-.598l.088-.416c-.2.176-.492.246-.686.246-.275 0-.375-.193-.304-.533L8.93 6.588zM9 4.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0z"/>
                </svg>`
            };
            return icons[type] || icons.info;
        }
    };

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
                
                // Handle password strength and matching
                if (this.type === 'password') {
                    handlePasswordValidation(this);
                }
            });
        });
        
        // Initialize password validation
        initPasswordValidation();
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

    // Password validation functions
    function initPasswordValidation() {
        // Initialize password change toggles for edit mode
        initPasswordChangeToggles();
        
        // Customer password validation
        const customerPassword = document.getElementById('txtCustomerPassword');
        const customerConfirmPassword = document.getElementById('txtCustomerConfirmPassword');
        
        if (customerPassword && customerConfirmPassword) {
            customerPassword.addEventListener('input', () => handlePasswordValidation(customerPassword));
            customerConfirmPassword.addEventListener('input', () => handlePasswordMatching('customer'));
            customerPassword.addEventListener('blur', () => handlePasswordMatching('customer'));
            customerConfirmPassword.addEventListener('blur', () => handlePasswordMatching('customer'));
        }
        
        // Admin password validation
        const adminPassword = document.getElementById('txtAdminPassword');
        const adminConfirmPassword = document.getElementById('txtAdminConfirmPassword');
        
        if (adminPassword && adminConfirmPassword) {
            adminPassword.addEventListener('input', () => handlePasswordValidation(adminPassword));
            adminConfirmPassword.addEventListener('input', () => handlePasswordMatching('admin'));
            adminPassword.addEventListener('blur', () => handlePasswordMatching('admin'));
            adminConfirmPassword.addEventListener('blur', () => handlePasswordMatching('admin'));
        }
    }

    function initPasswordChangeToggles() {
        // Admin password change toggle
        const adminChangeToggle = document.getElementById('chkChangePassword');
        const adminPasswordSection = document.getElementById('passwordChangeSection');
        
        if (adminChangeToggle && adminPasswordSection) {
            adminChangeToggle.addEventListener('change', function() {
                const adminPassword = document.getElementById('txtAdminPassword');
                const adminConfirmPassword = document.getElementById('txtAdminConfirmPassword');
                
                if (this.checked) {
                    adminPasswordSection.style.display = 'block';
                    if (adminPassword) adminPassword.setAttribute('required', 'required');
                    if (adminConfirmPassword) adminConfirmPassword.setAttribute('required', 'required');
                } else {
                    adminPasswordSection.style.display = 'none';
                    if (adminPassword) {
                        adminPassword.removeAttribute('required');
                        adminPassword.value = '';
                    }
                    if (adminConfirmPassword) {
                        adminConfirmPassword.removeAttribute('required');
                        adminConfirmPassword.value = '';
                    }
                    // Clear validation indicators
                    const strengthIndicator = document.getElementById('adminPasswordStrength');
                    const matchIndicator = document.getElementById('adminPasswordMatch');
                    if (strengthIndicator) strengthIndicator.innerHTML = '';
                    if (matchIndicator) matchIndicator.innerHTML = '';
                }
            });
        }
        
        // Customer password change toggle
        const customerChangeToggle = document.getElementById('chkChangeCustomerPassword');
        const customerPasswordSection = document.getElementById('customerPasswordChangeSection');
        
        if (customerChangeToggle && customerPasswordSection) {
            customerChangeToggle.addEventListener('change', function() {
                const customerPassword = document.getElementById('txtCustomerPassword');
                const customerConfirmPassword = document.getElementById('txtCustomerConfirmPassword');
                
                if (this.checked) {
                    customerPasswordSection.style.display = 'block';
                    if (customerPassword) customerPassword.setAttribute('required', 'required');
                    if (customerConfirmPassword) customerConfirmPassword.setAttribute('required', 'required');
                } else {
                    customerPasswordSection.style.display = 'none';
                    if (customerPassword) {
                        customerPassword.removeAttribute('required');
                        customerPassword.value = '';
                    }
                    if (customerConfirmPassword) {
                        customerConfirmPassword.removeAttribute('required');
                        customerConfirmPassword.value = '';
                    }
                    // Clear validation indicators
                    const strengthIndicator = document.getElementById('customerPasswordStrength');
                    const matchIndicator = document.getElementById('customerPasswordMatch');
                    if (strengthIndicator) strengthIndicator.innerHTML = '';
                    if (matchIndicator) matchIndicator.innerHTML = '';
                }
            });
        }
    }

    function handlePasswordValidation(passwordField) {
        const password = passwordField.value;
        const fieldId = passwordField.id;
        let strengthIndicatorId = '';
        
        if (fieldId.includes('Customer')) {
            strengthIndicatorId = 'customerPasswordStrength';
        } else if (fieldId.includes('Admin')) {
            strengthIndicatorId = 'adminPasswordStrength';
        }
        
        const strengthIndicator = document.getElementById(strengthIndicatorId);
        if (!strengthIndicator) return;
        
        const strength = calculatePasswordStrength(password);
        updatePasswordStrengthIndicator(strengthIndicator, strength, password.length);
        
        // Also check password matching when password changes
        if (fieldId.includes('Customer')) {
            handlePasswordMatching('customer');
        } else if (fieldId.includes('Admin')) {
            handlePasswordMatching('admin');
        }
    }

    function calculatePasswordStrength(password) {
        let score = 0;
        let feedback = [];
        
        if (password.length === 0) {
            return { score: 0, level: 'none', feedback: [] };
        }
        
        // Length check
        if (password.length >= 8) {
            score += 2;
        } else if (password.length >= 6) {
            score += 1;
            feedback.push('Use at least 8 characters');
        } else {
            feedback.push('Too short (minimum 6 characters)');
        }
        
        // Character variety checks
        if (/[a-z]/.test(password)) score += 1;
        if (/[A-Z]/.test(password)) score += 1;
        if (/[0-9]/.test(password)) score += 1;
        if (/[^A-Za-z0-9]/.test(password)) score += 2;
        
        // Bonus for good length
        if (password.length >= 12) score += 1;
        
        // Determine strength level
        let level = 'weak';
        if (score >= 7) level = 'strong';
        else if (score >= 5) level = 'medium';
        else if (score >= 3) level = 'fair';
        
        // Add feedback for missing elements
        if (!/[a-z]/.test(password)) feedback.push('Add lowercase letters');
        if (!/[A-Z]/.test(password)) feedback.push('Add uppercase letters');
        if (!/[0-9]/.test(password)) feedback.push('Add numbers');
        if (!/[^A-Za-z0-9]/.test(password)) feedback.push('Add special characters');
        
        return { score, level, feedback };
    }

    function updatePasswordStrengthIndicator(indicator, strength, passwordLength) {
        if (passwordLength === 0) {
            indicator.innerHTML = '';
            return;
        }
        
        const colors = {
            weak: '#e53e3e',
            fair: '#dd6b20',
            medium: '#d69e2e',
            strong: '#38a169'
        };
        
        const widths = {
            weak: '25%',
            fair: '50%',
            medium: '75%',
            strong: '100%'
        };
        
        indicator.innerHTML = `
            <div class="password-strength-bar">
                <div class="strength-bar-fill" style="width: ${widths[strength.level]}; background-color: ${colors[strength.level]};"></div>
            </div>
            <div class="strength-text" style="color: ${colors[strength.level]}; font-size: 0.8rem; margin-top: 0.25rem;">
                Password strength: ${strength.level.charAt(0).toUpperCase() + strength.level.slice(1)}
                ${strength.feedback.length > 0 ? '<br><small style="color: #666;">' + strength.feedback.slice(0, 2).join(', ') + '</small>' : ''}
            </div>
        `;
    }

    function handlePasswordMatching(type) {
        let passwordField, confirmPasswordField, matchIndicator;
        
        if (type === 'customer') {
            passwordField = document.getElementById('txtCustomerPassword');
            confirmPasswordField = document.getElementById('txtCustomerConfirmPassword');
            matchIndicator = document.getElementById('customerPasswordMatch');
        } else if (type === 'admin') {
            passwordField = document.getElementById('txtAdminPassword');
            confirmPasswordField = document.getElementById('txtAdminConfirmPassword');
            matchIndicator = document.getElementById('adminPasswordMatch');
        }
        
        if (!passwordField || !confirmPasswordField || !matchIndicator) return;
        
        const password = passwordField.value;
        const confirmPassword = confirmPasswordField.value;
        
        // Clear previous validation state
        confirmPasswordField.classList.remove('error', 'success');
        
        if (confirmPassword.length === 0) {
            matchIndicator.innerHTML = '';
            return;
        }
        
        if (password === confirmPassword) {
            confirmPasswordField.classList.add('success');
            matchIndicator.innerHTML = `
                <div style="color: #38a169; font-size: 0.8rem; margin-top: 0.25rem;">
                    <svg width="14" height="14" fill="currentColor" viewBox="0 0 20 20" style="margin-right: 0.25rem;">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Passwords match
                </div>
            `;
        } else {
            confirmPasswordField.classList.add('error');
            matchIndicator.innerHTML = `
                <div style="color: #e53e3e; font-size: 0.8rem; margin-top: 0.25rem;">
                    <svg width="14" height="14" fill="currentColor" viewBox="0 0 20 20" style="margin-right: 0.25rem;">
                        <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path>
                    </svg>
                    Passwords do not match
                </div>
            `;
        }
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
            
            .modern-form-input.success {
                border-color: #38a169 !important;
                box-shadow: 0 0 0 3px rgba(56, 161, 105, 0.1) !important;
            }
            
            .password-strength-bar {
                width: 100%;
                height: 4px;
                background-color: #e2e8f0;
                border-radius: 2px;
                margin-top: 0.5rem;
                overflow: hidden;
            }
            
            .strength-bar-fill {
                height: 100%;
                border-radius: 2px;
                transition: all 0.3s ease;
            }
            
            .password-strength-indicator,
            .password-match-indicator {
                margin-top: 0.25rem;
            }
            
            .strength-text {
                font-weight: 500;
            }
            
            /* Excel Import Modal Styles */
            .modal-backdrop {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.5);
                z-index: 1050;
                display: flex;
                align-items: center;
                justify-content: center;
            }
            
            .file-upload-container {
                position: relative;
            }
            
            .excel-columns-info {
                background-color: #f8f9fa;
                padding: 1rem;
                border-radius: 0.375rem;
                border: 1px solid #e9ecef;
            }
            
            .progress-container {
                margin-top: 0.5rem;
            }
            
            .progress-bar {
                width: 100%;
                height: 20px;
                background-color: #e9ecef;
                border-radius: 10px;
                overflow: hidden;
                position: relative;
            }
            
            .progress-fill {
                height: 100%;
                background: linear-gradient(90deg, #28a745, #20c997);
                border-radius: 10px;
                transition: width 0.3s ease;
                position: relative;
            }
            
            .progress-text {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-top: 0.5rem;
                font-size: 0.9rem;
                color: #6c757d;
            }
            
            .import-status {
                margin-top: 1rem;
            }
            
            .alert {
                padding: 0.75rem 1rem;
                border-radius: 0.375rem;
                border: 1px solid transparent;
            }
            
            .alert-success {
                color: #155724;
                background-color: #d4edda;
                border-color: #c3e6cb;
            }
            
            .alert-danger {
                color: #721c24;
                background-color: #f8d7da;
                border-color: #f5c6cb;
            }
            
            .modern-btn-info {
                background: linear-gradient(135deg, #17a2b8, #138496);
                color: white;
                border: none;
            }
            
            .modern-btn-info:hover {
                background: linear-gradient(135deg, #138496, #117a8b);
                transform: translateY(-1px);
            }
            
            .modern-btn-success {
                background: linear-gradient(135deg, #28a745, #20c997);
                color: white;
                border: none;
            }
            
            .modern-btn-success:hover {
                background: linear-gradient(135deg, #218838, #1e7e34);
                transform: translateY(-1px);
            }
            
            /* Import Results Styling */
            .import-results-summary {
                margin-top: 1rem;
                padding: 1rem;
                background-color: #f8f9fa;
                border-radius: 0.375rem;
                border: 1px solid #e9ecef;
            }
            
            .results-cards {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
                gap: 1rem;
                margin-bottom: 1rem;
            }
            
            .result-card {
                background: white;
                border-radius: 0.375rem;
                padding: 1rem;
                text-align: center;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                border: 2px solid transparent;
                transition: all 0.3s ease;
            }
            
            .success-card {
                border-color: #28a745;
                background: linear-gradient(135deg, #d4edda, #c3e6cb);
            }
            
            .error-card {
                border-color: #dc3545;
                background: linear-gradient(135deg, #f8d7da, #f5c6cb);
            }
            
            .total-card {
                border-color: #17a2b8;
                background: linear-gradient(135deg, #d1ecf1, #bee5eb);
            }
            
            .result-icon {
                margin-bottom: 0.5rem;
            }
            
            .success-card .result-icon {
                color: #155724;
            }
            
            .error-card .result-icon {
                color: #721c24;
            }
            
            .total-card .result-icon {
                color: #0c5460;
            }
            
            .result-number {
                font-size: 2rem;
                font-weight: bold;
                line-height: 1;
                margin-bottom: 0.25rem;
            }
            
            .success-card .result-number {
                color: #155724;
            }
            
            .error-card .result-number {
                color: #721c24;
            }
            
            .total-card .result-number {
                color: #0c5460;
            }
            
            .result-label {
                font-size: 0.875rem;
                font-weight: 500;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                opacity: 0.8;
            }
            
            .import-results-details {
                margin-top: 1rem;
            }
            
            .results-tabs {
                display: flex;
                border-bottom: 2px solid #e9ecef;
                margin-bottom: 1rem;
            }
            
            .tab-button {
                background: none;
                border: none;
                padding: 0.75rem 1rem;
                cursor: pointer;
                border-bottom: 2px solid transparent;
                transition: all 0.3s ease;
                display: flex;
                align-items: center;
                gap: 0.5rem;
                font-weight: 500;
                color: #6c757d;
            }
            
            .tab-button:hover {
                color: #495057;
                background-color: #f8f9fa;
            }
            
            .tab-button.active {
                color: #007bff;
                border-bottom-color: #007bff;
                background-color: #e3f2fd;
            }
            
            .tab-content {
                min-height: 200px;
            }
            
            .tab-pane {
                display: none;
            }
            
            .tab-pane.active {
                display: block;
            }
            
            .results-table-container {
                max-height: 400px;
                overflow-y: auto;
                border: 1px solid #e9ecef;
                border-radius: 0.375rem;
            }
            
            .results-table {
                width: 100%;
                border-collapse: collapse;
                font-size: 0.875rem;
            }
            
            .results-table th {
                background-color: #f8f9fa;
                padding: 0.75rem;
                text-align: left;
                font-weight: 600;
                border-bottom: 2px solid #e9ecef;
                position: sticky;
                top: 0;
                z-index: 10;
            }
            
            .results-table td {
                padding: 0.5rem 0.75rem;
                border-bottom: 1px solid #e9ecef;
                vertical-align: top;
            }
            
            .results-table tbody tr:hover {
                background-color: #f8f9fa;
            }
            
            .status-success {
                color: #155724;
                font-weight: 500;
            }
            
            .status-error {
                color: #721c24;
                font-weight: 500;
            }
            
            .error-message {
                color: #721c24;
                font-size: 0.8rem;
                max-width: 300px;
                word-wrap: break-word;
            }
            
            /* Enhanced Modal Scrolling */
            .modal-backdrop {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.5);
                display: flex;
                align-items: flex-start;
                justify-content: center;
                padding: 20px;
                box-sizing: border-box;
                overflow-y: auto;
            }
            
            .modal-backdrop .modern-card {
                background: white;
                border-radius: 0.5rem;
                box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
                padding: 1.5rem;
                position: relative;
                width: 100%;
                box-sizing: border-box;
            }
            
            /* Custom scrollbar for modal */
            .modal-backdrop .modern-card::-webkit-scrollbar {
                width: 8px;
            }
            
            .modal-backdrop .modern-card::-webkit-scrollbar-track {
                background: #f1f1f1;
                border-radius: 4px;
            }
            
            .modal-backdrop .modern-card::-webkit-scrollbar-thumb {
                background: #c1c1c1;
                border-radius: 4px;
            }
            
            .modal-backdrop .modern-card::-webkit-scrollbar-thumb:hover {
                background: #a8a8a8;
            }
            
            /* Responsive modal sizing */
            @media (max-width: 768px) {
                .modal-backdrop .modern-card {
                    max-width: 95vw;
                    max-height: 95vh;
                    margin: 10px auto;
                }
            }
            
            /* Modern Loading Spinner */
            .loading-container {
                display: flex;
                flex-direction: column;
                align-items: center;
                padding: 2rem 1rem;
                text-align: center;
            }
            
            .loading-spinner {
                position: relative;
                width: 60px;
                height: 60px;
                margin-bottom: 1.5rem;
            }
            
            .spinner-ring {
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                border: 3px solid transparent;
                border-top: 3px solid #007bff;
                border-radius: 50%;
                animation: spin 1.2s linear infinite;
            }
            
            .spinner-ring:nth-child(1) {
                animation-delay: 0s;
                border-top-color: #007bff;
            }
            
            .spinner-ring:nth-child(2) {
                animation-delay: 0.3s;
                border-top-color: #28a745;
                transform: scale(0.8);
            }
            
            .spinner-ring:nth-child(3) {
                animation-delay: 0.6s;
                border-top-color: #ffc107;
                transform: scale(0.6);
            }
            
            .spinner-ring:nth-child(4) {
                animation-delay: 0.9s;
                border-top-color: #dc3545;
                transform: scale(0.4);
            }
            
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
            
            .loading-text {
                color: #495057;
            }
            
            #loadingMessage {
                font-size: 1.1rem;
                font-weight: 500;
                margin-bottom: 0.5rem;
                color: #007bff;
            }
            
            .loading-subtext {
                font-size: 0.9rem;
                color: #6c757d;
                font-style: italic;
            }
            
            /* Enhanced DataTable Controls */
            .modern-datatable-container {
                background: white;
                border-radius: 0.5rem;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                overflow: hidden;
            }
            
            .datatable-header-controls {
                background: #f8f9fa;
                padding: 1rem;
                border-bottom: 1px solid #dee2e6;
            }
            
            .filter-group {
                display: flex;
                flex-direction: column;
                gap: 0.25rem;
            }
            
            .filter-label {
                font-size: 0.875rem;
                font-weight: 500;
                color: #495057;
                display: flex;
                align-items: center;
                gap: 0.5rem;
                margin-bottom: 0;
            }
            
            .modern-select-enhanced {
                padding: 0.5rem 0.75rem;
                border: 1px solid #ced4da;
                border-radius: 0.375rem;
                background-color: white;
                font-size: 0.875rem;
                transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
                min-width: 150px;
            }
            
            .modern-select-enhanced:focus {
                border-color: #007bff;
                outline: 0;
                box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
            }
            
            .modern-search-input {
                padding: 0.5rem 0.75rem;
                border: 1px solid #ced4da;
                border-radius: 0.375rem;
                background-color: white;
                font-size: 0.875rem;
                transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
                min-width: 200px;
            }
            
            .modern-search-input:focus {
                border-color: #007bff;
                outline: 0;
                box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
            }
            
            .modern-search-input::placeholder {
                color: #6c757d;
                font-style: italic;
            }
            
            /* DataTable Processing Indicator */
            .dataTables_processing {
                position: absolute;
                top: 50%;
                left: 50%;
                width: 200px;
                margin-left: -100px;
                margin-top: -26px;
                text-align: center;
                padding: 1rem;
                background: rgba(255, 255, 255, 0.9);
                border: 1px solid #ddd;
                border-radius: 0.375rem;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                font-weight: 500;
                color: #007bff;
            }
            
            /* DataTable Pagination Styling */
            .dataTables_wrapper .dataTables_paginate .paginate_button {
                padding: 0.375rem 0.75rem;
                margin: 0 0.125rem;
                border: 1px solid #dee2e6;
                border-radius: 0.375rem;
                background: white;
                color: #007bff;
                text-decoration: none;
                transition: all 0.15s ease-in-out;
            }
            
            .dataTables_wrapper .dataTables_paginate .paginate_button:hover {
                background: #007bff;
                color: white;
                border-color: #007bff;
            }
            
            .dataTables_wrapper .dataTables_paginate .paginate_button.current {
                background: #007bff;
                color: white;
                border-color: #007bff;
            }
            
            .dataTables_wrapper .dataTables_length select {
                padding: 0.375rem 0.75rem;
                border: 1px solid #ced4da;
                border-radius: 0.375rem;
                background-color: white;
                font-size: 0.875rem;
            }
            
            .dataTables_wrapper .dataTables_info {
                color: #6c757d;
                font-size: 0.875rem;
                padding-top: 0.5rem;
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
        setTimeout(function () {
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
                if (url.toLowerCase() === 'import' || url.toLowerCase() === '') {
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
                        'Import': ['import', 'upload', 'import'],
                        'Export': ['export', 'export']
                    };

                    for (const [menuName, patterns] of Object.entries(pagePatterns)) {
                        for (const pattern of patterns) {
                            //  if (url.toLowerCase().includes(pattern)) {
                            if (url.toLowerCase() == pattern.toLowerCase()) {
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
            if (submenuLink && activeMenu.textContent == "") {
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

    // Excel Import functionality
    function initExcelImport() {
        const importBtn = document.getElementById('btnImportSuppliers');
        const importModal = document.getElementById('importModal');
        const closeModalBtn = document.getElementById('closeImportModal');
        const cancelBtn = document.getElementById('cancelImport');
        const importForm = document.getElementById('importForm');
        
        if (!importBtn || !importModal) return;
        
        // Show import modal
        importBtn.addEventListener('click', function() {
            importModal.style.display = 'flex';
            importModal.style.alignItems = 'center';
            importModal.style.justifyContent = 'center';
            importModal.style.position = 'fixed';
            importModal.style.top = '0';
            importModal.style.left = '0';
            importModal.style.width = '100%';
            importModal.style.height = '100%';
            importModal.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
        });
        
        // Close modal functions
        function closeModal() {
            importModal.style.display = 'none';
            resetImportForm();
        }
        
        closeModalBtn.addEventListener('click', closeModal);
        cancelBtn.addEventListener('click', closeModal);
        
        // Close modal when clicking backdrop
        importModal.addEventListener('click', function(e) {
            if (e.target === importModal) {
                closeModal();
            }
        });
        
        // Handle form submission
        importForm.addEventListener('submit', function(e) {
            e.preventDefault();
            startExcelImport();
        });
    }
    
    function resetImportForm() {
        const form = document.getElementById('importForm');
        const progressSection = document.getElementById('importProgress');
        const startBtn = document.getElementById('startImport');
        
        if (form) form.reset();
        if (progressSection) progressSection.style.display = 'none';
        if (startBtn) {
            startBtn.disabled = false;
            startBtn.innerHTML = `
                <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM6.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L11 5.414V13a1 1 0 11-2 0V5.414L7.707 6.707a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
                </svg>
                Start Import
            `;
        }
    }
    
    function startExcelImport() {
        const fileInput = document.getElementById('excelFile');
        const validateData = document.getElementById('validateData').checked;
        const progressSection = document.getElementById('importProgress');
        const startBtn = document.getElementById('startImport');
        
        if (!fileInput.files || fileInput.files.length === 0) {
            ModernAlert.error('Please select an Excel file to import.', 'File Required');
            return;
        }
        
        const file = fileInput.files[0];
        const maxSize = 50 * 1024 * 1024; // 50MB
        
        if (file.size > maxSize) {
            ModernAlert.error('File size exceeds 50MB limit. Please select a smaller file.', 'File Too Large');
            return;
        }
        
        // Show progress section
        progressSection.style.display = 'block';
        startBtn.disabled = true;
        startBtn.innerHTML = 'Processing...';
        
        // Create FormData
        const formData = new FormData();
        formData.append('excelFile', file);
        formData.append('validateData', validateData);
        
        // Start the import process
        performExcelImport(formData);
    }
    
    function performExcelImport(formData) {
        const progressFill = document.getElementById('progressFill');
        const progressText = document.getElementById('progressText');
        const recordsProcessed = document.getElementById('recordsProcessed');
        const importStatus = document.getElementById('importStatus');
        
        // Use fetch API for the import
        fetch('./all-supplier?handler=ImportExcel', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Update progress to 100%
                updateProgress(100, data.totalRecords, data.totalRecords);
                
                // Show results summary
                displayImportResults(data);
                
                // Refresh the supplier list after delay
                //setTimeout(() => {
                //    window.location.reload();
                //}, 5000);
                
            } else {
                throw new Error(data.message || 'Import failed');
            }
        })
        .catch(error => {
            console.error('Import error:', error);
            importStatus.innerHTML = `
                <div class="alert alert-danger">
                    <strong>Import Failed!</strong><br>
                    Error: ${error.message}
                </div>
            `;
            ModernAlert.error(error.message, 'Import Failed');
        })
        .finally(() => {
            const startBtn = document.getElementById('startImport');
            if (startBtn) {
                startBtn.disabled = false;
                startBtn.innerHTML = `
                    <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM6.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L11 5.414V13a1 1 0 11-2 0V5.414L7.707 6.707a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Start Import
                `;
            }
        });
    }
    
    function updateProgress(percentage, processed, total) {
        const loadingMessage = document.getElementById('loadingMessage');
        const recordsProcessed = document.getElementById('recordsProcessed');
        
        // Update loading messages based on progress
        if (percentage < 25) {
            if (loadingMessage) loadingMessage.textContent = 'Reading Excel file...';
            if (recordsProcessed) recordsProcessed.textContent = 'Analyzing file structure and data';
        } else if (percentage < 50) {
            if (loadingMessage) loadingMessage.textContent = 'Validating data...';
            if (recordsProcessed) recordsProcessed.textContent = `Processing ${processed} of ${total} records`;
        } else if (percentage < 75) {
            if (loadingMessage) loadingMessage.textContent = 'Importing suppliers...';
            if (recordsProcessed) recordsProcessed.textContent = `Importing ${processed} of ${total} records`;
        } else if (percentage < 100) {
            if (loadingMessage) loadingMessage.textContent = 'Finalizing import...';
            if (recordsProcessed) recordsProcessed.textContent = `Almost done - ${processed} of ${total} records processed`;
        } else {
            if (loadingMessage) loadingMessage.textContent = 'Import completed!';
            if (recordsProcessed) recordsProcessed.textContent = `Successfully processed ${total} records`;
        }
    }
    
    function displayImportResults(data) {
        // Show results summary
        const resultsSummary = document.getElementById('importResultsSummary');
        const resultsDetails = document.getElementById('importResultsDetails');
        
        if (resultsSummary) {
            resultsSummary.style.display = 'block';
            
            // Update summary cards
            document.getElementById('successCount').textContent = data.successCount || 0;
            document.getElementById('errorCount').textContent = data.errorCount || 0;
            document.getElementById('totalCount').textContent = data.totalRecords || 0;
        }
        
        if (resultsDetails && (data.successfulRecords || data.errors)) {
            resultsDetails.style.display = 'block';
            
            // Populate successful records
            populateSuccessfulRecords(data.successfulRecords || []);
            
            // Populate failed records
            populateFailedRecords(data.errors || []);
            
            // Initialize tab functionality
            initResultsTabs();
        }
    }
    
    function populateSuccessfulRecords(successfulRecords) {
        const tbody = document.getElementById('successfulRecords');
        if (!tbody) return;
        
        tbody.innerHTML = '';
        
        if (successfulRecords.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" style="text-align: center; color: #6c757d;">No successful records to display</td></tr>';
            return;
        }
        
        successfulRecords.forEach((record, index) => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${record.rowNumber || index + 1}</td>
                <td>${record.supplierName || 'N/A'}</td>
                <td>${record.skuCode || 'N/A'}</td>
                <td><span class="status-success">✓ Imported</span></td>
            `;
            tbody.appendChild(row);
        });
    }
    
    function populateFailedRecords(errors) {
        const tbody = document.getElementById('failedRecords');
        if (!tbody) return;
        
        tbody.innerHTML = '';
        
        if (errors.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" style="text-align: center; color: #6c757d;">No failed records to display</td></tr>';
            return;
        }
        
        errors.forEach((error, index) => {
            const row = document.createElement('tr');
            
            // Parse error message to extract row info
            const rowMatch = error.match(/Row (\d+):/);
            const rowNumber = rowMatch ? rowMatch[1] : index + 1;
            const errorMessage = error.replace(/Row \d+:\s*/, '');
            
            // Try to extract supplier name and SKU from error message
            const supplierMatch = error.match(/Supplier '([^']+)'/);
            const skuMatch = error.match(/SKU '([^']+)'/);
            
            row.innerHTML = `
                <td>${rowNumber}</td>
                <td>${supplierMatch ? supplierMatch[1] : 'N/A'}</td>
                <td>${skuMatch ? skuMatch[1] : 'N/A'}</td>
                <td><span class="error-message">${errorMessage}</span></td>
            `;
            tbody.appendChild(row);
        });
    }
    
    function initResultsTabs() {
        const tabButtons = document.querySelectorAll('.tab-button');
        const tabPanes = document.querySelectorAll('.tab-pane');
        
        tabButtons.forEach(button => {
            button.addEventListener('click', function() {
                const targetTab = this.getAttribute('data-tab');
                
                // Remove active class from all buttons and panes
                tabButtons.forEach(btn => btn.classList.remove('active'));
                tabPanes.forEach(pane => pane.classList.remove('active'));
                
                // Add active class to clicked button
                this.classList.add('active');
                
                // Show corresponding tab pane
                const targetPane = document.getElementById(targetTab + 'Tab');
                if (targetPane) {
                    targetPane.classList.add('active');
                }
            });
        });
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
        initExcelImport();

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
