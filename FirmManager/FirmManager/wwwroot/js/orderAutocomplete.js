var availableTags = [];

$.getJSON("/api/data/ordersJson", function (data) {
    $.each(data, function (key, val) {
        availableTags.push(data[key].OrderNumber + "");
    });
});

$(".searchInput").autocomplete({
    source: availableTags
});

$(".ui-autocomplete").css("max-height", "200px").css("overflow-y", "auto").css("overflow-x", "hidden").css("border", "1 px solid black");