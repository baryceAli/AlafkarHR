



function loadCharts() {
    const ctx1 = document.getElementById('stockChart');
    new Chart(ctx1, {
        type: 'line',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr'],
            datasets: [{
                label: 'Stock In',
                data: [100, 200, 150, 300],
            }, {
                label: 'Stock Out',
                data: [80, 120, 100, 250],
            }]
        }
    });

    const ctx2 = document.getElementById('topProductsChart');
    new Chart(ctx2, {
        type: 'bar',
        data: {
            labels: ['Item A', 'Item B', 'Item C'],
            datasets: [{
                label: 'Top Products',
                data: [300, 250, 200]
            }]
        }
    });
}
