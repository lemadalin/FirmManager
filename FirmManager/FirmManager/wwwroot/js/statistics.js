$(document).ready(function () {
    (function () {
        var svg = d3.select("#customersPerState"),
            margin = {
                top: 20,
                right: 10,
                bottom: 30,
                left: 30
            },
            width = +svg.attr("width") - margin.left - margin.right,
            height = +svg.attr("height") - margin.top - margin.bottom,
            g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scaleBand()
            .rangeRound([0, width])
            .paddingOuter(0.3)
            .paddingInner(0.1);

        var y = d3.scaleLinear()
            .rangeRound([height, 0]);

        d3.tsv("/api/data/customersPerState").then(function (data) {
            x.domain(data.map(function (d) {
                return d.State;
            }));
            y.domain([0, d3.max(data, function (d) {
                return Number(d.Count);
            })]);

            g.append("g")
                .attr("transform", "translate(0," + height + ")")
                .call(d3.axisBottom(x))
                .append("text")
                .attr("fill", "#000")
                .attr("x", 1370)
                .attr("dy", "-0.5em")
                .attr("text-anchor", "end")
                .text("State");

            g.append("g")
                .call(d3.axisLeft(y))
                .append("text")
                .attr("fill", "#000")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", "0.71em")
                .attr("text-anchor", "end")
                .text("Count");

            g.selectAll(".bar")
                .data(data)
                .enter().append("rect")
                .attr("class", "bar")
                .attr("x", function (d) {
                    return x(d.State);
                })
                .attr("y", function (d) {
                    return y(Number(d.Count));
                })
                .attr("width", x.bandwidth())
                .attr("height", function (d) {
                    return height - y(Number(d.Count));
                });
        });
    })();

    (function () {
        var svg = d3.select("#ordersPerMonth"),
            margin = {
                top: 20,
                right: 10,
                bottom: 30,
                left: 30
            },
            width = +svg.attr("width") - margin.left - margin.right,
            height = +svg.attr("height") - margin.top - margin.bottom,
            g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scaleBand()
            .rangeRound([0, width])
            .paddingOuter(0.3)
            .paddingInner(0.1);

        var y = d3.scaleLinear()
            .rangeRound([height, 0]);

        d3.tsv("/api/data/ordersPerMonth").then(function (data) {
            x.domain(data.map(function (d) {
                return d.Month;
            }));
            y.domain([0, d3.max(data, function (d) {
                return Number(d.Count);
            })]);

            g.append("g")
                .attr("transform", "translate(0," + height + ")")
                .call(d3.axisBottom(x))
                .append("text")
                .attr("fill", "#000")
                .attr("x", 1370)
                .attr("dy", "-0.5em")
                .attr("text-anchor", "end")
                .text("Month");

            g.append("g")
                .call(d3.axisLeft(y))
                .append("text")
                .attr("fill", "#000")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", "0.71em")
                .attr("text-anchor", "end")
                .text("Count");

            g.selectAll(".bar")
                .data(data)
                .enter().append("rect")
                .attr("class", "bar")
                .attr("x", function (d) {
                    return x(d.Month);
                })
                .attr("y", function (d) {
                    return y(Number(d.Count));
                })
                .attr("width", x.bandwidth())
                .attr("height", function (d) {
                    return height - y(Number(d.Count));
                });
        });
    })();

    (function () {
        d3.tsv("/api/data/productsSoldPerCategory").then(function (data) {
            $("#placeholder-spinner-soldProducts").hide();
            var width = 500,
                height = 500,
                // Think back to 5th grade. Radius is 1/2 of the diameter. What is the limiting factor on the diameter? Width or height, whichever is smaller
                radius = Math.min(width, height) / 2;

            var color = d3.scaleOrdinal()
                .range(["#3366cc", "#dc3912", "#ff9900", "#109618", "#990099", "#0099c6",
                    "#dd4477", "#66aa00", "#b82e2e", "#316395", "#994499", "#22aa99", "#aaaa11",
                    "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac"]);

            var pie = d3.pie()
                .value(function (d) { return d.Count; })(data);

            var arc = d3.arc()
                .outerRadius(radius - 10)
                .innerRadius(0);

            var labelArc = d3.arc()
                .outerRadius(radius - 40)
                .innerRadius(radius - 40);

            var svg = d3.select("#productsSoldPerCategory")
                .append("svg")
                .attr("width", width)
                .attr("height", height)
                .append("g")
                .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

            var g = svg.selectAll("arc")
                .data(pie)
                .enter().append("g")
                .attr("class", "arc");

            g.append("path")
                .attr("d", arc)
                .style("fill", function (d) { return color(d.data.Category); });

            g.append("text")
                .attr("transform", function (d) {
                    var _d = arc.centroid(d);
                    _d[0] *= 1.45;
                    _d[1] *= 1.45;
                    return "translate(" + _d + ")";
                })
                .attr("dy", ".50em")
                .style("text-anchor", "middle")
                .style('fill', '#FFFFFF')
                .text(function (d) {
                    return d.data.Category;
                });

            g.append("text")
                .attr("transform", function (d) {
                    var _d = arc.centroid(d);
                    _d[0] *= 1.45;
                    _d[1] *= 1.45;
                    return "translate(" + _d + ")";
                })
                .attr("dy", "2em")
                .style("text-anchor", "middle")
                .style('fill', '#FFFFFF')
                .text(function (d) {
                    return numberWithCommas(d.data.Count);
                });

            var oldStyle;

            $("#productsSoldPerCategory .arc path").mouseover(function () {
                oldStyle = $(this).css("fill");
                $(this).css("fill", shadeBlend(0.2, oldStyle));
            });

            $("#productsSoldPerCategory .arc path").mouseout(function () {
                $(this).css("fill", oldStyle);
            });
        });
    })();

    (function () {
        d3.tsv("/api/data/totalSalesPerCategory").then(function (data) {
            $("#placeholder-spinner-totalSales").hide();
            var width = 500,
                height = 500,
                // Think back to 5th grade. Radius is 1/2 of the diameter. What is the limiting factor on the diameter? Width or height, whichever is smaller
                radius = Math.min(width, height) / 2;

            var color = d3.scaleOrdinal()
                .range(["#316395", "#994499", "#22aa99", "#aaaa11", "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac", "#3366cc", "#dc3912", "#ff9900", "#109618", "#990099", "#0099c6",
                    "#dd4477", "#66aa00", "#b82e2e"]);

            var pie = d3.pie()
                .value(function (d) { return d.Sales; })(data);

            var arc = d3.arc()
                .outerRadius(radius - 10)
                .innerRadius(0);

            var labelArc = d3.arc()
                .outerRadius(radius - 40)
                .innerRadius(radius - 40);

            var svg = d3.select("#totalSalesPerCategory")
                .append("svg")
                .attr("width", width)
                .attr("height", height)
                .append("g")
                .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

            var g = svg.selectAll("arc")
                .data(pie)
                .enter().append("g")
                .attr("class", "arc");

            g.append("path")
                .attr("d", arc)
                .style("fill", function (d) { return color(d.data.Category); });

            g.append("text")
                .attr("transform", function (d) {
                    var _d = arc.centroid(d);
                    _d[0] *= 1.45;
                    _d[1] *= 1.45;
                    return "translate(" + _d + ")";
                })
                .attr("dy", ".50em")
                .style("text-anchor", "middle")
                .style('fill', '#FFFFFF')
                .text(function (d) {
                    return d.data.Category;
                });

            g.append("text")
                .attr("transform", function (d) {
                    var _d = arc.centroid(d);
                    _d[0] *= 1.45;
                    _d[1] *= 1.45;
                    return "translate(" + _d + ")";
                })
                .attr("dy", "2em")
                .style("text-anchor", "middle")
                .style('fill', '#FFFFFF')
                .text(function (d) {
                    return "$" + numberWithCommas(d.data.Sales);
                });

            var oldStyle;

            $("#totalSalesPerCategory .arc path").mouseover(function () {
                oldStyle = $(this).css("fill");
                $(this).css("fill", shadeBlend(0.2, oldStyle));
            });

            $("#totalSalesPerCategory .arc path").mouseout(function () {
                $(this).css("fill", oldStyle);
            });
        });
    })();

    function shadeBlend(p, c0, c1) {
        var n = p < 0 ? p * -1 : p, u = Math.round, w = parseInt;
        if (c0.length > 7) {
            var f = c0.split(","), t = (c1 ? c1 : p < 0 ? "rgb(0,0,0)" : "rgb(255,255,255)").split(","), R = w(f[0].slice(4)), G = w(f[1]), B = w(f[2]);
            return "rgb(" + (u((w(t[0].slice(4)) - R) * n) + R) + "," + (u((w(t[1]) - G) * n) + G) + "," + (u((w(t[2]) - B) * n) + B) + ")"
        } else {
            var f = w(c0.slice(1), 16), t = w((c1 ? c1 : p < 0 ? "#000000" : "#FFFFFF").slice(1), 16), R1 = f >> 16, G1 = f >> 8 & 0x00FF, B1 = f & 0x0000FF;
            return "#" + (0x1000000 + (u(((t >> 16) - R1) * n) + R1) * 0x10000 + (u(((t >> 8 & 0x00FF) - G1) * n) + G1) * 0x100 + (u(((t & 0x0000FF) - B1) * n) + B1)).toString(16).slice(1)
        }
    }

    const numberWithCommas = (x) => {
        var parts = x.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    };
});