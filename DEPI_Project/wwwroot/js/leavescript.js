// Get all table rows
const rows = document.querySelectorAll('table tbody tr');
const leaveDetails = document.getElementById('leaveDetails'); 
const modal = new bootstrap.Modal(document.getElementById('leaveDetailsModal')); 

rows.forEach(row => {
    row.addEventListener('click', function () {
        const employee = this.cells[1].textContent.trim();
        const startDate = this.cells[2].textContent.trim();
        const endDate = this.cells[3].textContent.trim();
        const days = this.cells[4].textContent.trim();
        const type = this.cells[5].textContent.trim();
        const status = this.cells[6].textContent.trim();
        const requestDate = this.cells[7].textContent.trim();

        const leaveDetailHtml = `
            <div class="row">
                ${createDetailCard("Employee Name", employee)}
                ${createDetailCard("Start Date", startDate)}
                ${createDetailCard("End Date", endDate)}
                ${createDetailCard("Total Days", days)}
                ${createDetailCard("Leave Type", type)}
                ${createDetailCard("Status", status)}
                ${createDetailCard("Request Date", requestDate)}
            </div>
        `;

        leaveDetails.innerHTML = leaveDetailHtml;
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

document.addEventListener('DOMContentLoaded', function () {

    // ✅ checkbox selection and delete button handling
    const selectAll = document.getElementById("select-all");
    const checkboxes = document.querySelectorAll(".row-checkbox");
    const deleteBtn = document.getElementById("deleteSelectedBtn");

    function toggleDeleteBtn() {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked);
        deleteBtn.classList.toggle("d-none", !anyChecked);
    }

    selectAll.addEventListener("change", function () {
        checkboxes.forEach(cb => cb.checked = this.checked);
        toggleDeleteBtn();
    });

    checkboxes.forEach(cb => {
        cb.addEventListener("change", function () {
            if (!this.checked) {
                selectAll.checked = false;
            } else {
                const allChecked = Array.from(checkboxes).every(c => c.checked);
                selectAll.checked = allChecked;
            }
            toggleDeleteBtn();
        });
    });





    // Search functionality
    const nameSearchInput = document.getElementById("nameSearchInput");
    const dateSearchInput = document.getElementById("dateSearchInput");
    const statusSearchInput = document.getElementById("statusSearchInput");
    const typeSearchInput = document.getElementById("typeSearchInput"); // ✅ اضفتها هنا

    const totalRecordsElement = document.getElementById("totalRecords");

    function performSearch() {
        const nameFilter = nameSearchInput.value.toLowerCase();
        const dateFilter = dateSearchInput.value.toLowerCase();
        const statusFilter = statusSearchInput.value.toLowerCase();
        const typeFilter = typeSearchInput.value.toLowerCase(); // ✅ اضفتها هنا

        const rows = document.querySelectorAll("table tbody tr");
        let visibleCount = 0;

        rows.forEach(row => {
            const employeeName = row.cells[1].textContent.toLowerCase();
            const startDate = row.cells[2].textContent.toLowerCase();
            const endDate = row.cells[3].textContent.toLowerCase();
            const type = row.cells[5].textContent.toLowerCase().trim(); // ✅ اضفتها هنا
            const status = row.cells[6].textContent.toLowerCase().trim();

            const nameMatch = nameFilter === "" || employeeName.includes(nameFilter);
            const dateMatch = dateFilter === "" || startDate.includes(dateFilter) || endDate.includes(dateFilter);
            const statusMatch = statusFilter === "" || status.includes(statusFilter);
            const typeMatch = typeFilter === "" || type.includes(typeFilter); // ✅ اضفتها هنا

            if (nameMatch && dateMatch && statusMatch && typeMatch) {
                row.style.display = "";
                visibleCount++;
            } else {
                row.style.display = "none";
            }
        });

        if (totalRecordsElement) {
            totalRecordsElement.textContent = visibleCount;
        }

        currentPage = 1;
        updatePagination();
    }

    if (nameSearchInput) nameSearchInput.addEventListener("keyup", performSearch);
    if (dateSearchInput) dateSearchInput.addEventListener("keyup", performSearch);
    if (statusSearchInput) statusSearchInput.addEventListener("change", performSearch);
    if (typeSearchInput) typeSearchInput.addEventListener("change", performSearch); // ✅ اضفتها هنا

    // Pagination functionality
    const rowsPerPage = 8;
    let currentPage = 1;
    const pagination = document.querySelector('.pagination');

    function updatePagination() {
        const allRows = document.querySelectorAll("table tbody tr");
        const visibleRows = Array.from(allRows).filter(row => row.style.display !== "none");
        const totalVisibleRows = visibleRows.length;
        const totalPages = Math.ceil(totalVisibleRows / rowsPerPage);

        pagination.innerHTML = '';

        const prevLi = document.createElement('li');
        prevLi.className = 'page-item' + (currentPage === 1 ? ' disabled' : '');
        const prevLink = document.createElement('a');
        prevLink.className = 'page-link';
        prevLink.href = '#';
        prevLink.textContent = 'Previous';
        prevLink.addEventListener('click', function (e) {
            e.preventDefault();
            if (currentPage > 1) {
                currentPage--;
                updatePagination();
            }
        });
        prevLi.appendChild(prevLink);
        pagination.appendChild(prevLi);

        for (let i = 1; i <= totalPages; i++) {
            const pageLi = document.createElement('li');
            pageLi.className = 'page-item' + (i === currentPage ? ' active' : '');
            const pageLink = document.createElement('a');
            pageLink.className = 'page-link';
            pageLink.href = '#';
            pageLink.textContent = i;
            pageLink.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = i;
                updatePagination();
            });
            pageLi.appendChild(pageLink);
            pagination.appendChild(pageLi);
        }

        const nextLi = document.createElement('li');
        nextLi.className = 'page-item' + (currentPage === totalPages || totalPages === 0 ? ' disabled' : '');
        const nextLink = document.createElement('a');
        nextLink.className = 'page-link';
        nextLink.href = '#';
        nextLink.textContent = 'Next';
        nextLink.addEventListener('click', function (e) {
            e.preventDefault();
            if (currentPage < totalPages) {
                currentPage++;
                updatePagination();
            }
        });
        nextLi.appendChild(nextLink);
        pagination.appendChild(nextLi);

        visibleRows.forEach((row, index) => {
            if (index >= (currentPage - 1) * rowsPerPage && index < currentPage * rowsPerPage) {
                row.classList.remove('d-none');
            } else {
                row.classList.add('d-none');
            }
        });
    }

    updatePagination();
});
