document.addEventListener('DOMContentLoaded', function () {
    const modalElement = document.getElementById('jobDetailsModal');
    if (!modalElement) {
        console.error('Modal element #jobDetailsModal not found!');
        return;
    }
    const jobModal = new bootstrap.Modal(modalElement);
    
    const jobDetailsContent = document.getElementById('jobDetailsContent');
    if (!jobDetailsContent) {
        console.error('Modal content container #jobDetailsContent not found!');
        return;
    }

    const tableRows = document.querySelectorAll('table tbody tr.prow');

    tableRows.forEach(row => {
        row.addEventListener('click', function (event) {
            // Prevent modal from opening if an action element or checkbox inside the row was clicked
            const target = event.target;
            if (target.closest('a.action-link, button.action-button, input[type="checkbox"], form.action-form')) {
                // If the click originated from an action link, button, checkbox, or form, do not open the modal.
                // Let the default action (e.g., navigation, form submission, checkbox toggle) proceed.
                return; 
            }

            // Get job details from the clicked row cells
            // Ensure these indices match your table structure
            const jobTitle = this.cells[1].textContent.trim();
            const status = this.cells[2].textContent.trim();
            const datePosted = this.cells[3].textContent.trim();
            const role = this.cells[4].textContent.trim();
            const vacancies = this.cells[5].textContent.trim();
            const department = this.cells[6].textContent.trim();
            const jobType = this.cells[7].textContent.trim();

            // Create the HTML content for the modal
            const jobDetailHtml = `
                <div class="row">
                    ${createDetailCard("Job Title", jobTitle)}
                    ${createDetailCard("Status", status)}
                    ${createDetailCard("Date Posted", datePosted)}
                    ${createDetailCard("Role", role)}
                    ${createDetailCard("Vacancies", vacancies)}
                    ${createDetailCard("Department", department)}
                    ${createDetailCard("Job Type", jobType)}
                </div>
            `;

            // Update the modal content and display the modal
            jobDetailsContent.innerHTML = jobDetailHtml;
            jobModal.show();
        });
    });

    // Card template to display each detail in the modal
    // Parameters: labelText (e.g., "Job Title"), valueText (e.g., "Software Engineer")
    function createDetailCard(value, label) {
        return `
            <div class="col-md-6">
                <span>${value}</span>
                <div class="bg-light rounded px-3 py-3 mb-3 d-flex justify-content-between align-items-center" style="font-weight: bold;">
                    <span class="text-end" style="color: #198754;">${label}</span>
                </div>
            </div>
        `;
    }
});