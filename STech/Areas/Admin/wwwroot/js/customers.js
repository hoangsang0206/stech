const isCustomerPage = $('.page-wrapper').data('page') === 'customer';

$('.add-customer form').submit(function (e) {
    e.preventDefault();

    const customer_name = $(this).find('#CustomerName').val();
    const customer_email = $(this).find('#Email').val();
    const customer_phone = $(this).find('#Phone').val();
    const gender = $(this).find('input[name="Gender"]').val();

    const city = $(this).find('.city-select').val();
    const district = $(this).find('.district-select').val();
    const ward = $(this).find('.ward-select').val();
    const address = $(this).find('.specific-address').val();

    const submit_btn = $(this).find('button[type="submit"]');
    const element_html = showButtonLoader(submit_btn, '23px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/admin/customers/create',
        contentType: 'application/json',
        data: JSON.stringify({
            CustomerName: customer_name,
            Email: customer_email || null,
            Phone: customer_phone,
            Gender: gender,
            CityCode: city,
            DistrictCode: district,
            WardCode: ward,
            Address: address
        }),
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('success', 'Thêm thành công', response.message, () => {
                    if (isCustomerPage) {
                        location.reload();
                    }
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

$('.show-form-add-customer').click(() => {
    showForm('.add-customer');
})

$(document).on('click', '.edit-customer', function () {
    showForm('.update-customer');
})

$(document).on('click', '.delete-customer', function () {
    const customer_id = $(this).data('customer');

    showConfirmDialog('Xóa khách hàng này?', 'Hành động này không thể hoàn tác', () => {
        showWebLoader();
        $.ajax({
            type: 'DELETE',
            url: `/api/admin/customers/${customer_id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('info', 'Xóa thành công', 'Đã xóa khách hàng này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể xóa khách hàng', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể xóa khách hàng', null);
            }
        })
    })
})
