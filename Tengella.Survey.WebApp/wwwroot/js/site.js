﻿function minimizeAllExcept(currentCard) {
    $('.question-card').not(currentCard).find('.question-content').slideUp();
}

function updateEventListeners() {
    $('.card-body .card-title').off('click').on('click', function () {
        const card = $(this).closest('.question-card');
        minimizeAllExcept(card);
        card.find('.question-content').slideDown();
    });
}

updateEventListeners();
