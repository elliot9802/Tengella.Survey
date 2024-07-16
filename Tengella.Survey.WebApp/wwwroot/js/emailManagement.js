document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('sendEmailForm');
    const button = document.getElementById('sendEmailButton');

    const validateForm = () => {
        button.disabled = !form.checkValidity();
    };

    form.addEventListener('input', validateForm);
    validateForm();
});

function loadModal(entityType, entityId) {
    const url = entityId ? `/EmailManagement/Edit${entityType}/${entityId}` : `/EmailManagement/Create${entityType}`;
    const modalTitle = entityId ? `Edit ${entityType}` : `Create ${entityType}`;
    const modalLabel = `#${entityType.toLowerCase()}ModalLabel`;
    const modalBody = `#${entityType.toLowerCase()}Modal .modal-body`;

    $(modalLabel).text(modalTitle);
    $.get(url, function (data) {
        $(modalBody).html(data);
    });
}
