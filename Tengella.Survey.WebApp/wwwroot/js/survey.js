$(function () {
    let questionsToRemove = [];
    let optionsToRemove = [];
    $('#previewSurveyBtn').on('click', function () {
        if ($('#surveyForm').valid()) {
            $('<input>').attr({
                type: 'hidden',
                name: 'isPreview',
                value: 'true'
            }).appendTo('#surveyForm');
            $('#surveyForm').trigger('submit');
        }
    });

    $.validator.addMethod('optionscount', function (value, element) {
        const questionType = $(element).closest('.question-card').find('.question-type').val();
        if (questionType === 'Open') return true;
        const optionsCount = $(element).closest('.question-card').find('.options-container .input-group').length;
        return optionsCount >= 2 && optionsCount <= 10;
    }, "Varje fr�ga m�ste ha mellan 2 - 10 alternativ.");

    $('#surveyForm').validate({
        rules: {
            'Name': { required: true, maxlength: 100 },
            'Type': { required: true, maxlength: 50 },
            'ClosingDate': { required: true, date: true }
        },
        messages: {
            'Name': { required: "Enk�tnamn �r obligatoriskt", maxlength: "Enk�tnamn f�r inte �verstiga 100 tecken" },
            'Type': { required: "Enk�ttyp �r obligatorisk", maxlength: "Enk�ttyp f�r inte �verstiga 50 tecken" },
            'ClosingDate': { required: "Sista datum �r obligatoriskt", date: "Ange ett giltligt datum" }
        },
        errorElement: 'div',
        errorClass: 'text-danger'
    });

    $('#responseForm').validate({
        rules: {
            'Answer': { required: true, maxlength: 350 },
        },
        messages: {
            'Answer': { required: "Enk�tsvar �r obligatoriskt", maxlength: "Enk�tsvar f�r inte �verstiga 350 tecken" }
        },
        errorElement: 'div',
        errorClass: 'text-danger'
    });

    function validateOptionsCount() {
        let valid = true;
        $('.question-card').each(function () {
            const questionType = $(this).find('.question-type').val();
            if (questionType === 'Open') {
                $(this).find('.options-error').remove();
                return true;
            }
            const optionsCount = $(this).find('.options-container .input-group').length;
            const errorContainer = $(this).find('.options-container').next('.options-error');
            if (optionsCount < 2 || optionsCount > 10) {
                valid = false;
                if (!errorContainer.length) {
                    $(this).find('.options-container').after('<div class="options-error text-danger">Varje fr�ga m�ste ha mellan 2 - 10 alternativ.</div>');
                }
            } else {
                errorContainer.remove();
            }
        });
        return valid;
    }

    $('#surveyForm').on('submit', function (event) {
        if (!validateOptionsCount()) {
            event.preventDefault();
        }
    });

    $('#add-question-btn').on('click', function () {
        addQuestion();
        const newQuestion = $('#questions-container .question-card').last();
        minimizeAllExcept(newQuestion);
        newQuestion.find('.question-content').slideDown();
    });

    function addQuestion() {
        const container = $('#questions-container');
        const questionCount = container.children().length;
        const questionHtml = `
            <div class="card mb-3 question-card" id="question-${questionCount}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <h6 class="card-title" data-question-index="${questionCount}">Fr�ga ${questionCount + 1}</h6>
                        <button type="button" class="btn-close remove-question-btn" aria-label="Close" data-question-index="${questionCount}"></button>
                    </div>
                    <div class="question-content mt-3">
                        <div class="form-group">
                            <input type="hidden" name="Questions[${questionCount}].QuestionId" value="0" />
                            <input name="Questions[${questionCount}].Text" class="form-control question-text" placeholder="Ange fr�getext" />
                        </div>
                        <div class="form-group mt-3">
                            <select name="Questions[${questionCount}].Type" class="form-control question-type" data-question-index="${questionCount}">
                                <option value="Radio">Flervalsfr�ga</option>
                                <option value="Open">Kort svar</option>
                            </select>
                        </div>
                        <div class="options-container mt-3"></div>
                        <button type="button" class="btn btn-link add-option-btn mt-3" data-question-index="${questionCount}" style="display:none;">L�gg till alternativ</button>
                    </div>
                </div>
            </div>
        `;

        container.append(questionHtml);
        updateEventListeners();
    }

    function updateEventListeners() {
        $('.add-option-btn').off('click').on('click', handleAddOptionClick);
        $('.remove-question-btn').off('click').on('click', handleRemoveQuestionClick);
        $('.question-type').off('change').on('change', handleQuestionTypeChange);
        $('.remove-option-btn').off('click').on('click', handleRemoveOptionClick);
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

        $('.question-type').each(function () {
            const questionIndex = $(this).data('question-index');
            const addOptionBtn = $(`.add-option-btn[data-question-index="${questionIndex}"]`);
            const questionType = $(this).val();
            if (questionType === "Radio") {
                addOptionBtn.show();
            } else {
                addOptionBtn.hide();
            }
        });
    }

    function handleAddOptionClick(event) {
        const questionIndex = $(event.target).data('question-index');
        const optionsContainer = $(`#question-${questionIndex} .options-container`);
        const optionCount = optionsContainer.children().length;
        const optionHtml = `
            <div class="input-group mb-2" id="question-${questionIndex}-option-${optionCount}">
                <input name="Questions[${questionIndex}].Options[${optionCount}].Text" class="form-control" placeholder="Ange alternativtext" />
                <input type="hidden" name="Questions[${questionIndex}].Options[${optionCount}].OptionId" value="0" />
                <button type="button" class="btn-close remove-option-btn btn-outline-danger" aria-label="Close" data-question-index="${questionIndex}" data-option-index="${optionCount}"></button>
            </div>
        `;

        optionsContainer.append(optionHtml);
        updateEventListeners();
        validateOptionsCount();
    }

    function handleRemoveQuestionClick(event) {
        const questionIndex = $(event.target).data('question-index');
        const questionId = $(event.target).data('question-id');

        $(`#question-${questionIndex} .options-container .input-group`).each(function () {
            const optionId = $(this).find('input[name*="OptionId"]').val();
            if (optionId) {
                optionsToRemove.push(optionId);
            }
        });

        if (questionId) {
            questionsToRemove.push(questionId);
        }
        $(`#question-${questionIndex}`).remove();

        $('#questionsToRemove').val(questionsToRemove.join(','));
        $('#optionsToRemove').val(optionsToRemove.join(','));
        updateQuestionIndices();
        validateOptionsCount();
    }

    function handleRemoveOptionClick(event) {
        const questionIndex = $(event.target).data('question-index');
        const optionIndex = $(event.target).data('option-index');
        const optionId = $(event.target).data('option-id');
        if (optionId) {
            optionsToRemove.push(optionId);
        }
        $(`#question-${questionIndex}-option-${optionIndex}`).remove();
        $('#optionsToRemove').val(optionsToRemove.join(','));
        updateOptionIndices(questionIndex);
        validateOptionsCount();
    }

    function updateQuestionIndices() {
        $('#questions-container').children('.question-card').each(function (index) {
            const currentQuestionId = $(this).find('input[name*="QuestionId"]').val();
            $(this).attr('id', `question-${index}`);
            $(this).find('.card-title').text(`Question ${index + 1}`);
            $(this).find('.remove-question-btn').data('question-index', index);
            $(this).find('input[name^="Questions"]').filter('[name*="Text"]').attr('name', `Questions[${index}].Text`);
            $(this).find('input[name^="Questions"]').filter('[name*="QuestionId"]').attr('name', `Questions[${index}].QuestionId`).val(currentQuestionId);
            $(this).find('select[name^="Questions"]').attr('name', `Questions[${index}].Type`).data('question-index', index);
            updateOptionIndices(index);
        });
    }

    function updateOptionIndices(questionIndex) {
        $(`#question-${questionIndex} .options-container`).children().each(function (index) {
            const currentOptionId = $(this).find('input[name^="Questions"]').filter('[name*="OptionId"]').val();
            $(this).attr('id', `question-${questionIndex}-option-${index}`);
            $(this).find('input[name^="Questions"]').filter('[name*="Text"]').attr('name', `Questions[${questionIndex}].Options[${index}].Text`);
            $(this).find('input[name^="Questions"]').filter('[name*="OptionId"]').attr('name', `Questions[${questionIndex}].Options[${index}].OptionId`).val(currentOptionId);
            $(this).find('.remove-option-btn').data('option-index', index);
        });
        validateOptionsCount();
    }

    function handleQuestionTypeChange(event) {
        const questionIndex = $(event.target).data('question-index');
        const addOptionBtn = $(`.add-option-btn[data-question-index="${questionIndex}"]`);
        const optionsContainer = $(`#question-${questionIndex} .options-container`);

        if ($(event.target).val() === "Open") {
            addOptionBtn.hide();
            optionsContainer.empty();
        } else {
            addOptionBtn.show();
        }
        validateOptionsCount();
    }

    updateEventListeners();
});
