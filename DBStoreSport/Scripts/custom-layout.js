/**
 * Custom Layout JavaScript for SportStore
 * Enhanced functionality for modern UI
 */

(function() {
    'use strict';

    // DOM Ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeLayout();
        setupEventListeners();
        createScrollToTopButton();
    });

    /**
     * Initialize all layout functionality
     */
    function initializeLayout() {
        console.log('SportStore Layout Initialized');
        
        // Add loading animation to buttons
        setupLoadingAnimations();
        
        // Setup smooth scrolling
        setupSmoothScrolling();
        
        // Setup card hover effects
        setupCardHoverEffects();
        
        // Setup dropdown functionality
        setupDropdowns();
        
        // Setup sidebar toggle
        setupSidebarToggle();
        
        // Setup scroll to top
        setupScrollToTop();
        
        // Setup cart functionality
        setupCart();
    }

    /**
     * Setup all event listeners
     */
    function setupEventListeners() {
        // Window scroll event
        window.addEventListener('scroll', handleScroll);
        
        // Window resize event
        window.addEventListener('resize', handleResize);
        
        // Document click events
        document.addEventListener('click', handleDocumentClick);
    }

    /**
     * Setup loading animations for buttons
     */
    function setupLoadingAnimations() {
        const buttons = document.querySelectorAll('button[type="submit"], .btn-primary, .btn-success');
        
        buttons.forEach(button => {
            button.addEventListener('click', function(e) {
                if (!this.classList.contains('loading')) {
                    this.classList.add('loading');
                    const originalText = this.innerHTML;
                    this.innerHTML = '<span class="loading"></span> Đang xử lý...';
                    
                    // Reset after 2 seconds
                    setTimeout(() => {
                        this.classList.remove('loading');
                        this.innerHTML = originalText;
                    }, 2000);
                }
            });
        });
    }

    /**
     * Setup smooth scrolling for anchor links
     */
    function setupSmoothScrolling() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    /**
     * Setup card hover effects
     */
    function setupCardHoverEffects() {
        document.querySelectorAll('.card').forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-5px)';
                this.style.boxShadow = '0 10px 25px rgba(0,0,0,0.15)';
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
            });
        });
    }

    /**
     * Setup dropdown functionality
     */
    function setupDropdowns() {
        // Enhanced dropdown for admin menu
        const adminDropdownBtn = document.getElementById('adminDropdownBtn');
        const adminDropdownMenu = document.querySelector('#adminDropdownBtn + .dropdown-menu');
        
        if (adminDropdownBtn && adminDropdownMenu) {
            adminDropdownBtn.addEventListener('click', function(e) {
                e.stopPropagation();
                adminDropdownMenu.classList.toggle('show');
            });
            
            // Close dropdown when clicking outside
            document.addEventListener('click', function() {
                adminDropdownMenu.classList.remove('show');
            });
        }
    }

    /**
     * Setup sidebar toggle functionality
     */
    function setupSidebarToggle() {
        window.toggleSidebar = function() {
            const sidebar = document.getElementById('sidebarMenu');
            const content = document.getElementById('div-content');
            const toggleBtn = document.querySelector('.sidebar-toggle');
            
            if (!sidebar || !content || !toggleBtn) return;
            
            if (sidebar.style.display === 'none' || sidebar.style.display === '') {
                // Show sidebar
                sidebar.style.display = 'block';
                content.classList.remove('col-12');
                content.classList.add('col-lg-10', 'col-md-9');
                toggleBtn.innerHTML = '<i class="fas fa-times"></i><span>Ẩn danh mục</span>';
                
                // Add animation
                sidebar.style.animation = 'slideInLeft 0.3s ease';
            } else {
                // Hide sidebar
                sidebar.style.display = 'none';
                content.classList.remove('col-lg-10', 'col-md-9');
                content.classList.add('col-12');
                toggleBtn.innerHTML = '<i class="fas fa-bars"></i><span>Hiện danh mục</span>';
            }
        };
    }

    /**
     * Create scroll to top button
     */
    function createScrollToTopButton() {
        const scrollToTopBtn = document.createElement('button');
        scrollToTopBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
        scrollToTopBtn.className = 'btn btn-primary scroll-to-top';
        scrollToTopBtn.setAttribute('aria-label', 'Scroll to top');
        document.body.appendChild(scrollToTopBtn);
    }

    /**
     * Setup scroll to top functionality
     */
    function setupScrollToTop() {
        const scrollToTopBtn = document.querySelector('.scroll-to-top');
        
        if (scrollToTopBtn) {
            window.addEventListener('scroll', function() {
                if (window.pageYOffset > 300) {
                    scrollToTopBtn.style.display = 'block';
                } else {
                    scrollToTopBtn.style.display = 'none';
                }
            });

            scrollToTopBtn.addEventListener('click', function() {
                window.scrollTo({
                    top: 0,
                    behavior: 'smooth'
                });
            });
        }
    }

    /**
     * Handle scroll events
     */
    function handleScroll() {
        // Add scroll-based animations
        const elements = document.querySelectorAll('.fade-in');
        elements.forEach(element => {
            const elementTop = element.getBoundingClientRect().top;
            const elementVisible = 150;
            
            if (elementTop < window.innerHeight - elementVisible) {
                element.classList.add('visible');
            }
        });
    }

    /**
     * Handle window resize
     */
    function handleResize() {
        // Adjust sidebar on mobile
        if (window.innerWidth <= 768) {
            const sidebar = document.getElementById('sidebarMenu');
            const content = document.getElementById('div-content');
            
            if (sidebar && content) {
                sidebar.style.display = 'none';
                content.classList.remove('col-lg-10', 'col-md-9');
                content.classList.add('col-12');
            }
        }
    }

    /**
     * Handle document click events
     */
    function handleDocumentClick(e) {
        // Close dropdowns when clicking outside
        const dropdowns = document.querySelectorAll('.dropdown-menu.show');
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) {
                dropdown.classList.remove('show');
            }
        });
    }

    /**
     * Utility function to show notifications
     */
    window.showNotification = function(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} notification`;
        notification.innerHTML = `
            <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
            <span>${message}</span>
            <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
        `;
        
        // Add styles
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            animation: slideInRight 0.3s ease;
        `;
        
        document.body.appendChild(notification);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 5000);
    };

    /**
     * Utility function to show loading overlay
     */
    window.showLoading = function() {
        const overlay = document.createElement('div');
        overlay.className = 'loading-overlay';
        overlay.innerHTML = `
            <div class="loading-spinner">
                <div class="spinner"></div>
                <p>Đang tải...</p>
            </div>
        `;
        
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 9999;
        `;
        
        document.body.appendChild(overlay);
    };

    window.hideLoading = function() {
        const overlay = document.querySelector('.loading-overlay');
        if (overlay) {
            overlay.remove();
        }
    };

    /**
     * Utility function to format currency
     */
    window.formatCurrency = function(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    };

    /**
     * Setup cart functionality
     */
    function setupCart() {
        // Update cart count from session or local storage
        updateCartCount();
        
        // Add click event to cart button
        const cartBtn = document.querySelector('.cart-icon-btn');
        if (cartBtn) {
            cartBtn.addEventListener('click', function(e) {
                // Add loading effect
                this.style.transform = 'scale(0.95)';
                setTimeout(() => {
                    this.style.transform = '';
                }, 150);
            });
        }
    }

    /**
     * Update cart count display
     */
    function updateCartCount() {
        const cartCount = document.querySelector('.cart-count');
        if (cartCount) {
            // Try to get count from session or make AJAX call
            fetch('/ShoppingCart/GetCartJson')
                .then(response => response.json())
                .then(data => {
                    const itemCount = data.Items ? data.Items.length : 0;
                    cartCount.textContent = itemCount;
                    
                    // Hide count if 0
                    if (itemCount === 0) {
                        cartCount.style.display = 'none';
                    } else {
                        cartCount.style.display = 'flex';
                    }
                })
                .catch(error => {
                    console.log('Cart count update failed:', error);
                    cartCount.textContent = '0';
                    cartCount.style.display = 'none';
                });
        }
    }

    /**
     * Utility function to validate forms
     */
    window.validateForm = function(formElement) {
        const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;
        
        inputs.forEach(input => {
            if (!input.value.trim()) {
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
            }
        });
        
        return isValid;
    };

    // Add CSS animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInLeft {
            from {
                transform: translateX(-100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        .loading-spinner {
            text-align: center;
            color: white;
        }
        
        .spinner {
            width: 50px;
            height: 50px;
            border: 5px solid rgba(255,255,255,0.3);
            border-radius: 50%;
            border-top-color: white;
            animation: spin 1s ease-in-out infinite;
            margin: 0 auto 1rem;
        }
        
        .notification {
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            border-radius: 8px;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .notification .btn-close {
            margin-left: auto;
            background: none;
            border: none;
            color: inherit;
            font-size: 1.2rem;
            cursor: pointer;
        }
        
        .is-invalid {
            border-color: #dc3545 !important;
            box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
        }
    `;
    document.head.appendChild(style);

})();