var dataTable;

document.addEventListener("DOMContentLoaded", function () {
    var url = window.location.search;
    var status = "";

        if (url.includes("inprocess")) {
            status = "inprocess";
        } else if (url.includes("pending")) {
            status = "pending";
        } else if (url.includes("completed")) {
            status = "completed";
        } else if (url.includes("approved")) {
            status = "approved";
        } else {
            status = "all";
    }
    dataTable = new DataTable('#tblData', {
        ajax: {
            url: '/Admin/Order/GetAll?status=' + status,
            dataSrc: 'data'   // tell DataTables where the array is
        },
        columns: [
            { data: "id", width: "5%" },
            { data: "name", width: "25%" },
            { data: "phoneNumber", width: "20%" },
            { data: "applicationUser.email", width: "20%" },
            { data: "orderStatus", width: "10%" },
            { data: "orderTotal", width: "10%" },
            {
                data: "id",
                render: function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">

                                <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                            </div>
                            `;
                },
                width: "10%"
            }
        ]
    });
});
