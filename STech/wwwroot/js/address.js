$('#city-select').on('change', function () {
    const cityCode = $(this).val();
    if (cityCode) {
        $('#district-select').empty();
        $('#district-select').append('<option value="" selected>Chọn quận/huyện</option>');
        $('#ward-select').empty();
        $('#ward-select').append('<option value="" selected>Chọn phường/xã</option>');

        $.ajax({
            type: 'GET',
            url: `/api/address/districts/${cityCode}`,
            success: (response) => {
                if (response) {
                    response.map(district => {
                        $('#district-select').append(`<option value="${district.code}">${district.name_with_type}</option>`);
                    });
                }
            }
        })
    }
})

$('#district-select').on('change', function () {
    const districtCode = $(this).val();
    if (districtCode) {
        $('#ward-select').empty();
        $('#ward-select').append('<option value="" selected>Chọn phường/xã</option>');

        $.ajax({
            type: 'GET',
            url: `/api/address/wards/${districtCode}`,
            success: (response) => {
                if (response) {
                    response.map(ward => {
                        $('#ward-select').append(`<option value="${ward.code}">${ward.name_with_type}</option>`);
                    });
                }
            }
        })
    }
})