$(document).ready(function() {
    // Toggle password visibility
    $('#togglePassword').click(function() {
        const passwordField = $('#Password');
        const toggleIcon = $('#toggleIcon');
        
        if (passwordField.attr('type') === 'password') {
            passwordField.attr('type', 'text');
            toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordField.attr('type', 'password');
            toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Login form submission
    $('#loginForm').on('submit', function(e) {
        const loginBtn = $('#loginBtn');
        const loginSpinner = $('#loginSpinner');
        
        // Show loading state
        loginBtn.prop('disabled', true);
        loginSpinner.removeClass('d-none');
        
        // Form will submit normally, but this provides visual feedback
        setTimeout(function() {
            if (!$('.text-danger:visible').length) {
                loginBtn.prop('disabled', false);
                loginSpinner.addClass('d-none');
            }
        }, 3000);
    });

    // Auto-hide alerts
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Add floating label effect
    $('.form-control').on('focus blur', function(e) {
        const $this = $(this);
        const label = $this.prev('label');
        
        if (e.type === 'focus' || $this.val().length > 0) {
            label.addClass('active');
        } else if (e.type === 'blur' && $this.val().length === 0) {
            label.removeClass('active');
        }
    });
});