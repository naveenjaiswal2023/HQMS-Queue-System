$(document).ready(function () {
    $('.datatables').DataTable({  // ✅ correct selector
        paging: true,
        searching: true,
        ordering: true,
        lengthChange: false,
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        pageLength: 25,
        columnDefs: [
            {
                targets: 0,
                searchable: false,
                orderable: false,
            }
        ],
        order: [[1, 'asc']],
        drawCallback: function (settings) {
            let api = this.api();
            api.column(0, { search: 'applied', order: 'applied', page: 'current' })
                .nodes()
                .each(function (cell, i) {
                    cell.innerHTML = i + 1 + settings._iDisplayStart;
                });
        }
    });
});
