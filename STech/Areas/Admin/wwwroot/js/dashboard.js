const renderChart = () => {
    const ctx = document.getElementById('revenue-chart');
    $.ajax({
        type: 'GET',
        url: '/api/statistics/last-six-month-order-summary',
        success: (response) => {
            const labels = response.data.map(item => {
                return `${item.month}/${item.year}`;
            });

            const orders = response.data.map(item => {
                return item.totalOrders
            });

            const revenue = response.data.map(item => {
                return item.totalRevenue
            });

            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Doanh thu',
                        data: revenue,
                        borderWidth: 1,
                        yAxisID: 'y'
                    },
                    {
                        label: 'Số đơn hàng',
                        data: orders,
                        borderWidth: 1,
                        yAxisID: 'y1'
                    }]
                },
                options: {
                    y: {
                        type: 'linear',
                        position: 'left',
                        title: {
                            display: true,
                            text: 'Doanh thu'
                        }
                    },
                    y1: {
                        type: 'linear',
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Số đơn hàng'
                        },
                        grid: {
                            drawOnChartArea: false
                        }
                    }
                }
            });
        },
        error: (error) => { }
    })
}

$(document).ready(() => {
    renderChart();
})