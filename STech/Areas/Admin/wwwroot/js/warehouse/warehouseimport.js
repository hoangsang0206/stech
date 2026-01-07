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
        url: '/api/admin/warehouses/import',
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
                    <button class="page-table-btn btn-lightblue">
                        <i class="fa-solid fa-circle-info"></i>
                    </button>
                </td>
            </tr>
        `);
        
        i++;
    })
}

$('.filter-submit').click(() => {
    getImportList();
})