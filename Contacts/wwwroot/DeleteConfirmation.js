function confirmDelete(personId) {
    // Show the modal
    $('#deleteConfirmationModal').modal('show');

    // When the user clicks "Yes, Delete"
    $('#confirmDeleteButton').off('click').on('click', function () {
        window.location.href = window.location.origin + '/persons/delete?id=' + personId;
    });
}