const formatCurrency = (amout) => {
    const _amout = parseFloat(amout);

    if (_amout === 0) {
        return '0đ';
    }

    return _amout.toLocaleString('vi-VN') + 'đ';
}

const isOrderDetailPage = $('[data-page="order-detail"]').length > 0 || false;

const tippyButtons = () => {
    if (isOrderDetailPage) {
        return;
    }

    tippy('.view-order', {
        content: 'Xem chi tiết',
        placement: 'top'
    })

    tippy('.print-order', {
        content: 'In hóa đơn',
        placement: 'top'
    })

    tippy('.accept-order', {
        content: 'Xác nhận đơn hàng',
        placement: 'top'
    })
}

$(document).ready(() => {
    tippyButtons();
})

const updateOrderList = (invoices) => { 
    $('.order-list').empty();

    let index = 0;
    invoices.forEach((invoice) => {
        index++;

        let payment_status = ``;
        let order_status = ``;

        const total_items = invoice.invoiceDetails.reduce((accumulator, item) => {
            return accumulator + item.quantity;
        }, 0);

        switch (invoice.paymentStatus) {
            case "paid":
                payment_status = '<span class="page-badge badge-success">Đã thanh toán</span>';
                break;

            case "unpaid":
                payment_status = '<span class="page-badge badge-warning">Chờ thanh toán</span>';
                break;

            case "payment-failed":
                payment_status = '<span class="page-badge badge-error">Thanh toán thất bại</span>';
                break;

        }

        if (invoice.isCancelled) {
            order_status = '<span class="page-badge badge-error">Đã hủy</span>';
        }
        else if (invoice.isCompleted) {
            order_status = '<span class="page-badge badge-success">Đã giao hàng</span>';
        }
        else if (invoice.isAccepted) {
            order_status = '<span class="page-badge badge-warning">Chờ giao hàng</span>';
        }
        else {
            order_status = '<span class="page-badge badge-warning">Chờ xác nhận</span>';
        }

        $('.order-list').append(`
            <tr>
                <td class="fweight-600">${index}</td>
                <td>${invoice.invoiceId}</td>
                <td>${formatDateTime(invoice.orderDate)}</td>
                <td>${total_items}</td>
                <td class="fweight-600">${formatCurrency(invoice.total)}</td>
                <td>${invoice.paymentMed.paymentName}</td>
                <td>${payment_status}</td>
                <td>${order_status}</td>
                <td>
                    <div class="d-flex gap-1 align-items-center justify-content-end">
                        <button class="page-table-btn btn-lightblue view-order" data-order="${invoice.invoiceId}">
                            <i class="fa-solid fa-eye"></i>
                        </button>
                        <button class="page-table-btn btn-blue print-order" data-order="${invoice.invoiceId}">
                            <i class="fa-solid fa-print"></i>
                        </button>
                        ${!invoice.isAccepted && !invoice.isCancelled && !invoice.isCompleted ?
                            `<button class="page-table-btn btn-green accept-order" data-order="${invoice.invoiceId}">
                                <i class="fa-solid fa-check"></i>
                            </button>` : ''}
                    </div>
                </td>
            </tr>
        `);
    });

    tippyButtons();
}

const loadOrders = (page, filer_by, sort_by) => {
    if (isOrderDetailPage) {
        return;
    }

    $('#search').val(null);

    updateUrlPath(`/admin/orders`);
    updateParams({
        page: page,
        filter_by: filer_by,
        sort_by: sort_by
    });

    showWebLoader();

    $.ajax({
        url: `/api/admin/orders?page=${page}&filter_by=${filer_by}&sort_by=${sort_by}`,
        type: 'GET',
        success: (response) => {
            hideWebLoader(500);

            if (response.status) {
                loadPagination(response.data.totalPages, response.data.currentPage);
                updateOrderList(response.data.invoices);
            }
        },
        error: (xhr, status, error) => {
            hideWebLoader(0);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            } else {
                showDialog('error', 'Đã xảy ra lỗi', 'Không thể tải danh sách đơn hàng');
            }
        }
    })
}

$('.orders-nav').click(function () {
    const filter_by = $(this).data('load-orders') ?? '';
    loadOrders(1, filter_by, '');
})

$(document).on('click', '.pagination-item:not(.current)', function () {
    const page = $(this).data('page') || '1';
    const current_page = parseInt($('.pagination-item.current').data('page') || '1');
    const filter_by = $('.orders-nav.active').data('load-orders') || '';

    if (page === 'next') {
        loadOrders(current_page + 1, filter_by, '');
    } else if (page === 'previous') {
        loadOrders(current_page - 1, filter_by, '');
    } else {
        loadOrders(page, filter_by, '');
    }
})


