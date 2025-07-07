$(document).ready(function() {
    // Sidebar toggle
    $('.sidebar-toggle').click(function() {
        $('.sidebar').toggleClass('collapsed');
    });

    // Mobile sidebar toggle
    if ($(window).width() <= 768) {
        $('.sidebar-toggle').click(function() {
            $('.sidebar').toggleClass('show');
        });
    }

    // Auto-hide alerts
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Confirm actions
    $('[data-confirm]').click(function(e) {
        var message = $(this).data('confirm');
        if (!confirm(message)) {
            e.preventDefault();
        }
    });

    // Loading states for forms
    $('form').submit(function() {
        $(this).find('button[type="submit"]').prop('disabled', true).addClass('loading');
    });

    // DataTable-like functionality for search
    $('#searchInput').on('keyup', function() {
        var value = $(this).val().toLowerCase();
        $('table tbody tr').filter(function() {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Set active navigation
    var currentPath = window.location.pathname.toLowerCase();
    $('.sidebar .nav-link').each(function() {
        var href = $(this).attr('href');
        if (href && currentPath.includes(href.toLowerCase())) {
            $(this).addClass('active');
        }
    });

    // Responsive table
    if ($(window).width() <= 768) {
        $('.table-responsive').addClass('table-responsive-sm');
    }
});

// Function to show loading spinner
function showLoading(element) {
    $(element).prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status"></span> Loading...');
}

// Function to hide loading spinner
function hideLoading(element, originalText) {
    $(element).prop('disabled', false).html(originalText);
}

// Function to show toast notification
function showToast(message, type = 'success') {
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    var icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    var toast = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    $('.content').prepend(toast);
    
    setTimeout(function() {
        $('.alert').first().fadeOut('slow');
    }, 3000);
}