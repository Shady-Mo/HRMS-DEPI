// Handling Modal for attendance details
document.addEventListener('DOMContentLoaded', function () {
    // Initialize modal
    const attendanceDetailsModal = document.getElementById('attendanceDetailsModal');
    const modal = new bootstrap.Modal(attendanceDetailsModal);
    const attendanceDetails = document.getElementById('attendanceDetails');
    const rows = document.querySelectorAll('table tbody tr');

    // Add click event to each row to show details
    rows.forEach(row => {
        row.addEventListener('click', function (e) {
            // Avoid execution when clicking on checkbox or button
            if (e.target.type === 'checkbox' || e.target.closest('a') || e.target.closest('button')) {
                return;
            }

            const employeeName = this.cells[1].textContent;
            const date = this.cells[2].textContent;
            const timeIn = this.cells[3].textContent;
            const timeOut = this.cells[4].textContent;
            const totalHours = this.cells[5].textContent;
            const status = this.cells[6].textContent.trim();

            const attendanceDetailHtml = `
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">${employeeName} - Attendance Record</h5>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <p><strong>Date:</strong> ${date}</p>
                                <p><strong>Time In:</strong> ${timeIn}</p>
                                <p><strong>Time Out:</strong> ${timeOut}</p>
                            </div>
                            <div class="col-md-6">
                                <p><strong>Total Hours:</strong> ${totalHours}</p>
                                <p><strong>Status:</strong> ${status}</p>
                            </div>
                        </div>
                    </div>
                </div>
            `;

            attendanceDetails.innerHTML = attendanceDetailHtml;
            modal.show();
        });
    });

    // Multiple selection handling for delete
    const selectAll = document.getElementById("select-all");
    const checkboxes = document.querySelectorAll(".row-checkbox");
    const deleteBtn = document.getElementById("deleteSelectedBtn");

    // Function to toggle delete button based on selection
    function toggleDeleteBtn() {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked);
        deleteBtn.classList.toggle("d-none", !anyChecked);
    }

    // Select all checkbox handling
    selectAll.addEventListener("change", function () {
        checkboxes.forEach(cb => cb.checked = this.checked);
        toggleDeleteBtn();
    });

    // Individual checkbox handling
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

    // Enhanced search functionality with separate filters
    const nameSearchInput = document.getElementById("nameSearchInput");
    const dateSearchInput = document.getElementById("dateSearchInput");
    const statusSearchInput = document.getElementById("statusSearchInput");
    const totalRecordsElement = document.getElementById("totalRecords");

    function performSearch() {
        const nameFilter = nameSearchInput.value.toLowerCase();
        const dateFilter = dateSearchInput.value.toLowerCase();
        const statusFilter = statusSearchInput.value.toLowerCase();

        const rows = document.querySelectorAll("table tbody tr");
        let visibleCount = 0;

        rows.forEach(row => {
            const employeeName = row.cells[1].textContent.toLowerCase();
            const date = row.cells[2].textContent.toLowerCase();
            const status = row.cells[6].textContent.toLowerCase().trim();

            // Check if all active filters match their respective fields
            const nameMatch = nameFilter === "" || employeeName.includes(nameFilter);
            const dateMatch = dateFilter === "" || date.includes(dateFilter);
            const statusMatch = statusFilter === "" || status.includes(statusFilter);

            // Show row only if all active filters match
            if (nameMatch && dateMatch && statusMatch) {
                row.style.display = "";
                visibleCount++;
            } else {
                row.style.display = "none";
            }
        });

        // Update total records counter to show filtered count
        if (totalRecordsElement) {
            totalRecordsElement.textContent = visibleCount;
        }

        // Reset pagination to first page after search
        currentPage = 1;
        updatePagination();
    }

    // Apply search on input change
    if (nameSearchInput) nameSearchInput.addEventListener("keyup", performSearch);
    if (dateSearchInput) dateSearchInput.addEventListener("keyup", performSearch);
    if (statusSearchInput) statusSearchInput.addEventListener("change", performSearch);

    // Pagination functionality
    const rowsPerPage = 8;
    let currentPage = 1;
    const pagination = document.querySelector('.pagination');

    function updatePagination() {
        const allRows = document.querySelectorAll("table tbody tr");
        const visibleRows = Array.from(allRows).filter(row => row.style.display !== "none");
        const totalVisibleRows = visibleRows.length;
        const totalPages = Math.ceil(totalVisibleRows / rowsPerPage);

        // Update pagination UI
        pagination.innerHTML = '';

        // Previous button
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

        // Page numbers
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

        // Next button
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

        // Show/hide rows based on current page
        visibleRows.forEach((row, index) => {
            if (index >= (currentPage - 1) * rowsPerPage && index < currentPage * rowsPerPage) {
                row.classList.remove('d-none');
            } else {
                row.classList.add('d-none');
            }
        });
    }

    // Initialize pagination
    updatePagination();
});