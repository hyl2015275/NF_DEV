(function ($) {
    if (!$.Autocompleter) {
        alert("请先引用jquery.autocomplete.js");
        return;
    }
    $.fn.usbox = function (o) {
        var def = {
            urlOrData: "",
            width: "90%", //宽度
            addItem: false,
            removeItem: false,
            clickItem: function () { },
            completeOp: {}
        };
        $.extend(def, o);
        var co = $.extend({
            scroll: false, formatItem: function (row, i, max) {
                var result = row[0] +
                "[" + row[1] + "]" +
                "[" + row[2] + "]<br>";
                if (row[3] != null && row[3] != "") {
                    result += "[" + row[3] + "]";
                }
                if (row[4] != null && row[4] != "") {
                    result += "[" + row[4] + "]";
                }
                return result;
            }
        }, def.completeOp);
        return this.each(function (e) {
            var me = $(this);
            var isCard = $("#F_UserCard").length > 0;
            var input = !isCard ? $("#F_MeterCode") : $("#F_UserCard");
            input.autocomplete(def.urlOrData, co).result(function (event, data, formatted) {
                $(this).val("");
                additem(this, data);
            });
            me.bind("addboxitem", function (e, data) { additem(input, data); });
            function additem(inc, data) {
                $(inc).val(!isCard ? data[2] : data[1]);
                if (def.addItem) {
                    def.addItem(data);
                }
            }
            return me;
        });
    };
    $.fn.addboxitem = function (op) {
        $(this).trigger("addboxitem", [op]);
    };
})(jQuery)