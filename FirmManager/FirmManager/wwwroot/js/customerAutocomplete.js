var availableTags = [];

$.getJSON("/api/data/customersJson", function (data) {
    $.each(data, function (key, val) {
        availableTags.push(data[key].FirstName + " " + data[key].LastName);
    });
});

$(".searchInput").autocomplete({
    source: availableTags
});

$(".ui-autocomplete").css("max-height", "200px").css("overflow-y", "auto").css("overflow-x", "hidden").css("border", "1 px solid black");