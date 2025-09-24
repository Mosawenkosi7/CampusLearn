/* FORM VALIDATION */

// File upload functionality and form validation
    document.addEventListener('DOMContentLoaded', function() {
    // Get the form and input elements
    const form = document.getElementById("BookTutorForm");
    const studentName = document.getElementById("studentName");
    const studentEmail = document.getElementById("studentEmail");
    const module = document.getElementById("module");
    const preferredDate = document.getElementById("preferredDate");
    const preferredTime = document.getElementById("preferredTime");
    const sessionDuration = document.getElementById("sessionDuration");
    const location = document.getElementById("location");
    const bookingSummary = document.getElementById("BookingSummary");
    const agreeTerms = document.getElementById("agreeTerms");

    // Adding event listener to the form to prevent the default submission and validate inputs
    if (form) {
        form.addEventListener("submit", (e) => {
            e.preventDefault();
            validateInputs();
        });
    }

    // Function to display error messages
    const setError = (element, message) => {
        const inputControl = element.parentElement;
        const errorDisplay = inputControl.querySelector(".error");

        errorDisplay.innerText = message;
        inputControl.classList.add("error");
        inputControl.classList.remove("success");
    };

    // Function for success validation
    const setSuccess = (element) => {
        const inputControl = element.parentElement;
        const errorDisplay = inputControl.querySelector(".error");

        errorDisplay.innerText = "";
        inputControl.classList.add("success");
        inputControl.classList.remove("error");
    };

    // Function to validate email format
    const isValidEmail = (email) => {
        const re =
            /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
    };

    // Function to validate date (must be future date)
    const isValidDate = (date) => {
        const selectedDate = new Date(date);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return selectedDate > today;
    };

    // Function to validate all inputs
    const validateInputs = () => {
        const studentNameValue = studentName.value.trim();
        const studentEmailValue = studentEmail.value.trim();
        const moduleValue = module.value.trim();
        const preferredDateValue = preferredDate.value.trim();
        const preferredTimeValue = preferredTime.value.trim();
        const sessionDurationValue = sessionDuration.value.trim();
        const locationValue = location.value.trim();
        const bookingSummaryValue = bookingSummary.value.trim();

        // Validate student name
        if (studentNameValue === "") {
            setError(studentName, "Student name is required");
        } else {
            setSuccess(studentName);
        }

        // Validate email
        if (studentEmailValue === "") {
            setError(studentEmail, "Email is required");
        } else if (!isValidEmail(studentEmailValue)) {
            setError(studentEmail, "Provide a valid email address");
        } else {
            setSuccess(studentEmail);
        }

        // Validate module
        if (moduleValue === "") {
            setError(module, "Module is required");
        } else {
            setSuccess(module);
        }

        // Validate preferred date
        if (preferredDateValue === "") {
            setError(preferredDate, "Preferred date is required");
        } else if (!isValidDate(preferredDateValue)) {
            setError(preferredDate, "Please select a future date");
        } else {
            setSuccess(preferredDate);
        }

        // Validate preferred time
        if (preferredTimeValue === "" || preferredTimeValue === "Select time") {
            setError(preferredTime, "Please select a time");
        } else {
            setSuccess(preferredTime);
        }

        // Validate session duration
        if (sessionDurationValue === "" || sessionDurationValue === "Select duration") {
            setError(sessionDuration, "Please select a session duration");
        } else {
            setSuccess(sessionDuration);
        }

        // Validate location
        if (locationValue === "" || locationValue === "Select Location") {
            setError(location, "Please select a location");
        } else {
            setSuccess(location);
        }

        // Validate booking summary
        if (bookingSummaryValue === "") {
            setError(bookingSummary, "Booking summary is required");
        } else if (bookingSummaryValue.length < 10) {
            setError(bookingSummary, "Booking summary must be at least 10 characters long");
        } else {
            setSuccess(bookingSummary);
        }

        // Validate terms agreement
        if (!agreeTerms.checked) {
            setError(agreeTerms, "You must agree to the terms and conditions");
        } else {
            setSuccess(agreeTerms);
        }

        // Check if all validations passed
        const allInputControls = form.querySelectorAll('.input-control');
        let allValid = true;
        
        allInputControls.forEach(control => {
            if (control.classList.contains('error')) {
                allValid = false;
            }
        });

        if (allValid) {
            // All validations passed, submit the form
            form.submit();
        }
    };

    // File upload functionality
    const fileInput1 = document.getElementById('studentResource1');
    const fileInput2 = document.getElementById('studentResource2');
    const selectedFilesContainer1 = document.getElementById('selectedFilesContainer1');
    const selectedFilesContainer2 = document.getElementById('selectedFilesContainer2');
    const selectedFilesList1 = document.getElementById('selectedFilesList1');
    const selectedFilesList2 = document.getElementById('selectedFilesList2');
    const fileUploadDisplay1 = fileInput1?.parentElement.querySelector('.fileUploadDisplay');
    const fileUploadDisplay2 = fileInput2?.parentElement.querySelector('.fileUploadDisplay');
    
    // Handle file selection for first upload
    if (fileInput1) {
        fileInput1.addEventListener('change', function(e) {
            const files = Array.from(e.target.files);
            displaySelectedFiles(files, selectedFilesContainer1, selectedFilesList1);
        });
        
        // Handle drag and drop for first upload
        if (fileUploadDisplay1) {
            fileUploadDisplay1.addEventListener('dragover', function(e) {
                e.preventDefault();
                fileUploadDisplay1.style.backgroundColor = '#e8eaed';
            });
            
            fileUploadDisplay1.addEventListener('dragleave', function(e) {
                e.preventDefault();
                fileUploadDisplay1.style.backgroundColor = '#f1f3f4';
            });
            
            fileUploadDisplay1.addEventListener('drop', function(e) {
                e.preventDefault();
                fileUploadDisplay1.style.backgroundColor = '#f1f3f4';
                const files = Array.from(e.dataTransfer.files);
                fileInput1.files = e.dataTransfer.files;
                displaySelectedFiles(files, selectedFilesContainer1, selectedFilesList1);
            });
        }
    }
    
    // Handle file selection for second upload
    if (fileInput2) {
        fileInput2.addEventListener('change', function(e) {
        const files = Array.from(e.target.files);
            displaySelectedFiles(files, selectedFilesContainer2, selectedFilesList2);
    });
    
        // Handle drag and drop for second upload
        if (fileUploadDisplay2) {
            fileUploadDisplay2.addEventListener('dragover', function(e) {
        e.preventDefault();
                fileUploadDisplay2.style.backgroundColor = '#e8eaed';
    });
    
            fileUploadDisplay2.addEventListener('dragleave', function(e) {
        e.preventDefault();
                fileUploadDisplay2.style.backgroundColor = '#f1f3f4';
    });
    
            fileUploadDisplay2.addEventListener('drop', function(e) {
        e.preventDefault();
                fileUploadDisplay2.style.backgroundColor = '#f1f3f4';
        const files = Array.from(e.dataTransfer.files);
                fileInput2.files = e.dataTransfer.files;
                displaySelectedFiles(files, selectedFilesContainer2, selectedFilesList2);
    });
        }
    }
    
    // Display selected files
    function displaySelectedFiles(files, container, list) {
        if (!container || !list) return;
        
        if (files.length === 0) {
            container.style.display = 'none';
            return;
        }
        
        list.innerHTML = '';
        files.forEach(function(file, index) {
            const listItem = document.createElement('li');
            listItem.className = 'selectedFileItem';
            listItem.innerHTML = `
                <span class="fileName">${file.name}</span>
                <span class="fileSize">(${formatFileSize(file.size)})</span>
                <button type="button" class="removeFileBtn" onclick="removeFile(${index}, '${container.id}')">×</button>
            `;
            list.appendChild(listItem);
        });
        
        container.style.display = 'block';
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
    window.removeFile = function(index, containerId) {
        const container = document.getElementById(containerId);
        const fileInput = container.closest('.fileUploadContainer').querySelector('input[type="file"]');
        const list = container.querySelector('.selectedFilesList');
        
        const dt = new DataTransfer();
        const files = Array.from(fileInput.files);
        files.splice(index, 1);
        
        files.forEach(file => dt.items.add(file));
        fileInput.files = dt.files;
        
        displaySelectedFiles(files, container, list);
    };
});