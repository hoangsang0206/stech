const checkEditorEmpty = (editor) => {
    const content = editor.getText().trim();
    return content == null || content.length === 0;
}

const short_des_editor = new Quill('#short-description-editor', {
    theme: 'snow',
    placeholder: 'Mô tả ngắn cho sản phẩm',
    modules: {
        toolbar: [
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ list: 'ordered' }, { list: 'bullet' }],
            ['link'],
            ['clean'],
            [{ 'size': [] }],
            [{ 'align': [] }],
            [{ 'color': [] }, { 'background': [] }],
            [{ 'script': 'sub' }, { 'script': 'super' }],
        ],
    },
});

const description_editor = new Quill('#description-editor', {
    theme: 'snow',
    placeholder: 'Mô tả sản phẩm',
    modules: {
        toolbar: [
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ list: 'ordered' }, { list: 'bullet' }],
            ['link', 'image', 'video'], 
            ['clean'],
            [{ 'size': [] }],
            [{ 'align': [] }],
            [{ 'color': [] }, { 'background': [] }],
            [{ 'script': 'sub' }, { 'script': 'super' }],
        ],
    },
});;

function updateIframeStyles() {
    var iframes = document.querySelectorAll('.ql-editor iframe');
    iframes.forEach(function (iframe) {
        iframe.style.width = '100%';
        iframe.style.height = '100%';
        iframe.style.aspectRatio = '16/9';
    });
}

updateIframeStyles();

description_editor.on('text-change', () => {
    updateIframeStyles();
})

$(document).on('click', '.ql-editor img', function () {
    $(this).closest('p').remove();
})

$(document).on('click', '.click-delete-image', function () {
    const container = $(this).closest('.product-image-box');
    const img = container.find('img');
    if (img.data('image-id')) {
        img.data('status', 'deleted');
        container.hide();
    } else {
        container.remove();
    }
})

$('.click-upload-image').click(function () {
    $('#upload-product-images').click();
})

$('#upload-product-images').on('change', function (_e) {
    const files = _e.target.files;

    for (const file of files) {
        const reader = new FileReader();
        reader.readAsDataURL(file);

        reader.onload = (e) => {
            $('.product-images-container').append(`
                <div class="product-image-box position-relative box-shadow">
                    <img src="${e.target.result}" alt="" style="width: 100%" />

                    <a class="click-delete-image" href="javascript:">
                        <i class="fa-regular fa-trash-can"></i>
                    </a>
                 </div>
            `)
        }
    }
});

$(document).on('click', '.click-delete-spec', function () {
    $(this).closest('.product-spec').remove();
})

$('.click-add-spec').click(() => {
    $('.product-specs-container').append(`
        <div class="product-spec row m-0 mb-2 p-0">
            <div class="col-2 p-0 pe-2">
                <div class="page-input">
                    <textarea name="spec-name" placeholder="Tên thông số"></textarea>
                </div>
            </div>
            <div class="col-9 p-0">
                <div class="page-input">
                    <textarea name="spec-value" placeholder="Giá trị thông số"></textarea>
                </div>
            </div>
            <div class="col-1 p-0 d-flex align-items-center justify-content-end">
                <a href="javascript:" class="click-delete-spec">
                    <i class="fa-regular fa-trash-can" style="font-size: 1.2rem; color: #e30019"></i>
                </a>
            </div>
        </div>
    `);
})

//Submit
$('#update-product').submit((e) => {
    e.preventDefault();

    const product_id = $('#product_id').val();
    const product_name = $('#product_name').val();
    const price = parseFloat($('#product_price').val()) || null;
    const original_price = parseFloat($('#product_original_price').val()) || null;
    const manufactured_year = parseInt($('#product_manufactured_year').val()) || null;
    const warranty = parseInt($('#product_warranty').val()) || null;
    const brand_id = $('#product_brand').val();
    const category_id = $('#product_category').val();

    const short_description = !checkEditorEmpty(short_des_editor) ? short_des_editor.root.innerHTML : null;
    const description = !checkEditorEmpty(description_editor) ? description_editor.root.innerHTML : null;

    const images = $('.product-image-box img').toArray().map((image) => {
        return {
            Id: $(image).data('image-id') || null,
            Status: $(image).data('status') || 'old',
            ImageSrc: $(image).attr('src'),
        }
    });

    if (images == null || !images.length) {
        showDialog('warning', 'Chưa chọn ảnh', 'Vui lòng chọn ít nhất 1 ảnh cho sản phẩm');
        return;
    }

    const specifications = $('.product-spec').toArray().map((spec) => {
        return {
            Name: $(spec).find('textarea[name="spec-name"]').val() || null,
            Value: $(spec).find('textarea[name="spec-value"]').val() || null
        }
    });

    const data = {
        ProductId: product_id,
        ProductName: product_name,
        Price: price,
        OriginalPrice: original_price,
        ManufacturedYear: manufactured_year,
        Warranty: warranty,
        BrandId: brand_id,
        CategoryId: category_id,
        ShortDescription: short_description,
        Description: description,
        Images: images || null,
        Specifications: specifications || null
    }

    //console.log(data);

    showWebLoader();
    $.ajax({
        type: 'PUT',
        url: '/api/admin/products/update',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('info', 'Cập nhật thành công', 'Đã cập nhật thông tin sản phẩm này', () => {
                    window.location.reload();
                });
            } else {
                showDialog('error', 'Không thể cập nhật', response.message);
            }

            hideWebLoader(300);
        },
        error: () => {
            showErrorDialog();
            hideWebLoader(0);
        }
    })
})

$('.click-delete-product').click(function () {
    const id = $(this).data('product');

    showConfirmDialog('Xóa sản phẩm này?', 'Hành động này không thể hoàn tác', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/delete/1/${id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('info', 'Xóa thành công', 'Đã xóa sản phẩm này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể xóa sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể xóa sản phẩm', null);
            }
        })
    })
}) 

$('.click-restore-product').click(function () {
    const id = $(this).data('product');

    showConfirmDialog('Khôi phục phẩm này?', '', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/restore/1/${id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('info', 'Khôi phục thành công', 'Đã khôi phục sản phẩm này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể khôi phục sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể khôi phục sản phẩm', null);
            }
        })
    })
}) 

$('.click-activate-product').click(function () {
    const id = $(this).data('product');

    showConfirmDialog('Hiện sản phẩm này?', '', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/activate/1/${id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('info', 'Hiện sản phẩm thành công', 'Đã hiện sản phẩm này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể hiện sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể hiện sản phẩm', null);
            }
        })
    })
}) 

$('.click-deactivate-product').click(function () {
    const id = $(this).data('product');

    showConfirmDialog('Ẩn sản phẩm này?', '', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/deactivate/1/${id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('info', 'Ẩn sản phẩm thành công', 'Đã ẩn sản phẩm này', () => {
                        window.location.reload();
                    });
                } else {
                    showDialog('error', 'Không thể ẩn sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể ẩn sản phẩm', null);
            }
        })
    })
}) 
