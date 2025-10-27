/**
 * Dashboard JavaScript for SportStore
 * Enhanced functionality for admin dashboard
 */

(function() {
    'use strict';

    // DOM Ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeDashboard();
    });

    /**
     * Initialize dashboard functionality
     */
    function initializeDashboard() {
        console.log('Dashboard Initialized');
        
        // Initialize real-time clock
        updateDateTime();
        setInterval(updateDateTime, 1000);
        
        // Initialize charts
        initializeCharts();
        
        // Add interactive features
        addInteractiveFeatures();
        
        // Show welcome notification
        setTimeout(() => {
            showWelcomeNotification();
        }, 1000);
    }

    /**
     * Update current time and date
     */
    function updateDateTime() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('vi-VN');
        const dateString = now.toLocaleDateString('vi-VN', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
        
        const timeElement = document.getElementById('currentTime');
        const dateElement = document.getElementById('currentDate');
        
        if (timeElement) {
            timeElement.textContent = timeString;
        }
        
        if (dateElement) {
            dateElement.textContent = dateString;
        }
    }

    /**
     * Initialize Chart.js charts
     */
    function initializeCharts() {
        // Revenue Chart
        const revenueCtx = document.getElementById('revenueChart');
        if (revenueCtx) {
            const revenueChart = new Chart(revenueCtx.getContext('2d'), {
                type: 'line',
                data: {
                    labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
                    datasets: [{
                        label: 'Doanh thu (triệu VND)',
                        data: [12, 19, 15, 25, 22, 30, 28, 35, 32, 40, 38, 45],
                        borderColor: '#2563eb',
                        backgroundColor: 'rgba(37, 99, 235, 0.1)',
                        tension: 0.4,
                        fill: true,
                        borderWidth: 3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: false
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            titleColor: 'white',
                            bodyColor: 'white',
                            borderColor: '#2563eb',
                            borderWidth: 1
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: {
                                color: 'rgba(0,0,0,0.1)'
                            },
                            ticks: {
                                callback: function(value) {
                                    return value + 'M';
                                }
                            }
                        },
                        x: {
                            grid: {
                                display: false
                            }
                        }
                    },
                    interaction: {
                        intersect: false,
                        mode: 'index'
                    }
                }
            });
        }

        // Order Chart
        const orderCtx = document.getElementById('orderChart');
        if (orderCtx) {
            const orderChart = new Chart(orderCtx.getContext('2d'), {
                type: 'doughnut',
                data: {
                    labels: ['Đã xử lý', 'Đang xử lý', 'Chờ xử lý', 'Đã hủy'],
                    datasets: [{
                        data: [65, 20, 10, 5],
                        backgroundColor: [
                            '#10b981',
                            '#f59e0b',
                            '#3b82f6',
                            '#ef4444'
                        ],
                        borderWidth: 0,
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                padding: 20,
                                usePointStyle: true,
                                font: {
                                    size: 12
                                }
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            titleColor: 'white',
                            bodyColor: 'white',
                            callbacks: {
                                label: function(context) {
                                    return context.label + ': ' + context.parsed + '%';
                                }
                            }
                        }
                    }
                }
            });
        }
    }

    /**
     * Add interactive features
     */
    function addInteractiveFeatures() {
        // Stat cards hover effects
        document.querySelectorAll('.stat-card').forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-5px) scale(1.02)';
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0) scale(1)';
            });
        });

        // Quick action buttons
        document.querySelectorAll('.quick-action-btn').forEach(btn => {
            btn.addEventListener('click', function(e) {
                // Add loading state
                const originalText = this.querySelector('span').textContent;
                this.querySelector('span').textContent = 'Đang chuyển...';
                
                setTimeout(() => {
                    this.querySelector('span').textContent = originalText;
                }, 1000);
            });
        });

        // Export report button
        const exportBtn = document.querySelector('.card-actions .btn');
        if (exportBtn) {
            exportBtn.addEventListener('click', function() {
                showNotification('Đang xuất báo cáo...', 'info');
                
                setTimeout(() => {
                    showNotification('Báo cáo đã được xuất thành công!', 'success');
                }, 2000);
            });
        }
    }

    /**
     * Show welcome notification
     */
    function showWelcomeNotification() {
        if (typeof showNotification === 'function') {
            showNotification('Chào mừng bạn đến với Admin Dashboard!', 'success');
        } else {
            // Fallback notification
            const notification = document.createElement('div');
            notification.className = 'alert alert-success notification';
            notification.innerHTML = `
                <i class="fas fa-check-circle me-2"></i>
                <span>Chào mừng bạn đến với Admin Dashboard!</span>
                <button type="button" class="btn-close ms-auto" onclick="this.parentElement.remove()"></button>
            `;
            
            notification.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                min-width: 300px;
                animation: slideInRight 0.3s ease;
                border-radius: 12px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            `;
            
            document.body.appendChild(notification);
            
            setTimeout(() => {
                if (notification.parentElement) {
                    notification.remove();
                }
            }, 5000);
        }
    }

    /**
     * Show notification (fallback)
     */
    window.showDashboardNotification = function(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} notification`;
        
        const iconMap = {
            'success': 'check-circle',
            'error': 'exclamation-circle',
            'warning': 'exclamation-triangle',
            'info': 'info-circle'
        };
        
        notification.innerHTML = `
            <i class="fas fa-${iconMap[type] || 'info-circle'} me-2"></i>
            <span>${message}</span>
            <button type="button" class="btn-close ms-auto" onclick="this.parentElement.remove()"></button>
        `;
        
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            animation: slideInRight 0.3s ease;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        `;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 5000);
    };

    /**
     * Refresh dashboard data
     */
    window.refreshDashboardData = function() {
        showDashboardNotification('Đang cập nhật dữ liệu...', 'info');
        
        // Simulate data refresh
        setTimeout(() => {
            // Update stat cards with new data
            const statNumbers = document.querySelectorAll('.stat-card-number');
            statNumbers.forEach((number, index) => {
                const currentValue = parseInt(number.textContent.replace(/[^\d]/g, ''));
                const newValue = currentValue + Math.floor(Math.random() * 10);
                
                // Animate number change
                animateNumber(number, currentValue, newValue);
            });
            
            showDashboardNotification('Dữ liệu đã được cập nhật!', 'success');
        }, 1500);
    };

    /**
     * Animate number change
     */
    function animateNumber(element, start, end) {
        const duration = 1000;
        const startTime = performance.now();
        
        function updateNumber(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            const current = Math.floor(start + (end - start) * progress);
            element.textContent = element.textContent.replace(/\d+/, current);
            
            if (progress < 1) {
                requestAnimationFrame(updateNumber);
            }
        }
        
        requestAnimationFrame(updateNumber);
    }

    // Add CSS animations
    const style = document.createElement('style');
    style.textContent = `
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
        
        .notification {
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
    `;
    document.head.appendChild(style);

})(); 