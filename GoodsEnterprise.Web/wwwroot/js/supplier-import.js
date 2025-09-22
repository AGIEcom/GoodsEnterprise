// Supplier Import Configuration Manager
class SupplierImportConfig {
    constructor() {
        this.config = null;
        this.loadConfiguration();
    }

    async loadConfiguration() {
        try {
            const response = await fetch('/config/supplier-import-columns.json');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            this.config = await response.json();
            this.renderColumnInfo();
            this.setupValidation();
        } catch (error) {
            console.error('Failed to load supplier import configuration:', error);
            this.renderFallbackColumnInfo();
        }
    }

    renderColumnInfo() {
        if (!this.config) return;

        const container = document.getElementById('excel-columns-container');
        if (!container) return;

        const { requiredColumns, optionalColumns } = this.config.supplierImportColumns;
        
        container.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h6 class="text-success mb-2">
                        <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20" class="me-1">
                            <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Required Columns
                    </h6>
                    <ul class="column-list required-columns">
                        ${requiredColumns.map(col => `
                            <li class="column-item required" title="${col.description}">
                                <span class="column-name">${col.name}</span>
                                <span class="column-type">(${col.type})</span>
                                <span class="required-badge">*</span>
                            </li>
                        `).join('')}
                    </ul>
                </div>
                <div class="col-md-6">
                    <h6 class="text-info mb-2">
                        <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20" class="me-1">
                            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
                        </svg>
                        Optional Columns
                    </h6>
                    <ul class="column-list optional-columns">
                        ${optionalColumns.slice(0, Math.ceil(optionalColumns.length / 2)).map(col => `
                            <li class="column-item optional" title="${col.description}">
                                <span class="column-name">${col.name}</span>
                                <span class="column-type">(${col.type})</span>
                            </li>
                        `).join('')}
                    </ul>
                </div>
            </div>
            ${optionalColumns.length > 6 ? `
            <div class="row mt-2">
                <div class="col-12">
                    <h6 class="text-info mb-2">Additional Optional Columns</h6>
                    <div class="row">
                        <div class="col-md-6">
                            <ul class="column-list optional-columns">
                                ${optionalColumns.slice(Math.ceil(optionalColumns.length / 2)).map(col => `
                                    <li class="column-item optional" title="${col.description}">
                                        <span class="column-name">${col.name}</span>
                                        <span class="column-type">(${col.type})</span>
                                    </li>
                                `).join('')}
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            ` : ''}
        `;

        // Add CSS styles if not already present
        this.addColumnStyles();
    }

    renderFallbackColumnInfo() {
        const container = document.getElementById('excel-columns-container');
        if (!container) return;

        // Fallback to original hardcoded columns
        container.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h6 class="text-success mb-2">Required Columns</h6>
                    <ul style="font-size: 0.9rem; color: #666;">
                        <li>SupplierName *</li>
                        <li>SKUCode *</li>
                        <li>Email *</li>
                    </ul>
                </div>
                <div class="col-md-6">
                    <h6 class="text-info mb-2">Optional Columns</h6>
                    <ul style="font-size: 0.9rem; color: #666;">
                        <li>FirstName</li>
                        <li>LastName</li>
                        <li>Phone</li>
                        <li>Address1</li>
                        <li>Address2</li>
                        <li>Description</li>
                        <li>IsActive</li>
                        <li>IsPreferred</li>
                        <li>LeadTimeDays</li>
                        <li>MoqCase</li>
                        <li>LastCost</li>
                        <li>Incoterm</li>
                        <li>ValidFrom</li>
                        <li>ValidTo</li>
                    </ul>
                </div>
            </div>
        `;
    }

    addColumnStyles() {
        if (document.getElementById('supplier-import-styles')) return;

        const style = document.createElement('style');
        style.id = 'supplier-import-styles';
        style.textContent = `
            .column-list {
                list-style: none;
                padding: 0;
                margin: 0;
            }
            
            .column-item {
                padding: 4px 8px;
                margin: 2px 0;
                border-radius: 4px;
                font-size: 0.85rem;
                display: flex;
                align-items: center;
                gap: 4px;
                background-color: #f8f9fa;
                border-left: 3px solid transparent;
            }
            
            .column-item.required {
                border-left-color: #28a745;
                background-color: #f8fff9;
            }
            
            .column-item.optional {
                border-left-color: #17a2b8;
                background-color: #f0f9ff;
            }
            
            .column-name {
                font-weight: 500;
                color: #495057;
            }
            
            .column-type {
                font-size: 0.75rem;
                color: #6c757d;
                font-style: italic;
            }
            
            .required-badge {
                color: #dc3545;
                font-weight: bold;
                margin-left: auto;
            }
            
            .column-item:hover {
                background-color: #e9ecef;
                cursor: help;
            }
            
            /* Start Import Button States */
            .btn-disabled {
                background-color: #6c757d !important;
                border-color: #6c757d !important;
                color: #ffffff !important;
                cursor: not-allowed !important;
                opacity: 0.7 !important;
                pointer-events: none !important;
            }
            
            .btn-disabled:hover {
                background-color: #6c757d !important;
                border-color: #6c757d !important;
                transform: none !important;
                box-shadow: none !important;
                pointer-events: none !important;
            }
        `;
        document.head.appendChild(style);
    }

