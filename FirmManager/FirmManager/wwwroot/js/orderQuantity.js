$(document).ready(function () {
    function resetQuantities() {
        $("#quantities > div").each(function () {
            $(this).hide();
        });

        $("#quantities div > input").each(function () {
            $(this).val(0);
        });
    }
    resetQuantities();

    $("#selectedProducts").change(function () {
        resetQuantities();
        $(".selectedProduct input").removeClass("selectedProduct");

        $("#selectedProducts > option:selected").each(function () {
            $("#quantities > div:nth-child(" + $(this).val() + ")").show().addClass("selectedProduct");
        });

        $(".selectedProduct input").val(1);
    });

    $("#quantities div > input").keyup(function () {
        if ($(this).val() === "") {
            $(this).val(1);
        }
    });
});