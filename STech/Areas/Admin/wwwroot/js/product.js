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

    const short_description = short_des_editor.root.innerHTML;
    const description = description_editor.root.innerHTML;

    const images = $('.product-image-box img').toArray().map((image) => {
        return {
            Id: $(image).data('image-id') || null,
            Status: $(image).data('status') || 'old',
            ImageSrc: $(image).attr('src'),
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
        Images: images,
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