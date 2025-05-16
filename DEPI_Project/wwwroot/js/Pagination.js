
    document.addEventListener("DOMContentLoaded", function () {
        const rowsPerPage = 10;
        const rows = document.querySelectorAll(".prow");
        const pagination = document.querySelector(".pagination");
        let currentPage = 1;
        const totalPages = Math.ceil(rows.length / rowsPerPage);

        function displayRows(page) {
            const start = (page - 1) * rowsPerPage;
            const end = start + rowsPerPage;

            rows.forEach((row, index) => {
                row.style.display = (index >= start && index < end) ? "" : "none";
            });
        }

        function setActivePage(pageNumber) {
            document.querySelectorAll(".page-item").forEach(li => li.classList.remove("active"));
            const activeLink = document.querySelector(`.page-item[data-page="${pageNumber}"]`);
            if (activeLink) activeLink.classList.add("active");
        }

        function setupPagination() {
            pagination.innerHTML = "";

            // Previous button
            const prev = document.createElement("li");
            prev.classList.add("page-item");
            prev.innerHTML = `<a class="page-link" href="#">«Prev</a>`;
            prev.addEventListener("click", function (e) {
                e.preventDefault();
                if (currentPage > 1) {
                    currentPage--;
                    displayRows(currentPage);
                    setActivePage(currentPage);
                }
            });
            pagination.appendChild(prev);

            // Page numbers
            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement("li");
                li.classList.add("page-item");
                li.setAttribute("data-page", i);
                li.innerHTML = `<a class="page-link" href="#">${i}</a>`;

                li.addEventListener("click", function (e) {
                    e.preventDefault();
                    currentPage = i;
                    displayRows(currentPage);
                    setActivePage(currentPage);
                });

                pagination.appendChild(li);
            }

            // Next button
            const next = document.createElement("li");
            next.classList.add("page-item");
            next.innerHTML = `<a class="page-link" href="#">Next»</a>`;
            next.addEventListener("click", function (e) {
                e.preventDefault();
                if (currentPage < totalPages) {
                    currentPage++;
                    displayRows(currentPage);
                    setActivePage(currentPage);
                }
            });
            pagination.appendChild(next);

            // Initialize
            displayRows(currentPage);
            setActivePage(currentPage);
        }

        setupPagination();
    });

//search
document.getElementById("searchInput").addEventListener("keyup", function () {
    const filter = this.value.toLowerCase();
    const rows = document.querySelectorAll("table tbody tr");

    rows.forEach(row => {
        const cells = Array.from(row.cells);
        const match = cells.some(cell => cell.textContent.toLowerCase().includes(filter));

        row.style.display = match ? "" : "none";
    });
});

