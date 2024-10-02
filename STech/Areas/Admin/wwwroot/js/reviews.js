const tippyButtons = () => {
    tippy('.view-review', {
        content: 'Chi tiết',
        placement: 'top'
    })

    tippy('.delete-review', {
        content: 'Xóa đánh giá',
        placement: 'top'
    })

    tippy('.approve-review', {
        content: 'Duyệt',
        placement: 'top'
    })
}

tippyButtons();

$(document).on('click', '.approve-review', function () {
    showConfirmDialog('Phê duyệt đánh giá', 'Xác nhận duyệt đánh giá này?', () => {
        const review_id = $(this).data('review');

        $.ajax({
            type: 'PATCH',
            url: `/api/admin/reviews/approve/${review_id}`,
            success: (response) => {
                if (response.status) {
                    showDialogWithCallback('success', 'Đã phê duyệt đánh giá này', '', () => {
                        loadReviews(1);
                    })
                } else {
                    showDialog('error', 'Không thể duyệt đánh giá', response.message);
                }
            },
            error: () => {

            }
        })
    })
})

$(document).on('click', '.delete-review', function () {
    showConfirmDialog('Xóa đánh giá', 'Xác nhận xóa đánh giá này?', () => {
        const review_id = $(this).data('review');

        $.ajax({
            type: 'DELETE',
            url: `/api/admin/reviews/${review_id}`,
            success: (response) => {
                if (response.status) {
                    showDialogWithCallback('success', 'Xóa đánh giá thành công', '', () => {
                        loadReviews(1);
                    })
                } else {
                    showDialog('error', 'Không thể xóa đánh giá', response.message);
                }
            },
            error: () => {

            }
        })
    })
})

const starGroupHTML = (star) => {
    let stars = '';
    for (let i = 1; i <= star; i++) {
        stars += `<i class="fa-solid fa-star"></i>`;
    }

    for (let i = 1; i <= 5 - star; i++) {
        stars += `<i class="fa-regular fa-star"></i>`;
    }

    return `
         <div class="rating-star-group">
            ${stars}
         </div>
    `;
}

const reviewHTML = (review) => {
    const reviewerName = review.user?.fullName ?? review.reviewerName ?? "Không có tên";
    const reviewerAvatar = review.user?.avatar ?? "/admin/images/user-no-image.svg";

    return `
        <div class="review-item">
            <div class="mb-1 d-flex gap-3 align-items-center justify-content-between">
                <div class="d-flex gap-3 align-items-center">
                    <div class="reviewer-avatar">
                        <img src="${reviewerAvatar}" style="width: 2rem" alt="" />
                    </div>

                    <span class="fweight-500">
                        ${reviewerName}
                    </span>

                    ${review.IsPurchased === true ?
            `<span style="padding: .05rem .6rem" class="page-badge badge-success">Đã mua sản phẩm này</span>`
            :
            `<span style ="padding: .05rem .6rem" class="page-badge badge-warning">Chưa mua sản phẩm này</span>`
        }
                </div>
                <div>
                    <span style="font-size: .85rem" class="text-secondary">${formatDateTime(review.createAt)}</span>
                </div>
            </div>

            <div class="ps-3" style="margin-left: 2rem">
                <div class="d-flex gap-3 align-items-center">
                    ${starGroupHTML(review.rating)}

                    <span class="fweight-600">"${review.content}"</span>
                </div>


                <div class="d-flex gap-3 align-items-center justify-content-between mt-2">
                    <div>
                        <span style="font-size: .87rem">
                            <span class="text-secondary">Đã đánh giá</span>
                            <a class="text-decoration-none review-product-name" href="javascript:">${review.product.productName}</a>
                        </span>
                    </div>

                    <div class="d-flex gap-1 align-items-stretch">
                        <div class="d-flex align-items-center">
                            <span class="me-3">(${review.reviewImages.length} hình ảnh)</span>
                        </div>

                        <div class="d-flex align-items-center">
                            <span class="me-3">(${review.reviewReplies.length} phản hồi)</span>
                        </div>

                        <button class="page-btn btn-red delete-review" data-review="${review.id}">
                            <i class="fa-solid fa-trash-can"></i>
                        </button>

                        <a class="page-btn btn-lightblue view-review" href="/admin/reviews/1/${review.id}">
                            <i class="fa-solid fa-ellipsis"></i>
                        </a>

                        ${review.isProceeded !== true ?                       
                            `<button class="page-btn btn-green approve-review" data-review="${review.id}">
                                <i class="fa-solid fa-check"></i>
                            </button>` : ''
                        }
                    </div>
                </div>
            </div>
        </div>
    `;
}

const loadReviews = (page) => {
    const sort_by = $('.sort-selection .page-dropdown-btn').data('selected') || null;
    const status = $('.status-selection .page-dropdown-btn').data('selected') || null;
    const filter_by = $('.filter-selection .page-dropdown-btn').data('selected') || null;
    const search = $('.search-reviews #search').val() || null;
    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/reviews`,
        data: {
            search: search,
            page: page,
            sort_by: sort_by,
            status: status,
            filter_by: filter_by
        },
        success: (response) => {
            hideWebLoader(500);
            setTimeout(() => {
                $('.review-list').empty();

                response.data.reviews.map(review => {
                    $('.review-list').append(reviewHTML(review));
                })

                loadPagination(response.data.totalPages, response.data.currentPage);

                tippyButtons();

                updateParams({
                    search: search,
                    sort_by: sort_by,
                    status: status,
                    filter_by: filter_by
                })
            }, 500)
        },
        error: () => {

        }
    })
}


$('.page-dropdown-item').not('.selected').click(() => {
    loadReviews(1);
})

$('.search-reviews').submit((e) => {
    e.preventDefault();
    loadReviews(1);
})