$(document).on('click', '.accept-order', function () {
    showConfirmDialog('Xác nhận đơn hàng', 'Bạn có chắc chắn muốn xác nhận đơn hàng này không?', () => {
        const current_page = parseInt($('.pagination-item.current').data('page') || '1');
        const filter_by = $('.orders-nav.active').data('load-orders') || '';
        const order_id = $(this).data('order');

        if (order_id) {
            showWebLoader();

            $.ajax({
                type: 'PATCH',
                url: `/api/admin/orders/accept/${order_id}`,
                success: (response) => {
                    hideWebLoader();

                    if (response.status) {
                        if (isOrderDetailPage) {
                            showDialogWithCallback('success', 'Thành công', 'Đã xác nhận đơn hàng', () => {
                                location.reload();
                            });
                        } else {
                            loadOrders(current_page, filter_by, '');
                            showDialog('success', 'Thành công', 'Đã xác nhận đơn hàng');
                        }

                    } else {
                        showDialog('error', 'Đã xảy ra lỗi', response.message);
                    }
                },
                error: (xhr, status, error) => {
                    hideWebLoader(0);
                    if (xhr.status === 401) {
                        showUnauthorizedDialog();
                    } else {
                        showErrorDialog();
                    }
                }
            });
        }
    });
})

$(document).on('click', '.print-order', async function () {
    const order_id = $(this).data('order');

    if (order_id) {
        try {
            const response = await fetch(`/api/admin/orders/print-invoice/${order_id}`);

            if (!response.ok) {
                throw new Error('Đã xảy ra lỗi');
            }

            const blob = await response.blob();
            // const url = URL.createObjectURL(blob);
            // const a = document.createElement('a');
            //
            // a.style.display = 'none';
            // a.href = url;
            // a.download = `HĐ_${order_id}.pdf`;
            //
            // document.body.appendChild(a);
            // a.click();
            //
            // URL.revokeObjectURL(url);
            // document.body.removeChild(a);
            
            printBlobPdf(blob);
        } catch (error) {
            showDialog('error', 'Không thể tải hóa đơn', error);
        }
    }
})

$('.search-orders').submit(function (e) {
    e.preventDefault();

    const value = $(this).find('#search').val();

    if (value) {
        showWebLoader();
        updateUrlPath(`/admin/orders/search/${value}`);
        $.ajax({
            type: 'GET',
            url: `/api/admin/orders/search/${value}`,
            success: (response) => {
                if (response.status) {
                    $('.orders-nav').removeClass('active')
                    hideWebLoader(500);
                    updateOrderList(response.data);
                    loadPagination(1, 1);
                }
            },
            error: (xhr, status, error) => {
                hideWebLoader(0);
                if (xhr.status === 401) {
                    showUnauthorizedDialog();
                } else {
                    showErrorDialog();
                }
            }
        })
    }
})


$(document).on('click', '.view-order', function () {
    const order_id = $(this).data('order');
    window.location.href = `/admin/orders/detail?id=${order_id}`;
})

$(document).on('click', '.cancel-order', function () {
    showConfirmDialog('Hủy đơn hàng', 'Bạn có chắc chắn muốn hủy đơn hàng này không?', () => {
        const current_page = parseInt($('.pagination-item.current').data('page') || '1');
        const filter_by = $('.orders-nav.active').data('load-orders') || '';
        const order_id = $(this).data('order');

        if (order_id) {
            showWebLoader();

            $.ajax({
                type: 'PATCH',
                url: `/api/admin/orders/cancel/${order_id}`,
                success: (response) => {
                    hideWebLoader();

                    if (response.status) {
                        if (isOrderDetailPage) {
                            showDialogWithCallback('success', 'Thành công', 'Đã hủy đơn hàng', () => {
                                location.reload();
                            });
                        } else {
                            loadOrders(current_page, filter_by, '');
                            showDialog('success', 'Thành công', 'Đã hủy đơn hàng');
                        }
                    } else {
                        showDialog('error', 'Đã xảy ra lỗi', response.message);
                    }
                },
                error: (xhr, status, error) => {
                    hideWebLoader(0);
                    if (xhr.status === 401) {
                        showUnauthorizedDialog();
                    } else {
                        showErrorDialog();
                    }
                }
            });
        }
    });
})

$(document).on('click', '.confirm-completed', function () {
    showConfirmDialog('Xác nhận giao hàng', 'Xác nhận đơn hàng này đã được thanh toán và giao hàng thành công?', () => {
        const current_page = parseInt($('.pagination-item.current').data('page') || '1');
        const filter_by = $('.orders-nav.active').data('load-orders') || '';
        const order_id = $(this).data('order');

        if (order_id) {
            showWebLoader();

            $.ajax({
                type: 'PATCH',
                url: `/api/admin/orders/completed/${order_id}`,
                success: (response) => {
                    hideWebLoader();

                    if (response.status) {
                        if (isOrderDetailPage) {
                            showDialogWithCallback('success', 'Thành công', 'Đã xác nhận hoàn thành đơn hàng', () => {
                                location.reload();
                            });
                        } else {
                            loadOrders(current_page, filter_by, '');
                            showDialog('success', 'Thành công', 'Đã xác nhận hoàn thành đơn hàng');
                        }
                    } else {
                        showDialog('error', 'Đã xảy ra lỗi', response.message);
                    }
                },
                error: (xhr, status, error) => {
                    hideWebLoader(0);
                    if (xhr.status === 401) {
                        showUnauthorizedDialog();
                    } else {
                        showErrorDialog();
                    }
                }
            });
        }
    });
})