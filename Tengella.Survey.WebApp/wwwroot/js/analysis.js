$(function () {
    const totalResponses = $('#questions-container').data('total-responses');
    const newResponses = $('#questions-container').data('new-responses');

    function escapeSelector(selector) {
        return selector.replace(/([ #;?%&,.+*~\':"!^$[\]()=>|/@])/g, '\\$1');
    }

    $('.show-more-icon').on('click', function () {
        const $this = $(this);
        const questionId = $this.data('question-id');
        const escapedQuestionId = escapeSelector(questionId);
        const responsesList = $('#responses-' + escapedQuestionId);
        const currentCount = responsesList.children().length;

        if (!newResponses[questionId]) {
            return;
        }

        const responses = newResponses[questionId].slice(currentCount, currentCount + 5);

        responses.forEach(function (response) {
            const li = document.createElement('li');
            li.className = 'list-group-item';
            li.innerText = response;
            responsesList.append(li);
        });

        if (responsesList.children().length >= totalResponses[questionId]) {
            $this.addClass('d-none');
        }
        $('.minimize-icon[data-question-id="' + escapedQuestionId + '"]').removeClass('d-none');
    });

    $('.minimize-icon').on('click', function () {
        const $this = $(this);
        const questionId = $this.data('question-id');
        const escapedQuestionId = escapeSelector(questionId);
        const responsesList = $('#responses-' + escapedQuestionId);
        const currentCount = responsesList.children().length;

        if (currentCount > 5) {
            responsesList.children().slice(currentCount - 5).remove();
        }

        if (responsesList.children().length <= 5) {
            $this.addClass('d-none');
        }
        $('.show-more-icon[data-question-id="' + escapedQuestionId + '"]').removeClass('d-none');
    });

    $('.show-more-btn').on('click', function () {
        const $this = $(this);
        const questionId = $this.data('question-id');
        const escapedQuestionId = escapeSelector(questionId);
        const responsesList = $('#responses-' + escapedQuestionId);
        const currentCount = responsesList.children().length;

        const responses = newResponses[questionId].slice(currentCount, currentCount + 5);

        responses.forEach(function (response) {
            const li = document.createElement('li');
            li.className = 'list-group-item';
            li.innerText = response;
            responsesList.append(li);
        });

        if (responsesList.children().length >= totalResponses[questionId]) {
            $this.hide();
        }
        $('.minimize-btn[data-question-id="' + escapedQuestionId + '"]').removeClass('d-none');
    });

    $('.minimize-btn').on('click', function () {
        const $this = $(this);
        const questionId = $this.data('question-id');
        const escapedQuestionId = escapeSelector(questionId);
        const responsesList = $('#responses-' + escapedQuestionId);

        responsesList.children().slice(5).remove();
        $this.addClass('d-none');
        $('.show-more-btn[data-question-id="' + escapedQuestionId + '"]').show();
    });
});
