const isCustomerPage = $('.page-wrapper').data('page') === 'customer';

$('.form-customer form').submit(function (e) {
    e.preventDefault();

    const isUpdate = $(this).data('action') === 'update';
    const customerApiUrl = isUpdate ? `/api/admin/customers/update` : '/api/admin/customers/create';

    const customer_id = $(this).data('customer');
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
        type: `${ isUpdate ? 'PUT' : 'POST'}`,
        url: customerApiUrl,
        contentType: 'application/json',
        data: JSON.stringify({
            CustomerName: customer_name,
            Email: customer_email || null,
            Phone: customer_phone,
            Gender: gender,
            CityCode: city,
            DistrictCode: district,
            WardCode: ward,
            Address: address,
            ...(isUpdate && { CustomerId: customer_id })
        }),
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('success', isUpdate ? 'Cập nhật thành công' : 'Thêm thành công', response.message, () => {
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
        error: (xhr, status, error) => {
            hideButtonLoader(submit_btn, element_html);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            }
        }
    })
})

$('.show-form-add-customer').click(() => {
    showForm('.form-customer');
    $('.form-customer form').data('action', 'create');
    $('.form-customer .form-header').html('Thêm mới khách hàng');
    $('.form-customer form').find('button[type="submit"]').text('Thêm mới');
})

$(document).on('click', '.edit-customer', function () {
    showForm('.form-customer');

    const customer_id = $(this).data('customer');

    $('.form-customer form').data('action', 'update');
    $('.form-customer form').data('customer', customer_id);
    $('.form-customer .form-header').html('Cập nhật thông tin khách hàng');
    $('.form-customer form').find('button[type="submit"]').text('Cập nhật');

    $.ajax({
        type: 'GET',
        url: `/api/admin/customers/1/${customer_id}`,
        success: (response) => {
            if (response.status) {
                const customer = response.data;

                $('#CustomerName').val(customer.customerName).trigger('change');
                $('#Email').val(customer.email).trigger('change');
                $('#Phone').val(customer.phone).trigger('change');
                $(`input[name="Gender"][value="${customer.gender}"]`).click();

                $('.city-select').val(customer.provinceCode);
                $('.specific-address').val(customer.address).trigger('change');

                loadDistricts($('.form-customer form'), customer.provinceCode, customer.districtCode);
                loadWards($('.form-customer form'), customer.districtCode, customer.wardCode);
            } else {
                showDialog('error', response.message, null);
            }
        },
        error: (xhr, status, error) => {
            hideWebLoader(0);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            }
        }
    })
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
                    showDialogWithCallback('success', 'Xóa thành công', 'Đã xóa khách hàng này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể xóa khách hàng', response.message);
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
    })
})
