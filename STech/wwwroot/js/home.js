$(document).ready(() => {
    const saleElements = $('.section-sale').toArray();

    saleElements.map((item) => {
        const endDate = new Date($(item).data('end'));
        updateCountDown(item, endDate);
    });
})

