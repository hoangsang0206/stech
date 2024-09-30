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
                        location.reload();
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