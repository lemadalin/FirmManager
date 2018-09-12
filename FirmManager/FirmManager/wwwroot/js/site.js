$(document).ready(function () {
    function removeActiveClassAll() {
        $('.wrapper .sidebar ul li').each(function (element) {
            $(element).removeClass("active");
        });
    }

    function changeActiveClass() {
        var currentFullUrl = window.location.href;
        var currentUrlProtocol = window.location.protocol;
        var currentUrlHost = window.location.host;

        var simpleUrl = currentFullUrl.replace(currentUrlProtocol, "").replace(currentUrlHost, "").replace("///", "");

        if (simpleUrl.indexOf("Customers") !== -1) {
            removeActiveClassAll();
            $("#customers").addClass("active");
        }
        else if (simpleUrl.indexOf("Products") !== -1) {
            removeActiveClassAll();
            $("#products").addClass("active");
        }
        else if (simpleUrl.indexOf("Orders") !== -1) {
            removeActiveClassAll();
            $("#orders").addClass("active");
        }
        else if (simpleUrl.indexOf("Statistics") !== -1) {
            removeActiveClassAll();
            $("#statistics").addClass("active");
        }
        else if ((simpleUrl.indexOf("Home") !== -1) || simpleUrl.length === 0) {
            removeActiveClassAll();
            $("#home").addClass("active");
        }
    }

    changeActiveClass();

    $('#dismiss, .overlay').on('click', function () {
        // hide sidebar
        $('#sidebar').removeClass('active');
        // hide overlay
        $('.overlay').removeClass('active');
    });

    $('#sidebarCollapse').on('click', function () {
        // open sidebar
        $('#sidebar').addClass('active');
        // fade in the overlay
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });



    $(document).on("click", ".searchButton", function () {
        var form = $(this).closest("form");
        form.submit();
    });

    $('input[name="quantity"], .numberInput').keydown(function (e) {
        var key = e.charCode || e.keyCode || 0;
        return (
            key === 8 ||
            key === 9 ||
            key === 13 ||
            key === 46 ||
            key === 110 ||
            key === 190 ||
            (key >= 35 && key <= 40) ||
            (key >= 48 && key <= 57) ||
            (key >= 96 && key <= 105));
    });

    $('input[name="quantity"], .phoneNumberInput').keydown(function (e) {
        var key = e.charCode || e.keyCode || 0;
        return (
            key === 8 ||
            key === 9 ||
            key === 13 ||
            key === 46 ||
            key === 109 ||
            key === 110 ||
            key === 190 ||
            (key >= 35 && key <= 40) ||
            (key >= 48 && key <= 57) ||
            (key >= 96 && key <= 105));
    });

    $('.textInput').keydown(function (e) {
        var key = e.charCode || e.keyCode || 0;
        return (
            key === 8 ||
            key === 9 ||
            key === 13 ||
            key === 32 ||
            key === 46 ||
            key === 110 ||
            key === 190 ||
            (key >= 35 && key <= 40) ||
            (key >= 65 && key <= 90));
    });
});