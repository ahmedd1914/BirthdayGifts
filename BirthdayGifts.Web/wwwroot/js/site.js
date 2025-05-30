// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Auto-dismiss alerts after 5 seconds
$(document).ready(function() {
    setTimeout(function() {
        $('.alert').alert('close');
    }, 5000);
});

// Form validation enhancement
$(document).ready(function() {
    // Add custom validation styles
    $.validator.setDefaults({
        errorElement: 'span',
        errorClass: 'text-danger',
        validClass: 'text-success',
        highlight: function(element, errorClass, validClass) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        unhighlight: function(element, errorClass, validClass) {
            $(element).addClass('is-valid').removeClass('is-invalid');
        }
    });
});

// Confirm delete functionality
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

// Format date to local string
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Show loading indicator
function showLoading() {
    $('#loadingIndicator').removeClass('d-none');
}

// Hide loading indicator
function hideLoading() {
    $('#loadingIndicator').addClass('d-none');
}

// AJAX error handler
function handleAjaxError(xhr, status, error) {
    hideLoading();
    let errorMessage = 'An error occurred while processing your request.';
    
    if (xhr.responseJSON && xhr.responseJSON.message) {
        errorMessage = xhr.responseJSON.message;
    }
    
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            ${errorMessage}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('.container').prepend(alertHtml);
}

// Initialize tooltips
$(document).ready(function() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// Initialize popovers
$(document).ready(function() {
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function(popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});
