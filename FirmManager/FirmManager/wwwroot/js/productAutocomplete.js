var availableTags = [];

$.getJSON("/api/data/productsJson", function (data) {
    $.each(data, function (key, val) {
        availableTags.push(data[key].Title);
    });
});

$(".searchInput").autocomplete({
    source: availableTags
});

$(".ui-autocomplete").css("max-height", "200px").css("overflow-y", "auto").css("overflow-x", "hidden").css("border", "1 px solid black");