    setupValidation() {
        if (!this.config) return;

        // Initially disable the Start Import button
        this.disableStartImportButton('Please select a file to import');

        const fileInput = document.getElementById('supplierFileUpload'); 
        const importForm = document.getElementById('importForm');
        
        if (fileInput) {
            fileInput.addEventListener('change', (e) => {
                if (e.target.files[0]) {
                    this.validateFile(e.target.files[0]);
                    this.previewImport(e.target.files[0]);
                }
            });
        }

        if (importForm) {
            importForm.addEventListener('submit', (e) => this.handleImportSubmit(e));
        }
    }

    validateFile(file) {
        if (!file || !this.config) return;

        const settings = this.config.supplierImportColumns.importSettings;
        const maxSize = this.parseFileSize(settings.maxFileSize);
        
        // File size validation
        if (file.size > maxSize) {
            this.showValidationError(`File size (${this.formatFileSize(file.size)}) exceeds maximum allowed size (${settings.maxFileSize})`);
            this.disableStartImportButton('File size too large');
            return false;
        }
        
        // File type validation
        const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
        if (!settings.supportedFormats.includes(fileExtension)) {
            this.showValidationError(`File type ${fileExtension} is not supported. Allowed formats: ${settings.supportedFormats.join(', ')}`);
            this.disableStartImportButton('Invalid file type');
            return false;
        }
        
        this.showValidationSuccess(`File "${file.name}" is valid and ready for import.`);
        // Don't enable here - wait for preview results to check CanProceedWithImport
        return true;
    }

