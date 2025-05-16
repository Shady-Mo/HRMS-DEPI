// Bootstrap modal reference
const modal = new bootstrap.Modal(document.getElementById('candidateDetailsModal'));
const candidateDetails = document.getElementById('candidateDetails');

// Get all table rows
const rows = document.querySelectorAll('table tbody tr');
rows.forEach(row => {
    row.addEventListener('click', function () {
        const name = this.cells[1].textContent;
        const jobTitle = this.cells[2].textContent;
        const mobile = this.cells[3].textContent;
        const jobType = this.cells[4].textContent;
        const role = this.cells[5].textContent;
        const email = this.cells[6].textContent;

        const candidateDetailHtml = `
            <div class="row">
                ${createDetailCard("Name", name)}
                ${createDetailCard("Job Title", jobTitle)}
                ${createDetailCard("Mobile", mobile)}
                ${createDetailCard("Job Type", jobType)}
                ${createDetailCard("Role", role)}
                ${createDetailCard("Email", email)}
            </div>
        `;

        candidateDetails.innerHTML = candidateDetailHtml;
        modal.show();
    });
});

// Card template (value on left, label on right)
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