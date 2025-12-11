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
        error: (xhr, status, error) => {
            hideButtonLoader(submit_btn, element_html);
            if (xhr.status === 401) {
                showUnauthorizedDialog();
            }
        }
    })
})

$('.delete-warehouse').click(function () {
    const warehouseId = $(this).data('warehouse');
    
    showConfirmDialog('Xác nhận xóa kho hàng', 'Bạn có chắc chắn muốn xóa kho hàng này?', () => {
        showWebLoader();
        $.ajax({
            type: 'DELETE',
            url: `/api/admin/warehouses/${warehouseId}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('success', 'Xóa thành công', 'Đã xóa kho hàng này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể xóa kho hàng', response.message);
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

$('.page-search-form form').submit((e) => {
    e.preventDefault();
})

activeProductSearch(false);

const calculateTotalPrice = () => {
    const totalPrice = $('.selected-prod-item-total-price').toArray().reduce((total, item) => total + parseFloat($(item).data('item-total-price') || 0), 0);
    
    $('.total-price')
        .data('total', totalPrice)
        .text(`${(totalPrice).toLocaleString('vi-VN')}đ`);
}

const updateItemQuantity = (product, qty) => {
    const itemElement = $(`.selected-product-item[data-product="${product.productId}"]`);
    const selectedPrice = itemElement.find('.selected-prod-price').val();

    const price = () => {
        if (selectedPrice) {
            return parseFloat(selectedPrice) * qty;
        }

        return parseFloat(product.price) * qty
    }

    itemElement.data('qty', qty);
    itemElement.find('.selected-prod-qty').val(qty);
    itemElement.find('.selected-prod-item-total-price')
        .data('item-total-price', price())
        .text(`${price().toLocaleString('vi-VN')}đ`);

    calculateTotalPrice();
}

const removeItem = (product_id) => {
    $(`.selected-product-item[data-product="${product_id}"]`).remove();
    calculateTotalPrice();
}

$(document).on('change', 'input[name="search-product-item"]', function () {
    if ($(this).prop('checked')) {
        $('.search-list-product-results').removeClass('show');

        const total_products = parseInt($('.selected-product-item').length || 0);

        const product_id = $(this).val();
        const existed_product = $(`.selected-product-item[data-product="${product_id}"]`);

        showWebLoader();

        $.ajax({
            type: 'GET',
            url: `/api/admin/products/1/${product_id}`,
            success: (response) => {
                if (response.status) {
                    const product = response.data;

                    if (!existed_product.length) {
                        $('.selected-product-list').append(`
                            <tr class="selected-product-item" data-product="${product.productId}" data-qty="1">
                                <td class="py-2">${total_products + 1}</td>
                                <td class="py-2 ps-2">
                                    <div class="d-flex gap-1" style="max-width: 24rem">
                                        <div>
                                            <img src="${product.productImages[0].imageSrc || '/admin/images/no-image.jpg'}" alt="" style="width: 3rem" />
                                        </div>
                                        <div class="pe-2 overflow-hidden">
                                            <div class="text-overflow-1 fweight-500 w-100" style="font-size: .95rem; font-weight: 500" title="${product.productName} - ${product.productId}">${product.productName}</div>
                                            <div class="text-price" style="font-size: .95rem;">${product.price.toLocaleString('vi-VN')}đ</div>
                                        </div>
                                    </div>
                                </td>
                                <td class="py-2">
                                    <input type="number" min="1" value="1" class="table-input input-quantity selected-prod-qty" data-product="${product.productId}" />
                                </td>
                                <td class="py-2">
                                    <input style="width: 10rem" type="text" inputmode="decimal" pattern="[0-9,\\.]*" value="${product.price}" class="table-input selected-prod-price" data-product="${product.productId}" />
                                </td>
                                <td class="py-2">
                                    <div class="d-flex justify-content-end align-items-center gap-3">
                                        <div class="text-price selected-prod-item-total-price" style="font-size: .95rem;" data-item-total-price="${product.price}">${product.price.toLocaleString('vi-VN')}đ</div>
                                        <a href="javascript:void(0)" class="text-decoration-none text-danger remove-selected-item" data-product="${product.productId}">
                                            <i class="fa-regular fa-trash-can"></i>
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        `);
                    } else {
                        let qty = parseInt(existed_product.data('qty') || 1) + 1;
                        
                        updateItemQuantity(product, qty);
                    }
                    
                    hideWebLoader(300);
                } else {
                    hideWebLoader(0);
                    showDialog('error', 'Không thể thêm sản phẩm', 'Không tìm thấy sản phẩm này');
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

    $(this).prop('checked', false);
})

$(document).on('click', '.remove-selected-item', function () {
    const product_id = $(this).data('product');
    removeItem(product_id);
})

$(document).on('blur', '.selected-prod-qty', function () {
    const product_id = $(this).data('product');
    let qty = parseInt($(this).val() || 1);

    if (qty < 1) {
        qty = 1;
        $(this).val(qty);
    }

    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/products/1/${product_id}`,
        success: (response) => {
            if (response.status) {
                updateItemQuantity(response.data, qty);
                hideWebLoader(300);
            } else {
                hideWebLoader(0);
                showDialog('error', 'Không thể cập nhật số lượng', 'Không tìm thấy sản phẩm này');
                removeItem(product_id);
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

$(document).on('blur', '.selected-prod-price', function () {
    const product_id = $(this).data('product');

    const qty_element = $(`.selected-prod-qty[data-product="${product_id}"]`);
    let qty = parseInt($(qty_element).val() || 1);
    if (qty < 1) {
        qty = 1;
        $(qty_element).val(qty);
    }

    if (parseInt($(this).val()) < 0) {
        $(this).val(0);
    }

    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/products/1/${product_id}`,
        success: (response) => {
            if (response.status) {
                updateItemQuantity(response.data, qty);
                hideWebLoader(300);
            } else {
                hideWebLoader(0);
                showDialog('error', 'Không thể cập nhật giá', 'Không tìm thấy sản phẩm này');
                removeItem(product_id);
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

const getSelectedProducts = () => {
    const products = [];

    $('.selected-product-item').each(function () {
        const productId = $(this).data('product');
        const qty = parseInt($(this).find('.selected-prod-qty').val(), 10);
        const price = parseFloat($(this).find('.selected-prod-price').val());

        products.push({
            productId: productId,
            quantity: qty,
            unitPrice: price
        });
    });
    
    return products;
}

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