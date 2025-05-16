// Get all table rows
const rows = document.querySelectorAll('table tbody tr');
const departmentDetails = document.getElementById('departmentDetails'); 
const modal = new bootstrap.Modal(document.getElementById('detailsModal')); 

rows.forEach(row => {
    row.addEventListener('click', function () {
        const deptName = this.cells[0].textContent;
        const head = this.cells[1].textContent;
        const phone = this.cells[2].textContent;
        const capacity = this.cells[3].textContent;
        const year = this.cells[4].textContent;
        const totalEmployees = this.cells[5].textContent;

        const departmentDetailHtml = `
            <div class="row">
                ${createDetailCard("Department Name", deptName)}
                ${createDetailCard("Head of Department", head)}
                ${createDetailCard("Phone", phone)}
                ${createDetailCard("Employee Capacity", capacity)}
                ${createDetailCard("Established Year", year)}
                ${createDetailCard("Total Employees", totalEmployees)}
            </div>
        `;

        departmentDetails.innerHTML = departmentDetailHtml;
        modal.show();
    });
});

function createDetailCard(label, value) {
    return `
        <div class="col-md-6">
            <span>${label}</span>
            <div class="bg-light rounded px-3 py-3 mb-3 d-flex justify-content-between align-items-center" style="font-weight: bold;">
                <span class="text-end" style="color: #198754;">${value}</span>
            </div>
        </div>
    `;
}