    parseFileSize(sizeStr) {
        const units = { 'KB': 1024, 'MB': 1024 * 1024, 'GB': 1024 * 1024 * 1024 };
        const match = sizeStr.match(/^(\d+)(KB|MB|GB)$/i);
        if (match) {
            return parseInt(match[1]) * units[match[2].toUpperCase()];
        }
        return parseInt(sizeStr);
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    showValidationError(message) {
        this.showMessage(message, 'danger');
    }

    showValidationSuccess(message) {
        this.showMessage(message, 'success');
    }

    showMessage(message, type) {
        // Remove existing messages
        const existingMessages = document.querySelectorAll('.import-validation-message');
        existingMessages.forEach(msg => msg.remove());

        // Create new message
        const messageDiv = document.createElement('div');
        messageDiv.className = `alert alert-${type} import-validation-message mt-2`;
        messageDiv.innerHTML = `
            <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20" class="me-2">
                ${type === 'success' ? 
                    '<path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>' :
                    '<path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"></path>'
                }
            </svg>
            ${message}
        `;

        // Insert after file upload container
        const fileContainer = document.querySelector('.file-upload-container');
        if (fileContainer) {
            fileContainer.parentNode.insertBefore(messageDiv, fileContainer.nextSibling);
        }

        // Auto-remove success messages after 5 seconds
        if (type === 'success') {
            setTimeout(() => {
                if (messageDiv.parentNode) {
                    messageDiv.remove();
                }
            }, 5000);
        }
    }

    getValidationRules() {
        return this.config ? this.config.supplierImportColumns.validationRules : null;
    }

    getImportSettings() {
        return this.config ? this.config.supplierImportColumns.importSettings : null;
    }

    // Generate Excel template for download
    generateExcelTemplate() {
        if (!this.config) return;

        const { requiredColumns, optionalColumns } = this.config.supplierImportColumns;
        const allColumns = [...requiredColumns, ...optionalColumns];
        
        // Create Excel workbook using SheetJS
        const wb = XLSX.utils.book_new();
        
        // Prepare data for Excel
        const headers = allColumns.map(col => col.name);
        const sampleRow = allColumns.map(col => {
            switch(col.type) {
                case 'string': return 'Sample Text';
                case 'email': return 'example@company.com';
                case 'boolean': return true;
                case 'integer': return 30;
                case 'decimal': return 99.99;
                case 'date': return '2024-01-01';
                default: return 'Sample';
            }
        });
        
        // Create worksheet data
        const wsData = [
            headers,
            sampleRow,
            // Add a few more sample rows for better template
            allColumns.map(col => {
                switch(col.type) {
                    case 'string': return 'Another Company';
                    case 'email': return 'contact@supplier.com';
                    case 'boolean': return false;
                    case 'integer': return 45;
                    case 'decimal': return 150.75;
                    case 'date': return '2024-02-15';
                    default: return 'Example';
                }
            }),
            allColumns.map(col => {
                switch(col.type) {
                    case 'string': return 'Third Supplier Ltd';
                    case 'email': return 'info@thirdsupplier.com';
                    case 'boolean': return true;
                    case 'integer': return 60;
                    case 'decimal': return 200.00;
                    case 'date': return '2024-03-20';
                    default: return 'Demo';
                }
            })
        ];
        
        // Create worksheet
        const ws = XLSX.utils.aoa_to_sheet(wsData);
        
        // Set column widths for better readability
        const colWidths = allColumns.map(col => {
            switch(col.type) {
                case 'email': return { wch: 25 };
                case 'string': return { wch: 20 };
                case 'date': return { wch: 12 };
                default: return { wch: 15 };
            }
        });
        ws['!cols'] = colWidths;
        
        // Style the header row
        const headerRange = XLSX.utils.decode_range(ws['!ref']);
        for (let col = headerRange.s.c; col <= headerRange.e.c; col++) {
            const cellAddress = XLSX.utils.encode_cell({ r: 0, c: col });
            if (!ws[cellAddress]) continue;
            
            ws[cellAddress].s = {
                font: { bold: true, color: { rgb: "FFFFFF" } },
                fill: { fgColor: { rgb: "366092" } },
                alignment: { horizontal: "center", vertical: "center" }
            };
        }
        
        // Add worksheet to workbook
        XLSX.utils.book_append_sheet(wb, ws, "Supplier Import Template");
        
        // Add instructions sheet
        const instructionsData = [
            ["Supplier Import Template - Instructions"],
            [""],
            ["Required Columns:"],
            ...requiredColumns.map(col => [`• ${col.name}`, col.description || `Required ${col.type} field`]),
            [""],
            ["Optional Columns:"],
            ...optionalColumns.map(col => [`• ${col.name}`, col.description || `Optional ${col.type} field`]),
            [""],
            ["Data Format Guidelines:"],
            ["• Email: Must be valid email format (e.g., user@domain.com)"],
            ["• Boolean: Use true/false, 1/0, yes/no, or active/inactive"],
            ["• Integer: Whole numbers only (e.g., 30, 45, 60)"],
            ["• Decimal: Numbers with decimals (e.g., 99.99, 150.75)"],
            ["• Date: Use YYYY-MM-DD format (e.g., 2024-01-01)"],
            [""],
            ["Import Notes:"],
            ["• Remove sample data before importing your actual data"],
            ["• Ensure all required fields are filled"],
            ["• Check for duplicate SKU codes and email addresses"],
            ["• Lead time should be reasonable (typically 1-365 days)"],
            ["• File size limit: 50MB maximum"]
        ];
        
        const instructionsWs = XLSX.utils.aoa_to_sheet(instructionsData);
        
        // Style instructions sheet
        instructionsWs['!cols'] = [{ wch: 40 }, { wch: 50 }];
        
        // Style the title
        if (instructionsWs['A1']) {
            instructionsWs['A1'].s = {
                font: { bold: true, sz: 16, color: { rgb: "366092" } },
                alignment: { horizontal: "center" }
            };
        }
        
        XLSX.utils.book_append_sheet(wb, instructionsWs, "Instructions");
        // Generate Excel file and download
        const filename = `supplier-import-template-${new Date().toISOString().slice(0, 10)}.xlsx`;
        XLSX.writeFile(wb, filename);
        
        this.showValidationSuccess('Excel template downloaded successfully!');
        console.log('Supplier import configuration loaded successfully');
    }

    reset() {
        // Reset file input
        const fileInput = document.getElementById('supplierFileUpload');
        if (fileInput) {
            fileInput.value = '';
        }

        // Hide and clear preview results
        const previewContainer = document.getElementById('importPreviewResults');
        if (previewContainer) {
            previewContainer.innerHTML = '';
            previewContainer.style.display = 'none';
        }

        // Hide and clear import results
        const resultsContainer = document.getElementById('importResults');
        if (resultsContainer) {
            resultsContainer.innerHTML = '';
            resultsContainer.style.display = 'none';
        }

        // Reset button to initial disabled state
        this.disableStartImportButton('Please select a file to import');

        // Clear any validation messages
        const validationContainer = document.querySelector('.validation-container');
        if (validationContainer) {
            validationContainer.innerHTML = '';
        }

        console.log('Supplier import configuration has been reset');
    }

    // API Integration Methods
    async previewImport(file) {
        if (!file) return;

        try {
            this.showLoadingState('Analyzing file...');
            
            const formData = new FormData();
            formData.append('file', file);
            formData.append('validateData', true);

            const response = await fetch('/api/SupplierImport/preview', {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();
            
            if (result.success) {
                this.displayPreviewResults(result);
            } else {
                this.showValidationError(result.error || 'Failed to preview import data.');
                this.disableStartImportButton('Preview failed');
            }
        } catch (error) {
            console.error('Preview error:', error);
            this.showValidationError('Error analyzing file. Please try again.');
            this.disableStartImportButton('Error analyzing file');
        } finally {
            this.hideLoadingState();
        }
    }

    async handleImportSubmit(event) {
        event.preventDefault();
        
        const fileInput = document.getElementById('supplierFileUpload');
        const file = fileInput?.files[0];
        
        if (!file) {
            this.showValidationError('Please select a file to import.');
            return;
        }

        try {
            this.showLoadingState('Starting import...');
            
            const formData = new FormData();
            formData.append('file', file);
            formData.append('validateData',  true);

            const response = await fetch('/api/SupplierImport/import', {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();
            
            if (result.success) {
                this.displayImportResults(result);
                if (result.data.status === 'Processing') {
                    this.startProgressTracking(result.importId);
                }
            } else {
                this.showValidationError(result.error || 'Import failed.');
            }
        } catch (error) {
            console.error('Import error:', error);
            this.showValidationError('Error during import. Please try again.');
        } finally {
            this.hideLoadingState();
        }
    }

    displayPreviewResults(previewData) {
        const container = document.getElementById('importPreviewResults') || this.createPreviewContainer();
        
        // Control Start Import button based on CanProceedWithImport
        this.updateStartImportButtonState(previewData);
        
        container.innerHTML = `
            <div class="preview-summary">
                <h5>Import Preview</h5>
                <div class="row">
                    <div class="col-md-3">
                        <div class="result-card">
                            <div class="result-number">${previewData.data.totalRecords}</div>
                            <div class="result-label">Total Records</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card success">
                            <div class="result-number">${previewData.data.validRecords}</div>
                            <div class="result-label">Valid Records</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card error">
                            <div class="result-number">${previewData.data.invalidRecords}</div>
                            <div class="result-label">Invalid Records</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card warning">
                            <div class="result-number">${previewData.data.duplicates?.length || 0}</div>
                            <div class="result-label">Duplicates</div>
                        </div>
                    </div>
                </div>
            </div>

            ${previewData.data.errors?.length > 0 ? `
                <div class="import-status error">
                    <strong>File Errors:</strong>
                    <ul class="mb-0 mt-2">
                        ${previewData.data.errors.map(error => `
                            <li><i class="fas fa-times-circle text-danger me-2"></i>${error}</li>
                        `).join('')}
                    </ul>
                </div>
            ` : ''}

            ${previewData.data.warnings?.length > 0 ? `
                <div class="import-status warning">
                    <strong>File Warnings:</strong>
                    <ul class="mb-0 mt-2">
                        ${previewData.data.warnings.map(warning => `
                            <li><i class="fas fa-exclamation-triangle text-warning me-2"></i>${warning}</li>
                        `).join('')}
                    </ul>
                </div>
            ` : ''}
            
            <div class="import-status ${previewData.data.canProceedWithImport ? 'success' : 'error'}">
                ${previewData.data.canProceedWithImport ?
                    '<i class="fas fa-check-circle me-2"></i>File is ready for import' : 
                    '<i class="fas fa-times-circle me-2"></i>File has issues that must be resolved before import'}
            </div>
            
            ${this.generatePreviewTables(previewData)}
        `;
        
        container.style.display = 'block';
    }

    updateStartImportButtonState(previewData) {
        const startImportBtn = document.getElementById('startImport');
        if (!startImportBtn) return;

        // Handle case where previewData might be undefined or null
        if (!previewData || !previewData.data) {
            console.error('Preview data is undefined or missing data property');
            this.disableStartImportButton('Invalid preview data');
            return;
        }

        // Check if import can proceed based on server response
        const canProceed = previewData.data.CanProceedWithImport === true || previewData.data.canProceedWithImport === true;

        if (canProceed) {
            // Enable the button and show success state
            startImportBtn.disabled = false;
            startImportBtn.classList.remove('btn-disabled');
            startImportBtn.classList.add('btn-primary');
            startImportBtn.innerHTML = `
                <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM6.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L11 5.414V13a1 1 0 11-2 0V5.414L7.707 6.707a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
                </svg>
                Start Import (${previewData.data.validRecords || 0} records)
            `;
            startImportBtn.title = `Ready to import ${previewData.data.validRecords || 0} valid records`;
            
            // Remove click prevention when enabling
            startImportBtn.style.pointerEvents = '';
            startImportBtn.removeAttribute('aria-disabled');
            
            // Remove the prevention handler if it exists
            if (startImportBtn._preventClickHandler) {
                startImportBtn.removeEventListener('click', startImportBtn._preventClickHandler, true);
                delete startImportBtn._preventClickHandler;
            }
        } else {
            // Disable the button and show error state
            startImportBtn.disabled = true;
            startImportBtn.classList.add('btn-disabled');
            startImportBtn.classList.remove('btn-primary');
            startImportBtn.innerHTML = `
                <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"></path>
                </svg>
                Cannot Import (${previewData.data.invalidRecords || 0} errors)
            `;
            startImportBtn.title = 'Please fix validation errors before importing';
        }
    }

    disableStartImportButton(reason) {
        const startImportBtn = document.getElementById('startImport');
        if (!startImportBtn) return;

        startImportBtn.disabled = true;
        startImportBtn.classList.add('btn-disabled');
        startImportBtn.classList.remove('btn-primary');
        startImportBtn.innerHTML = `
            <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"></path>
            </svg>
            Cannot Import
        `;
        startImportBtn.title = reason || 'Please fix validation errors before importing';
        
        // Add click event prevention
        startImportBtn.style.pointerEvents = 'none';
        startImportBtn.setAttribute('aria-disabled', 'true');
        
        // Remove any existing click handlers and add a prevention handler
        const preventClick = (e) => {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
            return false;
        };
        
        // Store the prevention handler for later removal
        startImportBtn._preventClickHandler = preventClick;
        startImportBtn.addEventListener('click', preventClick, true);
    }

    displayImportResults(result) {
        const container = document.getElementById('importResults') || this.createResultsContainer();
        
        container.innerHTML = `
            <div class="import-summary">
                <h5>Import Results</h5>
                <div class="row">
                    <div class="col-md-3">
                        <div class="result-card">
                            <div class="result-number">${result.data.totalRecords}</div>
                            <div class="result-label">Total Records</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card success">
                            <div class="result-number">${result.data.successfulImports}</div>
                            <div class="result-label">Successful</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card error">
                            <div class="result-number">${result.data.failedImports}</div>
                            <div class="result-label">Failed</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="result-card">
                            <div class="result-number">${Math.round(result.data.duration || 0)}s</div>
                            <div class="result-label">Duration</div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="import-status ${result.success ? 'success' : 'error'}">
                ${result.message}
            </div>
            
            ${ this.generateResultsTables(result) }
        `;
        /**/
        container.style.display = 'block';
    }

    generateResultsTables(result) {
        let tablesHtml = '';
        
        // Successful Records Table
        if (result.summary?.successfulSuppliers?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-success mb-0">
                            <i class="fas fa-check-circle me-2"></i>
                            Successful Imports (${result.data.successfulImports} records)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-success" onclick="window.supplierImportConfig.exportTableData('success')">
                            <i class="fas fa-download me-1"></i>Export
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover success-table">
                            <thead class="table-success">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Supplier Name</th>
                                    <th>SKU Code</th>
                                    <th>Email</th>
                                    <th style="width: 80px;">ID</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${result.summary.successfulSuppliers.map(supplier => `
                                    <tr>
                                        <td class="text-center">${supplier.rowNumber}</td>
                                        <td class="fw-medium">${supplier.supplierName || 'N/A'}</td>
                                        <td><code class="bg-light px-2 py-1 rounded">${supplier.skuCode || 'N/A'}</code></td>
                                        <td>${supplier.email || 'N/A'}</td>
                                        <td class="text-center"><span class="badge bg-primary">${supplier.id || 'N/A'}</span></td>
                                        <td class="text-center">
                                            <span class="badge bg-success">
                                                <i class="fas fa-check me-1"></i>Success
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        // Failed Records Table
        if (result.summary?.failedSuppliers?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-danger mb-0">
                            <i class="fas fa-exclamation-circle me-2"></i>
                            Failed Imports (${result.data.failedImports} records)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="window.supplierImportConfig.exportTableData('failed')">
                            <i class="fas fa-download me-1"></i>Export Errors
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover error-table">
                            <thead class="table-danger">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Supplier Name</th>
                                    <th>SKU Code</th>
                                    <th>Email</th>
                                    <th>Errors</th>
                                    <th>Warnings</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${result.summary.failedSuppliers.map(supplier => `
                                    <tr>
                                        <td class="text-center">${supplier.rowNumber}</td>
                                        <td class="fw-medium">${supplier.supplierName || 'N/A'}</td>
                                        <td><code class="bg-light px-2 py-1 rounded">${supplier.skuCode || 'N/A'}</code></td>
                                        <td>${supplier.email || 'N/A'}</td>
                                        <td>
                                            ${supplier.errors?.length > 0 ? `
                                                <div class="error-list">
                                                    ${supplier.errors.map(error => `
                                                        <div class="error-item">
                                                            <i class="fas fa-times-circle text-danger me-1"></i>
                                                            <small>${error}</small>
                                                        </div>
                                                    `).join('')}
                                                </div>
                                            ` : '<span class="text-muted">No errors</span>'}
                                        </td>
                                        <td>
                                            ${supplier.warnings?.length > 0 ? `
                                                <div class="warning-list">
                                                    ${supplier.warnings.map(warning => `
                                                        <div class="warning-item">
                                                            <i class="fas fa-exclamation-triangle text-warning me-1"></i>
                                                            <small>${warning}</small>
                                                        </div>
                                                    `).join('')}
                                                </div>
                                            ` : '<span class="text-muted">No warnings</span>'}
                                        </td>
                                        <td class="text-center">
                                            <span class="badge bg-danger">
                                                <i class="fas fa-times me-1"></i>Failed
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        // Duplicates Table
        if (result.summary?.duplicates?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-warning mb-0">
                            <i class="fas fa-copy me-2"></i>
                            Duplicate Records (${result.summary.duplicates.length} found)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-warning" onclick="window.supplierImportConfig.exportTableData('duplicates')">
                            <i class="fas fa-download me-1"></i>Export Duplicates
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover duplicate-table">
                            <thead class="table-warning">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Field</th>
                                    <th>Value</th>
                                    <th>Duplicate Type</th>
                                    <th>Conflicting Rows</th>
                                    <th>Existing ID</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${result.summary.duplicates.map(duplicate => `
                                    <tr>
                                        <td class="text-center">${duplicate.rowNumber}</td>
                                        <td><span class="badge bg-secondary">${duplicate.field}</span></td>
                                        <td><code class="bg-light px-2 py-1 rounded">${duplicate.value}</code></td>
                                        <td>
                                            <span class="badge ${duplicate.duplicateType === 'Database' ? 'bg-info' : 
                                                                 duplicate.duplicateType === 'Batch' ? 'bg-warning' : 'bg-dark'}">
                                                ${duplicate.duplicateType}
                                            </span>
                                        </td>
                                        <td>
                                            ${duplicate.conflictingRows?.length > 0 ? 
                                                duplicate.conflictingRows.map(row => `<span class="badge bg-light text-dark me-1">${row}</span>`).join('') : 
                                                '<span class="text-muted">None</span>'}
                                        </td>
                                        <td class="text-center">
                                            ${duplicate.existingId ? `<span class="badge bg-primary">${duplicate.existingId}</span>` : '<span class="text-muted">N/A</span>'}
                                        </td>
                                        <td class="text-center">
                                            <span class="badge bg-warning">
                                                <i class="fas fa-copy me-1"></i>Duplicate
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        return tablesHtml;
    }

    generatePreviewTables(previewData) {
        let tablesHtml = '';
        
        // Valid Records Preview Table
        if (previewData.data?.validSuppliers?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-success mb-0">
                            <i class="fas fa-check-circle me-2"></i>
                            Valid Records Preview (${previewData.data.validRecords} records)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-success" onclick="window.supplierImportConfig.exportTableData('preview-valid')">
                            <i class="fas fa-download me-1"></i>Export Valid
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover success-table preview-valid-table">
                            <thead class="table-success">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Supplier Name</th>
                                    <th>SKU Code</th>
                                    <th>Email</th>
                                    <th>Phone</th>
                                    <th>Lead Time</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${previewData.data.validSuppliers.slice(0, 10).map(supplier => `
                                    <tr>
                                        <td class="text-center">${supplier.rowNumber}</td>
                                        <td class="fw-medium">${supplier.supplierName || 'N/A'}</td>
                                        <td><code class="bg-light px-2 py-1 rounded">${supplier.skuCode || 'N/A'}</code></td>
                                        <td>${supplier.email || 'N/A'}</td>
                                        <td>${supplier.phone || 'N/A'}</td>
                                        <td class="text-center">${supplier.leadTimeDays || 'N/A'} days</td>
                                        <td class="text-center">
                                            <span class="badge bg-success">
                                                <i class="fas fa-check me-1"></i>Valid
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                                ${previewData.data.validSuppliers.length > 10 ? `
                                    <tr>
                                        <td colspan="7" class="text-center text-muted py-3">
                                            <i class="fas fa-ellipsis-h me-2"></i>
                                            Showing first 10 records. ${previewData.data.validSuppliers.length - 10} more records available.
                                        </td>
                                    </tr>
                                ` : ''}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        // Invalid Records Preview Table
        if (previewData.data?.invalidSuppliers?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-danger mb-0">
                            <i class="fas fa-exclamation-circle me-2"></i>
                            Invalid Records Preview (${previewData.data.invalidRecords} records)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="window.supplierImportConfig.exportTableData('preview-invalid')">
                            <i class="fas fa-download me-1"></i>Export Errors
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover error-table preview-invalid-table">
                            <thead class="table-danger">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Supplier Name</th>
                                    <th>SKU Code</th>
                                    <th>Email</th>
                                    <th>Validation Errors</th>
                                    <th>Warnings</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${previewData.data.invalidSuppliers.slice(0, 10).map(supplier => `
                                    <tr>
                                        <td class="text-center">${supplier.rowNumber}</td>
                                        <td class="fw-medium">${supplier.supplierName || 'N/A'}</td>
                                        <td><code class="bg-light px-2 py-1 rounded">${supplier.skuCode || 'N/A'}</code></td>
                                        <td>${supplier.email || 'N/A'}</td>
                                        <td>
                                            ${supplier.validationErrors?.length > 0 ? `
                                                <div class="error-list">
                                                    ${supplier.validationErrors.map(error => `
                                                        <div class="error-item">
                                                            <i class="fas fa-times-circle text-danger me-1"></i>
                                                            <small>${error}</small>
                                                        </div>
                                                    `).join('')}
                                                </div>
                                            ` : '<span class="text-muted">No errors</span>'}
                                        </td>
                                        <td>
                                            ${supplier.validationWarnings?.length > 0 ? `
                                                <div class="warning-list">
                                                    ${supplier.validationWarnings.map(warning => `
                                                        <div class="warning-item">
                                                            <i class="fas fa-exclamation-triangle text-warning me-1"></i>
                                                            <small>${warning}</small>
                                                        </div>
                                                    `).join('')}
                                                </div>
                                            ` : '<span class="text-muted">No warnings</span>'}
                                        </td>
                                        <td class="text-center">
                                            <span class="badge bg-danger">
                                                <i class="fas fa-times me-1"></i>Invalid
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                                ${previewData.data.invalidSuppliers.length > 10 ? `
                                    <tr>
                                        <td colspan="7" class="text-center text-muted py-3">
                                            <i class="fas fa-ellipsis-h me-2"></i>
                                            Showing first 10 records. ${previewData.data.invalidSuppliers.length - 10} more records available.
                                        </td>
                                    </tr>
                                ` : ''}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        // Duplicates Preview Table
        if (previewData.data?.duplicates?.length > 0) {
            tablesHtml += `
                <div class="results-section mt-4">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6 class="text-warning mb-0">
                            <i class="fas fa-copy me-2"></i>
                            Duplicate Records Preview (${previewData.data.duplicates.length} found)
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-warning" onclick="window.supplierImportConfig.exportTableData('preview-duplicates')">
                            <i class="fas fa-download me-1"></i>Export Duplicates
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-hover duplicate-table preview-duplicates-table">
                            <thead class="table-warning">
                                <tr>
                                    <th style="width: 60px;">Row #</th>
                                    <th>Field</th>
                                    <th>Value</th>
                                    <th>Duplicate Type</th>
                                    <th>Conflicting Rows</th>
                                    <th>Existing ID</th>
                                    <th style="width: 100px;">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${previewData.data.duplicates.slice(0, 10).map(duplicate => `
                                    <tr>
                                        <td class="text-center">${duplicate.rowNumber}</td>
                                        <td><span class="badge bg-secondary">${duplicate.field}</span></td>
                                        <td><code class="bg-light px-2 py-1 rounded">${duplicate.value}</code></td>
                                        <td>
                                            <span class="badge ${duplicate.duplicateType === 'Database' ? 'bg-info' : 
                                                                 duplicate.duplicateType === 'Batch' ? 'bg-warning' : 'bg-dark'}">
                                                ${duplicate.duplicateType}
                                            </span>
                                        </td>
                                        <td>
                                            ${duplicate.conflictingRows?.length > 0 ? 
                                                duplicate.conflictingRows.map(row => `<span class="badge bg-light text-dark me-1">${row}</span>`).join('') : 
                                                '<span class="text-muted">None</span>'}
                                        </td>
                                        <td class="text-center">
                                            ${duplicate.existingId ? `<span class="badge bg-primary">${duplicate.existingId}</span>` : '<span class="text-muted">N/A</span>'}
                                        </td>
                                        <td class="text-center">
                                            <span class="badge bg-warning">
                                                <i class="fas fa-copy me-1"></i>Duplicate
                                            </span>
                                        </td>
                                    </tr>
                                `).join('')}
                                ${previewData.data.duplicates.length > 10 ? `
                                    <tr>
                                        <td colspan="7" class="text-center text-muted py-3">
                                            <i class="fas fa-ellipsis-h me-2"></i>
                                            Showing first 10 records. ${previewData.data.duplicates.length - 10} more records available.
                                        </td>
                                    </tr>
                                ` : ''}
                            </tbody>
                        </table>
                    </div>
                </div>
            `;
        }
        
        return tablesHtml;
    }

    async startProgressTracking(importId) {
        const progressContainer = this.createProgressContainer();
        let attempts = 0;
        const maxAttempts = 60; // 5 minutes with 5-second intervals
        
        const trackProgress = async () => {
            try {
                const response = await fetch(`/api/SupplierImport/progress/${importId}`);
                const result = await response.json();
                
                if (result.success) {
                    this.updateProgressDisplay(result.data);
                    
                    if (result.data.status === 'Processing' && attempts < maxAttempts) {
                        attempts++;
                        setTimeout(trackProgress, 5000); // Check every 5 seconds
                    }
                } else {
                    console.error('Failed to get progress:', result.error);
                }
            } catch (error) {
                console.error('Progress tracking error:', error);
            }
        };
        
        trackProgress();
    }

    updateProgressDisplay(progress) {
        const container = document.getElementById('progressContainer');
        if (!container) return;
        
        container.innerHTML = `
            <div class="progress-info">
                <div class="progress-status">${progress.status}</div>
                <div class="progress-operation">${progress.currentOperation}</div>
                <div class="progress-bar-container">
                    <div class="progress-bar" style="width: ${progress.progressPercentage}%"></div>
                </div>
                <div class="progress-stats">
                    ${progress.processedRecords} / ${progress.totalRecords} records processed
                </div>
            </div>
        `;
    }

    createPreviewContainer() {
        const container = document.createElement('div');
        container.id = 'importPreviewResults';
        container.className = 'import-preview-results';
        container.style.display = 'none';
        
        const form = document.getElementById('importForm');
        if (form) {
            form.appendChild(container);
        }
        
        return container;
    }

    createResultsContainer() {
        const container = document.createElement('div');
        container.id = 'importResults';
        container.className = 'import-results';
        container.style.display = 'none';
        
        const form = document.getElementById('importForm');
        if (form) {
            form.appendChild(container);
        }
        
        return container;
    }

    createProgressContainer() {
        const container = document.createElement('div');
        container.id = 'progressContainer';
        container.className = 'progress-container';
        
        const form = document.getElementById('importForm');
        if (form) {
            form.appendChild(container);
        }
        
        return container;
    }

    showLoadingState(message) {
        const button = document.getElementById('startImport');
        if (button) {
            button.disabled = true;
            button.innerHTML = `
                <div class="spinner-border spinner-border-sm me-2" role="status"></div>
                ${message}
            `;
        }
    }

    hideLoadingState() {
        const button = document.getElementById('startImport');
        if (button) {
            button.disabled = false;
            button.innerHTML = `
                <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM6.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L11 5.414V13a1 1 0 11-2 0V5.414L7.707 6.707a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
                </svg>
                Start Import
            `;
        }
    }

    // Export table data functionality
    exportTableData(type) {
        const timestamp = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        let filename = `supplier-import-${type}-${timestamp}.csv`;
        let csvContent = '';
        let tableSelector = '';

        switch(type) {
            case 'success':
                tableSelector = '.success-table';
                csvContent = 'Row #,Supplier Name,SKU Code,Email,ID,Status\n';
                break;
            case 'failed':
                tableSelector = '.error-table';
                csvContent = 'Row #,Supplier Name,SKU Code,Email,Errors,Warnings,Status\n';
                break;
            case 'duplicates':
                tableSelector = '.duplicate-table';
                csvContent = 'Row #,Field,Value,Duplicate Type,Conflicting Rows,Existing ID,Status\n';
                break;
            case 'preview-valid':
                tableSelector = '.preview-valid-table';
                csvContent = 'Row #,Supplier Name,SKU Code,Email,Phone,Lead Time,Status\n';
                break;
            case 'preview-invalid':
                tableSelector = '.preview-invalid-table';
                csvContent = 'Row #,Supplier Name,SKU Code,Email,Validation Errors,Warnings,Status\n';
                break;
            case 'preview-duplicates':
                tableSelector = '.preview-duplicates-table';
                csvContent = 'Row #,Field,Value,Duplicate Type,Conflicting Rows,Existing ID,Status\n';
                break;
        }

        const table = document.querySelector(tableSelector);
        if (!table) {
            this.showValidationError('No data available to export.');
            return;
        }

        const rows = table.querySelectorAll('tbody tr');
        rows.forEach(row => {
            const cells = row.querySelectorAll('td');
            const rowData = Array.from(cells).map(cell => {
                // Clean up cell content (remove HTML tags and extra spaces)
                let text = cell.textContent.trim();
                // Escape commas and quotes for CSV
                if (text.includes(',') || text.includes('"')) {
                    text = `"${text.replace(/"/g, '""')}"`;
                }
                return text;
            });
            csvContent += rowData.join(',') + '\n';
        });

        // Create and download file
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        this.showValidationSuccess(`${type.charAt(0).toUpperCase() + type.slice(1)} data exported successfully!`);
    }

    // Add enhanced CSS for table styling
    addTableStyles() {
        if (document.getElementById('supplier-import-table-styles')) return;

        const style = document.createElement('style');
        style.id = 'supplier-import-table-styles';
        style.textContent = `
            /* Results Section Styling */
            .results-section {
                background: #fff;
                border-radius: 8px;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                padding: 20px;
                margin-bottom: 20px;
            }

            .results-section h6 {
                font-weight: 600;
                font-size: 1.1rem;
            }

            /* Table Styling */
            .table-responsive {
                border-radius: 6px;
                overflow: hidden;
                border: 1px solid #dee2e6;
            }

            .success-table, .error-table, .duplicate-table {
                margin-bottom: 0;
                font-size: 0.9rem;
            }

            .success-table thead th {
                background-color: #d1e7dd !important;
                border-color: #badbcc !important;
                color: #0f5132 !important;
                font-weight: 600;
                text-align: center;
                vertical-align: middle;
            }

            .error-table thead th {
                background-color: #f8d7da !important;
                border-color: #f5c2c7 !important;
                color: #842029 !important;
                font-weight: 600;
                text-align: center;
                vertical-align: middle;
            }

            .duplicate-table thead th {
                background-color: #fff3cd !important;
                border-color: #ffecb5 !important;
                color: #664d03 !important;
                font-weight: 600;
                text-align: center;
                vertical-align: middle;
            }

            /* Row Styling */
            .success-table tbody tr:hover {
                background-color: #f8f9fa;
            }

            .error-table tbody tr:hover {
                background-color: #f8f9fa;
            }

            .duplicate-table tbody tr:hover {
                background-color: #f8f9fa;
            }

            /* Cell Content Styling */
            .error-list, .warning-list {
                max-width: 300px;
            }

            .error-item, .warning-item {
                margin-bottom: 4px;
                padding: 2px 0;
                border-bottom: 1px solid #eee;
            }

            .error-item:last-child, .warning-item:last-child {
                border-bottom: none;
                margin-bottom: 0;
            }

            .error-item small, .warning-item small {
                display: block;
                line-height: 1.3;
                word-wrap: break-word;
            }

            /* Badge Styling */
            .badge {
                font-size: 0.75rem;
                padding: 0.35em 0.65em;
            }

            /* Code Styling */
            code {
                font-size: 0.85rem;
                font-weight: 500;
            }

            /* Status Column */
            .success-table .badge.bg-success {
                background-color: #198754 !important;
            }

            .error-table .badge.bg-danger {
                background-color: #dc3545 !important;
            }

            .duplicate-table .badge.bg-warning {
                background-color: #ffc107 !important;
                color: #000 !important;
            }

            /* Export Button Styling */
            .btn-outline-success:hover {
                background-color: #198754;
                border-color: #198754;
            }

            .btn-outline-danger:hover {
                background-color: #dc3545;
                border-color: #dc3545;
            }

            .btn-outline-warning:hover {
                background-color: #ffc107;
                border-color: #ffc107;
                color: #000;
            }

            /* Responsive Design */
            @media (max-width: 768px) {
                .results-section {
                    padding: 15px;
                }
                
                .table-responsive {
                    font-size: 0.8rem;
                }
                
                .error-list, .warning-list {
                    max-width: 200px;
                }
            }

            /* Loading Animation for Tables */
            .table-loading {
                position: relative;
            }

            .table-loading::after {
                content: '';
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(255, 255, 255, 0.8);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 10;
            }

            /* Summary Cards Enhancement */
            .result-card {
                transition: transform 0.2s ease;
            }

            .result-card:hover {
                transform: translateY(-2px);
            }

            .result-card.success {
                border-left: 4px solid #198754;
            }

            .result-card.error {
                border-left: 4px solid #dc3545;
            }

            .result-card.warning {
                border-left: 4px solid #ffc107;
            }

            /* Preview-specific styling */
            .preview-summary {
                background: #f8f9fa;
                border-radius: 8px;
                padding: 20px;
                margin-bottom: 20px;
                border: 1px solid #dee2e6;
            }

            .preview-summary h5 {
                color: #495057;
                margin-bottom: 20px;
                font-weight: 600;
            }

            .import-status.warning {
                background-color: #fff3cd;
                color: #664d03;
                border: 1px solid #ffecb5;
                border-radius: 6px;
                padding: 12px 16px;
                margin: 16px 0;
            }

            /* Preview table pagination indicator */
            .table tbody tr:last-child td {
                border-bottom: 1px solid #dee2e6;
            }

            .table tbody tr:last-child.pagination-indicator td {
                background-color: #f8f9fa;
                font-style: italic;
                border-top: 2px solid #dee2e6;
            }

            .import-status.success {
                background-color: #d1e7dd;
                color: #0f5132;
                border: 1px solid #badbcc;
                border-radius: 6px;
                padding: 12px 16px;
                margin: 16px 0;
            }

            .import-status.error {
                background-color: #f8d7da;
                color: #842029;
                border: 1px solid #f5c2c7;
                border-radius: 6px;
                padding: 12px 16px;
                margin: 16px 0;
            }
        `;
        document.head.appendChild(style);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.supplierImportConfig = new SupplierImportConfig();
    
    // Add table styles
    if (window.supplierImportConfig) {
        window.supplierImportConfig.addTableStyles();
    }
    
    // Add event listener for download template button
    const downloadBtn = document.getElementById('downloadTemplate');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', function() {
            if (window.supplierImportConfig) {
                window.supplierImportConfig.generateExcelTemplate();
            }
        });
    }
});

// Export for use in other scripts
window.SupplierImportConfig = SupplierImportConfig;
