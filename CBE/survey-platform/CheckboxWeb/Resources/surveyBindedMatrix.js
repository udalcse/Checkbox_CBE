; (function ($) {

    // Create the defaults once
    var pluginName = 'bindedMatrix',
       defaults = {
           inputLength: 400,
           addRowBtnTitle: "Add row",
           addColumnBtnTitle: "Add Column",
           removeColumnTitle: "Remove",
           addRowBtnClass: ".matrix-add-row-btn",
           inputClass: "Answer uniform-input text"
       };

    function BindedMatrix(element, options) {
        this.element = element;

        this.options = $.extend({}, defaults, options);

        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    BindedMatrix.prototype.init = function () {

        var self = this;

        var matrixElementId = $(this.element).attr("id");

        this.options.matrixContainer = this.element;
        this.options.rowsCounterName = matrixElementId + "-binded-matrix-rows-count";
        this.options.columnsCounterName = matrixElementId + "-binded-matrix-columns-count";

        this.bindedMatrixStructureInit();
    
        if (this.options.columnFixed) {
        
            $(this.options.matrixContainer).on("click", ".matrix-add-row-btn", function (event) {
                self.addRow(event, self);
            });

            $(this.options.matrixContainer).on("click", ".matrix-remove-row-link", function (event) {
                self.removeRow(event,self);
            });

        } else {
            $(this.options.matrixContainer).on("click", ".matrix-remove-column-link", function (event) {
                self.removeColumn(event, self);
            });
            $(this.options.matrixContainer).on("click", ".matrix-add-column-link", function (event) {
                self.addColumn(event, self);
            });

        }
    };

    BindedMatrix.prototype.validateRow = function (row) {

        var headerInput = $(row).find("td:first input");

        if (headerInput.val() === '') {
            headerInput.addClass("account-profile-input-required");
            return false;
        } else {
            headerInput.removeClass("account-profile-input-required");
            return true;
        }
    }

    BindedMatrix.prototype.appryTableRowStyle = function (clonedRow) {
        var lastRow = $(this.element).find("tr:last");

        if ($(lastRow).hasClass("AlternatingItem")) {
            $(clonedRow).attr("class", "Item");
        }
        else
        {
            $(clonedRow).attr("class", "AlternatingItem");
        }
    }

    BindedMatrix.prototype.addRow = function (event, context) {

        event.preventDefault();

        var targetRow = $(event.currentTarget).closest("tr");

        //validate if there are row headers
        if (context.options.hasRowHeaders && !context.validateRow($(targetRow))) {
            return;
        }

        var rowToAdd = targetRow.clone();
        rowToAdd.find("td:last button").remove();

        var selector = context.options.isMobile == "True" ? "textarea" : "input";

        rowToAdd.find(selector).each(function () {
            $(this).attr("name", $(context.options.matrixContainer).attr("id") + "-binded-matrix-input-" + context.guid());
        });

        rowToAdd.find("td:last").append("<a href='#' class='matrix-remove-row-link'>Remove Row</a>");

        //add row header value
        var rowHeader = $("<input type = 'hidden'  name = '" + $(context.options.matrixContainer).attr("id") + "-binded-matrix-row-header-" + context.guid() + "' />");
        
        if (context.options.hasRowHeaders) {
            var headerValue = $(rowToAdd).find("td:first input").val();

            rowHeader.attr("value", headerValue);
            $(rowToAdd).find("td:first").text(headerValue).addClass("padding-right150");
            $(rowToAdd).find("td:first input").remove();
        }

        $(rowToAdd).find("td:first > input").remove();
        $(rowToAdd).find("td:first").append(rowHeader);

        $(event.currentTarget).closest("tr").before(rowToAdd);

        //reset values
        $(targetRow).find('textarea, input').each(function () {
            $(this).val("");
        });

        $("[name='" + context.options.rowsCounterName + "']").get(0).value++;
    
    }

    BindedMatrix.prototype.removeRow = function (event, context) {
        var rowsLength = $(context.options.matrixContainer).find("tr").not(":first").length;
        if (rowsLength > context.options.baseRowsCount) {
            $(event.target).closest("tr").remove();
            $("[name='" + context.options.rowsCounterName + "']").get(0).value--;
        }
    }

    BindedMatrix.prototype.initRemoveRowActions = function () {

        var self = this;

        $(this.options.matrixContainer).find(".header").append("<td class='BorderBottom BorderLeft' colspan='1' rowspan='1'>Actions</td>");
        $(this.options.matrixContainer).find("tr").not(".header").append("<td class='BorderLeft'></td>");

        var disabled = "";

        if (this.options.isPreview) {
            disabled = ".not-active-link";
        }

        $(this.options.matrixContainer).find("tr").not(".header").each(function (index) {
            if (index >= self.options.baseRowsCount) {
                $(this).find("td:last").append("<a href='#' class='matrix-remove-row-link"+ disabled +"'>Remove Row</a>");
            }
        });
    }
    
    BindedMatrix.prototype.initCustomStructureInputs = function () {

        var self = this;
    
        if (this.options.columnFixed) {
            $(self.options.matrixContainer).find("tr").not(".header").each(function (index) {
                if (index >= self.options.baseRowsCount) {

                    var selector = self.options.isMobile == "True" ? "td textarea" : "td input:not(:first)";

                    $(this).find(selector).each(function () {
                        $(this).attr("name", $(self.options.matrixContainer).attr("id") + "-binded-matrix-input-" + self.guid());
                    });
                }
            });
        }
        else {
            $(this.options.matrixContainer).find("tr").not(".header").each(function () {
                $(this).find("td").not(":first").each(function (index) {
                    if (index >= self.options.baseColumnsCount) {

                        var selector = self.options.isMobile == "True" ? "textarea" : "input";

                        $(this).find(selector).each(function () {
                            $(this).attr("name", $(self.options.matrixContainer).attr("id") + "-binded-matrix-input-" + self.guid());
                        });
                    }
                });
            });
        }
    }

    BindedMatrix.prototype.addCustomHeaderInputs = function () {
        var self = this;
        if (this.options.columnFixed) {
            //add row headers to custom structure
            $(this.options.matrixContainer).find("tr").not(".header").each(function (index) {
                if (index >= self.options.baseRowsCount) {
                    var headerInput = $("<input type = 'hidden'  name = '" + $(self.options.matrixContainer).attr("id") + "-binded-matrix-row-header-" + self.guid() + "' />");
                    if (self.options.hasRowHeaders) {
                        headerInput.attr("value", $(this).find("td:first").text().trim());
                    }

                    $(this).find("td:first").append(headerInput);
                }
            });
        } else {
            $(this.options.matrixContainer).find(".header td").not(":first").each(function (index) {
                if (index >= self.options.baseColumnsCount) {
                    var headerInput = $("<input type = 'hidden'  name = '" + $(self.options.matrixContainer).attr("id") + "-binded-matrix-column-header-" + self.guid() + "' />");

                    if (self.options.hasColumnHeaders) {
                        headerInput.attr("value", $(this).text().trim());
                    }

                    $(this).append(headerInput);
                }
            });
        }
    }

    BindedMatrix.prototype.addColumn = function(event, context) {

        event.preventDefault();

        var allColumns = $(event.currentTarget).closest("tr").find("td");
        var column = $(event.currentTarget).closest("td");
        var columnIndex = $(allColumns).index(column);

        //if has headers validate them 
        if (context.options.hasColumnHeaders) {
            var header = $(context.options.matrixContainer).find("tr.header").find("td:eq(" + columnIndex + " ) > input");

            if ($(header).val() === '') {
                $(header).addClass("account-profile-input-required");
                return;
            } else {
                $(header).removeClass("account-profile-input-required");
            }
        }

        debugger;

        var selector = context.options.isMobile == "True" ? "textarea" : "input";

        $(context.options.matrixContainer).find("tr").each(function(index) {

            var td = $(this).find("td:eq(" + columnIndex + " )");
            var newColumn = td.clone();

         

            if (index === 0) {
                $(newColumn).find("input").attr("name", $(context.options.matrixContainer).attr("id") + "-binded-matrix-column-header-" + context.guid());
            } else {
                $(newColumn).find(selector).attr("name", $(context.options.matrixContainer).attr("id") + "-binded-matrix-input-" + context.guid());
            }


            var addColumnButton = $(newColumn).find(".matrix-add-column-link");
            if (addColumnButton.length > 0) {
                $(newColumn).find(".matrix-add-column-link").remove();
                $(newColumn).append("<a href='#'  class='matrix-remove-column-link'>Remove Column</a>");
            }

            td.before(newColumn);

            if (index === 0) {
                td.find('input').not(".matrix-add-column-link").val("");
            } else {
                td.find(selector).not(".matrix-add-column-link").val("");
            }

        });

        $("[name='" + context.options.columnsCounterName + "']").get(0).value++;
    }

    BindedMatrix.prototype.removeColumn = function (event,context) {
        event.preventDefault();
        var allColumns = $(event.currentTarget).closest("tr").find("td");
        var column = $(event.currentTarget).closest("td");
        var index = $(allColumns).index(column);

        $(context.options.matrixContainer).find("tr").each(function () {
            $(this).find("td:eq(" + index + " )").remove();
        });
        $("[name='" + context.options.columnsCounterName + "']").get(0).value--;
    }

    BindedMatrix.prototype.applyRowHeaderStyles = function () {
        $(this.options.matrixContainer).find("tr").not(".header").each(function (parameters) {
            $(this).find("td:first").addClass("padding-right150");
        });
    }

    BindedMatrix.prototype.generateAddRow = function() {
        var primaryRow = $(this.options.matrixContainer).find("tr").not(".header").first().clone();
        var self = this;

        var selector = self.options.isMobile == "True" ? "td textarea" : "td input";

        // reset values 
        $(primaryRow).find(selector).each(function () {
            $(this).val("");
            $(this).attr("name", $(self.options.matrixContainer).attr("id") + "-binded-matrix-new-row-input-" + self.guid());
        });


        if (this.options.hasRowHeaders) {
            $(primaryRow).find("td:first").text("");
            $(primaryRow).find("td:first").append("<input class='" + this.options.inputClass + "'  type='text' maxlength='" + this.options.inputLength + "' name = '" + $(this.options.matrixContainer).attr("id") + "-binded-matrix-new-row-header-" + this.guid() + "'>");
        } else {
            $(primaryRow).find("td:first").append($("<input type = 'hidden'  name = '" + $(this.options.matrixContainer).attr("id") + "-binded-matrix-new-row-header-" + this.guid() + "' />"));
        }

        ////disable if it is preview mode
        var disabled = "";

        if (this.options.isPreview) {
            disabled = "disabled";
        }

        $(primaryRow).find("td:last").append("<button class='matrix-add-row-btn' " + disabled + ">" + this.options.addRowBtnTitle + " </button>");
        
        $(this.options.matrixContainer).append(primaryRow);
    }

    BindedMatrix.prototype.generateAddColumnRow = function () {
        var self = this;
        var actionsRow = $("<tr class='AlternatingItem'></tr>").append("<td style='font-weight : bold;'>Actions</td>");

        var disabled = "";

        if (this.options.isPreview) {
            disabled = ".not-active-link";
        }

        //add remove column actions
        for (var i = 0; i < this.options.columnsCount; i++) {
            if (i !== this.options.baseColumnsCount - 1) {
                actionsRow.append("<td><a href='#'  class='matrix-remove-column-link" + disabled + "'>Remove Column</a></td>");
            } else {
                actionsRow.append("<td></td>");
            }
        }

        var addColumnBtn = $("<td></td>").append($('<input>').attr({
            type: 'button',
            value: this.options.addColumnBtnTitle,
            'class': "matrix-add-column-link",
            disabled: this.options.isPreview ? true : false
        }));

        actionsRow.append(addColumnBtn);

        $(this.options.matrixContainer).find("tr").each(function (index) {
            //if it is header add a proper name for it
            if (index === 0) {
                if (self.options.hasColumnHeaders) {
                    $(this).append("<td colspan='1' rowspan='1'><input type = 'text' style = 'width:95%'  name = '" + $(self.options.matrixContainer).attr("id") + "-binded-matrix-new-row-header-" + self.guid() + "' /></td>");
                } else {
                    $(this).append("<td colspan='1' rowspan='1'><input type = 'hidden'  name = '" + $(self.options.matrixContainer).attr("id") + "-binded-matrix-new-row-header-" + self.guid() + "' /></td>");
                }
            } else {

                var selector = self.options.isMobile == "True" ? "textarea" : "input";

                var previousTd = $(this).find("td:last").clone();
                previousTd.find(selector).attr("name", $(self.options.matrixContainer)
                    .attr("id") + "-binded-matrix-new-row-input-" + self.guid())
                    .val("");
                $(this).append(previousTd);
            }
        });

        $(this.options.matrixContainer).append(actionsRow);

        if (!this.options.hasColumnHeaders) {
            $(this.element).attr("data-has-headers", false);
        }
    }



    BindedMatrix.prototype.applyGridLineRowStyles = function() {
        if (!this.options.hasRowHeaders && this.options.gridLine === 'Vertical') {
            var rows = $(this.options.matrixContainer).find("tr");

            $(rows).each(function() {
                $(this).find("td:first")
                    .removeClass("BorderTop")
                    .removeClass("BorderRight")
                    .removeClass("BorderBoth");
            });
        }
    }

    BindedMatrix.prototype.bindedMatrixStructureInit = function () {

        // hidden inputs to figure out how many rows and columns are in the matrix on the backend 
        this.addStructureCounters();
        this.addCustomHeaderInputs();
        this.initCustomStructureInputs();
        this.applyGridLineRowStyles();

        // if we are able to add rows generate table with proper fields otherwise generate actions buttons for modifing 
        if (this.options.columnFixed) {
            this.initRemoveRowActions();
            this.generateAddRow();
        } else {
            this.generateAddColumnRow();
        }
    }

    BindedMatrix.prototype.addStructureCounters = function () {

        $(this.options.matrixContainer).after($('<input>').attr({
            type: 'hidden',
            value: this.options.rowsCount,
            name: this.options.rowsCounterName
        }));

        $(this.options.matrixContainer).after($('<input>').attr({
            type: 'hidden',
            value: this.options.columnsCount,
            name: this.options.columnsCounterName
        }));
    }
    
    BindedMatrix.prototype.guid = function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }
 
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, 'plugin_' + pluginName)) {
                $.data(this, 'plugin_' + pluginName,
                new BindedMatrix(this, options));
            }
        });
    }

})(jQuery);