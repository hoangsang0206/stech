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

let typingTimeOut;
$('#search-list-products').keyup(() => {
    clearTimeout(typingTimeOut);
    
    typingTimeOut = setTimeout(() => {
        const searchValue = $('#search-list-products').val();

        $.ajax({
            type: 'GET',
            url: `/api/admin/products/search-by-id-or-name/${searchValue}`,
            success: (response) => {
                $('.product-table-list-result').empty();
                
                response.data.products.forEach(product => {
                    const total_qty = product.warehouseProducts.reduce((total, item) => total + item.quantity, 0);

                    $('.product-table-list-result').append(`
                        <tr>
                            <td>${product.productId}</td>
                            <td>${product.productName}</td>
                            <td>${product.price.toLocaleString('vi-VN')}đ</td>
                            <td>${total_qty}</td>
                            <td>
                                <button class="page-table-btn btn-blue click-select-product" 
                                    data-product="${product.productId}"
                                    data-price="${product.price}"
                                    data-product-name="${product.productName}">
                                    <i class="fa-solid fa-plus"></i>
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: () => {
                $('.product-table-list-result').empty();
            }
        })
        
    }, 300);
});


let selectedProducts = [];

const updateSelectedProductList = () => {
    $('.selected-product-list').empty();
    selectedProducts.map(item => {
        $('.selected-product-list').append(`
            <tr data-product="${item.productId}" data-qty="${item.quantity}" data-price="${item.unitPrice}">
                <td title="${item.productName}" class="text-nowrap">${item.productId}</td>
                <td>${item.quantity}</td>
                <td>${(item.unitPrice).toLocaleString('vi-VN')}đ</td>
                <td><i class="fa-solid fa-pen-to-square edit-import-item" data-product="${item.productId}"></i></td>
            </tr>
        `);
    })

    $('.selected-product-list').append(`
        <tr>
            <td class="fw-bold text-end">Tổng cộng:</td>
            <td class="fweight-600">${selectedProducts.reduce((total, item) => total + item.quantity, 0)}</td>
            <td class="fweight-600">${(selectedProducts.reduce((total, item) => total + item.unitPrice, 0)).toLocaleString('vi-VN')}đ</td>
        </tr>
        
        <tr>
            <td colspan="2" class="fw-bold text-end">Tổng tiền:</td>
            <td class="fweight-600">${selectedProducts.reduce((total, item) => total + item.quantity * item.unitPrice, 0).toLocaleString('vi-VN')}đ</td>
        </tr>
    `);
}

$(document).on('click', '.click-select-product', function () { 
    const productId = $(this).data('product');
    const productName = $(this).data('product-name');
    const price = $(this).data('price');

    const selectedProduct = selectedProducts.find(item => item.productId === productId);
    
    if (selectedProduct) {
        selectedProduct.quantity++;
    } else {
        selectedProducts.push({
            productId: productId,
            productName: productName,
            quantity: 1,
            unitPrice: price
        });
    }
    
    updateSelectedProductList();
})

$(document).on('click', '.edit-import-item', function () {
    const productId = $(this).data('product');
    const selectedProduct = selectedProducts.find(item => item.productId === productId);
    
    const editElement = $(this).closest('tr');
    const editQty = selectedProduct.quantity;
    const editPrice = selectedProduct.unitPrice;
    
    const editHtml = `
        <td title="${selectedProduct.productName}" class="text-nowrap">${productId}</td>
        <td>
            <div class="page-input"><input type="number" value="${editQty}" min="1" required></div>
        </td>
        <td>
            <div class="page-input"><input type="number" class="form-control" value="${editPrice}" min="1" required></div>
        </td>
        <td><i class="fa-solid fa-check confirm-edit-import-item"></i></td>
    `;
    
    editElement.html(editHtml);
})

$(document).on('click', '.confirm-edit-import-item', function () {
    const editElement = $(this).closest('tr');
    const productId = editElement.data('product');
    const selectedProduct = selectedProducts.find(item => item.productId === productId);
    
    const qty = parseInt(editElement.find('input[type="number"]').eq(0).val());
    const price = parseInt(editElement.find('input[type="number"]').eq(1).val());
    
    if (qty <= 0 || price <= 0) { 
        showDialog('error', 'Lỗi nhập liệu', 'Số lượng và giá phải lớn hơn 0');
        return;
    }
    
    if (isNaN(qty) || isNaN(price)) {
        showDialog('error', 'Lỗi nhập liệu', 'Số lượng và giá phải là số');
        return;
    }
    
    selectedProduct.quantity = qty;
    selectedProduct.unitPrice = price;
    
    const newHtml = `
        <td>${productId}</td>
        <td>${qty}</td>
        <td>${price.toLocaleString('vi-VN')}đ</td>
        <td><i class="fa-solid fa-pen-to-square edit-import-item" data-product="${productId}"></i></td>
    `;
    
    editElement.html(newHtml);
    updateSelectedProductList();
})

$('.create-import').click(() => {
    const warehouseId = $('#warehouse_id').val();
    const supplierId = $('#supplier_id').val();
    
    if (!warehouseId || !supplierId) {
        showDialog('error', 'Dữ liệu không hợp lệ.', 'Vui lòng nhập đầy đủ thông tin.');
        return;
    }
    
    const note = $('#ImportNote').val() || null;

    const products = selectedProducts.map(({productName, ...rest}) => rest);

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
                    selectedProducts = [];
                    updateSelectedProductList();
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
