$(document).ready(function () {
    const $form = $('#sendEmailForm');
    const $button = $('#sendEmailButton');
    const $deleteSelectedRecipientsButton = $('#deleteSelectedRecipients');
    const $deleteSelectedTemplatesButton = $('#deleteSelectedTemplates');

    const validateForm = () => $button.prop('disabled', !$form[0].checkValidity());

    const params = new URLSearchParams(window.location.search);
    const activeTab = params.get('activeTab');
    if (activeTab) {
        $('.nav-link').removeClass('active');
        $('.tab-pane').removeClass('show active');
        $(`#${activeTab}-tab`).addClass('active');
        $(`#${activeTab}`).addClass('show active');
    }

    function loadModal(entityType, entityId = null) {
        const url = entityId ? `/EmailManagement/Edit${entityType}/${entityId}` : `/EmailManagement/Create${entityType}`;
        const modalTitle = entityId ? `Edit ${entityType}` : `Create ${entityType}`;
        const modalLabel = `#${entityType.toLowerCase()}ModalLabel`;
        const modalBody = `#${entityType.toLowerCase()}Modal .modal-body`;

        $(modalLabel).text(modalTitle);
        $.get(url, function (data) {
            $(modalBody).html(data);
            $(`#${entityType.toLowerCase()}Modal`).modal('show');
        });
    }

    function loadDetailsModal(entityId) {
        const url = `/EmailManagement/RecipientDetails/${entityId}`;
        $.get(url, function (data) {
            $('#recipientDetailsModal .modal-body').html(data);
            $('#recipientDetailsModal').modal('show');
        });
    }

    $form.on('input', validateForm);
    $form.on('submit', event => {
        if (!$form[0].checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }
        $form.addClass('was-validated');
    });

    $deleteSelectedRecipientsButton.on('click', function () {
        $('#deleteConfirmationModal').modal('show');
    });

    $deleteSelectedTemplatesButton.on('click', function () {
        $('#deleteConfirmationModal').modal('show');
    });

    $('#confirmDeleteButton').on('click', function () {
        const selectedRecipients = $('.select-recipient:checked').map((_, item) => $(item).data('id')).get();
        const selectedTemplates = $('.select-template:checked').map((_, item) => $(item).data('id')).get();

        if (selectedRecipients.length > 0) {
            const $form = $('<form>', { method: 'post', action: '/EmailManagement/DeleteRecipients' });
            selectedRecipients.forEach(id => {
                $form.append($('<input>', { type: 'hidden', name: 'ids', value: id }));
            });
            $('body').append($form);
            $form.submit();
        }

        if (selectedTemplates.length > 0) {
            const $form = $('<form>', { method: 'post', action: '/EmailManagement/DeleteEmailTemplates' });
            selectedTemplates.forEach(id => {
                $form.append($('<input>', { type: 'hidden', name: 'ids', value: id }));
            });
            $('body').append($form);
            $form.submit();
        }

        $('#deleteConfirmationModal').modal('hide');
    });

    const toggleDeleteButtons = () => {
        $deleteSelectedRecipientsButton.prop('disabled', $('.select-recipient:checked').length === 0);
        $deleteSelectedTemplatesButton.prop('disabled', $('.select-template:checked').length === 0);
    };

    const toggleSelectAllCheckboxes = (checkbox, itemClass) => {
        $(`.${itemClass}`).prop('checked', $(checkbox).prop('checked'));
        toggleDeleteButtons();
    };

    $('#selectAllRecipients').on('change', function () {
        toggleSelectAllCheckboxes(this, 'select-recipient');
    });

    $('#selectAllTemplates').on('change', function () {
        toggleSelectAllCheckboxes(this, 'select-template');
    });

    $('.select-recipient, .select-template').on('change', toggleDeleteButtons);

    validateForm();

    // Attach loadModal to global window object for usage in HTML attributes
    window.loadModal = loadModal;
    window.loadDetailsModal = loadDetailsModal;
});
