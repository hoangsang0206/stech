tippy('.edit-supplier', {
    content: 'Sửa thông tin NCC',
    placement: 'top'
})

tippy('.edit-supplier', {
    content: 'Xóa NCC',
    placement: 'top'
})

$('.click-add-supplier').click(() => {
    showForm('.form-supplier');
    $('.form-supplier form').data('action', 'create');
    $('.form-supplier .form-header').html('Thêm nhà cung cấp');
    $('.form-supplier form').find('button[type="submit"]').text('Thêm mới');
    $('.form-supplier form').find('#SupplierId').prop('disabled', false);
})

$('.edit-supplier').click(function () {
    showForm('.form-supplier');

    const form = $('.form-supplier form');

    form.data('action', 'update');
    $('.form-supplier .form-header').html('Sửa thông tin nhà cung cấp');
    form.find('button[type="submit"]').text('Cập nhật');
    form.find('#SupplierId').prop('disabled', true);

    const supplierId = $(this).data('supplier');

    $.ajax({
        type: 'GET',
        url: `/api/admin/suppliers/1/${supplierId}`,
        success: function (response) {
            if(response.status) {
                const supplier = response.data;
                form.find('#SupplierId').val(supplier.supplierId).trigger('change');
                form.find('#SupplierName').val(supplier.supplierName).trigger('change');
                form.find('#Address').val(supplier.address).trigger('change');
                form.find('#Phone').val(supplier.phone).trigger('change');
            } else {
                showDialog('error', 'Đã xảy ra lỗi', response.message);
            }
        }
    })
})

$('.form-supplier form').submit(function (e) {
    e.preventDefault();

    const isUpdate = $(this).data('action') === 'update';
    const supplierApiUrl = isUpdate ? `/api/admin/suppliers/update` : '/api/admin/suppliers/create';

    const supplierId = $(this).find('#SupplierId').val();
    const supplierName = $(this).find('#SupplierName').val();
    const supplierAddress = $(this).find('#Address').val();
    const supplierPhone = $(this).find('#Phone').val();

    const submit_btn = $(this).find('button[type="submit"]');
    const element_html = showButtonLoader(submit_btn, '23px', '4px');

    $.ajax({
        type: `${ isUpdate ? 'PUT' : 'POST'}`,
        url: supplierApiUrl,
        contentType: 'application/json',
        data: JSON.stringify({
            SupplierId: supplierId,
            SupplierName: supplierName,
            Address: supplierAddress,
            Phone: supplierPhone
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
        error: (xhr, status, error) => {
            hideButtonLoader(submit_btn, element_html);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            }
        }
    })
})

$('.delete-supplier').click(function () {
    const supplierId = $(this).data('supplier');

    showConfirmDialog('Xác nhận xóa nhà cung cấp', 'Bạn có chắc chắn muốn xóa nhà cung cấp này?', () => {
        showWebLoader();
        $.ajax({
            type: 'DELETE',
            url: `/api/admin/suppliers/${supplierId}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('success', 'Xóa thành công', 'Đã xóa nhà cung cấp này này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể xóa nhà cung cấp', response.message);
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
    });
})