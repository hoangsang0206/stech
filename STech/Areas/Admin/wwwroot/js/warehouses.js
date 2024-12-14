tippy('.edit-warehouse', {
    content: 'Sửa thông tin kho',
    placement: 'top'
})

tippy('.delete-warehouse', {
    content: 'Xóa kho',
    placement: 'top'
})

tippy('.view-inventory', {
    content: 'Tồn kho',
    placement: 'top'
})

tippy('.import-export-history', {
    content: 'Lịch sử nhập xuất',
    placement: 'top'
})

$('.show-add-warehouse').click(() => {
    showForm('.form-warehouse');
    $('.form-warehouse form').data('action', 'create');
    $('.form-warehouse .form-header').html('Thêm kho hàng');
    $('.form-warehouse form').find('button[type="submit"]').text('Thêm mới');
    $('.form-warehouse form').find('#WarehouseId').prop('disabled', false);
})

$('.edit-warehouse').click(function () { 
    showForm('.form-warehouse');
    
    const form = $('.form-warehouse form');
    
    form.data('action', 'update');
    $('.form-warehouse .form-header').html('Sửa thông tin kho hàng');
    form.find('button[type="submit"]').text('Cập nhật');
    form.find('#WarehouseId').prop('disabled', true);
    
    const warehouseId = $(this).data('warehouse');
    
    $.ajax({
        type: 'GET',
        url: `/api/admin/warehouses/1/${warehouseId}`,
        success: function (response) {
            if(response.status) {
                const warehouse = response.data;
                form.find('#WarehouseId').val(warehouse.warehouseId).trigger('change');
                form.find('#WarehouseName').val(warehouse.warehouseName).trigger('change');
                $('.city-select').val(warehouse.provinceCode);
                $('.specific-address').val(warehouse.address).trigger('change');

                loadDistricts(form, warehouse.provinceCode, warehouse.districtCode);
                loadWards(form, warehouse.districtCode, warehouse.wardCode);
            } else {
                showDialog('error', 'Đã xảy ra lỗi', response.message);
            }
        }
    })
})

$('.form-warehouse form').submit(function (e) {
    e.preventDefault();

    const isUpdate = $(this).data('action') === 'update';
    const warehouseApiUrl = isUpdate ? `/api/admin/warehouses/update` : '/api/admin/warehouses/create';

    const warehouse_id = $(this).find('#WarehouseId').val();
    const warehouse_name = $(this).find('#WarehouseName').val();

    const city = $(this).find('.city-select').val();
    const district = $(this).find('.district-select').val();
    const ward = $(this).find('.ward-select').val();
    const address = $(this).find('.specific-address').val();

    const submit_btn = $(this).find('button[type="submit"]');
    const element_html = showButtonLoader(submit_btn, '23px', '4px');

    $.ajax({
        type: `${ isUpdate ? 'PUT' : 'POST'}`,
        url: warehouseApiUrl,
        contentType: 'application/json',
        data: JSON.stringify({
            WarehouseId: warehouse_id,
            WarehouseName: warehouse_name,
            ProvinceCode: city,
            DistrictCode: district,
            WardCode: ward,
            Address: address
        }),
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('success', isUpdate ? 'Cập nhật thành công' : 'Thêm thành công', response.message, () => {
                    location.reload();
                });
                clearFormInput($(this));
                closeForm($(this).closest('.form-container'));
            } else {
                showDialog('error', 'Đã xảy ra lỗi', response.message);
            }

            hideButtonLoader(submit_btn, element_html);
        },
        error: () => {
            hideButtonLoader(submit_btn, element_html);
        }
    })
})