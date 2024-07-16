$(document).ready(() => {
    const params = new URLSearchParams(window.location.search);
    const activeTab = params.get('activeTab');
    if (activeTab) {
        $('.nav-link').removeClass('active');
        $('.tab-pane').removeClass('show active');
        $(`#${activeTab}-tab`).addClass('active');
        $(`#${activeTab}`).addClass('show active');
    }

    const $form = $('#sendEmailForm');
    const $button = $('#sendEmailButton');
    const $deleteSelectedRecipientsButton = $('#deleteSelectedRecipients');
    const $deleteSelectedTemplatesButton = $('#deleteSelectedTemplates');
    const $selectAllRecipientsCheckbox = $('#selectAllRecipients');
    const $selectAllTemplatesCheckbox = $('#selectAllTemplates');

    const validateForm = () => $button.prop('disabled', !$form[0].checkValidity());

    const toggleDeleteButtons = () => {
        $deleteSelectedRecipientsButton.prop('disabled', $('.select-recipient:checked').length === 0);
        $deleteSelectedTemplatesButton.prop('disabled', $('.select-template:checked').length === 0);
    };

    const toggleSelectAllCheckboxes = (checkbox, itemClass) => {
        $(`.${itemClass}`).prop('checked', $(checkbox).prop('checked'));
        toggleDeleteButtons();
    };

    $form.on('input', validateForm);

    $form.on('submit', event => {
        if (!$form[0].checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }
    });

    $selectAllRecipientsCheckbox.on('change', () => toggleSelectAllCheckboxes($selectAllRecipientsCheckbox, 'select-recipient'));
    $selectAllTemplatesCheckbox.on('change', () => toggleSelectAllCheckboxes($selectAllTemplatesCheckbox, 'select-template'));

    $('.select-recipient, .select-template').on('change', toggleDeleteButtons);

    $deleteSelectedRecipientsButton.on('click', () => {
        const selectedRecipients = $('.select-recipient:checked').map((_, item) => $(item).data('id')).get();
        console.log(selectedRecipients);
        if (selectedRecipients.length > 0) {
            const $form = $('<form>', { method: 'post', action: '/EmailManagement/DeleteRecipients' });
            selectedRecipients.forEach(id => {
                $form.append($('<input>', { type: 'hidden', name: 'ids', value: id }));
            });
            $('body').append($form);
            $form.submit();
        }
    });

    $deleteSelectedTemplatesButton.on('click', () => {
        const selectedTemplates = $('.select-template:checked').map((_, item) => $(item).data('id')).get();
        if (selectedTemplates.length > 0) {
            const $form = $('<form>', { method: 'post', action: '/EmailManagement/DeleteEmailTemplates' });
            selectedTemplates.forEach(id => {
                $form.append($('<input>', { type: 'hidden', name: 'ids', value: id }));
            });
            $('body').append($form);
            $form.submit();
        }
    });

    validateForm();
    toggleDeleteButtons();
});
