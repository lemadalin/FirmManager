$(document).ready(function () {
    var currentFullUrl = window.location.href;
    var url = new URL(currentFullUrl);
    var pageNumber = url.searchParams.get("page");

    if (pageNumber < 1 || !/^\d+$/.test(pageNumber)) {
        pageNumber = "1";
    }

    var pageNumberInt = Number.parseInt(pageNumber);
    var totalPages = $("#pages a").length;

    $("#pages a").hide().each(function () {
        $(this).removeClass("activePage");
    });

    $("#pages a:nth-child(" + pageNumberInt + ")").addClass("activePage");

    if (pageNumberInt >= 1 && pageNumberInt < 7) {
        for (var x = 0; x < 8; x++) {
            $("#pages a:nth-child(" + x + ")").show();
        }
    }
    else if (pageNumberInt > totalPages - 6 && pageNumberInt <= totalPages) {
        for (var y = totalPages + 1; y > totalPages - 7; y--) {
            $("#pages a:nth-child(" + y + ")").show();
        }
    }
    else {
        for (var z = pageNumberInt - 3; z <= pageNumberInt + 3; z++) {
            $("#pages a:nth-child(" + z + ")").show();
        }
    }
});