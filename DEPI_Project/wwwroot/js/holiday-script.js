document.addEventListener('DOMContentLoaded', function () {
    // Initialize modal
    const holidayDetailsModal = document.getElementById('holidayDetailsModal');
    const modal = new bootstrap.Modal(holidayDetailsModal);
    const holidayDetails = document.getElementById('holidayDetails');
    const rows = document.querySelectorAll('table tbody tr');

    // Add click event to each row to show details
    rows.forEach(row => {
        row.addEventListener('click', function (e) {
            if (e.target.type === 'checkbox' || e.target.closest('a') || e.target.closest('button')) {
                return;
            }

            const holidayName = this.cells[1].textContent;
            const startDate = this.cells[2].textContent;
            const endDate = this.cells[3].textContent;
            const duration = this.cells[4].textContent;
            const createdBy = this.cells[5].textContent;

            const holidayDetailHtml = `
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">${holidayName}</h5>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <p><strong>Start Date:</strong> ${startDate}</p>
                                <p><strong>End Date:</strong> ${endDate}</p>
                            </div>
                            <div class="col-md-6">
                                <p><strong>Duration:</strong> ${duration}</p>
                                <p><strong>Created By:</strong> ${createdBy}</p>
                            </div>
                        </div>
                    </div>
                </div>
            `;

            holidayDetails.innerHTML = holidayDetailHtml;
            modal.show();
        });
    });

    // Multiple selection handling for delete
    const selectAll = document.getElementById("select-all");
    const checkboxes = document.querySelectorAll(".row-checkbox");
    const deleteBtn = document.getElementById("deleteSelectedBtn");

    function toggleDeleteBtn() {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked);
        deleteBtn.classList.toggle("d-none", !anyChecked);
    }

    if (selectAll) {
        selectAll.addEventListener("change", function () {
            checkboxes.forEach(cb => cb.checked = this.checked);
            toggleDeleteBtn();
        });
    }

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

    // Enhanced search functionality without user filter
    const nameSearchInput = document.getElementById("nameSearchInput");
    const dateSearchInput = document.getElementById("dateSearchInput");
    const totalRecordsElement = document.getElementById("totalRecords");

    function performSearch() {
        const nameFilter = nameSearchInput.value.toLowerCase();
        const dateFilter = dateSearchInput.value.toLowerCase();

        const rows = document.querySelectorAll("table tbody tr");
        let visibleCount = 0;

        rows.forEach(row => {
            const holidayName = row.cells[1].textContent.toLowerCase();
            const startDate = row.cells[2].textContent.toLowerCase();
            const endDate = row.cells[3].textContent.toLowerCase();

            const nameMatch = nameFilter === "" || holidayName.includes(nameFilter);
            const dateMatch = dateFilter === "" || startDate.includes(dateFilter) || endDate.includes(dateFilter);

            if (nameMatch && dateMatch) {
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

    // Pagination
    const rowsPerPage = 8;
    let currentPage = 1;
    const pagination = document.querySelector('.pagination');

    function updatePagination() {
        if (!pagination) return;

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
