// Create import
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
