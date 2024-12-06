tippy('.edit-warehouse', {
    content: 'Sửa thông tin kho',
    placement: 'top'
})

tippy('.delete-warehouse', {
    content: 'Xóa kho',
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