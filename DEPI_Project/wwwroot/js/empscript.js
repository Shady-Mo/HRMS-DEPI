// Bootstrap modal reference
const modal = new bootstrap.Modal(document.getElementById('employeeDetailsModal'));
const employeeDetails = document.getElementById('employeeDetails');

// Get all table rows
const rows = document.querySelectorAll('table tbody tr');
rows.forEach(row => {
    row.addEventListener('click', function () {
        const name = this.cells[1].textContent;
        const position = this.cells[2].textContent;
        const department = this.cells[3].textContent;
        const mobile = this.cells[4].textContent;
        const gender = this.cells[5].textContent;
        const  email= this.cells[6].textContent;
        const hireDate = this.cells[7].textContent;
        const address = this.cells[8].textContent;
        const status = this.cells[9].textContent;
        const salary = this.cells[10].textContent;
        const contractType = this.cells[11].textContent;
        const maritalStatus = this.cells[12].textContent;

        const employeeDetailHtml = `
            <div class="row">
                ${createDetailCard("Name", name)}
                ${createDetailCard("Position", position)}
                ${createDetailCard("Department", department)}
                ${createDetailCard("Mobile", mobile)}
                ${createDetailCard("Gender", gender)}
                ${createDetailCard("Hire Date", hireDate)}
                ${createDetailCard("Email", email)}
                ${createDetailCard("Address", address)}
                ${createDetailCard("Status", status)}
                ${createDetailCard("Salary", salary)}
                ${createDetailCard("Contract Type", contractType)}
                ${createDetailCard("Marital Status", maritalStatus)}
            </div>
        `;

        employeeDetails.innerHTML = employeeDetailHtml;
        modal.show();
    });
});

// Card template (value on left, label on right)
function createDetailCard( value,label) {
    return `
        <div class="col-md-6 ">
         <span>${value}</span>
            <div class="bg-light rounded px-3 py-3 mb-3 d-flex justify-content-between align-items-center" style="font-weight: bold;">
             <span class="text-end" style="color: #198754;">${label}</span>
            </div>
        </div>
    `;
}


     const selectAll = document.getElementById("select-all");
    const checkboxes = document.querySelectorAll(".row-checkbox");
    const deleteBtn = document.getElementById("deleteSelectedBtn");

    // دالة لتبديل حالة زر الحذف بناءً على التحديد
    function toggleDeleteBtn() {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked); // التحقق إذا كان هناك أي عنصر محدد
        deleteBtn.classList.toggle("d-none", !anyChecked); // إذا لا يوجد عناصر محددة، إخفاء الزر
    }

    // عند تغيير حالة اختيار الكل
    selectAll.addEventListener("change", function () {
        checkboxes.forEach(cb => cb.checked = this.checked); // تحديد أو إزالة التحديد عن كل العناصر
        toggleDeleteBtn(); // تحديث حالة الزر
    });

    // عند تغيير حالة اختيار أي عنصر
    checkboxes.forEach(cb => {
        cb.addEventListener("change", function () {
            if (!this.checked) {
                selectAll.checked = false; // إذا لم يكن هناك اختيار، قم بإلغاء اختيار "اختر الكل"
            } else {
                const allChecked = Array.from(checkboxes).every(c => c.checked); // تحقق إذا كانت كل العناصر محددة
                selectAll.checked = allChecked; // إذا كانت كل العناصر محددة، حدد "اختر الكل"
            }
            toggleDeleteBtn(); // تحديث حالة الزر
        });
    });

   