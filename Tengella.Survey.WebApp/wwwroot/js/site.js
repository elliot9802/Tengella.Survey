function minimizeAllExcept(currentCard) {
    $('.question-card').not(currentCard).find('.question-content').slideUp();
}

function updateEventListeners() {
    $('.card-body .card-title').off('click').on('click', function () {
        const card = $(this).closest('.question-card');
        const questionContent = card.find('.question-content');

        if (questionContent.is(':visible')) {
            questionContent.slideUp();
        } else {
            minimizeAllExcept(card);
            questionContent.slideDown();
        }
    });
}

updateEventListeners();
