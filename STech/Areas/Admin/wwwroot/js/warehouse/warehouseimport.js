$('.create-import').click(() => {
    const warehouseId = $('#warehouse_id').val();
    const supplierId = $('#supplier_id').val();

    if (!warehouseId || !supplierId) {
        showDialog('error', 'Dữ liệu không hợp lệ.', 'Vui lòng nhập đầy đủ thông tin.');
        return;
    }

    const note = $('#ImportNote').val() || null;

    const products = getSelectedProducts();

    if (!products || products.length <= 0) {
        showDialog('error', 'Dữ liệu không hợp lệ', 'Vui lòng chọn sản phẩm cần nhập.');
        return;
    }

    const submit_btn = $('.create-import');
    const element_html = showButtonLoader(submit_btn, '23px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/admin/warehouses/import/create',
        contentType: 'application/json',
        data: JSON.stringify({
            note: note,
            warehouseId: warehouseId,
            supplierId: supplierId,
            warehouseImportDetails: products,
        }),
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('success', 'Nhập kho thành công', response.message, () => {
                    $(".selected-product-list").empty();
                });
            } else {
                showDialog('error', 'Đã xảy ra lỗi', response.message);
            }

            hideButtonLoader(submit_btn, element_html);
        },
        error: (xhr, status, error) => {
            hideButtonLoader(submit_btn, element_html);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            }
        }
    })
})


activeDateRangePicker((start, end) => {
    
})

const getImportList = () => {
    const importId = $('.filter-import-id').val();

    const warehouseId = $('.filter-select-warehouse').val();
    const supplierId = $('.filter-select-supplier').val();
    const date = getDateRangePickerValue();
    const productId = $('.filter-prod-id').val();
    const staffId = $('.filter-staff-id').val();

    const sort = getDropdownSelectedValue('.sort-selection');
    const status = getDropdownSelectedValue('.status-selection');
    
    let dateRange;
    
    if (date && date.start && date.end) {
        dateRange = date.start + ' - ' + date.end;
    }
    
    const data = {
        id: importId,
        wId: warehouseId,
        sId: supplierId,
        eId: staffId,
        pId: productId,
        date: dateRange,
        sort: sort,
        status: status,
    }
  
    showWebLoader();
    $.ajax({
        type: 'GET',
        url: '/api/admin/warehouses/import-list',
        data: data,
        success: (response) => {
            renderImportList(response.data.items);
            hideWebLoader(500);
        },
        error: (xhr, status, error) => {
            showErrorDialog();
            hideWebLoader(500);
        }
    })

    updateParams(data);
}

$('.filter-import-id').focus(() => {
    $('.page-input input, .page-input select').not('.filter-import-id').prop('disabled', true);
})


$('.filter-import-id').blur(() => {
    const value = $('.filter-import-id').val();
    
    if (!value) {
        $('.page-input input, .page-input select').prop('disabled', false);
    }
})

const tippyButtons = () => {
    tippy('.view-import-detail', {
        content: 'Xem chi tiết',
        placement: 'top'
    })

    tippy('.accept-import', {
        content: 'Xác nhận hoàn thành',
        placement: 'top'
    })
}
tippyButtons();

const renderImportList = (data) => {
    const element = $('.import-list');
    let i = 1;
    element.empty();
    
    data.forEach((item) => {
        let statusHtml = `<span class=\"page-badge badge-warning\">Chờ xác nhận</span>`;

        if (item.status === 'completed')
        {
            statusHtml = `<span class=\"page-badge badge-success\">Hoàn thành</span>`;
        }
        else if (item.status === 'cancelled')
        {
            statusHtml = `<span class=\"page-badge badge-error\">Đã hủy</span>`;
        }

        element.append(`
            <tr>
                <td>${i}</td>
                <td>${item.wiid}</td>
                <td>${formatDateTime(item.dateCreate)}</td>
                <td>${formatDateTime(item.dateImport)}</td>
                <td>${item.warehouseImportDetails.reduce((sum, t) => sum + t.quantity, 0)}</td>
                <td>${formatCurrency(item.warehouseImportDetails.reduce((sum, t) => sum + (t.quantity * t.unitPrice), 0))}</td>
                <td>${statusHtml}</td>
                <td>
                    <div class="d-flex gap-1 justify-content-end">
                        <button class="page-table-btn btn-lightblue view-import-detail" 
                                onclick="window.location.href='/admin/warehouses/import/detail/${item.wiid}'">
                            <i class="fa-solid fa-circle-info"></i>
                        </button>
                    
                        ${
                            item.status === 'waiting' ?
                            `<button class="page-table-btn btn-green accept-import" data-id="${item.wiid}">
                                <i class="fa-solid fa-check"></i>
                            </button>` : ''
                        }
                    </div>
                </td>
            </tr>
        `);
        
        i++;
    })
    
    tippyButtons();
}

$('.filter-submit').click(() => {
    getImportList();
})

$(document).ready(() => {
    const params = new URLSearchParams(window.location.search);
    const sort = params.get('sort');
    const status = params.get('status');
    
    activeDropdown('.sort-selection', sort);
    activeDropdown('.status-selection', status);
})

$(document).on('click', '.accept-import', function() {
    const id = $(this).data('id');
    
    if (id) {
        showConfirmDialog('Xác nhận hoàn thành', `Xác nhận đã hoàn thành cho phiếu nhập ${id} ?`,
            () => {
                showWebLoader();

                $.ajax({
                    type: 'PATCH',
                    url: `/api/admin/warehouses/import/accept/${id}`,
                    success: (response) => {
                        if (response.status) {
                            showDialogWithCallback('success', 'Hoàn tất', `Đã xác nhận hoàn thành phiếu nhập ${id}.`,
                                () => { getImportList() });
                        } else {
                            showDialog('error', 'Không tìm thấy', `Phiếu nhập ${id} không tồn tại.`);
                        }
                        
                        hideWebLoader(500);
                    },
                    error: (xhr, status, error) => {
                        showErrorDialog();
                        hideWebLoader(500);
                    }
                })
            });
    }
})