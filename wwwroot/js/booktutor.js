// BookTutor Page JavaScript
document.addEventListener('DOMContentLoaded', function() {
    const fileInput = document.getElementById('studentResources');
    const selectedFilesContainer = document.getElementById('selectedFilesContainer');
    const selectedFilesList = document.getElementById('selectedFilesList');
    const fileUploadDisplay = document.querySelector('.fileUploadDisplay');
    const form = document.querySelector('form');
    
    // Form validation
    initializeFormValidation();
    
    // Handle file selection
    fileInput.addEventListener('change', function(e) {
        const files = Array.from(e.target.files);
        displaySelectedFiles(files);
    });
    
    // Handle drag and drop
    fileUploadDisplay.addEventListener('dragover', function(e) {
        e.preventDefault();
        fileUploadDisplay.style.backgroundColor = '#e8eaed';
    });
    
    fileUploadDisplay.addEventListener('dragleave', function(e) {
        e.preventDefault();
        fileUploadDisplay.style.backgroundColor = '#f1f3f4';
    });
    
    fileUploadDisplay.addEventListener('drop', function(e) {
        e.preventDefault();
        fileUploadDisplay.style.backgroundColor = '#f1f3f4';
        const files = Array.from(e.dataTransfer.files);
        fileInput.files = e.dataTransfer.files;
        displaySelectedFiles(files);
    });
    
    // Display selected files
    function displaySelectedFiles(files) {
        if (files.length === 0) {
            selectedFilesContainer.style.display = 'none';
            return;
        }
        
        selectedFilesList.innerHTML = '';
        files.forEach(function(file, index) {
            const listItem = document.createElement('li');
            listItem.className = 'selectedFileItem';
            listItem.innerHTML = `
                <span class="fileName">${file.name}</span>
                <span class="fileSize">(${formatFileSize(file.size)})</span>
                <button type="button" class="removeFileBtn" onclick="removeFile(${index})">×</button>
            `;
            selectedFilesList.appendChild(listItem);
        });
        
        selectedFilesContainer.style.display = 'block';
    }
    
    // Format file size
    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }
    
    // Remove file function (global scope for onclick)
    window.removeFile = function(index) {
        const dt = new DataTransfer();
        const files = Array.from(fileInput.files);
        files.splice(index, 1);
        
        files.forEach(file => dt.items.add(file));
        fileInput.files = dt.files;
        
        displaySelectedFiles(files);
    };
    
    // Form validation functions
    function initializeFormValidation() {
        const requiredFields = [
            { id: 'studentName', name: 'Student Name' },
            { id: 'studentEmail', name: 'Email Address' },
            { id: 'subject', name: 'Subject' },
            { id: 'preferredDate', name: 'Preferred Date' },
            { id: 'preferredTime', name: 'Preferred Time' },
            { id: 'sessionDuration', name: 'Session Duration' }
        ];
        
        // Add validation to each required field
        requiredFields.forEach(field => {
            const element = document.getElementById(field.id);
            if (element) {
                element.addEventListener('blur', () => validateField(field.id, field.name));
                element.addEventListener('input', () => clearValidation(field.id));
            }
        });
        
        // Add validation to terms checkbox
        const termsCheckbox = document.getElementById('agreeTerms');
        if (termsCheckbox) {
            termsCheckbox.addEventListener('change', () => validateTerms());
        }
        
        // Prevent form submission if validation fails
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            let isValid = true;
            
            // Clear all existing validation first
            requiredFields.forEach(field => {
                clearValidation(field.id);
            });
            
            // Validate all required fields and show errors for empty ones
            requiredFields.forEach(field => {
                const fieldElement = document.getElementById(field.id);
                const value = fieldElement.value.trim();
                
                if (!value) {
                    // Force show error for empty fields
                    showFieldError(field.id, `${field.name} is required`);
                    isValid = false;
                } else {
                    // Validate non-empty fields normally
                    if (!validateField(field.id, field.name)) {
                        isValid = false;
                    }
                }
            });
            
            // Validate terms agreement
            if (!validateTerms()) {
                isValid = false;
            }
            
            if (isValid) {
                // All validations passed, submit the form
                form.submit();
            } else {
                // Show general error message
                showFormError('Please fix all errors before submitting the form.');
            }
        });
    }
    
    function validateField(fieldId, fieldName) {
        const field = document.getElementById(fieldId);
        const value = field.value.trim();
        
        if (!value) {
            showFieldError(fieldId, `${fieldName} is required`);
            return false;
        }
        
        // Additional validation for email
        if (fieldId === 'studentEmail') {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(value)) {
                showFieldError(fieldId, 'Please enter a valid email address');
                return false;
            }
        }
        
        // Additional validation for date (must be future date)
        if (fieldId === 'preferredDate') {
            const selectedDate = new Date(value);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            
            if (selectedDate <= today) {
                showFieldError(fieldId, 'Please select a future date');
                return false;
            }
        }
        
        showFieldSuccess(fieldId);
        return true;
    }
    
    function validateTerms() {
        const termsCheckbox = document.getElementById('agreeTerms');
        const termsContainer = document.querySelector('.termsAgreementContainer');
        
        if (!termsCheckbox.checked) {
            showTermsError('You must agree to the terms and conditions');
            return false;
        }
        
        showTermsSuccess();
        return true;
    }
    
    function showFieldError(fieldId, message) {
        const field = document.getElementById(fieldId);
        const container = field.closest('.formFieldHalf') || field.closest('.notesFieldContainer');
        
        // Add error styling
        field.classList.add('field-error');
        
        // Add error message
        const errorElement = document.createElement('div');
        errorElement.className = 'field-error-message';
        errorElement.textContent = message;
        container.appendChild(errorElement);
    }
    
    function showFieldSuccess(fieldId) {
        const field = document.getElementById(fieldId);
        const container = field.closest('.formFieldHalf') || field.closest('.notesFieldContainer');
        
        // Remove existing validation elements
        clearValidation(fieldId);
        
        // Add success styling only (no text message)
        field.classList.add('field-success');
    }
    
    function showTermsError(message) {
        const termsContainer = document.querySelector('.termsAgreementContainer');
        
        // Remove existing validation elements
        const existingError = termsContainer.querySelector('.terms-error-message');
        const existingSuccess = termsContainer.querySelector('.terms-success-message');
        if (existingError) existingError.remove();
        if (existingSuccess) existingSuccess.remove();
        
        // Add error styling
        termsContainer.classList.add('terms-error');
        
        // Add error message
        const errorElement = document.createElement('div');
        errorElement.className = 'terms-error-message';
        errorElement.textContent = message;
        termsContainer.appendChild(errorElement);
    }
    
    function showTermsSuccess() {
        const termsContainer = document.querySelector('.termsAgreementContainer');
        
        // Remove existing validation elements
        const existingError = termsContainer.querySelector('.terms-error-message');
        const existingSuccess = termsContainer.querySelector('.terms-success-message');
        if (existingError) existingError.remove();
        if (existingSuccess) existingSuccess.remove();
        
        // Add success styling only (no text message)
        termsContainer.classList.add('terms-success');
    }
    
    function clearValidation(fieldId) {
        const field = document.getElementById(fieldId);
        const container = field.closest('.formFieldHalf') || field.closest('.notesFieldContainer');
        
        // Remove styling
        field.classList.remove('field-error', 'field-success');
        
        // Remove messages
        const errorMessage = container.querySelector('.field-error-message');
        const successMessage = container.querySelector('.field-success-message');
        if (errorMessage) errorMessage.remove();
        if (successMessage) successMessage.remove();
    }
    
    function showFormError(message) {
        // Remove existing form error
        const existingError = document.querySelector('.form-error-message');
        if (existingError) existingError.remove();
        
        // Create error message
        const errorElement = document.createElement('div');
        errorElement.className = 'form-error-message';
        errorElement.textContent = message;
        
        // Insert before submit button
        const submitContainer = document.querySelector('.submitButtonContainer');
        submitContainer.parentNode.insertBefore(errorElement, submitContainer);
    }
});
